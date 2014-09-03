using System.Data.SQLite;

namespace Fastlite.DrivenDb.Tests.SqLite.Infrastructure
{
   public sealed class SqLiteAccessor : DbAccessor2
   {
      public SqLiteAccessor(string cstring)
         : base(DbAccessorType2.SqLite, () => new SQLiteConnection(cstring))
      {
         
      }
   }
}
