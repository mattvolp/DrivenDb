using System.Collections.Generic;
using DrivenDb.Core;

namespace DrivenDb.Data.Internal
{
   internal class DbWriter
      : IDbWriter
   {
      public int Write(string query, params object[] parameters)
      {
         throw new System.NotImplementedException();
      }

      public int Write(IDbEntity entity)
      {
         throw new System.NotImplementedException();
      }

      public int Write(IEnumerable<IDbEntity> entities)
      {
         throw new System.NotImplementedException();
      }
   }
}
