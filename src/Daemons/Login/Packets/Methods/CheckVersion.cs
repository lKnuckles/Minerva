﻿/*
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

using System.Timers;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void CheckVersion(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var version = packet.ReadInt();

            var ip = client.RemoteEndPoint.ToString().Split(':')[0];
            var kick = false;
            var ver = client.Metadata["version"] as int[];
            var clientVersion = ver[0];
            var ignore = ver[2];

            if (version != clientVersion && ignore == 0)
            {
                Log.Notice(string.Format("Failed ClientAuth IP: {0} Client ver: {1} Server ver: {2}", ip, version, clientVersion));

                if (version > clientVersion)
                    events.VersionMismatch("login.CheckVersion", new VersionCheckEventArgs(version, ip, VersionCheckResult.NewerClient));
                else
                    events.VersionMismatch("login.CheckVersion", new VersionCheckEventArgs(version, ip, VersionCheckResult.OlderClient));

                kick = true;
            }

            if (ignore == 0)
                version = clientVersion;

            events.VersionMismatch("login.CheckVersion", new VersionCheckEventArgs(version, ip, VersionCheckResult.Match));

            builder.New(0x7A);
            {
                builder += version;     // Client Version
                builder += 0x0059077C;  // Debug
                builder += 0;           // Reserved
                builder += 0;           // Reserved
            }

            client.Send(builder, "CheckVersion");

            if (kick)
            {
                events.ClientDisconnected(client, client);
                return;
            }

            var id = -2;

            var sleep = new Timer(500);
            sleep.Elapsed += (s, _e) => 
            {
                if (id == -2)
                {
                    var syncServer = client.Metadata["syncServer"] as SyncReceiver;
                    id = Authentication.GetUser(syncServer, (ulong)client.Metadata["magic"]);
                }

                if (id > 0)
                {
                    client.AccountID = id;
                    SendChannels.SendChannelList(client);

                    var timer = new Timer(5000);
                    timer.AutoReset = true;
                    timer.Elapsed += (sender, e) => { SendChannels.SendChannelList(client); };

                    timer.Start();
                    client.Metadata["timer"] = timer;
                }

                sleep.Stop();
            };

            sleep.Start();
        }
    }
}