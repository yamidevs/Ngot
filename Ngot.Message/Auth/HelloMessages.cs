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
			get
			{
				return 3;
			}
		}
		
		public string salt;
		public IEnumerable<sbyte> key;
		
		public HelloMessages()
		{
		}

        public HelloMessages(string salt, IEnumerable<sbyte> key)
		{
			this.salt = salt;
			this.key = key;
		}
		
		public override void Serialize(IDataWriter writer)
		{
			writer.WriteUTF(salt);
			writer.WriteUShort((ushort)key.Count());
			foreach (var entry in key)
			{
				writer.WriteSByte(entry);
			}
		}
		
		public override void Deserialize(IDataReader reader)
		{
			salt = reader.ReadUTF();
			int limit = reader.ReadUShort();
			key = new sbyte[limit];
			for (int i = 0; i < limit; i++)
			{
				(key as sbyte[])[i] = reader.ReadSByte();
			}
		}
    }
}
 