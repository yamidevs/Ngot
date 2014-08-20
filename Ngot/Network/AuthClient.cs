using Common.IO;
using Ngot.Common;
using Ngot.Common.Cryptographie;
using Ngot.Core;
using Ngot.Messages;
using Ngot.Messages.Auth;
using Ngot.Utils;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ngot.Network
{
    public class AuthClient : Client
    {
        private readonly RSACryptoServiceProvider m_rsaProvider = new RSACryptoServiceProvider();

       
       /* public Processor Processor
        {
            get;
            set;
        }*/


        public AuthClient(Socket socket, Server server)
            : base(socket, server)
        {
            this.HelloClient();
         //   Processor = new Processor(this);
          //  this.Processors = Processor;
        }

        public void HelloClient()
        {
           // Send(new ProtocolRequired(1457, 1457));
            Send(new HelloMessages(Randoms.RandomString(32), GenerateRSAPublicKey()));
        }

        private sbyte[] GenerateRSAPublicKey()
        {
            var exportParameters = m_rsaProvider.ExportParameters(false);
            var keyParameters = new RsaKeyParameters(false, new BigInteger(1, exportParameters.Modulus), new BigInteger(1, exportParameters.Exponent));

            var stringBuilder = new StringBuilder();
            var writer = new PemWriter(new StringWriter(stringBuilder));
            writer.WriteObject(keyParameters);

            string key = stringBuilder.ToString();

            string partial = key.Remove(key.IndexOf("-----END PUBLIC KEY-----")).Remove(0, "-----BEGIN PUBLIC KEY-----\n".Length);

            return Convert.FromBase64String(partial).Select(entry => (sbyte)entry).ToArray();
        }

        public override bool DataArriavls(BufferSegment data)
        {
        
            return true;
           /* if (data.Length == 0)
            {
                return false;
            }
            foreach (var packet in Encoding.UTF8.GetString(data.SegmentData).Replace("\x0a", "").Split('\x00').Where(x => x != ""))
            {
                Console.WriteLine("PACKET  : " + packet);
            //    Processor.Parser(packet);
            }
            return true;*/
        }

        public void Send(Message Messages)
        {
            base.send(Messages);
        }


    }
}
