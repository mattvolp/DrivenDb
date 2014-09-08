using System;
using System.Linq;

namespace Fastlite.DrivenDb
{
   internal sealed class DbMapperFactory : IDbMapperLoader
   {
      public IDbMapper<T> Load<T>(DbRecordList<T> records)
      {
         //if(!records.Any())
         //   throw new InvalidOperationException("Empty record list cannot be mapped");

         //var record = records.First();

         return new DbMapper<T>(r =>
            {
               for (var i = 0; i < r.Values.Count; i++)
               {
                  var property = typeof (T).GetProperty(r.Names[i]);

                  property.SetMethod.Invoke(r.Entity, new object[] {Convert.ChangeType(r.Values[i], property.PropertyType)});
               }               
            });
      }
   }
}
