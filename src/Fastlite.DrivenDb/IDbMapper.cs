namespace Fastlite.DrivenDb
{
   public interface IDbMapper
   {
      void Map<T>(DbRecordSet<T> record);
   }
}