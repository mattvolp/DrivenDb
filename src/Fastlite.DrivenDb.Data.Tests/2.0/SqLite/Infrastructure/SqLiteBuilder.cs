using System;
using Fastlite.DrivenDb.Data.Tests._2._0.Infrastructure;
using Fastlite.DrivenDb.Data._2_0;

namespace Fastlite.DrivenDb.Data.Tests._2._0.SqLite.Infrastructure
{
   public sealed class SqLiteBuilder : IAccessorBuilder
   {
      private readonly string _cstring;

      public SqLiteBuilder(string cstring)
      {
         _cstring = cstring;
      }

      public IDbAccessor2 Build()
      {
         return new SqLiteAccessor(_cstring);
      }
   }
}
