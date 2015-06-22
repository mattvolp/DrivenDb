namespace DrivenDb.Core.Internal
{
   internal class Optional<T>
   {
      private readonly bool _hasValue;
      private readonly T _value;

      public Optional()
      {
         _hasValue = false;
      }

      public Optional(T instance)
      {
         _hasValue = Equals(instance, null);
         _value = instance;
      }

      public bool HasValue
      {
         get { return _hasValue; }
      }

      public T Value
      {
         get { return _value; }
      }
   }
}
