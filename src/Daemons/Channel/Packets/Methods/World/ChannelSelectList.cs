/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://bitbucket.org/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void ChannelSelectList(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var channels = GetChannels(syncServer);

            if (channels == null)
                return;

            var ipuint = BitConverter.ToUInt32(Global.Config.IP.GetAddressBytes(), 0);

            builder.New(0x840);
            {
                foreach (var server in channels)
                {
                    if(server.id == ((int)client.Metadata["server"]))
                    {
                        builder += (byte)server.channels.Length;
                        foreach (var channel in server.channels)
                        {
                            builder += (byte)server.id;
                            builder += (byte)channel.id;
                            builder += (ushort)channel.curPlayers;
                            builder += new byte[21];
                            builder += (byte)255;
                            builder += (ushort)channel.maxPlayers;
                            if (ipuint == 0)
                                builder += channel.ip;
                            else
                                builder += ipuint;
                            builder += (ushort)channel.port;
                            builder += (byte)0;
                            builder += channel.type;
                            builder += new byte[3];
                        }
                    }
                }
            }
            
            client.Send(builder, "ChannelSelectList");
        }

        public static ServerData[] GetChannels(SyncReceiver sync)
        {
            var packet = sync.CreateIPC(IPC.ChannelList);
            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            var num = recv.ReadInt32();
            var serverList = new ServerData[num];
            
            for (int i = 0; i < num; i++)
            {
                var serverId = recv.ReadInt32();
                var channelLength = recv.ReadInt32();

                serverList[i] = new ServerData(serverId, channelLength);

                for (int j = 0; j < channelLength; j++)
                {
                    var id = recv.ReadInt32();
                    var type = recv.ReadInt32();
                    var ip = recv.ReadUInt32();
                    var port = recv.ReadInt16();
                    var maxPlayers = recv.ReadInt16();
                    var curPlayers = recv.ReadInt16();
                    serverList[i].channels[j] = new ChannelData(id, type, ip, port, maxPlayers, curPlayers);
                }
            }

            return serverList;
        }

    }
}
