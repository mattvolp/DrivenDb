using System.Data;

namespace Fastlite.DrivenDb.Data._2._0
{
   internal interface IDbConnectionFactory
   {
      IDbConnection Create();
   }
}
