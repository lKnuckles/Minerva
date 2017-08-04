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
using System.Net.Sockets;
using System.Threading;

#endregion

namespace Minerva
{
    class Server
    {
        TcpListener listener;
        Thread thread;

        SyncReceiver syncServer;

        HashSet<ClientHandler> clients;

        PacketHandler packets;
        EventHandler events;
        ScriptHandler scripts;

        int ticks = Environment.TickCount;
        int count = 1;

        public Server()
        {
            Console.Title = "Minerva Login Server";
            Console.CursorVisible = false;

            var start = Environment.TickCount;

            Util.Info.PrintLogo();
            Console.WriteLine();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Log.Start("Login"); // Start logging service

            Log.Message("Registering events...", Log.DefaultFG);
            clients = new HashSet<ClientHandler>();
            events = new EventHandler();
            events.OnClientDisconnect += (sender, client) => { Log.Notice("Client {0} disconnected from Login Server", client.RemoteEndPoint); clients.Remove(client); };
            events.OnError += (sender, message) => Log.Error(message);
            events.OnReceivePacket += (sender, e) => Log.Received(e.Name, e.Opcode, e.Length);
            events.OnSendPacket += (sender, e) => Log.Sent(e.Name, e.Opcode, e.Length);

            Log.Message("Compiling and registering scripts...", Log.DefaultFG);
            scripts = new ScriptHandler();
            scripts.Concatenate("Events", new string[] { "mscorlib" });
            scripts.Run("Events");
            scripts.CreateInstance("Events");
            var result = scripts.Invoke("_init_", events);

            try
            {
                Log.Message("Reading configuration...", Log.DefaultFG);
                Configuration.Load();

                Log.Message("Registering packets...", Log.DefaultFG);
                packets = new PacketHandler("login", new PacketProtocol().GetType(), events);

                RSA.GenerateKeyPair();

                listener = new TcpListener(Configuration.IP, Configuration.Port);
                thread = new Thread(Listen);
                thread.Start();

                syncServer = new SyncReceiver(Configuration.MasterIP, Configuration.MasterPort);

                Log.Notice("Minerva started in: {0} seconds", (Environment.TickCount - start) / 1000.0f);
            }
            catch (Exception e) 
            {
                Log.Error(e.Message);
                #if DEBUG
                throw e;
                #endif
            }
        }

        void Listen()
        {
            listener.Start();

            Log.Notice("Login Server listening for clients on {0}", listener.LocalEndpoint);

            while (true)
            {
                // Waits for a client to connect to the server
                var client = listener.AcceptTcpClient();

                Log.Notice("Client {0} connected to Login Server", client.Client.RemoteEndPoint);

                string[] url = { Configuration.CashURL, Configuration.CashChargeURL, Configuration.GuildURL };
                int[] ver = { Configuration.ClientVersion, Configuration.NormalMagicKey, Configuration.IgnoreClientVersion };

                var timestamp = Environment.TickCount - ticks;
                var key = ((ulong)count << 32) + (ulong)timestamp;
                var c = new ClientHandler(client, packets, events);
                c.Metadata["timestamp"] = (uint)timestamp;
                c.Metadata["count"] = (ushort)count++;
                c.Metadata["magic"] = key;
                c.Metadata["url"] = url;
                c.Metadata["version"] = ver;
                c.Metadata["syncServer"] = syncServer;
                c.Start();
                clients.Add(c);
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Fatal exception!");
            var ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Log.Error(ex.Message + "\n" + ex.StackTrace);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}