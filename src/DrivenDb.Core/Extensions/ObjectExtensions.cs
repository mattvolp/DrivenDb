using System;

namespace DrivenDb.Core.Extensions
{
   internal static class ObjectExtensions
   {
      public static T Chain<T>(this T t, Action<T> action)
      {
         action(t);
         return t;
      }

      public static T2 Map<T, T2>(this T t, Func<T, T2> func)
      {
         return func(t);         
      }

      public static T Hitch<T>(this T t, Func<T, T> func)
      {
         return func(t);
      }

      public static void Ignore<T>(this T t)
      {
         // noop
      }
   }
}
