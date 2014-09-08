namespace Fastlite.DrivenDb
{
   public interface IDbMapper<T>
   {
      void Map(DbRecordList<T> records);
   }
}