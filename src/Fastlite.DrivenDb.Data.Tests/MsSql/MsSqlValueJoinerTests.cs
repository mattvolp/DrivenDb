using System;
using Fastlite.DrivenDb.Data.Access.Base;
using Fastlite.DrivenDb.Data.Access.MsSql;
using Xunit;

namespace Fastlite.DrivenDb.Data.Tests.MsSql
{
    public class MsSqlValueJoinerTests
    {
        [Fact]
        public void JoinEnumerableTest()
        {
            var joiner = new ValueJoiner();

            var strings = new[] { "a", "b", "c" };

            Assert.Equal(joiner.Join(strings), "'a','b','c'");

            var ints = new[] { 1, 2, 3 };

            Assert.Equal(joiner.Join(ints), "1,2,3");

            var nints = new[] { 1, default(int?), 3 };

            Assert.Equal(joiner.Join(nints), "1,NULL,3");

            var longs = new[] { 1L, 2, 3 };

            Assert.Equal(joiner.Join(longs), "1,2,3");

            var nlongs = new[] { 1, default(long?), 3 };

            Assert.Equal(joiner.Join(nlongs), "1,NULL,3");

            var guids = new[]
            {
               new Guid("9161411a-3509-42fb-bc76-7149435a6a07"), 
               new Guid("509dfd00-10ef-4c2c-9a8e-9877e343a48c"), 
               new Guid("41119b17-6fde-4763-84ed-70cca0a195c0")
            };

            Assert.Equal(joiner.Join(guids), "'9161411a-3509-42fb-bc76-7149435a6a07','509dfd00-10ef-4c2c-9a8e-9877e343a48c','41119b17-6fde-4763-84ed-70cca0a195c0'");

            var nguids = new[]
            {
               new Guid("9161411a-3509-42fb-bc76-7149435a6a07"), 
               default(Guid?),
               new Guid("41119b17-6fde-4763-84ed-70cca0a195c0")
            };

            Assert.Equal(joiner.Join(nguids), "'9161411a-3509-42fb-bc76-7149435a6a07',NULL,'41119b17-6fde-4763-84ed-70cca0a195c0'");
        }

        [Fact]
        public void JoinStringsTest()
        {
            var joiner = new ValueJoiner();
            var joined = joiner.Join(new[] { "a", "b", "c" });

            Assert.Equal(joined, "'a','b','c'");
        }

        [Fact]
        public void JoinIntTest()
        {
            var joiner = new ValueJoiner();
            var joined = joiner.Join(new[] { 1, 2, 3 });

            Assert.Equal(joined, "1,2,3");
        }

        [Fact]
        public void JoinNullableIntTest()
        {
            var joiner = new ValueJoiner();
            var joined = joiner.Join(new[] { 1, default(int?), 3 });

            Assert.Equal(joined, "1,NULL,3");
        }

        [Fact]
        public void JoinLongTest()
        {
            var joiner = new ValueJoiner();
            var joined = joiner.Join(new[] { 1L, 2, 3 });

            Assert.Equal(joined, "1,2,3");
        }

        [Fact]
        public void JoinNullableLongTest()
        {
            var joiner = new ValueJoiner();
            var joined = joiner.Join(new[] { 1, default(long?), 3 });

            Assert.Equal(joined, "1,NULL,3");
        }

        [Fact]
        public void JoinGuidTest()
        {
            var joiner = new ValueJoiner();

            var values = new[]
            {
               new Guid("9161411a-3509-42fb-bc76-7149435a6a07"), 
               new Guid("509dfd00-10ef-4c2c-9a8e-9877e343a48c"), 
               new Guid("41119b17-6fde-4763-84ed-70cca0a195c0")
            };

            var joined = joiner.Join(values);

            Assert.Equal(joined, "'9161411a-3509-42fb-bc76-7149435a6a07','509dfd00-10ef-4c2c-9a8e-9877e343a48c','41119b17-6fde-4763-84ed-70cca0a195c0'");
        }

        [Fact]
        public void JoinNullableGuidTest()
        {
            var joiner = new ValueJoiner();

            var values = new[]
            {
               new Guid("9161411a-3509-42fb-bc76-7149435a6a07"), 
               default(Guid?),
               new Guid("41119b17-6fde-4763-84ed-70cca0a195c0")
            };

            var joined = joiner.Join(values);

            Assert.Equal(joined, "'9161411a-3509-42fb-bc76-7149435a6a07',NULL,'41119b17-6fde-4763-84ed-70cca0a195c0'");
        }
    }
}
