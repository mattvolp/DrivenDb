
namespace Fastlite.DrivenDb
{
   public interface IDbAccessor2
   {
      IDbReaderBuilder Read(string query, params object[] parameters);
   }
}