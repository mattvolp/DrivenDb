using System;
using System.Data;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Base;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Access.MsSql.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql
{
   class MsSqlScripter : DbScripter, IMsSqlScripter
   {
      // TODO: smells
      private new readonly Func<IMsSqlBuilder> _builders;

      public MsSqlScripter(IDb db, IValueJoiner joiner, Func<IMsSqlBuilder> builders) : base(db, joiner, builders)
      {
         _builders = builders;
      }

      public void ScriptInsertWithScopeIdentity<T>(IDbCommand command, T entity, int index, bool returnId)
         where T : IDbEntity, new()
      {
         var builder = _builders();
         var parameters = InitializeBuilderForInsert(builder, entity);

         AppendQuery(command, index, builder.ToInsertWithScopeIdentity(index, returnId), parameters);
      }

      public void ScriptUpdateOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new()
      {
         var builder = _builders();          
         var parameters = InitializeBuilderForUpdate(builder, entity);

         AppendQuery(command, index, builder.ToUpdateOutputDeleted(index, deleted), parameters);
      }

      public void ScriptDeleteOutputDeleted<T>(IDbCommand command, int index, T entity, string[] deleted)
         where T : IDbEntity, new()
      {
         var builder = _builders();
         var parameters = InitializeBuilderForDelete(builder, entity);

         AppendQuery(command, index, builder.ToDeleteOutputDeleted(index, deleted), parameters);
      }
   }
}
