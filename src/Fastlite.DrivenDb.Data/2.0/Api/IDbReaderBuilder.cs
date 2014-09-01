using Fastlite.DrivenDb.Core._2._0.Framework;

namespace Fastlite.DrivenDb.Data._2._0
{
   public interface IDbReaderBuilder
   {
      DbResultCollection<T> As<T>();
   }
}