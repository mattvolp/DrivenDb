namespace Fastlite.DrivenDb.Data.Utility
{
   internal sealed class QueryMapper
   {
      private readonly object _deserializer;

      public QueryMapper(object deserializer)
      {
         _deserializer = deserializer;
      }

      public object Deserializer
      {
         get { return _deserializer; }
      }
   }
}