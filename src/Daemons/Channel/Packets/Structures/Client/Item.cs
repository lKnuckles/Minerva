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

using System.Runtime.InteropServices;

#endregion

namespace Minerva.Structures.Client
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Item : ClientHandlerStructure
    {
        /// <summary>Item ID, Bonus, IsBound</summary>
        public uint ID;
        /// <summary>Amount, UpgradeType</summary>
        public byte Upgrade1;
        /// <summary>Amount, UpgradeType</summary>
        public byte Upgrade2;
        /// <summary>Amount, UpgradeType</summary>
        public byte Upgrade3;
        /// <summary>Amount, UpgradeType</summary>
        public byte Upgrade4;
        /// <summary>CraftType, CraftBonus</summary>
        public byte Craft;
        /// <summary>Upgrades lsh 4</summary>
        public byte Upgrades;
        /// <summary>Unk ushort</summary>
        public ushort Unk;
        /// <summary>Position in equipment.</summary>
        public ushort Slot;
        /// <summary>Cash item expiration as UNIX timestamp.</summary>
        public uint ExpirationDate;

        public static ushort Size { get { return (ushort)Marshal.SizeOf(typeof(Item)); } }

        public dynamic ToDatabase()
        {
            var result = new Database.Item();

            return result;
        }
    }
}
