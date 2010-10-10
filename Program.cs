using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Settings = Cod4RconConsoleTool.Properties.Settings;
using System.Threading;
namespace Cod4RconConsoleTool
{
    class Program
    {
        public static IPAddress IP;
        public static int Port;
        public static string RconPassword;
        public static string RconCommand;
        static void Main(string[] args)
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.Default.IP.ToString()))
                    IP = IPAddress.Parse(Settings.Default.IP);
                if (!string.IsNullOrEmpty(Settings.Default.Port.ToString()))
                    Port = Settings.Default.Port;
                if (!string.IsNullOrEmpty(Settings.Default.RconPassword))
                    RconPassword = Settings.Default.RconPassword;
                if (!string.IsNullOrEmpty(Settings.Default.RconCommand))
                    RconCommand = Settings.Default.RconCommand;
                if (IP != null && Port != 0 && RconPassword != null && RconCommand != null)
                {
                    Console.WriteLine("Would you like to use your previous information?\nAs Follows:\n{0}\n{1}\n{2}\nPlease enter yes or no",IP,Port,RconPassword);
                    if (Console.ReadLine().ToLower() == "yes")
                    {
                        if (!string.IsNullOrEmpty(RconCommand))
                        {
                            Console.WriteLine("Would you like to run the last used command again?\n{0} - yes or no",RconCommand);
                            if(Console.ReadLine().ToLower() == "yes")
                            {
                                Console.WriteLine("Executing previous command...");
                                Console.WriteLine(Rcon(IP, Port, RconPassword, RconCommand));
                            }
                            else
                            {
                                Console.WriteLine("Enter command to execute");
                                RconCommand = Console.ReadLine();
                                Console.WriteLine(Rcon(IP, Port, RconPassword, RconCommand));
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Enter IP");
                    IP = IPAddress.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Port");
                    Port = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter RCon Password");
                    RconPassword = Console.ReadLine();
                    Console.WriteLine("Enter RCon Command To Run");
                    RconCommand = Console.ReadLine();
                    Console.WriteLine("Connecting..");
                    Console.WriteLine(Rcon(IP, Port, RconPassword, RconCommand));
                }
                    Console.WriteLine("Command Executed, you are now at an rcon terminal\n(in english you can now just type commands to be sent directly do not put rcon at the start)\nIt will be added automaticlly for you.\nTo close the program type exitrcon");
                var input = "";
                    while (input.ToLower() != "exitrcon")
                    {
                        input = Console.ReadLine();
                        if (input == "exitrcon")
                            return;
                        RconCommand = input;
                        Console.WriteLine(Rcon(IP, Port, RconPassword, input));
                    }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error value was empty, please try again!\nPress enter to start again...");
                Console.ReadLine();
                Main(null);
            }
            finally
            {
                Settings.Default.IP = IP.ToString();
                Settings.Default.Port = Port;
                Settings.Default.RconPassword = RconPassword;
                Settings.Default.RconCommand = RconCommand;
                Settings.Default.Save();
                Console.WriteLine("Program finished,press enter to exit or enter r to start from the beginning again");
                var reply = Console.ReadLine();
                if (reply.ToLower() == "r")
                {
                    Main(null);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
        public static string Rcon(IPAddress gameServerIP, int gameServerPort, string password, string rconCommand)
        {

            Socket Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Connect(gameServerIP, gameServerPort);

            string command = "rcon " + password + " " + rconCommand;
            Byte[] bufferTemp = Encoding.ASCII.GetBytes(command);
            Byte[] bufferSend = new Byte[bufferTemp.Length + 5];


            // Send the standard 5 characters before the messageessage, prevents disconnect
            bufferSend[0] = Byte.Parse("255");
            bufferSend[1] = Byte.Parse("255");
            bufferSend[2] = Byte.Parse("255");
            bufferSend[3] = Byte.Parse("255");
            bufferSend[4] = Byte.Parse("02");

            int j = 5;

            for (int i = 0; i < bufferTemp.Length; i++)
            {
                bufferSend[j] = bufferTemp[i];
                j++;
            }


            //IPEndPoint remoteIpEndPoint IPEndPoint(IPAddress.Any, 0);
            Client.Send(bufferSend, SocketFlags.None);
            string result = "";
            Thread.Sleep(50);
            while (Client.Available > 0)
            {
                // Use a large recieve buffer to make sure we can take the response
                Byte[] bufferRec = new Byte[Client.Available];
                Client.Receive(bufferRec);
                result = result + Encoding.ASCII.GetString(bufferRec).Replace("????print","").Replace("\0", ""); // Remove whitespace
            }
            return result;

        }
    }
}
