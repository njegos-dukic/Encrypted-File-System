using System;
using System.Collections.Generic;
using System.Text;

namespace EncFS
{
    class User
    {
        public string Username { get; private set; }
        // TODO: Add digital certificate.
        
        public User(string username)
        {
            this.Username = username;
            // TODO: Initialize digital certificate.
        }
    }
}
