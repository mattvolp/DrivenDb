using System.ComponentModel;
using System.Runtime.Serialization;
using DrivenDb.Collections;
using Xunit;

namespace DrivenDb.Tests.Utility
{    
    public class EntityComparerTests
    {
        [Fact]
        public void ComparisonTest()
        {
            var one = new TableA() { Id = 1, Name = "one", Value = 11 };
            var two = new TableA() { Id = 2, Name = "two", Value = 22 };
            var six = new TableA() { Id = 6, Name = "six", Value = 66 };

            var sut = new EntityComparer<TableA>((a,b) => a.Id.CompareTo(b.Id));

            Assert.Equal(sut.Compare(one, two), -1);
            Assert.Equal(sut.Compare(two, six), -1);
            Assert.Equal(sut.Compare(two, one), 1);
            Assert.Equal(sut.Compare(six, two), 1);
            Assert.Equal(sut.Compare(one, one), 0);
            Assert.Equal(sut.Compare(two, two), 0);
            Assert.Equal(sut.Compare(six, six), 0);
        }

        [Fact]
        public void ComparisonTest2()
        {
            var one = new TableA() { Id = 1, Name = "one", Value = 11 };
            var two = new TableA() { Id = 2, Name = "two", Value = 22 };
            var six = new TableA() { Id = 6, Name = "six", Value = 66 };

            var sut = new EntityComparer<TableA>(null);

            Assert.Equal(sut.Compare(one, two), -1);
            Assert.Equal(sut.Compare(two, six), -1);
            Assert.Equal(sut.Compare(two, one), 1);
            Assert.Equal(sut.Compare(six, two), 1);
            Assert.Equal(sut.Compare(one, one), 0);
            Assert.Equal(sut.Compare(two, two), 0);
            Assert.Equal(sut.Compare(six, six), 0);
        }

       [DbTable(Schema = null, Name = "TableA")]
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
