using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DrivenDb.Core
{
   [DataContract]
   public class DbEntity : IDbEntity
   {
      [DataMember]
      protected readonly Dictionary<string, object> _changes = new Dictionary<string, object>();

      [DataMember]
      protected EntityState _state = EntityState.New;

      public IEnumerable<DbEntityChange> Changes
      {
         get
         {
            lock (_changes)
            {
               return _changes.Select(kvp => new DbEntityChange(kvp.Key, kvp.Value))
                  .ToArray();
            }
         }
      }

      public EntityState State
      {
         get { return _state; }         
      }

      public void Change(string columnName, object value)
      {
         lock (_changes)
         {
            if (_state != EntityState.New && _state != EntityState.Deleted)
            {
               _state = EntityState.Updated;
            }

            _changes[columnName] = value;
         }
      }

      public void Delete()
      {
         _state = EntityState.Deleted;
      }

      public void Reset()
      {
         lock (_changes)
         {
            _state = EntityState.Current;
            _changes.Clear();
         }
      }
   }
}
