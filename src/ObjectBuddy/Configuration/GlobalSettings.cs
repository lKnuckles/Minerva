/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://bitbucket.org/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU Generalpublic static License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Generalpublic static License for more details.

    You should have received a copy of the GNU Generalpublic static License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace Minerva
{
    public static class Global
    {
        public static  class Config
        {
            public static  string ConfigPath;
            static IniReader conf;
            public static  IPAddress IP;

            #region Master Server Region
                public static IPAddress MasterIP;
                public static int MasterPort;
                public static string LoginDB;
                public static string LoginDBIP;
                public static string LoginDBUser;
                public static string LoginDBPass;
                public static string LoginDBType;
                public static Dictionary<int, string> ServerDBs = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBIPs = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBUsers = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBPasses = new Dictionary<int, string>();
                public static Dictionary<int, string> ServerDBTypes = new Dictionary<int, string>();

                public static  void LoadMasterServer(int server)
                {
                    var section = string.Format("server{0}db", server);

                    ServerDBTypes.Add(server, conf.GetValue(section, "type", ""));
                    ServerDBs.Add(server, conf.GetValue(section, "name", ""));
                    ServerDBIPs.Add(server, conf.GetValue(section, "ip", ""));
                    ServerDBUsers.Add(server, conf.GetValue(section, "user", ""));
                    ServerDBPasses.Add(server, conf.GetValue(section, "password", ""));
                }
            #endregion

            #region Login Server Region
                public static IPAddress LoginIP;
                public static int LoginPort;
                public static int ClientVersion;
                public static int NormalMagicKey;
                public static int IgnoreClientVersion;
                public static string CashURL;
                public static string CashChargeURL;
                public static string GuildURL;
            #endregion

            #region Channel Server Region
                public static Dictionary<string, int> ChannelPort = new Dictionary<string, int>();

                public static void LoadChannel(int server, int channel)
                {
                    ChannelPort.Add(string.Format("Channel_{0}_{1}", server, channel), conf.GetValue("Channels", string.Format("Channel_{0}_{1}_port", server, channel), 0));
                }

                public static int getChannelPort(int server, int channel)
                {
                    return ChannelPort[string.Format("Channel_{0}_{1}", server, channel)];
                }
            #endregion

            #region Chat Config
                public static int ChatPort;
            #endregion

            static Config() 
            {
                    ConfigPath = "data/Config.ini";
                    conf = new IniReader(ConfigPath);
                    IP = IPAddress.Parse(conf.GetValue("Common", "ServerIp", "0"));

                    //Master Server
                    MasterIP = IPAddress.Parse(conf.GetValue("Master", "MasterIp", "127.0.0.1"));
                    MasterPort = conf.GetValue("Master", "MasterPort", 0);
                    LoginDB = conf.GetValue("logindb", "name", "");
                    LoginDBIP = conf.GetValue("logindb", "ip", "");
                    LoginDBUser = conf.GetValue("logindb", "user", "");
                    LoginDBPass = conf.GetValue("logindb", "password", "");
                    LoginDBType = conf.GetValue("logindb", "type", "");

                    //Login Server
                    LoginIP = IPAddress.Parse(conf.GetValue("Login", "LoginIP", "127.0.0.1"));
                    LoginPort = conf.GetValue("Login", "LoginPort", 0);
                    ClientVersion = conf.GetValue("Login", "client_version", 0);
                    NormalMagicKey = conf.GetValue("Login", "magic_key", 0);
                    IgnoreClientVersion = conf.GetValue("Login", "ignore_client_version", 0);
                    CashURL = conf.GetValue("Login", "cash", "http://localhost/cashshop/?v1=");
                    CashChargeURL = conf.GetValue("Login", "cash_charge", "http://localhost/cashshop/?v1=");
                    GuildURL = conf.GetValue("Login", "guild", "http://localhost/guild/?EncVal=");

                    //Channel Server
                    /* Currently Unnecessary as values added are not loaded but set by server */

                    //Chat Server
                    ChatPort = conf.GetValue("Chat", "ChatPort", 0);
            }
        }
    }
}
