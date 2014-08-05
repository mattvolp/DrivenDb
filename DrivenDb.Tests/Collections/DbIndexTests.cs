using System;
using DrivenDb.Collections;
using DrivenDb.Tests.Collections.Interfaces;

namespace DrivenDb.Tests.Collections
{
    public class DbIndexTests : IDbIndexTests
    {
        protected override IDbIndex<K, T> CreateIDbIndex<K, T>(Func<T, K> extractor)
        {
            return new DbIndex<K, T>(new DbIndexCore<K, T>(extractor));
        }
    }
}
