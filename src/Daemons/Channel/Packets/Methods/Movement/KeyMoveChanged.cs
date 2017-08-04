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

using System;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void KeyMoveChanged(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var unk1 = packet.ReadLong();
            var unk2 = packet.ReadLong();
            packet.Skip(8);
            var unk3 = packet.ReadByte();

            Character character = client.Metadata["fullchar"] as Character;
            var id = character.id;
            var timestamp = (uint)client.Metadata["timestamp"];
            var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

            builder.New(0x196);
            {
                builder += id;
                builder += Environment.TickCount - (int)timestamp;
                builder += unk1;
                builder += unk2;
                builder += unk3;
            }

            foreach (var c in clients)
            {
                c.Send(builder, "NFY_KeyMoveBegined");
            }
        }
    }
}