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
using System.Text;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void Unknown_7D6(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            packet.Skip(4);
            var details = RSA.Decrypt(packet.ReadBytes(256));
            var tmpdata = new byte[16];

            Array.Copy(details, tmpdata, 16);
            var name = Encoding.UTF8.GetString(tmpdata).Trim('\0');

            Array.Copy(details, 65, tmpdata, 0, 16);
            var pass = Encoding.UTF8.GetString(tmpdata).Trim('\0');

            Log.Notice("Login authentication with name: " + name);

            var ip = client.RemoteEndPoint.ToString().Split(':')[0];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var data = Authentication.FetchAccount(syncServer, name, pass);
            var kick = false;

            if (data == null || data.id == -1)
            {
                builder.New(0x7D6);
                {
                    builder += 0;
                    builder += 0;
                    builder += (byte)0;
                    builder += (int)AccountStatus.OutOfService;
                }

                events.FailedLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, LoginResult.OutOfService));
                client.Send(builder, "Unknown_7D6");
                return;
            }

            builder.New(0x7D6);
            {
                switch (data.status)
                {
                    case AuthStatus.Unverified:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Unverified;
                        break;
                    case AuthStatus.Banned:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Banned;
                        break;
                    case AuthStatus.IncorrectName:
                    case AuthStatus.IncorrectPass:
                        builder += 0;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Incorrect;
                        var tmp = data.status == AuthStatus.IncorrectName ? LoginResult.UnknownUsername : LoginResult.WrongPassword;
                        events.FailedLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, tmp));
                        break;
                    case AuthStatus.Verified:
                        client.AccountID = data.id;
                        Authentication.UpdateIPDate(syncServer, client.AccountID, ip, DateTime.Now);
                        builder += 1;
                        builder += 0;
                        builder += (byte)0;
                        builder += (int)AccountStatus.Normal;
                        events.SuccessfulLogin("login.AuthAccount", new LoginEventArgs(name, pass, ip, LoginResult.Success, client.AccountID));
                        break;
                    default: kick = true; break;
                }
            }

            client.Send(builder, "Unknown_7D6");

            if (kick)
                client.Disconnect();

            if (data.id > 0 && data.status == AuthStatus.Verified)
            {
                builder.New(0x7D6);
                {
                    builder += 1;
                    builder += 0x0700;
                    builder += (byte)0;

                    builder += (int)AccountStatus.Normal;
                    builder += 0x2FD49F;
                    builder += (byte)1;
                    builder += 0x67;
                    builder += (long)0;
                    builder += (byte)0;
                    builder += 0x55746A01;
                    builder += (byte)0;
                    builder += 1;
                    builder += 0;
                    builder += 0x02;
                    builder += "A6318A0A7D294CE2A7019511FEDB7AD7";           // AuthKey
                    builder += (short)0;
                    builder += (byte)1;                                      // Char Num of all servers
                    builder += (byte)1;                                      // Server ID
                    builder += (byte)1;                                      // CharNum
                }

                client.Send(builder, "Unknown_7D6");

                URLToClient(packet, builder, client, events);
                SystemMessg(packet, builder, client, events);

                SendChannels.SendChannelList(client);

                var timer = new System.Timers.Timer(5000);
                timer.AutoReset = true;
                timer.Elapsed += (sender, e) => { SendChannels.SendChannelList(client); };

                timer.Start();
                client.Metadata["timer"] = timer;
            }
        }
    }
}