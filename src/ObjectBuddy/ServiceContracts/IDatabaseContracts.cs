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
using System.ServiceModel;

using Character = System.Tuple<int, byte, string, uint, byte, byte, byte, System.Tuple<byte, bool, byte, byte, byte, System.DateTime>>;
using CharacterEquipment = System.Collections.Generic.List<byte[]>;
using CharacterStats = System.Tuple<ushort, ushort, ushort, ushort, ushort, ushort, ulong, System.Tuple<uint, uint, uint, uint, uint, byte, ushort, System.Tuple<ushort, byte, ushort, ushort, ulong, uint>>>;
using CharacterItems = System.Collections.Generic.List<System.Tuple<byte[], ushort, byte>>;

#endregion

namespace Minerva
{
    [ServiceContract]
    public interface IDatabaseContracts
    {
        [OperationContract]
        void HeartBeat();

        #region World

        #region Character

        [OperationContract]
        CharacterStats GetStats(int server, int charId);

        #endregion

        #region Inventory

        [OperationContract]
        void GetInventory(int server, int charId);

        [OperationContract]
        List<byte[]> GetEquipment(int server, int character);

        [OperationContract]
        void MoveItem(int server, int charId, int oldslot, int newslot);

        [OperationContract]
        void RemoveItem(int server, int charId, int slot);

        [OperationContract]
        void AddItem(int server, int charId, int slot, byte[] item, int amount);

        [OperationContract]
        Tuple<byte[], int> GetItem(int server, int charId, int slot);

        [OperationContract]
        void EquipItem(int server, int charId, int itemslot, string equipslot);

        #endregion

        #endregion
    }
}