using System;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BitClickServer
{
    public class Client
    {
        //Upon socket connecting to server, set his log state to false so client is required to log in
        private bool _loggedIn;

        public Socket _socket { get; set; }

        public Player Player { get; set; }
        public BlockRoom CurrentRoom { get; set; }


        private int Id;
        private int playerId;
        private string userName;

        //Bind socket to client
        public Client(ref Socket socket)
        {
            _socket = socket;
            _loggedIn = false;

            //set Id to -1, states its not logged
            Id = -1;
        }

        public bool LoggedIn()
        {
            return _loggedIn;
        }

        public void LogIn(ref int id, ref int palyerid, ref Player p)
        {
            Id = id;
            playerId = palyerid;
            _loggedIn = true;

            Player = p;

            //Deserialize strings to actual objects insdie inventory
            Player.Inventory.DeserializeData();

            //Loads player equipment into memory of acutaly objects of items
            Player.Inventory.equipment.item_Real = DatabaseManager.GetMultipleItems(ref Player.Inventory.equipment.items);
            Player.CalculateStats();

            Player.Inventory.bag.item_Real = new Item[Player.Inventory.BagSize];

            Program.AddClientToLoggedUsers(this);
        }

        public void Disconnect()
        {
            //check if in room
            if (CurrentRoom != null)
            {
                CurrentRoom.RemoveClient(this);
            }
            //remove from dictionary that keeps track of loggedIn users
            Program.RemoveClientFromLoggedUsers(this);

            _loggedIn = false;
            Id = -1;

            //unload data as well
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public int GetId()
        {
            return Id;
        }

        public void SetUserName(ref string n)
        {
            userName = n;
        }

        public string GetName()
        {
            return userName;
        }

        public int GetPlayerId()
        {
            return playerId;
        }

    }
}
