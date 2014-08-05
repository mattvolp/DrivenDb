using System;
using System.Collections.Generic;
using DrivenDb.Collections;
using DrivenDb.Tests.Collections.Interfaces;

namespace DrivenDb.Tests.Collections
{
    public class DbViewTests : IDbViewTests
    {
        protected override IDbIndex<K, T> CreateIDbIndexWithFail<K, T>(IEnumerable<T> list, Func<T,K> extractor)            
        {
            return list.ToIndexWithFail(extractor);
        }
    }
}
