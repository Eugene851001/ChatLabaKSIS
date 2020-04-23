using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SerializeHandler
{
    public class SerializerXml: ISerialize
    {
        public byte[] Serialize(object obj, string path, Type type)
        {

            MemoryStream container = new MemoryStream();
            try
            {
                XmlSerializer formatter = new XmlSerializer(type);
                formatter.Serialize(container, obj);
            }
            finally
            {
                ;//  container.Close();
            }
            return container.ToArray();
        }

        public object Deserialize(Stream source, Type type)
        {
            XmlSerializer formatter = new XmlSerializer(type);
            object info = formatter.Deserialize(source);
            return info;
        }
    }
}
