using System;
using System.Collections.Generic;
using System.Text;

namespace EncFS
{
    class User
    {
        public string Username { get; private set; }
        private string password;
        // TODO: Add digital certificate.
        
        public User(string username, string password)
        {
            this.Username = username;
            this.password = password;
            // TODO: Initialize digital certificate.
        }

    }
}
