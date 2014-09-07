using System.Data.SQLite;

namespace Fastlite.DrivenDb.Tests.SqLite.Infrastructure
{
   public sealed class SqLiteAccessor : DbAccessor
   {
      public SqLiteAccessor(string cstring)
         : base(new DbMapperCache(new DbMapperFactory()), () => new SQLiteConnection(cstring))
      {
         
      }
   }
}
