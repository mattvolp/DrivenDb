namespace DrivenDb.Data
{
   // TODO: test
   public struct DbResult<T>
   {
      private readonly bool _isSet;
      private readonly T _value;

      public DbResult(T value)
      {
         _isSet = Equals(value, null);
         _value = value;
      }

      public bool HasValue
      {
         get { return _isSet; }
      }

      public T Value
      {
         get { return _value; }
      }
   }
}