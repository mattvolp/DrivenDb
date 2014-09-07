using Fastlite.DrivenDb.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Tests.SqLite.Infrastructure
{
   public sealed class SqLiteBuilder : IAccessorBuilder
   {
      private readonly string _cstring;

      public SqLiteBuilder(string cstring)
      {
         _cstring = cstring;
      }

      public IDbAccessor Build()
      {
         return new SqLiteAccessor(_cstring);
      }
   }
}
