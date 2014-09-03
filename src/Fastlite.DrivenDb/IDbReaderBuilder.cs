namespace Fastlite.DrivenDb
{
   public interface IDbReaderBuilder
   {
      DbResultCollection<T> As<T>();
   }
}