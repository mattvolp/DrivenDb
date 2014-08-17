using System;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   public interface IDbTestFixture : IDisposable
   {
      DbAccessorBuilder CreateAccessor();      
   }
}
