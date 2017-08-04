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
using System.Net;

#endregion

namespace Minerva
{
    // Config-loading class
    public class Configuration
    {
        public static IPAddress IP;
        public static int Port;
        public static string MasterIP;
        public static int MasterPort;

        public static void Load(string name)
        {
            var conf = new IniReader(string.Format("config/{0}.ini", name));

            IP = IPAddress.Parse(conf.GetValue("listen", "ip", ""));
            Port = conf.GetValue("listen", "port", 0);

            MasterIP = conf.GetValue("master", "ip", "localhost");
            MasterPort = conf.GetValue("master", "port", 0);
        }
    }
}