using System;
using System.Linq;

namespace BitClickServer
{
    public static class LoginManager
    {
        //passing client, so we can set it's id, so people cannot log into same account multiple times
        public static (Constants.LoginCodes, int, int, string, Player) CheckCredentials(ref string email, ref string password)
        {
            //Checking if client login credentials are good TODO
            Account account = new Account
            {
                // Set email
                Email = email,

                // Set password
                Password = password
            };

            using (var db = new GameContext())
            {
                var request = db.Accounts.SingleOrDefault(o => o.Email == account.Email);

                if (request != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(account.Password, request.Password))
                    {
                        Player player = null;
                        try
                        {
                            var p = db.Players.SingleOrDefault(o => o.Id == request.PlayerId);
                            player = p as Player;

                            //ties inventory to player
                            player.Inventory = db.Inventory.SingleOrDefault(o => o.Id == p.InventoryId);

                            //scary check, will disable it most likely
                            if (player.Inventory.CheckValidity())
                            {
                                db.SaveChanges();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        if (request.Player != null)
                        {
                            if (request.Player.Name != null)
                                return (Constants.LoginCodes.LOGIN_SUCCESSFUL, request.Id, request.PlayerId, request.Player.Name, player);
                        }

                        return (Constants.LoginCodes.LOGIN_SUCCESSFUL_MISSING_USERNAME, request.Id, request.PlayerId, "", player);
                    }
                }
            }

            //in case everything goes wrong, when this is returned do something with user
            return (Constants.LoginCodes.LOGIN_BAD, -1, -1, "", null);
        }
    }
}
