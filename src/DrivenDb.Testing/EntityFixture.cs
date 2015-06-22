using System;
using System.IO;
using System.Runtime.Serialization;

namespace DrivenDb.Testing
{
   internal class EntityFixture
      : IDisposable
   {
      private readonly EntityFactory _factory;

      public EntityFixture(EntityFactory factory)
      {
         _factory = factory;
      }

      public EntityProxy CreateProxy(string typeName, params object[] parameters)
      {
         return _factory.Create(typeName, parameters);
      }

      public EntityProxy SerializeAndDeserialize(EntityProxy proxy)
      {
         var serializer = new DataContractSerializer(proxy.Instance.GetType());

         using (var stream = new MemoryStream())
         {
            serializer.WriteObject(stream, proxy.Instance);

            stream.Flush();
            stream.Position = 0;

            var deserialized = new EntityProxy(serializer.ReadObject(stream));

            return deserialized;
         }
      }

      public void Dispose()
      {
         
      }
   }
}
