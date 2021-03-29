namespace EncryptedFileSystem
{
    class User
    {
        public string Username { get; private set; }

        public SymmetricCypher CypherType { get; set; }

        public HashFunction HashType { get; set; }

        public User(string username, string passwordHash, string cypher, string hash)
        {
            this.Username = username;

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

            switch (hash)
            {
                case "1":
                    HashType = HashFunction.SHA256;
                    break;

                case "2":
                    HashType = HashFunction.MD5;
                    break;

                case "3":
                    HashType = HashFunction.SHA1;
                    break;

                default:
                    return;
            }
        }

        public void SetCypherAndHash(string cypher, string hash)
        {
            switch (cypher.Trim())
            {
                case "DES3":
                    CypherType = SymmetricCypher.DES3;
                    break;

                case "AES256":
                    CypherType = SymmetricCypher.AES256;
                    break;

                case "RC4":
                    CypherType = SymmetricCypher.RC4;
                    break;

                default:
                    break;
            }

            switch (hash.Trim())
            {
                case "SHA256":
                    HashType = HashFunction.SHA256;
                    break;

                case "MD5":
                    HashType = HashFunction.MD5;
                    break;

                case "SHA1":
                    HashType = HashFunction.SHA1;
                    break;

                default:
                    return;
            }
        }
    }
}
