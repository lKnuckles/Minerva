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
        public static void SkillToMobs(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            var skillid = packet.ReadUShort();
            var unk = packet.ReadInt();
            var unk2 = packet.ReadByte();
            var mobid = packet.ReadUShort();
            var unk3 = packet.ReadShort();
            var unk4 = packet.ReadByte();
            var dmg = packet.ReadUInt();
            var unk5 = packet.ReadByte();
            var unk6 = packet.ReadShort();
            var unk7 = packet.ReadShort();
            var unk8 = packet.ReadShort();
            var unk9 = packet.ReadShort();
            var unk10 = packet.ReadShort();
            var unk11 = packet.ReadByte();
            var unk12 = packet.ReadShort();
            var unk13 = packet.ReadShort();

#if DEBUG
            Log.Notice("" + skillid + " " + unk + " " + unk2 + " " + mobid + " " + unk3 + " " + unk4 + " " + dmg + " " + unk5 + " " + unk6 + " " + unk7 + " " + unk8 + " " + unk9 + " " + unk10 + " " + unk11 + " " + unk12 + " " + unk13 + "");
#endif

            //packet.Skip(6);
            var attack = packet.ReadUShort();

            Character character = client.Metadata["fullchar"] as Character;
            uint hp = character.stats.curhp;
            uint mp = character.stats.curmp;

            character.stats.exp += 100;
            var exp = character.stats.exp;

            /*
            builder.New(0x00AE);
            {
                builder += skillid;
                builder += (byte)1;
                builder += (uint)hp;
                builder += (uint)mp;
                builder += (ushort)0;
                builder += (ushort)1;
                builder += (uint)2;
                builder += (ushort)3;
                builder += (uint)4;
                builder += (ushort)5;
                builder += (uint)6;
                builder += (uint)7;
                builder += (uint)8;
                builder += (uint)9;
                builder += (uint)10;
                builder += (uint)11;
                builder += (uint)12;
                builder += (uint)0xFFFFFFFF;
                builder += (byte)1;
                builder += (uint)13;
                builder += (uint)14;
                builder += (byte)1;
                builder += (ushort)15;
                builder += (ushort)16;
                builder += (ushort)17;
                builder += (uint)18;
                builder += (uint)19;
                builder += (uint)20;
                builder += (uint)21;
                builder += (uint)22;
                builder += (byte)1;
            }*/

            builder.New(0x00AE);
            {
                builder += (ushort)skillid;     //skillid
                builder += hp;                  //Hp
                builder += mp;                  //Mp
                builder += (ushort)0;           //Unk
                builder += (ulong)exp;          //Exp
                builder += (int)0xFFFFFFF;      //Skill Exp
                builder += (ushort)0;           //AP
                builder += (uint)0;             //AXP
                builder += (ulong)0;            //Unk1
                builder += (ulong)0;            //Unk2
                builder += (ulong)0;            //Unk3
                builder += (uint)0xFFFFFFFF;
                builder += (byte)1;
                builder += hp;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)1;
                builder += (byte)27;
                builder += (byte)0;
                builder += (byte)1;
                builder += (byte)2;
                builder += (byte)2;
                builder += (byte)2;
                builder += (byte)1;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)0;
                builder += (byte)1;
            }

            client.Send(builder, "SkillToMobs");
        }
    }
}
