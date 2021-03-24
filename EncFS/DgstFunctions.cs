using System;
using System.Collections.Generic;
using System.Text;

namespace EncFS
{
    class DgstFunctions
    {
        public static string HashPassword(string password)
        {
            return Utils.ExecutePowerShellCommand("openssl passwd -6 -salt ST " + password).Trim();
        }
    }

    enum DgstFunction
    {
        SHA256,
        MD5,
        BLAKE
    }
}
