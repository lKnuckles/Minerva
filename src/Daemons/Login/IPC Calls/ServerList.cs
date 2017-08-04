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
    class ServerList
    {
        public static ServerData[] GetChannels(SyncReceiver sync)
        {
            var packet = sync.CreateIPC(IPC.ChannelList);
            sync.Send(packet);

            var recv = sync.ReadIPC();

            if (recv == null)
                return null;

            var num = recv.ReadInt32();

            if (num <= 0)
                return null;

            var serverList = new ServerData[num];

            for (var i = 0; i < num; i++)
            {
                var serverId = recv.ReadInt32();
                var channelLength = recv.ReadInt32();

                if (serverId < 0)
                    continue;

                serverList[i] = new ServerData(serverId, channelLength);

                for (var j = 0; j < channelLength; j++)
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
