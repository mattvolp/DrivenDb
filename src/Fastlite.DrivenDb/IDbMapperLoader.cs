namespace Fastlite.DrivenDb
{
   public interface IDbMapperLoader
   {
      IDbMapper Load<T>(DbRecordSet<T> recordset);
   }
}