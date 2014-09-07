using System;

namespace Fastlite.DrivenDb
{
   internal sealed class DbMapperFactory : IDbMapperLoader
   {
      public IDbMapper Load<T>(DbRecordSet<T> recordset)
      {
         throw new NotImplementedException();
      }
   }
}
