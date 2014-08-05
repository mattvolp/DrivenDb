using DrivenDb.Base;
using Moq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace DrivenDb.Tests.Language.Base
{
   public class DbAggregatorTests
   {
      [Fact]
      public void DbAggregator_AggregatesWithMoreThanOnePrimaryError()
      {
         var accessor = new Mock<IDbAccessor>();
         var aggregate = new EmptyAggregate();
         var sut = new DbAggregator();

         Assert.Throws<InvalidAggregateStructure>(
            () => sut.WriteAggregate(accessor.Object, aggregate)
            );
      }

      [Fact]
      public void DbAggregator_AggregatesWithLessThanOnePrimaryError()
      {
         var accessor = new Mock<IDbAccessor>();
         var aggregate = new MultiPrimaryAggregate();
         var sut = new DbAggregator();

         Assert.Throws<InvalidAggregateStructure>(
            () => sut.WriteAggregate(accessor.Object, aggregate)
            );
      }

      [Fact]
      public void DbAggregator_NullAggregateIsIgnored()
      {
         var accessor = new Mock<IDbAccessor>();
         var actual = false;

         accessor.Setup(a => a.WriteEntity(It.IsAny<IDbEntity>()))
            .Callback(() => actual = true);

         var sut = new DbAggregator();

         sut.WriteAggregate(accessor.Object, null);

         Assert.False(actual);
      }

      [Fact]
      public void DbAggregator_NullAggregatesAreIgnored()
      {
         var accessor = new Mock<IDbAccessor>();
         var actual = false;

         accessor.Setup(a => a.WriteEntity(It.IsAny<IDbEntity>()))
            .Callback(() => actual = true);

         var sut = new DbAggregator();

         sut.WriteAggregate(accessor.Object, null);

         Assert.False(actual);
      }

      [Fact]
      public void DbAggregator_AllNonNullPrimariesAreWritten()
      {
         var aggregates = new[]
            {
               new SinglePrimaryAggregate() {Primary = new MyPrimary() {Id = 1, Name = "one"}},
               null,
               new SinglePrimaryAggregate() {Primary = new MyPrimary() {Id = 3, Name = "three"}},
            };

         var accessor = new Mock<IDbAccessor>();
         var actual = 0;

         accessor.Setup(a => a.WriteEntities(It.IsAny<IEnumerable<IDbEntity>>()))
            .Callback<IEnumerable<IDbEntity>>((e) => actual += e.Count());

         var sut = new DbAggregator();

         sut.WriteAggregates(accessor.Object, aggregates);

         Assert.Equal(2, actual);
      }

      [Fact]
      public void DbAggregator_AllSingleForeignsAreUpdateAfterPrimariesAreWritten()
      {
         var aggregates = new[]
            {
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 1, Name = "one"}, Foreign1 = new MyForeign1()},
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 2, Name = "two"}, Foreign1 = new MyForeign1()},
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 3, Name = "three"}, Foreign1 = new MyForeign1()},
            };

         var accessor = new Mock<IDbAccessor>();

         var sut = new DbAggregator();

         sut.WriteAggregates(accessor.Object, aggregates);

         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign1.PrimaryId == 1));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign1.PrimaryId == 2));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign1.PrimaryId == 3));
      }

      [Fact]
      public void DbAggregator_AllMultiForeignsAreUpdateAfterPrimariesAreWritten()
      {
         var aggregates = new[]
            {
               new EnumerableForeignAggregate() {Primary = new MyPrimary() {Id = 1, Name = "one"}, Foreigns = new[]
                  {
                     new MyForeign1(), new MyForeign1(),
                  }},
               new EnumerableForeignAggregate() {Primary = new MyPrimary() {Id = 2, Name = "two"}, Foreigns = new[]
                  {
                     new MyForeign1(), new MyForeign1(),
                  }},
               new EnumerableForeignAggregate() {Primary = new MyPrimary() {Id = 3, Name = "three"}, Foreigns = new[]
                  {
                     new MyForeign1(), new MyForeign1(),
                  }},
            };

         var accessor = new Mock<IDbAccessor>();
         var sut = new DbAggregator();

         sut.WriteAggregates(accessor.Object, aggregates);

         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreigns.All(f => f.PrimaryId == 1)));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreigns.All(f => f.PrimaryId == 2)));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreigns.All(f => f.PrimaryId == 3)));
      }

      [Fact]
      public void DbAggregator_AllSingleForeignsAreSavedAfterUpdate()
      {
         var aggregates = new[]
            {
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 1, Name = "one"}, Foreign1 = new MyForeign1()},
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 2, Name = "two"}, Foreign1 = new MyForeign1()},
               new SingleForeignAggregate() {Primary = new MyPrimary() {Id = 3, Name = "three"}, Foreign1 = new MyForeign1()},
            };

         var accessor = new Mock<IDbAccessor>();
         var actual = 0;

         accessor.Setup(a => a.WriteEntities(It.IsAny<IEnumerable<IDbEntity>>()))
            .Callback<IEnumerable<IDbEntity>>((e) => actual += e.Count());

         var sut = new DbAggregator();

         sut.WriteAggregates(accessor.Object, aggregates);

         Assert.Equal(6, actual);
      }

      [Fact]
      public void DbAggregator_AllMixedForeignsAreSavedAfterUpdate()
      {
         var aggregates = new[]
            {
               new MixedForeignsAggregate()
                  {
                     Primary = new MyPrimary() {Id = 1, Name = "one"},
                     Foreign = new MyForeign1(),
                     Foreigns = new[]
                        {
                           new MyForeign2(), new MyForeign2(),
                        }
                  },

               new MixedForeignsAggregate()
                  {
                     Primary = new MyPrimary() {Id = 2, Name = "two"},
                     Foreign = new MyForeign1(),
                     Foreigns = new[]
                        {
                           new MyForeign2(), new MyForeign2(),
                        }
                  },
               new MixedForeignsAggregate()
                  {
                     Primary = new MyPrimary() {Id = 3, Name = "three"},
                     Foreign = new MyForeign1(),
                     Foreigns = new[]
                        {
                           new MyForeign2(), new MyForeign2(),
                        }
                  },
            };

         var accessor = new Mock<IDbAccessor>();
         var actual = 0;

         accessor.Setup(a => a.WriteEntities(It.IsAny<IEnumerable<IDbEntity>>()))
            .Callback<IEnumerable<IDbEntity>>((e) => actual += e.Count());

         var sut = new DbAggregator();

         sut.WriteAggregates(accessor.Object, aggregates);

         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign.PrimaryId == 1 && a.Foreigns.All(f => f.MyPrimaryId == 1)));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign.PrimaryId == 2 && a.Foreigns.All(f => f.MyPrimaryId == 2)));
         Assert.NotNull(aggregates.SingleOrDefault(a => a.Foreign.PrimaryId == 3 && a.Foreigns.All(f => f.MyPrimaryId == 3)));
         Assert.Equal(12, actual);
      }

      //
      // ===========================================================================
      //

      public class EmptyAggregate : IDbAggregate
      {
      }

      public class MultiPrimaryAggregate : IDbAggregate
      {
         [DbPrimary]
         public MyPrimary Primary1
         {
            get;
            set;
         }

         [DbPrimary]
         public MyPrimary Primary2
         {
            get;
            set;
         }
      }

      public class SinglePrimaryAggregate : IDbAggregate
      {
         [DbPrimary]
         public MyPrimary Primary
         {
            get;
            set;
         }
      }

      public class SingleForeignAggregate : IDbAggregate
      {
         [DbPrimary]
         public MyPrimary Primary
         {
            get;
            set;
         }

         [DbForeign(PrimaryProperty = "Id", ForeignProperty = "PrimaryId")]
         public MyForeign1 Foreign1
         {
            get;
            set;
         }
      }

      public class EnumerableForeignAggregate : IDbAggregate
      {
         [DbPrimary]
         public MyPrimary Primary
         {
            get;
            set;
         }

         [DbForeign(PrimaryProperty = "Id", ForeignProperty = "PrimaryId")]
         public IEnumerable<MyForeign1> Foreigns
         {
            get;
            set;
         }
      }

      public class MixedForeignsAggregate : IDbAggregate
      {
         [DbPrimary]
         public MyPrimary Primary
         {
            get;
            set;
         }

         [DbForeign(PrimaryProperty = "Id", ForeignProperty = "PrimaryId")]
         public MyForeign1 Foreign
         {
            get;
            set;
         }

         [DbForeign(PrimaryProperty = "Id", ForeignProperty = "MyPrimaryId")]
         public IEnumerable<MyForeign2> Foreigns
         {
            get;
            set;
         }
      }

      [DbTable(Name = "MyPrimary")]
      public class MyPrimary : DbEntity<MyPrimary>, INotifyPropertyChanged
      {
         private int m_Id;
         private string m_Name;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
         public int Id
         {
            get { return m_Id; }
            set
            {
               m_Id = value;
               OnPropertyChanged("Id");
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Name")]
         public string Name
         {
            get { return m_Name; }
            set
            {
               m_Name = value;
               OnPropertyChanged("Name");
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;

         protected virtual void OnPropertyChanged(string propertyName)
         {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      [DbTable(Name = "MyForeign1")]
      public class MyForeign1 : DbEntity<MyForeign1>, INotifyPropertyChanged
      {
         private int m_Id;
         private int m_PrimaryId;
         private string m_Name;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
         public int Id
         {
            get { return m_Id; }
            set
            {
               m_Id = value;
               OnPropertyChanged("Id");
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "PrimaryId")]
         public int PrimaryId
         {
            get { return m_PrimaryId; }
            set
            {
               m_PrimaryId = value;
               OnPropertyChanged("PrimaryId");
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Name")]
         public string Name
         {
            get { return m_Name; }
            set
            {
               m_Name = value;
               OnPropertyChanged("Name");
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;

         protected virtual void OnPropertyChanged(string propertyName)
         {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      [DbTable(Name = "MyForeign2")]
      public class MyForeign2 : DbEntity<MyForeign2>, INotifyPropertyChanged
      {
         private int m_Id;
         private int m_MyPrimaryId;
         private string m_Name;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
         public int Id
         {
            get { return m_Id; }
            set
            {
               m_Id = value;
               OnPropertyChanged("Id");
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyPrimaryId")]
         public int MyPrimaryId
         {
            get { return m_MyPrimaryId; }
            set
            {
               m_MyPrimaryId = value;
               OnPropertyChanged("MyPrimaryId");
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Name")]
         public string Name
         {
            get { return m_Name; }
            set
            {
               m_Name = value;
               OnPropertyChanged("Name");
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;

         protected virtual void OnPropertyChanged(string propertyName)
         {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}