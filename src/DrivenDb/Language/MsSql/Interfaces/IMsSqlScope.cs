using System.Collections.Generic;

namespace DrivenDb.MsSql
{
   public interface IMsSqlScope : IDbScope
   {
      void WriteEntityUsingScopeIdentity<T>(T entity)
         where T : IDbEntity, new();

      void WriteEntitiesUsingScopeIdentity<T>(IEnumerable<T> entities)
         where T : IDbEntity, new();
   }
}