using System;
using System.Reflection;

namespace Fastlite.DrivenDb.Data.Utility
{
   internal sealed class AnonFactory
   {
      private readonly ConstructorInfo _ctor;

      internal AnonFactory(Type type)
      {
         _ctor = type.GetConstructors()[0];
         ParamCount = _ctor.GetParameters().Length;
      }

      public readonly int ParamCount;

      public T Create<T>(params object[] args)
      {
         return (T)_ctor.Invoke(args);
      }
   }
}