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
using System.Threading;
using System.IO;

#endregion

namespace Minerva
{
    public class Log
    {
        static string Server = "none";

        // The default background colour to use when outputting text to the console.
        public static ConsoleColor DefaultBG = ConsoleColor.Black;
        // The default foreground colour to use when outputting text to the console.
        public static ConsoleColor DefaultFG = ConsoleColor.DarkGreen;
        // The colour to use when displaying an error message in the console.
        static ConsoleColor error = ConsoleColor.Red;
        // The colour to use when displaying a warning message in the console.
        static ConsoleColor warning = ConsoleColor.Yellow;
        // The colour to use when displaying a notice message in the console.
        static ConsoleColor notice = ConsoleColor.Green;

        static Queue<LogMessage> messages = new Queue<LogMessage>();
        static bool stopped = false;

        static void Listen()
        {
            while (!stopped)
            {
                if (messages.Count > 0)
                {
                    lock (messages)
                    {
                        Message(messages.Dequeue());
                    }
                }
                else
                {
                    Thread.Sleep(150);
                }
            }
        }

        static void Message(LogMessage m)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMsg = "";
            string message = m.Message;
            ConsoleColor colour = m.Colour;
            string tag = m.Tag;

            Console.Write("[{0}] ", time);

            if (tag != "")
            {
                bool bright = !((int)colour < 8);
                Console.BackgroundColor = !bright ? (ConsoleColor)((int)colour | 8) : (ConsoleColor)((int)colour ^ 8);
                Console.ForegroundColor = !bright ? ConsoleColor.Black : ConsoleColor.White;
                Console.Write(" ##{0}## ", tag);
                logMsg = string.Format("##{0}## ", tag);
            }

            Console.BackgroundColor = DefaultBG;
            Console.ForegroundColor = colour;

            if (tag != "")
            {
                Console.Write(" {0}\n", message);
                logMsg = string.Format("[{0}] {1}{2}", time, logMsg, message);
            }
            else
            {
                Console.Write("{0}\n", message);
                logMsg = string.Format("[{0}] {1}", time, message);
            }

            Console.ForegroundColor = DefaultFG;
            WriteFile(Server, logMsg);
        }

        public static void Received(string type, int opcode, int size)
        {
            Message(string.Format("Received packet: CSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG);
        }

        public static void Sent(string type, int opcode, int size)
        {
            Message(string.Format("Sent packet: SSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG);
        }

        public static void Error(string message)
        {
            Message(message, error, "ERROR");
        }

        public static void Error(string format, params object[] arg)
        {
            Error(string.Format(format, arg));
        }

        public static void Notice(string message)
        {
            Message(message, notice, "NOTICE");
        }

        public static void Notice(string format, params object[] arg)
        {
            Notice(string.Format(format, arg));
        }

        public static void Warning(string message)
        {
            Message(message, warning, "WARNING");
        }

        public static void Warning(string format, params object[] arg)
        {
            Warning(string.Format(format, arg));
        }

        public static void Message(string message, ConsoleColor colour, string tag = "")
        {
            lock (messages)
            {
                messages.Enqueue(new LogMessage(message, colour, tag));
            }
        }

        public static void Start(string server)
        {
            Console.ForegroundColor = DefaultFG;
            Server = server;
            Message("Starting Log Service...", Log.DefaultFG);
            var t = new Thread(new ThreadStart(Listen));
            t.Start();
        }

        public static void Stop()
        {
            stopped = true;
        }

        static void WriteFile(string server, string args)
        {
            string address = "logs/" + server + ".log";
            StreamWriter logFile = null;

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            if (!File.Exists(address))
                logFile = new StreamWriter(address);
            else
                logFile = File.AppendText(address);

            logFile.WriteLine(args);
            logFile.Close();
            logFile.Dispose();
        }
    }

    public struct LogMessage
    {
        public string Message;
        public ConsoleColor Colour;
        public string Tag;

        public LogMessage(string message, ConsoleColor colour, string tag = "")
        {
            Message = message;
            Colour = colour;
            Tag = tag;
        }
    }
}