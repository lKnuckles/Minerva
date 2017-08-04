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
using System.ServiceModel;
using System.Threading;

using Channel = System.Tuple<int, uint, short, short, short>;

#endregion

namespace Minerva
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class PipeServer : IDatabaseContracts
    {
        Dictionary<ulong, int> users;
        SortedDictionary<byte, SortedDictionary<byte, ChannelData>> channels;
        
        DatabaseHandler logindb;
        Dictionary<int, DatabaseHandler> serverdbs;

        Dictionary<int, List<int>> initialCharStats;
        Dictionary<int, List<Tuple<byte[], ushort, byte>>> initialCharItems;
        Dictionary<int, List<int[]>> initialCharSkills;
        Dictionary<int, List<int[]>> initialCharQuickslots;

        public PipeServer(Dictionary<int, List<int>> stats, Dictionary<int, List<Tuple<byte[], ushort, byte>>> items, Dictionary<int, List<int[]>> skills, Dictionary<int, List<int[]>> quickslots)
        {
            initialCharStats = stats;
            initialCharItems = items;
            initialCharSkills = skills;
            initialCharQuickslots = quickslots;

            if (initialCharItems.ContainsKey(-1))
            {
                foreach (var i in initialCharItems)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharItems[-1]);

                initialCharItems.Remove(-1);
            }

            if (initialCharSkills.ContainsKey(-1))
            {
                foreach (var i in initialCharSkills)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharSkills[-1]);

                initialCharSkills.Remove(-1);
            }

            if (initialCharQuickslots.ContainsKey(-1))
            {
                foreach (var i in initialCharQuickslots)
                    if (i.Key != -1)
                        i.Value.AddRange(initialCharQuickslots[-1]);

                initialCharQuickslots.Remove(-1);
            }
        }
        public void RunPipe()
        {
            users = new Dictionary<ulong, int>();
            channels = new SortedDictionary<byte, SortedDictionary<byte, ChannelData>>();

            //var database = new ServiceHost(this, new Uri[] { new Uri(String.Format("net.tcp://{0}:9004", Configuration.IP)) }); Ported to Global.Global.Config
            var database = new ServiceHost(this, new Uri[] { new Uri(string.Format("net.tcp://{0}:9004", Global.Config.MasterIP)) });

            database.AddServiceEndpoint(typeof(IDatabaseContracts), new NetTcpBinding(SecurityMode.None), "Database");

            database.Open();

            Log.Message("Connecting to Login Database...", Log.DefaultFG);
            //logindb = new DatabaseHandler(Configuration.LoginDBType, Configuration.LoginDBIP, Configuration.LoginDB, Configuration.LoginDBUser, Configuration.LoginDBPass); Ported to Global.Global.Config
            logindb = new DatabaseHandler(Global.Config.LoginDBType, Global.Config.LoginDBIP, Global.Config.LoginDB, Global.Config.LoginDBUser, Global.Config.LoginDBPass);

            serverdbs = new Dictionary<int, DatabaseHandler>();

            //StartSync(Configuration.IP, 9001); Ported to Global.Global.Config
            StartSync(Global.Config.MasterIP.ToString(), 9001);
        }
    }
}