namespace Fastlite.DrivenDb
{
   public interface IDbMapperLoader
   {
      IDbMapper<T> Load<T>(DbRecordList<T> recordset);
   }
}