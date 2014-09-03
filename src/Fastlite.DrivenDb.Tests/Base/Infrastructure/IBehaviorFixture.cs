using System;

namespace Fastlite.DrivenDb.Tests.Base.Infrastructure
{
   public interface IBehaviorFixture : IDisposable
   {
      IAccessorBuilder CreateAccessor();
   }
}
