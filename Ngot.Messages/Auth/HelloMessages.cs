using Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngot.Messages.Auth
{
    public class HelloMessages : Message
    {
       public const uint Id = 3;
        public override uint MessageId
        {
            get { return Id; }
        }

        public string salt;
        public byte[] key;


        public HelloMessages()
        {
        }

        public HelloMessages(string salt, byte[] key)
        {
            this.salt = salt;
            this.key = key;
        }


        public override void Serialize(IDataWriter writer)
        {

            writer.WriteUTF(salt);
            writer.WriteUShort((ushort)key.Length);
            foreach (var entry in key)
            {
                writer.WriteByte(entry);
            }
        }
    }
}
