
using Fastlite.DrivenDb.Data._2._0;

namespace Fastlite.DrivenDb.Data._2_0
{
   public interface IDbAccessor2
   {
      IDbReaderBuilder Read(string query, params object[] parameters);
   }
}