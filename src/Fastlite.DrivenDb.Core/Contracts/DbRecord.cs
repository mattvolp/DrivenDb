/**************************************************************************************
 * Original Author : Anthony Leatherwood (fastlite@outlook.com)
 * Source Location : https://github.com/Fastlite/DrivenDb
 *
 * This source is subject to the Mozilla Public License, version 2.0.
 * Link: https://github.com/Fastlite/DrivenDb/blob/master/LICENSE
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts.Arguments;
using Fastlite.DrivenDb.Core.Contracts.Attributes;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Core.Utility;

// ReSharper disable StaticFieldInGenericType
namespace Fastlite.DrivenDb.Core.Contracts
{
   public delegate void StateChangedEvent(object sender, StateChangedEventArgs e);

   [DataContract]
   public abstract class DbRecord<T> : IDbRecord
      where T : IDbRecord
   {
      // ReSharper disable InconsistentNaming
      protected static readonly EntityAccessor<T> __entityAccessor = new EntityAccessor<T>(true);
      protected static readonly KeyValuePair<string, DbColumnAttribute> __identityColumn;
      protected static readonly KeyValuePair<string, DbColumnAttribute>[] __primaryColumns;
      protected static readonly IDictionary<string, DbColumnAttribute> __columns;
      protected static readonly DbTableAttribute __table;
      protected static readonly DbSequenceAttribute __sequence;
      // ReSharper restore InconsistentNaming

      private static readonly bool? _isIdentity32;

      static DbRecord()
      {
         var type = typeof(T);

         __table = AttributeHelper.GetTableAttribute(type);
         __sequence = AttributeHelper.GetSequenceAttribute(type);
         __columns = AttributeHelper.GetColumnAttributes(type);

         __primaryColumns = __columns
            .Where(c => c.Value.IsPrimaryKey)
            .ToArray();

         __identityColumn = __columns
            .FirstOrDefault(c => c.Value.IsDbGenerated);

         if (__identityColumn.Value != null)
         {
            _isIdentity32 = __entityAccessor.GetPropertyInfo(__identityColumn.Key).PropertyType == typeof(int);
         }
      }

      // ReSharper disable InconsistentNaming
      [DataMember]
      protected HashSet<string> __changes = new HashSet<string>();

      [DataMember]
      protected DateTime? __lastModified;

      [DataMember]
      protected DateTime? __lastUpdated;

      [DataMember]
      protected EntityState __state;

      protected T __instance;
      // ReSharper restore InconsistentNaming

      protected DbRecord()
      {
         Initialize();
      }

      public event StateChangedEvent StateChanged;

      public IDbRecord Record
      {
         get { return this; }
      }

      protected void ChangeState(EntityState state)
      {
         var previous = __state;

         __state = state;

         if (previous != state && StateChanged != null)
         {
            StateChanged(this, new StateChangedEventArgs(previous, state));
         }
      }

      [OnDeserialized]
      private void OnDeserialized(StreamingContext context)
      {
         Initialize();
      }

      private void Initialize()
      {
         __instance = (T) (object) this;         
      }
      
      DateTime? IDbRecord.LastUpdated
      {
         get { return __lastUpdated; }
      }

      DateTime? IDbRecord.LastModified
      {
         get { return __lastModified; }
      }

      EntityState IDbRecord.State
      {
         get { return __state; }
      }

      // TODO: this need to a defined construct, not just a string
      string IDbRecord.Schema
      {
         get { return __table.Schema; }
      }

      DbTableAttribute IDbRecord.Table
      {
         get { return __table; }
      }

      DbSequenceAttribute IDbRecord.Sequence
      {
         get { return __sequence; }
      }

      DbColumnAttribute IDbRecord.IdentityColumn
      {
         get { return __identityColumn.Value; }
      }

      DbColumnAttribute[] IDbRecord.PrimaryColumns
      {
         get { return __primaryColumns.Select(c => c.Value).ToArray(); }
      }

      // TODO: needs to be a defined construct, not just a dictionary
      IDictionary<string, DbColumnAttribute> IDbRecord.Columns
      {
         get { return __columns; }
      }

      IEnumerable<string> IDbRecord.Changes
      {
         get { return __changes; }
      }

      void IDbRecord.SetIdentity(long identity)
      {
         if (__identityColumn.Value != null && _isIdentity32.HasValue)
         {
            if (_isIdentity32.Value)
            {
               __entityAccessor.SetPropertyValue(__instance, __identityColumn.Value.Name, (int) identity);
            }
            else
            {
               __entityAccessor.SetPropertyValue(__instance, __identityColumn.Value.Name, identity);
            }
         }
      }

      object IDbRecord.GetProperty(string property)
      {
         return __entityAccessor.GetPropertyValue<object>(__instance, property);
      }

      void IDbRecord.SetProperty(string property, object value)
      {
         __entityAccessor.SetPropertyValue(__instance, property, value);
      }

      void IDbRecord.Reset()
      {
         ChangeState(EntityState.Current);

         __lastModified = null;
         __lastUpdated = DateTime.Now;
         __changes.Clear();
      }
   }
}