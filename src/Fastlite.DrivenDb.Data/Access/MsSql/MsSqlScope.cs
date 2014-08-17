using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Base;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Access.MsSql.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql
{
   internal sealed class MsSqlScope : DbScope, IMsSqlScope
   {
      private readonly IMsSqlScripter _scripter;

      internal MsSqlScope(IDb db, MsSqlAccessor accessor, IMsSqlScripter scripter)
         : base(db, accessor)
      {
         _scripter = scripter;
      }

      public void WriteEntityUsingScopeIdentity<T>(T entity) where T : IDbEntity, new()
      {
         WriteEntitiesUsingScopeIdentity(new[] { entity });
      }

      public void WriteEntitiesUsingScopeIdentity<T>(IEnumerable<T> entities) where T : IDbEntity, new()
      {
         _accessor.WriteEntities(_connection, _transaction, entities,
                       (c, i, e) => _scripter.ScriptInsertWithScopeIdentity(c, e, i, true)
                       , null
                       , null
                       , null
                       , true
            );
      }
   }
}