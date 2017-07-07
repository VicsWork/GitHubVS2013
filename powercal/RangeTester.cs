using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace RangeTester
{
    class RangeTester
    {

        const string _SUCCESS_RESPONSE = "STATUS 00";
        const string _PING_REGEX = @"(PING (\d*) (-\d*) (\d*) (-\d*))";

        const int _LEGACY_RTM_PORT_NUMBER = 10001;

        int _server_port = 4900;
        public int Server_Port { get { return _server_port; } set { _server_port = value; } }

        string _server_host;
        public string Server_Host { get { return _server_host; } set { _server_host = value; } }

        string _client_host;
        public string Client_Host { get { return _client_host; } set { _client_host = value; } }

        int _client_port = 4900;
        public int Client_Port { get { return _client_port; } set { _client_port = value; } }

        int _channel = 17;
        public int Channel { get { return _channel; } set { _channel = value; } }


        public RangeTester()
        {

        }
        public RangeTester(string server_host, string client_host)
        {
            Server_Host = server_host;
            Client_Host = client_host;
        }

        /// <summary>
        /// Sets filter, channel and does a ping
        /// </summary>
        /// <param name="ping_count"></param>
        /// <returns></returns>
        public RtPingResults Ping(int ping_count)
        {
            // Init Client Connection
            TcpClient clinet_tcp = new TcpClient();
            clinet_tcp.Connect(Client_Host, Client_Port);

            var clientStream = clinet_tcp.GetStream();
            var clientReader = new StreamReader(clientStream);
            var clientWriter = new StreamWriter(clientStream);

            clientStream.ReadTimeout = 125;
            clientWriter.AutoFlush = true;

            var clientFilter = (UInt32)((new Random((int)DateTime.Now.Ticks).NextDouble()) * UInt32.MaxValue);
            clientWriter.WriteLine("mfglib end");
            clientReader.ReadLine();

            clientWriter.WriteLine("mfglib rt-init {0} 0", clientFilter);
            string response = clientReader.ReadLine();
            if (response != _SUCCESS_RESPONSE)
            {
                throw new Exception("Unable to set client filter");
            }
            response = clientReader.ReadLine();
            if (response != _SUCCESS_RESPONSE)
            {
                throw new Exception("Unable to set client filter");
            }

            clientWriter.WriteLine("mfglib channel set {0}", Channel);
            response = clientReader.ReadLine();
            if (response != _SUCCESS_RESPONSE)
            {
                throw new Exception("Unable to set client channel");
            }

            var txLqi = new List<int>();
            var txRssi = new List<int>();
            var rxLqi = new List<int>();
            var rxRssi = new List<int>();

            int read_fail_count = 0;
            for (var i = 0; i < ping_count;)
            {
                clientWriter.WriteLine("mfglib rt-ping");
                Thread.Sleep(25);
                try
                {
                    response = clientReader.ReadLine();
                    read_fail_count = 0;
                    i++;
                }
                catch
                {
                    // Reader timed out
                    read_fail_count++;
                    if (read_fail_count > 5)
                        throw new Exception("Too many reader timeouts waiting for ping response");

                    continue;
                }

                if (Regex.IsMatch(response, _PING_REGEX))
                {
                    var match = Regex.Match(response, _PING_REGEX);

                    txLqi.Add(int.Parse(match.Groups[2].Value));
                    txRssi.Add(int.Parse(match.Groups[3].Value));
                    rxLqi.Add(int.Parse(match.Groups[4].Value));
                    rxRssi.Add(int.Parse(match.Groups[5].Value));
                }
                else
                {
                    throw new Exception("Invalid Ping Response: " + response);
                }
            }

            clinet_tcp.Close();

            RtPingResults results = new RtPingResults(txLqi.Average(), txRssi.Average(), rxLqi.Average(), rxRssi.Average());
            return results;

        }

        /// <summary>
        /// Initializes the RT server channel, etc
        /// 
        /// Note: The Digi X2/Lion fish combo seems to have special code
        /// to automate starting mfglib, etc.  The only command vailable is to
        /// set the channel.  So a special "legacy mode" was created to be able to use
        /// that setup.  The normal approach requires to ISA3 Adaters and server and client
        /// running the normal range test firmware.
        /// </summary>
        public void Server_init()
        {
            string response = "";

            var rtm = new TcpClient();
            rtm.Connect(Server_Host, Server_Port);
            var rtmStream = rtm.GetStream();
            var rtmReader = new StreamReader(rtmStream);
            var rtmWriter = new StreamWriter(rtmStream);
            rtmWriter.AutoFlush = true;

            // Init RTM Connection
            if(Server_Port != _LEGACY_RTM_PORT_NUMBER)
            {
                // These command will generate "ERR No such command" for the legacy rtm.  
                rtmWriter.WriteLine("mfglib end");
                response = rtmReader.ReadLine();
                rtmWriter.WriteLine("mfglib start");
                rtmWriter.WriteLine("mfglib channel set {0}", Channel);
                response = rtmReader.ReadLine();
                rtmWriter.WriteLine("mfglib setPower 3 6");
                rtmWriter.WriteLine("mfglib rt-master 1");
            }
            else
            {
                // Do this mainly to flush out any bad data and the next write works!!!
                rtmWriter.WriteLine("mfglib channel set {0}", Channel);
            }

            // Verify we can talk to server
            // Note also this seems to be the only command available in lagacy mode
            rtmWriter.WriteLine("mfglib channel set {0}", Channel);
            //Console.WriteLine("mfglib channel set {0}", Channel);

            response = rtmReader.ReadLine();
            if (response != _SUCCESS_RESPONSE)
            {
                throw new Exception("Unable to set server channel");
            }

            rtm.Close();
        }
    }

    class RtPingResults
    {
        public double TxLqi;
        public double TxRssi;
        public double RxLqi;
        public double RxRssi;

        public RtPingResults(double txLqi, double txRssi, double rxLqi, double rxRssi)
        {
            TxLqi = txLqi;
            TxRssi = txRssi;
            RxLqi = rxLqi;
            RxRssi = rxRssi;

        }
    }

}