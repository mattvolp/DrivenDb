using System;
using DrivenDb.Utility;
using Xunit;

namespace DrivenDb.Tests.Utility
{    
    public class WeakTests
    {
        [Fact]
        public void WeakTargetTest()
        {
            var target = new object();
            var weak = new Weak<object>(target);

            Assert.True(weak.IsAlive);

            target = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete();

            Assert.False(weak.IsAlive);
        }
    }
}
