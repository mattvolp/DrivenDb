using System.Data.SQLite;
using Fastlite.DrivenDb.Data._2._0;
using Fastlite.DrivenDb.Data._2_0;

namespace Fastlite.DrivenDb.Data.Tests._2._0.SqLite.Infrastructure
{
   public sealed class SqLiteAccessor : DbAccessor2
   {
      public SqLiteAccessor(string cstring)
         : base(DbAccessorType2.SqLite, () => new SQLiteConnection(cstring))
      {
         
      }
   }
}
