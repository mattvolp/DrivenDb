
namespace Fastlite.DrivenDb
{
   public interface IDbAccessor
   {
      IDbReaderBuilder Read(string query, params object[] parameters);
   }
}