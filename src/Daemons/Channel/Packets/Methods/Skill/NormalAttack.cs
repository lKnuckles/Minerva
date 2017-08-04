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
        public static void NormalAttack(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var MobId = packet.ReadUShort();
            var unk1 = packet.ReadByte();
            var unk2 = packet.ReadByte();
            var unk3 = packet.ReadByte();
            var unk4 = packet.ReadByte();

            Character character = client.Metadata["fullchar"] as Character;
            var cid = character.id;
            var server = (int)client.Metadata["server"];
            var syncServer = client.Metadata["syncServer"] as SyncReceiver;
            var slot = character.slot;

#if DEBUG
            Log.Message(string.Format("'{0}' '{1}' '{2}' '{3}' '{4}'", MobId, unk1, unk2, unk3, unk4), ConsoleColor.White, "Sent: ");
#endif

            uint lvl = character.level;
            uint atk = (lvl * character.stats.str_stat) - 17;   //Attempt dmg formula

            ushort maxhp = character.stats.maxhp;
            ushort maxmp = character.stats.maxhp;

            ushort curhp = character.stats.curhp;
            ushort curmp = character.stats.curmp;
            var map = client.Metadata["map"] as IMap;

            builder.New(0x00B0);
            {
                builder += MobId;
                builder += unk1;
                builder += unk2;
                builder += unk3;
                builder += (uint)curhp;
                builder += (uint)curmp;
                builder += (ushort)0;
                builder += (byte)0;     //Critical = 1
                builder += (ulong)0;
                builder += (uint)100;
                builder += (uint)0;     //Current Mob Life
                builder += (ushort)0;   //AP
                builder += (uint)0;     //AXP
                builder += (byte)1;     //Display dmg if value >= 1
                builder += (uint)curhp; //Hp hover (where 0/20 sp is displayed)
                builder += (uint)0;     //Dmg Reflected??
            }

            if (curhp > 0)
            {
                character.stats.curhp = curhp;
                character.stats.curmp = curmp;
            }
            else
            {
                character.stats.curhp = maxhp;
                character.stats.curmp = maxmp;

                var p = client.CreatePacket("ErrorCode", packet.Opcode, (ushort)0, (ushort)15, (ushort)map.ID);
                client.Send(p, "ErrorCode");

                builder.New(0x042B);
                {
                    builder += 0;
                    builder += 0;
                    builder += 0;
                }

                client.Send(builder, "unk2");
            }

#if DEBUG
            string notice = "";
            for (int i = 0; i < builder.Size; i++)
            {
                notice += builder.Data[i] + " ";
            }

            Log.Notice(notice);
#endif

            client.Send(builder, "NormalAttack");
        }
    }
}