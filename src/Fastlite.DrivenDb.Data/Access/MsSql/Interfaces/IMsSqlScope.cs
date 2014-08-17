using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql.Interfaces
{
   public interface IMsSqlScope : IDbScope
   {
      void WriteEntityUsingScopeIdentity<T>(T entity)
         where T : IDbEntity, new();

      void WriteEntitiesUsingScopeIdentity<T>(IEnumerable<T> entities)
         where T : IDbEntity, new();
   }
}