using System;
using static BitClickServer.Constants;

namespace BitClickServer
{
    public static class RegisterManager
    {
        public static RegistrationCodes Register(ref string email, ref string password)
        {
            if (string.IsNullOrEmpty(email))
                return RegistrationCodes.REGISTRATION_EMAIL_BAD;

            if (password.Length < PASSWORD_MIN_LEN || password.Length > Constants.PASSWORD_MAX_LEN)
                return RegistrationCodes.REGISTRATION_PASSWORD_BAD;

            // Create a new player
            Player player = new Player();

            //Create inventory for player
            player.Inventory = new Inventory();

            // Create new account
            Account account = new Account
            {
                // Set email
                Email = email,

                // Set password to hashed password
                Password = BCrypt.Net.BCrypt.HashPassword(password),

                // Attach player
                Player = player
            };

            // Add account and save account to database
            // Return true if successfully saved to DB
            using (var db = new GameContext())
            {
                try {
                    db.Accounts.Add(account);
                    db.SaveChanges();
                } catch(Exception e) {
                    Console.WriteLine("Error saving account to DB");
                    Console.WriteLine(e.Message);

                    return RegistrationCodes.REGISTRATION_EMAIL_BAD;
                }
            }

            return RegistrationCodes.REGISTRATION_SUCCESSFUL;
        }
    }
}
