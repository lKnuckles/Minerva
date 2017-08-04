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
        public void GetSubPass(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();

            var packet = new IPCWriter(IPC.GetSubPass);

            if (!logindb.GetSubPass(id))
            {
                packet.Write(false);
            }
            else
            {
                packet.Write(true);
            }

            receiver.Send(packet);
        }

        public void SetSubPass(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            var subpw = data.ReadString();
            var question = data.ReadByte();
            var answer = data.ReadString();

            logindb.SetSubPass(id, subpw, question, answer);
        }

        public void GetSubPassQuestion(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();

            var packet = new IPCWriter(IPC.GetSubPassQuestion);
            var question = logindb.GetSubPassQuestion(id);
            packet.Write((byte)question);

            receiver.Send(packet);
        }

        public void CheckSubPassAnswer(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            string answer = data.ReadString();

            var packet = new IPCWriter(IPC.CheckSubPassAnswer);
            var status = logindb.CheckSubPwAnswer(id, answer);
            packet.Write(status);

            receiver.Send(packet);
        }

        public void CheckSubPass(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            string pass = data.ReadString();

            var packet = new IPCWriter(IPC.CheckSubPass);
            var status = logindb.CheckSubPw(id, pass);
            packet.Write(status);

            receiver.Send(packet);
        }

        public void RemoveSubPass(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();

            logindb.RemoveSubPass(id);
        }

        public void GetSubPassTime(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();

            var packet = new IPCWriter(IPC.GetSubPassTime);
            var time = logindb.GetSubPassTime(id);

            if (time == null)
                packet.Write(true);

            if (time >= DateTime.Now)
                packet.Write(true);

            if (time < DateTime.Now)
                packet.Write(false);

            receiver.Send(packet);
        }

        public void SetSubPassTime(IPCReceiver receiver, IPCReader data)
        {
            var id = data.ReadInt32();
            var time = data.ReadByte();
            logindb.SetSubPassTime(id, time);
        }
    }
}
