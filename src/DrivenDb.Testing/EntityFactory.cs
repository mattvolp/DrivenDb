using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.Testing
{
   public class EntityFactory
   {
      private readonly Assembly _assembly;

      public EntityFactory(Assembly assembly)
      {
         _assembly = assembly;
      }

      public EntityProxy Create(string typeName, params object[] parameters)
      {
         var type = _assembly.GetTypes().Single(t => t.Name == typeName);

         Assert.IsNotNull(type);

         var instance = Activator.CreateInstance(type, parameters);
         var proxy = new EntityProxy(instance);

         return proxy;
      }
   }
}