using System.Data;

namespace Fastlite.DrivenDb
{
   internal interface IDbConnectionFactory
   {
      IDbConnection Create();
   }
}
