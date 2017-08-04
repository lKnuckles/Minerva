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
    public partial class PipeServer : IDatabaseContracts
    {
        public void FetchAccount(IPCReceiver receiver, IPCReader data)
        {
            var name = data.ReadString();
            var pass = data.ReadString();

            var packet = new IPCWriter(IPC.FetchAccount);

            if (!logindb.FetchAccount(name))
            {
                packet.Write(0x00);
                packet.Write((byte)0x03);
            }
            else
            {
                logindb.ReadRow();
                var id = (logindb["id"] as int?).Value;
                var _pass = logindb["password"].ToString();
                var auth = (logindb["auth"] as byte?).Value;

                if (pass != _pass)
                {
                    packet.Write(0x00);
                    packet.Write((byte)0x04);
                }
                else
                {
                    packet.Write(id);
                    packet.Write(auth);
                }
                
            }

            receiver.Send(packet);
        }

        public void UpdateIPDate(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            var ip = data.ReadString();
            var date = DateTime.FromBinary(data.ReadInt64());

            logindb.UpdateIPDate(id, ip, date);
        }

        public void VerifyPassword(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            var pass = data.ReadString();

            var packet = new IPCWriter(IPC.VerifyPassword);
            var status = logindb.VerifyPassword(id, pass);

            packet.Write(status);

            receiver.Send(packet);
        }
    }
}
