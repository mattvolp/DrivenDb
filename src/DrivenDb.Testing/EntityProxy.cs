
namespace DrivenDb.Testing
{
   public class EntityProxy
   {
      private readonly object _instance;

      public EntityProxy(object instance)
      {
         _instance = instance;
      }

      public object Instance
      {
         get { return _instance; }
      }

      public void SetProperty(string name, object value)
      {
         var setter = _instance.GetType()
            .GetProperty(name)
            .GetSetMethod();

         setter.Invoke(_instance, new[] { value });
      }

      public T GetProperty<T>(string name)
      {
         var getter = _instance.GetType()
            .GetProperty(name)
            .GetGetMethod();

         var result = getter.Invoke(_instance, null);

         return (T) result;
      }

      public T ExecuteMethod<T>(string name, params object[] parameters)
      {
         var method = _instance.GetType()
            .GetMethod(name);

         var result = method.Invoke(_instance, parameters);

         return (T) result;
      }

      public T GetField<T>(string name)
      {
         var field = _instance.GetType()
            .GetField(name);

         var result = field.GetValue(_instance);

         return (T) result;
      }
   }
}