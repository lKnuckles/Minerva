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
        public static void GetCharacters(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var id = Authentication.GetUser(syncServer, (ulong)client.Metadata["magic"]);

            if (id <= 0)
            {
                client.Disconnect();
                return;
            }

            client.AccountID = id;

            var characters = CharacterManagement.GetCharacters(syncServer, server, id);
            var subpass = SubpassManagement.GetSubpass(syncServer, id);
            var slotorder = CharacterManagement.GetSlotOrder(syncServer, server, id);

            if (slotorder == -1)
                slotorder = 0x00123456;

            client.Metadata["slotorder"] = slotorder;

            builder.New(0x85);
            {
                if (!subpass)
                    builder += 0;   // not exist
                else
                    builder += 1;   // exists

                builder += new byte[9];
                builder += (byte)1;
                builder += 0;       // selected char id
                builder += slotorder;
                builder += 0;

                for (int i = 0; i < characters.Length; i++)
                {
                    var charId = characters[i].id;
                    var style = (uint)characters[i]._class;
                    style += (uint)(characters[i].face << 8);
                    style += (uint)(characters[i].colour << 13);
                    style += (uint)(characters[i].hair << 17);
                    style += (!characters[i].gender) ? 0 : (uint)(1 << 26);

                    TimeSpan date = (characters[i].created - new DateTime(1970, 1, 1, 0, 0, 0));

                    var eq = characters[i].equipment;
                    ushort head = (eq.head != null) ? (ushort)(BitConverter.ToUInt16(eq.head, 0) + (eq.head[0x02] * 0x2000)) : (ushort)0;
                    ushort body = (eq.body != null) ? (ushort)(BitConverter.ToUInt16(eq.body, 0) + (eq.body[0x02] * 0x2000)) : (ushort)0;
                    ushort hands = (eq.hands != null) ? (ushort)(BitConverter.ToUInt16(eq.hands, 0) + (eq.hands[0x02] * 0x2000)) : (ushort)0;
                    ushort feet = (eq.feet != null) ? (ushort)(BitConverter.ToUInt16(eq.feet, 0) + (eq.feet[0x02] * 0x2000)) : (ushort)0;
                    ushort righthand = (eq.righthand != null) ? (ushort)(BitConverter.ToUInt16(eq.righthand, 0) + (eq.righthand[0x02] * 0x2000)) : (ushort)0;
                    ushort lefthand = (eq.lefthand != null) ? (ushort)(BitConverter.ToUInt16(eq.lefthand, 0) + (eq.lefthand[0x02] * 0x2000)) : (ushort)0;
                    ushort back = (eq.back != null) ? (ushort)(BitConverter.ToUInt16(eq.back, 0) + (eq.back[0x02] * 0x2000)) : (ushort)0;

                    builder += charId;
                    builder += (long)date.TotalSeconds;     // created
                    builder += style;
                    builder += characters[i].level;
                    builder += 1;
                    builder += 0;
                    builder += 0;
                    builder += (byte)0;
                    builder += characters[i].map;
                    builder += (ushort)characters[i].x;
                    builder += (ushort)characters[i].y;

                    builder += (long)head;
                    builder += (long)body;
                    builder += (long)hands;
                    builder += (long)feet;
                    builder += (long)righthand;
                    builder += (long)lefthand;
                    builder += (long)back;

                    builder += new byte[364];

                    builder += (byte)(characters[i].name.Length + 1);
                    builder += characters[i].name;
                    builder += (short)0;
                    builder += 0;
                    builder += 0;
                }
            }

            client.Send(builder, "GetMyChartr");
        }
    }
}