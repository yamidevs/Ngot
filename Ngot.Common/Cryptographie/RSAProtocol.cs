using Ngot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ngot.Common.Cryptographie
{
    public class RSAProtocol
    {
        public static byte[] PublickKey;
        public int[] Key;
        public byte[] KeyBytes;
        public string Salt;
        public string Ticket;

        public RSAProtocol()
        {
            try
            {
                UTF8Encoding encod = new UTF8Encoding();
                Ticket = Randoms.RandomString(42);
                Salt = Randoms.RandomString(32);
                GenerateKey();
            }
            catch
            {
            }
        }

        private static RSACryptoServiceProvider m_RSAProvider;

        private void GenerateKey()
        {
            CspParameters csp = new CspParameters();
            csp.ProviderType = 1;
            csp.KeyNumber = 1;
            m_RSAProvider = new RSACryptoServiceProvider(1024, csp);
            KeyBytes = AsnKeyBuilder.PublicKeyToX509(m_RSAProvider.ExportParameters(false)).GetBytes();
            PublickKey = KeyBytes;
        }
        public static string DecryptCredentials(byte[] credentials)
        {
            try
            {
                return Encoding.Default.GetString(m_RSAProvider.Decrypt(credentials, false)).Substring(32);
            }
            catch
            {
                return null;
            }
        }
    }
}
