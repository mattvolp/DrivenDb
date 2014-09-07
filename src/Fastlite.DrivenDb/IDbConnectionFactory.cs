using System.Data;

namespace Fastlite.DrivenDb
{
   public interface IDbConnectionFactory
   {
      IDbConnection Create();
   }
}
