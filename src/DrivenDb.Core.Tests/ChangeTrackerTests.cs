using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.Core.Tests
{
   [TestClass]
   public class ChangeTrackerTests
   {
      [TestMethod]
      public void DeleteAdjustStateProperly()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.New;
         tracker.Change("test", null);
         tracker.Delete();

         Assert.AreEqual(EntityState.Deleted, tracker.State);         
      }

      [TestMethod]
      public void ResetAdjustStateProperly()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.New;
         tracker.Change("test", null);
         tracker.Reset();

         Assert.AreEqual(EntityState.Current, tracker.State);
         Assert.AreEqual(0, tracker.Changes.Count());
      }

      [TestMethod]
      public void OnCreationStateIsNew()
      {
         var tracker = CreateStateTracker();

         Assert.AreEqual(EntityState.New, tracker.State);
      }

      [TestMethod]
      public void ChangesUpdateStateWhenCurrent()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.Current;
         tracker.Change("test", null);

         Assert.AreEqual(EntityState.Updated, tracker.State);
      }

      [TestMethod]
      public void ChangesDoNotUpdateStateWhenNew()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.New;
         tracker.Change("test", null);

         Assert.AreEqual(EntityState.New, tracker.State);
      }

      [TestMethod]
      public void ChangesDoNotUpdateStateWhenDeleted()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.Deleted;
         tracker.Change("test", null);

         Assert.AreEqual(EntityState.Deleted, tracker.State);
      }

      [TestMethod]
      public void ChangesGetAddedWhenDeletedStateDoesntChange()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.Deleted;
         tracker.Change("test", null);

         Assert.AreEqual("test", tracker.Changes.First().ColumnName);
      }

      [TestMethod]
      public void ChangesGetAddedWhenNewStateDoesntChange()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.New;
         tracker.Change("test", null);

         Assert.AreEqual("test", tracker.Changes.First().ColumnName);
      }

      [TestMethod]
      public void ChangesGetAddedWhenUpdatedStateDoesntChange()
      {
         var tracker = CreateStateTracker();

         tracker.State = EntityState.Updated;
         tracker.Change("test", null);

         Assert.AreEqual("test", tracker.Changes.First().ColumnName);
      }

      private DbEntityLikeness CreateStateTracker()
      {
         return new DbEntityLikeness();
      }

      private class DbEntityLikeness
         : DbEntity
      {
         public EntityState State
         {
            get { return _state; }
            set { _state = value; }
         }
      }
   }
}
