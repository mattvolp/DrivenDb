namespace Fastlite.DrivenDb
{
   public interface IDbReaderBuilder
   {
      DbResultList<T> As<T>();
   }
}