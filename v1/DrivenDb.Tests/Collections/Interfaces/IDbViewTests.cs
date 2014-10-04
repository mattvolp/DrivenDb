using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using DrivenDb.Collections;
using Xunit;

namespace DrivenDb.Tests.Collections.Interfaces
{
    public abstract class IDbViewTests
    {
        [Fact]
        public void FilterTest()
        {
            var one = new TableA() { Id = 1, Name = "one", Value = 11 };
            var two = new TableA() { Id = 2, Name = "two", Value = 22 };
            var six = new TableA() { Id = 6, Name = "six", Value = 66 };

            var list = new List<TableA>() { one, two };

            var index = CreateIDbIndexWithFail<long, TableA>(list, (i) => i.Id);
            var sut = index.ViewAs((a) => a.Value > 11);

            Assert.True(sut.Count() == 1);
            Assert.True(sut.Single().Id == 2);
            Assert.True(sut.Contains(two));
            Assert.False(sut.Contains(one));

            index.AddOrFail(six);

            Assert.True(sut.Count() == 2);
            Assert.True(sut.Contains(two));
            Assert.True(sut.Contains(six));
            Assert.False(sut.Contains(one));
        }

        [Fact]
        public void OrderTest()
        {
            var one = new TableA() { Id = 1, Name = "one", Value = 11 };
            var two = new TableA() { Id = 2, Name = "two", Value = 22 };
            var six = new TableA() { Id = 6, Name = "six", Value = 66 };

            var list = new List<TableA>() { one, two };

            var index = CreateIDbIndexWithFail<long, TableA>(list, (i) => i.Id);
            var sut = index.ViewAs((a,b) => a.Id.CompareTo(b.Id) * -1);

            Assert.True(sut.Count() == 2);
            Assert.True(sut.ToArray()[0].Id == 2);
            Assert.True(sut.ToArray()[1].Id == 1);
            Assert.True(sut.Contains(one)); 
            Assert.True(sut.Contains(two));                       
            Assert.False(sut.Contains(six));

            index.AddOrFail(six);

            Assert.True(sut.Count() == 3);
            Assert.True(sut.ToArray()[0].Id == 6);
            Assert.True(sut.ToArray()[1].Id == 2);
            Assert.True(sut.ToArray()[2].Id == 1);
            Assert.True(sut.Contains(one));
            Assert.True(sut.Contains(two)); 
            Assert.True(sut.Contains(six));                       
        }

        protected abstract IDbIndex<K, T> CreateIDbIndexWithFail<K, T>(IEnumerable<T> list, Func<T, K> extractor)
            where T : class, IDbEntity<T>;
            
        [DbTable(Name="TableA")]
        protected class TableA : DbEntity<TableA>, INotifyPropertyChanged
        {
            [DataMember]
            private long m_Id;
            [DataMember]
            private string m_Name;
            [DataMember]
            private long m_Value;

            [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
            public long Id
            {
                get { return m_Id; }
                set
                {
                    m_Id = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                }
            }

            [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Name")]
            public string Name
            {
                get { return m_Name; }
                set
                {
                    m_Name = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }

            [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Value")]
            public long Value
            {
                get { return m_Value; }
                set
                {
                    m_Value = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
