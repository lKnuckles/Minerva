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

using CItem = Minerva.Structures.Client.Equipment;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void MessageEvent(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
        {
            Character character = client.Metadata["fullchar"] as Character;
            var name = character.name;
            var x = (ushort)character.x;
            var y = (ushort)character.y;
            var id = character.id;
            var map = client.Metadata["map"] as IMap;
            var clients = map.GetSurroundingClients(client, 1);

            int unk = packet.ReadInt();
            packet.Skip(4);
            int size = packet.ReadShort() - 3;
            packet.Skip(2);
            var type = packet.ReadByte();
            var msg = packet.ReadString(size);

            var smsg = msg.Split(' ');

            if (msg == "/quit")
            {
                client.Disconnect();
                return;
            }

            #region DebugCommands
#if DEBUG
            if (smsg.Length > 1 && string.IsNullOrEmpty(smsg[0]) == false)
            {
                switch (smsg[0])
                {
                    case "_drop":
                        int itemid = 0;

                        if (int.TryParse(smsg[1], out itemid))
                            map.DropItem(x, y, itemid, (uint)id, (uint)id);

                        break;

                    case "_warp":
                        ushort mapid = 0;
                        ushort curx = 0;
                        ushort cury = 0;

                        if (!ushort.TryParse(smsg[1], out mapid))
                            return;

                        if (!ushort.TryParse(smsg[2], out curx))
                            return;

                        if (!ushort.TryParse(smsg[3], out cury))
                            return;

                        var p = client.CreatePacket("PC_WarpCommand", mapid, curx, cury, client);

                        events.Warped("world.Initialized", client, mapid, curx, cury);

                        client.Send(p, "PC_WarpCommand");

                        break;

                    case "_levelup":
                        ushort level = 0;

                        if (!ushort.TryParse(smsg[1], out level))
                            return;

                        builder.New(0x03F0);
                        {
                            builder += (byte)2;
                            builder += (uint)1371;
                            builder += (uint)1367;
                        }

                        client.Send(builder, "LevelUp");

                        break;

                    case "_spawn":
                        ushort mobid = 0;
                        ushort specid = 0;

                        if (!ushort.TryParse(smsg[1], out mobid))
                            return;

                        if (!ushort.TryParse(smsg[2], out specid))
                            return;

                        var p1 = client.CreatePacket("MobSpawned", mobid, specid);

                        client.Send(p1, "MobSpawned");

                        break;

                    case "/Partytime":
                        uint pid = 1337;

                        foreach (var c in clients)
                        {
                            builder.New(0xC9);
                            {
                                builder += (int)pid;
                                builder += (byte)12;

                                //b = builder.Data;
                            }

                            var timestamp = (uint)c.Metadata["timestamp"];
                            var style = (uint)c.Metadata["style"];

                            c.Send(builder, "NFY_DelUserList");

                            builder.New(0xC8);
                            {
                                builder += (short)0x3101;
                                builder += (int)pid++;
                                builder += (short)0x000D;
                                builder += (short)0x0100;
                                builder += 1;
                                builder += Environment.TickCount - (int)timestamp;
                                builder += (short)(x + 1);
                                builder += (short)y;
                                builder += (short)(x + 1);
                                builder += (short)y;
                                builder += (byte)0;
                                builder += 0;
                                builder += (short)0;
                                builder += (int)style;
                                builder += (byte)1;
                                builder += (byte)0;
                                builder += 0;
                                builder += 0;
                                builder += (byte)0;

                                var equipment = (List<CItem>)c.Metadata["equipment"];

                                builder += (byte)(equipment.Count);
                                builder += (short)0;
                                builder += (byte)0;
                                builder += 0;

                                name = "PARTY TIME!!";

                                builder += (byte)(name.Length + 1);
                                builder += name;
                                builder += (byte)0;

                                foreach (var e in equipment)
                                {
                                    builder += (short)e.ID;
                                    builder += e.Slot;
                                }

                            }

                            c.Send(builder, "NFY_NewUserList");
                        }
                        return;
                }
            }
#endif
            #endregion

            builder.New(0xD9);
            {
                builder += id;
                builder += (byte)0;
                builder += (byte)unk;
                builder += (byte)0;
                builder += (byte)(msg.Length + 3);
                builder += (byte)0;
                builder += (byte)254;
                builder += (byte)254;
                builder += type;
                builder += msg;
                builder += new byte[3];
            }

            foreach (var c in clients)
            {
                c.Send(builder, "NFY_MessageEvnt");
            }

            events.SaidLocal("world.MessageEvnt", id, name, msg);
        }
    }
}
