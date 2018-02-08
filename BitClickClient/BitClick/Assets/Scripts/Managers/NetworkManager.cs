using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class NetworkManager : MonoBehaviour
    {
        //Debug get rid of this later
        private NetworkDebugger networkDebugger;

        private PacketHandler packetHandler;

        private void Awake()
        {
            //singleton initialization of UnityThreadHelper
            var ugly = UnityThreadHelper.Dispatcher;

            networkDebugger = GetComponent<NetworkDebugger>();
            packetHandler = GetComponent<PacketHandler>();
        }

        //Open client socket, that can both listen and send data
        private static Socket _clientSocket;

        private static byte[] _buffer = new byte[Constants.BUFFER_SIZE];

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                networkDebugger.entirePanel.SetActive(!networkDebugger.entirePanel.activeSelf);
                NetworkDebugger.isEnabled = !NetworkDebugger.isEnabled;
            }
        }

        public void Connect()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("93.142.166.126"), 50000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 50000);

            if(_clientSocket.Connected)
                _clientSocket.Close();

            _clientSocket.BeginConnect(endPoint, ConnectCallback, null);
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                _clientSocket.EndConnect(AR);
                _buffer = new byte[2048];
                //_clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                Debug.Log(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.Log(ex.Message);
            }
            finally
            {
                _buffer = new byte[Constants.BUFFER_SIZE];
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            //number of recieved bytes
            int received = _clientSocket.EndReceive(AR);

            if (received == 0)
            {
                return;
            }

            //temporary buffer, that copies last bytes it got to process them
            byte[] dataBuff = new byte[received];
            Array.Copy(_buffer, dataBuff, received);

            //Initialize packet
            Package packet = null;

            packet = PacketHandler.ParsePacket(ref dataBuff);
            try
            {
                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    

                    try
                    {
                        networkDebugger.AddPacketToHistory(false, ref dataBuff, ref packet);
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return;
            }

            //RecieveCallback is async function, means its executed on another thread, and using dispatcher we can force things to be executed on main thread


            packetHandler.HandlePacket(packet);

            //Continiue to recieve packets from servers
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        public void SendPacket(ref Package packet)
        {
            byte[] bytesToSend = PacketHandler.SerializePacket(ref packet);
            _clientSocket.Send(bytesToSend);

            //Debug stuff
            networkDebugger.AddPacketToHistory(true, ref bytesToSend, ref packet);

            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }

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
    }
}
