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
using System.Collections.Generic;

using CharacterStats = System.Tuple<ushort, ushort, ushort, ushort, ushort, ushort, ulong, System.Tuple<uint, uint, uint, uint, uint, byte, ushort, System.Tuple<ushort, byte, ushort, ushort, ulong, uint>>>;

#endregion

namespace Minerva
{
    public partial class PipeServer : IDatabaseContracts
    {
        public void GetCharacters(IPCReceiver receiver, IPCReader data)
        {
            var serverId = data.ReadByte();
            var account = data.ReadInt32();

            var charNum = serverdbs[serverId].GetCharactersCount(account);

            var packet = new IPCWriter(IPC.GetCharacterList);
            packet.Write(charNum);

            serverdbs[serverId].GetCharacters(account);

            while (serverdbs[serverId].ReadRow())
            {
                packet.Write((serverdbs[serverId]["id"] as int?).Value);
                packet.Write((serverdbs[serverId]["slot"] as byte?).Value);
                packet.Write(serverdbs[serverId]["name"] as string);
                packet.Write((serverdbs[serverId]["level"] as uint?).Value);
                packet.Write((serverdbs[serverId]["class"] as byte?).Value);
                packet.Write((serverdbs[serverId]["face"] as byte?).Value);
                packet.Write((serverdbs[serverId]["hair"] as byte?).Value);
                packet.Write((serverdbs[serverId]["colour"] as byte?).Value);
                packet.Write((serverdbs[serverId]["gender"] as bool?).Value);
                packet.Write((serverdbs[serverId]["map"] as byte?).Value);
                packet.Write((serverdbs[serverId]["x"] as byte?).Value);
                packet.Write((serverdbs[serverId]["y"] as byte?).Value);
                packet.Write((serverdbs[serverId]["created"] as DateTime?).Value.ToBinary());

                packet.Write(serverdbs[serverId]["head"] as byte[]);
                packet.Write(serverdbs[serverId]["body"] as byte[]);
                packet.Write(serverdbs[serverId]["hands"] as byte[]);
                packet.Write(serverdbs[serverId]["feet"] as byte[]);
                packet.Write(serverdbs[serverId]["righthand"] as byte[]);
                packet.Write(serverdbs[serverId]["lefthand"] as byte[]);
                packet.Write(serverdbs[serverId]["back"] as byte[]);
            }

            receiver.Send(packet);
        }

        public CharacterStats GetStats(int server, int charId)
        {
            serverdbs[server].GetStats(charId);
            serverdbs[server].ReadRow();

            return new CharacterStats((serverdbs[server]["curhp"] as ushort?).Value,
                (serverdbs[server]["maxhp"] as ushort?).Value,
                (serverdbs[server]["curmp"] as ushort?).Value,
                (serverdbs[server]["maxmp"] as ushort?).Value,
                (serverdbs[server]["cursp"] as ushort?).Value,
                (serverdbs[server]["maxsp"] as ushort?).Value,
                (serverdbs[server]["exp"] as ulong?).Value,
                new Tuple<uint, uint, uint, uint, uint, byte, ushort, Tuple<ushort, byte, ushort, ushort, ulong, uint>>((serverdbs[server]["str_stat"] as uint?).Value,
                    (serverdbs[server]["int_stat"] as uint?).Value,
                    (serverdbs[server]["dex_stat"] as uint?).Value,
                    (serverdbs[server]["honour"] as uint?).Value,
                    (serverdbs[server]["rank"] as uint?).Value,
                    (serverdbs[server]["swordrank"] as byte?).Value,
                    (serverdbs[server]["swordxp"] as ushort?).Value,
                    Tuple.Create((serverdbs[server]["swordpoints"] as ushort?).Value,
                        (serverdbs[server]["magicrank"] as byte?).Value,
                        (serverdbs[server]["magicxp"] as ushort?).Value,
                        (serverdbs[server]["magicpoints"] as ushort?).Value,
                        (serverdbs[server]["alz"] as ulong?).Value,
                        (serverdbs[server]["pnt_stat"] as uint?).Value
                    )
                )
            );

        }

        public void CreateCharacter(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();
            var name = data.ReadString();
            var _class = data.ReadByte();
            var gender = data.ReadBoolean();
            var face = data.ReadByte();
            var hair = data.ReadByte();
            var colour = data.ReadByte();

            var stats = initialCharStats[_class];
            var items = initialCharItems[_class];
            var skills = initialCharSkills[_class];
            var quickslots = initialCharQuickslots[_class];

            var status = serverdbs[server].CreateCharacter(id, slot, name, _class, gender, face, hair, colour, stats.ToArray(), items, skills, quickslots);

            var packet = new IPCWriter(IPC.CreateCharacter);
            packet.Write((byte)status);

            receiver.Send(packet);
        }

        public void DeleteCharacter(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();

            var packet = new IPCWriter(IPC.DeleteCharacter);
            var status = serverdbs[server].DeleteCharacter(id, slot);

            packet.Write(status);

            receiver.Send(packet);
        }

        public void GetFullCharacter(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();

            var packet = new IPCWriter(IPC.GetFullCharacter);

            serverdbs[server].GetCharacter(id, slot);
            serverdbs[server].ReadRow();

            var charId = (serverdbs[server]["id"] as int?).Value;

            packet.Write(charId);
            packet.Write((serverdbs[server]["slot"] as byte?).Value);
            packet.Write(serverdbs[server]["name"] as string);
            packet.Write((serverdbs[server]["level"] as uint?).Value);
            packet.Write((serverdbs[server]["class"] as byte?).Value);
            packet.Write((serverdbs[server]["face"] as byte?).Value);
            packet.Write((serverdbs[server]["hair"] as byte?).Value);
            packet.Write((serverdbs[server]["colour"] as byte?).Value);
            packet.Write((serverdbs[server]["gender"] as bool?).Value);
            packet.Write((serverdbs[server]["map"] as byte?).Value);
            packet.Write((serverdbs[server]["x"] as byte?).Value);
            packet.Write((serverdbs[server]["y"] as byte?).Value);
            packet.Write((serverdbs[server]["created"] as DateTime?).Value.ToBinary());

            packet.Write(serverdbs[server]["head"] as byte[]);
            packet.Write(serverdbs[server]["body"] as byte[]);
            packet.Write(serverdbs[server]["hands"] as byte[]);
            packet.Write(serverdbs[server]["feet"] as byte[]);
            packet.Write(serverdbs[server]["righthand"] as byte[]);
            packet.Write(serverdbs[server]["lefthand"] as byte[]);
            packet.Write(serverdbs[server]["back"] as byte[]);

            serverdbs[server].GetStats(charId);
            serverdbs[server].ReadRow();

            packet.Write((serverdbs[server]["curhp"] as ushort?).Value);
            packet.Write((serverdbs[server]["maxhp"] as ushort?).Value);
            packet.Write((serverdbs[server]["curmp"] as ushort?).Value);
            packet.Write((serverdbs[server]["maxmp"] as ushort?).Value);
            packet.Write((serverdbs[server]["cursp"] as ushort?).Value);
            packet.Write((serverdbs[server]["maxsp"] as ushort?).Value);
            packet.Write((serverdbs[server]["exp"] as ulong?).Value);
            packet.Write((serverdbs[server]["alz"] as ulong?).Value);

            packet.Write((serverdbs[server]["str_stat"] as uint?).Value);
            packet.Write((serverdbs[server]["int_stat"] as uint?).Value);
            packet.Write((serverdbs[server]["dex_stat"] as uint?).Value);
            packet.Write((serverdbs[server]["pnt_stat"] as uint?).Value);

            packet.Write((serverdbs[server]["honour"] as uint?).Value);
            packet.Write((serverdbs[server]["rank"] as uint?).Value);
            packet.Write((serverdbs[server]["swordrank"] as byte?).Value);
            packet.Write((serverdbs[server]["swordxp"] as ushort?).Value);
            packet.Write((serverdbs[server]["swordpoints"] as ushort?).Value);
            packet.Write((serverdbs[server]["magicrank"] as byte?).Value);
            packet.Write((serverdbs[server]["magicxp"] as ushort?).Value);
            packet.Write((serverdbs[server]["magicpoints"] as ushort?).Value);

            var itemCount = serverdbs[server].GetItemCount(charId);
            serverdbs[server].GetInventory(charId);

            packet.Write(itemCount);

            while (serverdbs[server].ReadRow())
            {
                packet.Write((byte[])serverdbs[server]["item"] as byte[]);
                packet.Write((serverdbs[server]["amount"] as ushort?).Value);
                packet.Write((serverdbs[server]["slot"] as byte?).Value);
            }

            var skillCount = serverdbs[server].GetSkillCount(charId);
            serverdbs[server].GetSkills(charId);

            packet.Write(skillCount);

            while (serverdbs[server].ReadRow())
            {
                packet.Write((serverdbs[server]["skill"] as ushort?).Value);
                packet.Write((serverdbs[server]["level"] as byte?).Value);
                packet.Write((serverdbs[server]["slot"] as byte?).Value);
            }

            var quickSlotCount = serverdbs[server].GetQuickSlotCount(charId);
            serverdbs[server].GetQuickslots(charId);

            packet.Write(quickSlotCount);

            while (serverdbs[server].ReadRow())
            {
                packet.Write((serverdbs[server]["skill"] as byte?).Value);
                packet.Write((serverdbs[server]["slot"] as byte?).Value);
            }

            receiver.Send(packet);
        }

        public void UpdateCharacterPosition(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var slot = data.ReadByte();
            var map = data.ReadByte();
            var x = data.ReadByte();
            var y = data.ReadByte();
            serverdbs[server].UpdateCharacterPosition(id, slot, map, x, y);
        }

        public void SetQuickSlots(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var quickslot = data.ReadInt32();
            var position = data.ReadInt32();
            serverdbs[server].SetQuickSlots(id, quickslot, position);
        }

        public void SetSlotOrder(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var order = data.ReadInt32();
            serverdbs[server].SetSlotOrder(id, order);
        }

        public void GetSlotOrder(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();

            var packet = new IPCWriter(IPC.GetSlotOrder);
            var order = serverdbs[server].GetSlotOrder(id);

            packet.Write(order);

            receiver.Send(packet);
        }

        public void UpdateStatPoints(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var str = data.ReadInt32();
            var dex = data.ReadInt32();
            var intel = data.ReadInt32();
            var pnt = data.ReadInt32();

            serverdbs[server].UpdateStatPoints(id, str, dex, intel, pnt);
        }

        public void UpdateSkillPoints(IPCReceiver receiver, IPCReader data)
        {
            var server = data.ReadByte();
            var id = data.ReadInt32();
            var skillid = data.ReadUInt16();
            var level = data.ReadUInt16();
            var slot = data.ReadByte();

            serverdbs[server].UpdateSkillPoints(id, skillid, level, slot);
        }
    }
}
