using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using DrivenDb.Collections;
using Xunit;

namespace DrivenDb.Tests.Collections.Interfaces
{
    public abstract class IDbIndexTests
    {
        [Fact]
        public void AddTest()
        {
            var my1 = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };
            var my2 = new MyTable()
            {
                MyIdentity = 2,
                MyNumber = 2,
                MyString = "Two"
            };
            var my3 = new MyTable()
            {
                MyIdentity = 3,
                MyNumber = 3,
                MyString = "Three"
            };

            var index = CreateIDbIndex<int, MyTable>((i) => i.MyIdentity);

            index.AddOrFail(my1);
            index.AddOrFail(my2);
            index.AddOrFail(my3);

            Assert.Equal(index.Count, 3);
            Assert.Equal(index[1].MyIdentity, 1);
            Assert.Equal(index[2].MyIdentity, 2);
            Assert.Equal(index[3].MyIdentity, 3);
        }

        [Fact]
        public void AddFailTest()
        {
            var my1 = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };

            var index = CreateIDbIndex<int, MyTable>((i) => i.MyIdentity);

            index.AddOrFail(my1);

            try
            {
                index.AddOrFail(my1);
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void AddMergeTest()
        {
            var my1a = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };
            var my1b = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };

            my1a.Entity.Reset();
            my1b.Entity.Reset();

            my1a.MyNumber = 666;
            my1b.MyString = "Whatev";

            var index = CreateIDbIndex<int, MyTable>((i) => i.MyIdentity);

            index.AddOrFail(my1a);
            index.AddOrMerge(my1b);

            Assert.Equal(index[1].MyIdentity, 1);
            Assert.Equal(index[1].MyNumber, 666);
            Assert.Equal(index[1].MyString, "Whatev");
            //Assert.Equal(index[1].MyIdentity, 1);
            //Assert.Equal(index[1].MyNumber, 666);
            //Assert.Equal(index[1].MyString, "Whatev");
        }

        [Fact]
        public void AddIgnoreTest()
        {
            var my1a = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };
            var my1b = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };

            my1a.Entity.Reset();
            my1b.Entity.Reset();

            my1a.MyNumber = 666;
            my1b.MyString = "Whatev";

            var index = CreateIDbIndex<int, MyTable>((i) => i.MyIdentity);

            index.AddOrFail(my1a);
            index.AddOrIgnore(my1b);

            Assert.Equal(index[1].MyIdentity, 1);
            Assert.Equal(index[1].MyNumber, 666);
            Assert.Equal(index[1].MyString, "One");
            //Assert.Equal(index[my1b].MyIdentity, 1);
            //Assert.Equal(index[my1b].MyNumber, 666);
            //Assert.Equal(index[my1b].MyString, "One");
        }

        [Fact]
        public void AddReplaceTest()
        {
            var my1a = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };
            var my1b = new MyTable()
            {
                MyIdentity = 1,
                MyNumber = 1,
                MyString = "One"
            };

            my1a.Entity.Reset();
            my1b.Entity.Reset();

            my1a.MyNumber = 666;
            my1b.MyString = "Whatev";

            var index = CreateIDbIndex<int, MyTable>((i) => i.MyIdentity);

            index.AddOrFail(my1a);
            index.AddOrReplace(my1b);

            Assert.Equal(index[1].MyIdentity, 1);
            Assert.Equal(index[1].MyNumber, 1);
            Assert.Equal(index[1].MyString, "Whatev");
            //Assert.Equal(index[my1b].MyIdentity, 1);
            //Assert.Equal(index[my1b].MyNumber, 1);
            //Assert.Equal(index[my1b].MyString, "Whatev");
        }

        protected abstract IDbIndex<K,T> CreateIDbIndex<K,T>(Func<T,K> extractor) 
            where T : class, IDbEntity<T>;

        [DbTable(Name = "MyTable")]
        private class MyTable : DbEntity<MyTable>, INotifyPropertyChanged
        {
            private int m_MyIdentity;
            private string m_MyString;
            private int m_MyNumber;

            [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
            public int MyIdentity
            {
                get { return m_MyIdentity; }
                set
                {
                    m_MyIdentity = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
                }
            }

            [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
            public string MyString
            {
                get { return m_MyString; }
                set
                {
                    m_MyString = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
                }
            }

            [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
            public int MyNumber
            {
                get { return m_MyNumber; }
                set
                {
                    m_MyNumber = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
