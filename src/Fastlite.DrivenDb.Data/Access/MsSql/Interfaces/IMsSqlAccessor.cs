using System;
using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql.Interfaces
{
   public interface IMsSqlAccessor : IDbAccessor
   {
      void WriteEntityUsingScopeIdentity<T>(T entity)
         where T : IDbEntity, new();

      void WriteEntitiesUsingScopeIdentity<T>(IEnumerable<T> entities)
         where T : IDbEntity, new();

      Tuple<T, D> WriteEntityAndOutputDeleted<T, D>(T entity, D deleted)
         where T : IDbEntity, new()
         where D : class;

      IEnumerable<Tuple<T, D>> WriteEntitiesAndOutputDeleted<T, D>(IEnumerable<T> entities, D deleted)
         where T : IDbEntity, new()
         where D : class;

      new IMsSqlScope CreateScope();
   }
}