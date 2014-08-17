using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   internal static class DataContracts
   {
      public static void Serialize<T>(T instance, Stream stream)
      {
         using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, new UTF8Encoding(), false))
         {
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(writer, instance);
         }
      }

      public static T Deserialize<T>(Stream stream)
      {
         T instance;

         using (var reader = XmlDictionaryReader.CreateDictionaryReader(XmlReader.Create(stream)))
         {
            var deserializer = new DataContractSerializer(typeof(T));
            instance = (T)deserializer.ReadObject(reader);
         }

         return instance;
      }
   }
}
