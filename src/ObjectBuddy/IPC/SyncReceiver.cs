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
    public class SyncReceiver
    {
        public delegate void SyncSuccessHandler(object sender, EventArgs e);

        public event SyncSuccessHandler OnSyncSuccess;

        public void SyncSuccess(object sender, EventArgs e)
        {
            if (OnSyncSuccess != null) OnSyncSuccess(sender, e);
        }

        ISocket syncClient { get; set; }
        Thread syncThread;

        string ip;
        int port;
        bool syncRunning = false;

        public SyncReceiver(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            syncThread = new Thread(Listen);
            syncThread.Start();
        }

        void Listen()
        {
            Thread.Sleep(1000);
            Log.Notice("Starting Sync with the Master Server");
            Thread.Sleep(1000);

            while (true)
            {
                if (syncClient == null && !syncRunning)
                {
                    try
                    {
                        syncClient = AweSock.TcpConnect(ip, port);
                        syncRunning = true;
                        Log.Notice("Established Sync connection to the Master Server");
                        SyncSuccess(this, new EventArgs());
                    }
                    catch (Exception)
                    {
                        syncRunning = false;
                        Log.Error("Sync with the Master Server failed!");
                    }
                }

                if (syncRunning)
                {
                    if (!syncClient.GetSocket().Connected)
                    {
                        syncClient = null;
                        syncRunning = false;
                    }
                }

                Thread.Sleep(1);
            }
        }

        public IPCWriter CreateIPC(IPC opcode)
        {
            return new IPCWriter(opcode);
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
                AweSock.SendMessage(syncClient, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool ReadIPC(AwesomeSockets.Buffer packet)
        {
            try
            {
                AweSock.ReceiveMessage(syncClient, packet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IPCReader ReadIPC()
        {
            AwesomeSockets.Buffer inBuf = AwesomeSockets.Buffer.New();

            if (!ReadIPC(inBuf))
                return null;

            var reader = new IPCReader(inBuf);

            if (reader.ReadOpcode() == null)
                return null;

            return reader;
        }
    }
}
