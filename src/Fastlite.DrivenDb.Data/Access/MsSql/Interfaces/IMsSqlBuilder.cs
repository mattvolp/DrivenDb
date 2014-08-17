using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql.Interfaces
{
   internal interface IMsSqlBuilder : ISqlBuilder
   {
      string ToInsertWithScopeIdentity(int index, bool returnId);

      string ToUpdateOutputDeleted(int index, string[] columns);

      string ToDeleteOutputDeleted(int index, string[] columns);
   }
}