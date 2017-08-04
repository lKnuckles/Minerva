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
using System.Linq;

using DItem = Minerva.Structures.Database.Item;
using CEquipment = Minerva.Structures.Client.Equipment;

#endregion

namespace Minerva
{
    public class Equipment
    {
        public byte[] head      { get; set; }
        public byte[] body      { get; set; }
        public byte[] hands     { get; set; }
        public byte[] feet      { get; set; }
        public byte[] righthand { get; set; }
        public byte[] lefthand  { get; set; }
        public byte[] back      { get; set; }

        List<CEquipment> cequipment;

        public List<CEquipment> GetEquipment()
        {
            cequipment = new List<CEquipment>();
            if (EquipmentExist(head)) AddEquipmentArray(head, 0);
            if (EquipmentExist(body)) AddEquipmentArray(body, 1);
            if (EquipmentExist(hands)) AddEquipmentArray(hands, 2);
            if (EquipmentExist(feet)) AddEquipmentArray(feet, 3);
            if (EquipmentExist(righthand)) AddEquipmentArray(righthand, 4);
            if (EquipmentExist(lefthand)) AddEquipmentArray(lefthand, 5);
            if (EquipmentExist(back)) AddEquipmentArray(back, 6);

            return cequipment;
        }

        bool EquipmentExist(byte[] eq)
        {
            if (eq != null && eq.Length > 0 && eq.Any(b => b != 0))
                return true;

            return false;
        }
        
        void AddEquipmentArray(byte[] eq, int slot)
        {
            var de = (DItem)(eq.ToStructure<DItem>());
            var _slot = (EquipmentSlots)slot;
            cequipment.Add(de.ToClient(_slot));
        }
    }
}
