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

        Dictionary<ulong, ClientHandler> clients;
        Dictionary<int, Map> maps;

        PacketHandler packets;
        EventHandler events;
        //ScriptHandler scripts;       

        int ticks = Environment.TickCount;
        int count = 1;

        int server, channel;

        MapLoader mapLoader;

        public Server(int server, int channel)
        {
            this.server = server;
            this.channel = channel;

            Console.Title = "Minerva Channel Server";
            Console.CursorVisible = false;

            int start = Environment.TickCount;

            Util.Info.PrintLogo();
            Console.WriteLine();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;


            Log.Start(string.Format("Channel_{0}_{1}", server, channel));    // Start logging service

            clients = new Dictionary<ulong, ClientHandler>();
            events = new EventHandler();
            events.OnClientDisconnect += (sender, client) => { Log.Notice("Client {0} disconnected from Channel Server", client.RemoteEndPoint); clients.Remove((ulong)client.Metadata["magic"]); };
            events.OnError += (sender, message) => { Log.Error(message); };
            events.OnReceivePacket += (sender, e) => { Log.Received(e.Name, e.Opcode, e.Length); };
            events.OnSendPacket += (sender, e) => { Log.Sent(e.Name, e.Opcode, e.Length); };
            events.OnWarp += (sender, client, map, x, y) => { client.Metadata["map"] = maps[map]; maps[map].MoveClient(client, x / 16, y / 16); maps[map].UpdateCells(client); };

            /*Console.WriteLine("Compiling and registering scripts...");
            scripts = new ScriptHandler();
            scripts.Concatenate("Events", new string[] { "mscorlib" });
            scripts.Run("Events");
            scripts.CreateInstance("Events");
            dynamic result = scripts.Invoke("_init_", events);*/

            try
            {
                Log.Message("Reading configuration...", Log.DefaultFG);
                //Configuration.Load(String.Format("Channel_{0}_{1}", server, channel)); Ported to Global.Global.Config)
                Global.Config.LoadChannel(server, channel);

                mapLoader = new MapLoader();
                maps = mapLoader.LoadMaps();

                Log.Message("Registering packets...", Log.DefaultFG);
                packets = new PacketHandler("world", new PacketProtocol().GetType(), events);

                //listener = new TcpListener(System.Net.IPAddress.Any, Configuration.Port); Ported to Global.Global.Config
                listener = new TcpListener(System.Net.IPAddress.Any, Global.Config.getChannelPort(server, channel));
                thread = new Thread(Listen);
                thread.Start();

                //syncServer = new SyncReceiver(Configuration.MasterIP, Configuration.MasterPort); Ported to Global.Global.Config
                syncServer = new SyncReceiver(Global.Config.MasterIP.ToString(), Global.Config.MasterPort);
                syncServer.OnSyncSuccess += (sender, e) => 
                {
                    //var aa = Configuration.IP.GetAddressBytes(); Ported to Global.Global.Config
                    var aa = Global.Config.IP.GetAddressBytes();
                    var address = BitConverter.ToUInt32(aa, 0);
                    //Authentication.RegisterChannel(syncServer, server, channel, 0, address, Configuration.Port, 100); Ported to Global.Global.Config
                    Authentication.RegisterChannel(syncServer, server, channel, 0, address, Global.Config.getChannelPort(server,channel), 100);
                };

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

            Log.Notice("Channel Server listening for clients on {0}", listener.LocalEndpoint);

            while (true)
            {
                // blocks until a client has connected to the server
                TcpClient client = this.listener.AcceptTcpClient();

                Log.Notice("Client {0} connected to Channel Server", client.Client.RemoteEndPoint);

                int timestamp = Environment.TickCount - ticks;
                ulong key = ((ulong)count << 32) + (ulong)timestamp;
                var c = new ClientHandler(client, packets, events);
                c.Metadata["timestamp"] = (uint)timestamp;
                c.Metadata["count"] = (ushort)count++;
                c.Metadata["magic"] = key;
                c.Metadata["clients"] = clients;
                c.Metadata["server"] = server;
                c.Metadata["channel"] = channel;
                c.Metadata["syncServer"] = syncServer;
                c.Metadata["chServer"] = this;
                c.Start();

                
                if(clients.Count>0) {
                    ClientHandler[] clientarr = new ClientHandler[clients.Values.Count];
                    clients.Values.CopyTo(clientarr,0);
                    ClientHandler d = clientarr[clientarr.Length-1];
                    var oldVal = d.RemoteEndPoint as System.Net.IPEndPoint;
                    var newVal = c.RemoteEndPoint as System.Net.IPEndPoint;
                    if (!oldVal.Address.Equals(newVal.Address))
                        clients.Add(key, c);
                }else
                {
                    clients.Add(key,c);
                }

                /*
                foreach (var item in clients.Values)
                {
                    bool alive = (bool)item.Metadata["alive"];
                    if (!alive)
                    {
                        item.Disconnect();
                    }
                }*/
            }
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Fatal exception!");
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Log.Error(ex.Message + "\n" + ex.StackTrace);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}