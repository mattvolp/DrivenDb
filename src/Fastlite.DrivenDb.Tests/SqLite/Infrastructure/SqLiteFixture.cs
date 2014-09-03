using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Fastlite.DrivenDb.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Tests.SqLite.Infrastructure
{
   public sealed class SqLiteFixture : IBehaviorFixture
   {
      private readonly string _filename;
      private readonly string _cstring;

      public SqLiteFixture()
      {
         _filename = Path.GetTempFileName();
         _cstring = String.Format("Data Source={0};Version=3;", _filename);

         CreateDatabase(_cstring);
      }

      public IAccessorBuilder CreateAccessor()
      {
         return new SqLiteBuilder(_cstring);
      }

      public void Dispose()
      {
         File.Delete(_filename);
      }

      private static void CreateDatabase(string cstring)
      {
         const string SCRIPT = @"Fastlite.DrivenDb.Tests.SqLite.Resources.SqLiteDatabase.sql";

         using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SCRIPT))
         {
            if (stream == null)
            {
               throw new FileNotFoundException(SCRIPT);
            }

            using (var reader = new StreamReader(stream))
            using (var connection = new SQLiteConnection(cstring))
            {
               connection.Open();

               using (var command = connection.CreateCommand())
               {
                  command.CommandText = reader.ReadToEnd();
                  command.ExecuteNonQuery();
               }
            }
         }
      }
   }
}
