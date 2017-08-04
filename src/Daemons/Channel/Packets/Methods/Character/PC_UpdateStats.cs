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
    partial class PacketProtocol
    {
        public static PacketBuilder PC_UpdateStats(object[] args)
        {
            var client = (ClientHandler)args[0];
            var type = (byte)args[1];
            var diff = (ushort)args[2];

            Character character = client.Metadata["fullchar"] as Character;


            var hp = character.stats.curhp;
            var mp = character.stats.curmp;

            var p = new PacketBuilder();

            p.New(0x11F);
            {
                p += (int)type; // Sub Opcode (3 = hp, 4 = mp)
                p += new byte[11];

                if (type == 3)
                {
                    p += (short)0;
                    //p += (short)diff;    // Damaged HP
                    p += (short)hp;     // Current HP
                }
                else if (type == 4)
                {
#if DEBUG
                    p += (short)0;  //In debug only to test validity
#endif
                    p += (int)mp;   // Current MP
                }

                p += 0;

                return p;
            }
        }
    }
}