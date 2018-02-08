using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BitClickServer
{
    class Program
    {
        //Using TCP. Data flow is too small, and TCP can absolve us of some headache
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static byte[] _buffer = new byte[Constants.BUFFER_SIZE];

        //Upon loging in, add to dictionary client object that is tied to socket.
        static Dictionary<Socket, Client> _connectedClients = new Dictionary<Socket, Client>();

        //Logged "loaded" clients.
        static Dictionary<int, Socket> _loggedClients = new Dictionary<int, Socket>();

        // Dictionary with blockrooms indexed by the level of the Block in it
        public static Dictionary<uint, BlockRoom> _blockRooms = new Dictionary<uint, BlockRoom>();

        static void Main(string[] args)
        {
            SetupServer();

            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            Console.WriteLine("Receive buffer size set to " + Constants.BUFFER_SIZE + " bytes.");

            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 50000));
            _serverSocket.Listen(1000);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            Console.WriteLine("Server started successfully!");

            DebugCreateLevels();

            //Starts timer that checks rooms
            System.Timers.Timer levelManager = new System.Timers.Timer();
            levelManager.Elapsed += new System.Timers.ElapsedEventHandler(LevelManager.StartChecking);
            levelManager.Interval = Constants.LEVEL_CHECK_TIME_INTERVAL_MS;
            levelManager.Start();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);

            if (!_connectedClients.ContainsKey(socket))
            {
                _connectedClients.Add(socket, new Client(ref socket));
                Console.WriteLine("Client connected Socket hash: " + socket.GetHashCode());
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            }

            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        //Incoming data bytes from clients are handled here.
        private static void RecieveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket) AR.AsyncState;

            Client client = GetClient(ref socket);

            int recieved = 0;
            //gets number of recieved bytes
            try
            {
                recieved = socket.EndReceive(AR);
            }
            catch
            {
                recieved = 0;
            }

            //if its 0, do nothing
            if (recieved == 0)
            {
                //callback was made, but without data, usually means just flags
                CloseConnection(ref client);

                return;
            }

            //copies from global buffer into "local buffer"
            byte[] dataBuff = new byte[recieved];
            Array.Copy(_buffer, dataBuff, recieved);

            Package packet = null;

            packet = PacketHandler.ParsePacket(ref dataBuff);
            if(packet == null)
            {
                Console.WriteLine("Error in parsing of packet");
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback),
                socket);
                return;
            }

            PrintData(false, client._socket, ref packet);

            PacketHandler.HandlePacket(ref client, ref packet);

            //keep recieving on socket
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback),
                    socket);
            }
            catch
            {

            }
        }

        private static void SendCallback(IAsyncResult AR)
        {
            try
            {
                Socket clientSocket = (Socket)AR.AsyncState;
                clientSocket.EndSend(AR);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SendPacket(ref Client client, ref Package packet)
        {
            try
            {
                PrintData(true, client._socket, ref packet);

                //Serializes packet into byte array
                byte[] data = PacketHandler.SerializePacket(ref packet);
                client._socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), client._socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(ref client);
            }
        }

        //overload incase we have to "extract" client from Dictionary, its reference aswell.
        public static void SendPacket(Client client, ref Package packet)
        {
            try
            {
                PrintData(true, client._socket, ref packet);

                //Serializes packet into byte array
                byte[] data = PacketHandler.SerializePacket(ref packet);
                client._socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), client._socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CloseConnection(ref client);
            }
        }

        //TODO: move those functions somewhere else
        public static bool IsClientLogged(int id)
        {
            return _loggedClients.ContainsKey(id);
        }


        public static Client GetClient(ref Socket socket)
        {
            return _connectedClients[socket];
        }

        public static Client GetClientByid(int id)
        {
            if (_loggedClients.ContainsKey(id))
            {
                return _connectedClients[_loggedClients[id]];
            }
            return null;
        }

        public static void AddClientToLoggedUsers(Client client)
        {
            //if client id exists, and gets request from new socket, just change it
            try
            {
                Console.WriteLine(client.GetId() + " added to loggedClients Dictionary");
                _loggedClients.Add(client.GetId(), client._socket);
            }
            catch
            {
                //nada
            }
        }

        public static void RemoveClientFromLoggedUsers(Client client)
        {
            try
            {
                _loggedClients.Remove(client.GetId());
                Console.WriteLine(client.GetId() + " removed from _loggedClients Dictionary");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
 
        }
        //till here

        public static void CloseConnection(ref Client client)
        {
            Console.WriteLine(client._socket.GetHashCode() + " has disconnected!");

            //incase user is in _loggedClients, aka he logged in with credentials, remove him from dictionary
            client.Disconnect();


            //close sockets to it
            client._socket.Shutdown(SocketShutdown.Both);
            client._socket.Close();

            //remove the reference from all sockets dictionary
            _connectedClients.Remove(client._socket); 
        }

        public static BlockRoom AddToBlockRoom(ref Client client, ref uint level)
        {
            // Get level of client 
           // var level = client.Player.Level;
            if (_blockRooms[level].isAlive)
            {
                _blockRooms[level].AddClient(client);
                return _blockRooms[level];
            }
            return null;
        }

        //Helper function TODO: move this somewhere else
        public static string PrintBytes(ref byte[] byteArray)
        {
            var sb = new StringBuilder("Packet: { ");
            for (var i = 0; i < byteArray.Length; i++)
            {
                var b = byteArray[i];
                sb.Append(b.ToString("X"));
                if (i < byteArray.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

        static void DebugCreateLevels()
        {
            for (uint i = 1; i <= 70; i++)
            {
                _blockRooms.Add(i, new BlockRoom(i));
            }
        }

        public static void PrintData(bool SoR, Socket socket, ref Package packet)
        {
            if (SoR)
            {
                Console.WriteLine("Sending packet to " + socket.GetHashCode());
            }
            else
            {
                Console.WriteLine("Received packet from " + socket.GetHashCode());
            }

            Console.WriteLine("Packet type: " + packet.Type + " : " + JsonConvert.SerializeObject(packet, Constants.jsonSettings));
        }
    }
}
