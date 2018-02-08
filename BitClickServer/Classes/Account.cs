using System;

namespace BitClickServer
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public Account()
        {
        }
    }
}
