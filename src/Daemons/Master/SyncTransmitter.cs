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
using System.Threading;

using AwesomeSockets;

#endregion

namespace Minerva
{
    public partial class PipeServer : IDatabaseContracts
    {
        static PipeServer p;
        ISocket syncServer { get; set; }
        Thread syncThread;

        string ip;
        int port;

        public static PipeServer Instance
        {
            get
            {
                return p;
            }
        }

        public void StartSync(string ip, int port)
        {
            p = this;
            this.ip = ip;
            this.port = port;
            syncThread = new Thread(Listen);
            syncThread.Start();
        }

        void Listen()
        {
            syncServer = AweSock.TcpListen(port);
            Thread.Sleep(3000);
            Log.Notice("Master Server is listening on {0}:{1}", ip, port);

            while (true)
            {
                var client = AweSock.TcpAccept(syncServer);
                var receiver = new IPCReceiver(client);
            }
        }

        public static void ProcessPacket(IPCReceiver receiver, AwesomeSockets.Buffer buffer)
        {
            var reader = new IPCReader(buffer);
            var packet = reader.ReadOpcode();
            PipeServer p = PipeServer.Instance;

            if (packet == null)
                return;

            switch ((IPC)packet)
            {
                case IPC.FetchAccount: p.FetchAccount(receiver, reader); break;
                case IPC.UpdateIPDate: p.UpdateIPDate(receiver, reader); break;

                case IPC.AddUser: p.AddUser(receiver, reader); break;
                case IPC.GetUser: p.GetUser(receiver, reader); break;

                case IPC.ChannelList: p.GetChannels(receiver, reader); break;
                case IPC.RegisterChannel: p.AddChannel(receiver, reader); break;

                case IPC.GetCharacterList: p.GetCharacters(receiver, reader); break;
                case IPC.CreateCharacter: p.CreateCharacter(receiver, reader); break;
                case IPC.DeleteCharacter: p.DeleteCharacter(receiver, reader); break;
                case IPC.VerifyPassword: p.VerifyPassword(receiver, reader); break;
                case IPC.UpdatePosition: p.UpdateCharacterPosition(receiver, reader); break;
                case IPC.GetFullCharacter: p.GetFullCharacter(receiver, reader); break;
                case IPC.GetSlotOrder: p.GetSlotOrder(receiver, reader); break;
                case IPC.SetSlotOrder: p.SetSlotOrder(receiver, reader); break;
                case IPC.SetQuickSlots: p.SetQuickSlots(receiver, reader); break;
                case IPC.UpdateStatPoints: p.UpdateStatPoints(receiver, reader); break;
                case IPC.UpdateSkillPoints: p.UpdateSkillPoints(receiver, reader); break;

                case IPC.GetSubPass: p.GetSubPass(receiver, reader); break;
                case IPC.SetSubPass: p.SetSubPass(receiver, reader); break;
                case IPC.GetSubPassQuestion: p.GetSubPassQuestion(receiver, reader); break;
                case IPC.CheckSubPassAnswer: p.CheckSubPassAnswer(receiver, reader); break;
                case IPC.CheckSubPass: p.CheckSubPass(receiver, reader); break;
                case IPC.RemoveSubPass: p.RemoveSubPass(receiver, reader); break;
                case IPC.GetSubPassTime: p.GetSubPassTime(receiver, reader); break;
                case IPC.SetSubPassTime: p.SetSubPassTime(receiver, reader); break;
            }
        }
    }

    public class IPCReceiver
    {
        ISocket socket;
        Thread rThread;
        AwesomeSockets.Buffer recvBuffer;

        public IPCReceiver(ISocket socket)
        {
            this.socket = socket;
            recvBuffer = AwesomeSockets.Buffer.New();
            rThread = new Thread(ReceiveThread);
            rThread.Start();
        }

        public void Send(IPCWriter writer)
        {
            var packet = writer.GetRawPacket();
            AwesomeSockets.Buffer.FinalizeBuffer(packet);
            SendIPC(packet);
        }

        bool SendIPC(AwesomeSockets.Buffer packet)
        {
            try
            {
                AweSock.SendMessage(socket, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void ReceiveThread()
        {
            while (true)
            {
                AwesomeSockets.Buffer.ClearBuffer(recvBuffer);
                try
                {
                    AweSock.ReceiveMessage(socket, recvBuffer);
                    PipeServer.ProcessPacket(this, recvBuffer);
                }
                catch (Exception)
                {
                    rThread.Abort();
                }
            }
        }
    }
}