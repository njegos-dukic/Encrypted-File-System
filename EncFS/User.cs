﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EncFS
{
    class User
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }

        public SymmetricCypher CypherType { get; private set; }
        public DgstFunction DgstType { get; private set; }
        // TODO: Add digital certificate.

        public User(string username, string passwordHash, string cypher, string dgst)
        {
            this.Username = username;
            this.PasswordHash = passwordHash;

            switch (cypher)
            {
                case "1":
                    CypherType = SymmetricCypher.DES3;
                    break;

                case "2":
                    CypherType = SymmetricCypher.AES256;
                    break;

                case "3":
                    CypherType = SymmetricCypher.RC4;
                    break;

                default:
                    return;
            }

            switch (dgst)
            {
                case "1":
                    DgstType = DgstFunction.SHA256;
                    break;

                case "2":
                    DgstType = DgstFunction.MD5;
                    break;

                case "3":
                    DgstType = DgstFunction.SHA1;
                    break;

                default:
                    return;
            }

            // TODO: Initialize digital certificate.
        }
    }
}
