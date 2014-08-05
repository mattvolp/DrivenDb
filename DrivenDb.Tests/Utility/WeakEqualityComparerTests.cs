using System;
using DrivenDb.Utility;
using Xunit;

namespace DrivenDb.Tests.Utility
{
    public class WeakEqualityComparerTests
    {
        [Fact]
        public void EqualsTest()
        {
            var obj1 = new object();
            var obj2 = new object();

            var weak1 = new Weak<object>(obj1);
            var weak2 = new Weak<object>(obj1);
            var weak3 = new Weak<object>(obj2);

            var sut = new WeakEqualityComparer<object>();

            Assert.True(sut.Equals(weak1, weak2));
            Assert.False(sut.Equals(weak2, weak3));

            obj2 = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete();

            var hold = new Weak<object>(obj1);

            Assert.True(sut.Equals(weak1, weak2));
            Assert.False(sut.Equals(weak2, weak3));
        }

        [Fact]
        public void GetHashCodeTest()
        {
            var obj1 = new object();
            var obj2 = new object();

            var weak1 = new Weak<object>(obj1);
            var weak2 = new Weak<object>(obj1);
            var weak3 = new Weak<object>(obj2);

            var sut = new WeakEqualityComparer<object>();
            
            Assert.Equal(sut.GetHashCode(weak1), sut.GetHashCode(weak2));
            Assert.NotEqual(sut.GetHashCode(weak2), sut.GetHashCode(weak3));

            obj2 = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete();

            var hold = new Weak<object>(obj1);
            
            Assert.Equal(sut.GetHashCode(weak1), sut.GetHashCode(weak2));
            Assert.NotEqual(sut.GetHashCode(weak2), sut.GetHashCode(weak3));
        }
    }
}
