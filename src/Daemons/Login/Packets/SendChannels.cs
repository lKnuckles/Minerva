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

namespace Minerva
{
    class SendChannels
    {
        public static void SendChannelList(ClientHandler client)
        {
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var channels = ServerList.GetChannels(syncServer);

            if (channels == null)
                return;
            
            var p = new PacketBuilder();

            p.New(0x79);
            {
                p += (byte)channels.Length;

                foreach (var server in channels)
                {
                    p += server.id;
                    p += new byte[3];
                    p += (byte)server.channels.Length;

                    foreach (var channel in server.channels)
                    {
                        p += (byte)server.id;
                        p += (byte)channel.id;
                        p += (ushort)channel.curPlayers;
                        p += new byte[21];
                        p += (byte)255;
                        p += (ushort)channel.maxPlayers;
                        p += channel.ip;
                        p += (ushort)channel.port;
                        p += (byte)0;
                        p += channel.type;
                        p += new byte[3];
                    }
                }
            }

            client.Send(p, "ServerState");
        }
    }
}
