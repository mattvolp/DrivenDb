using System;

namespace Fastlite.DrivenDb.Data.Tests._2._0.Infrastructure
{
   public interface IBehaviorFixture : IDisposable
   {
      IAccessorBuilder CreateAccessor();
   }
}
