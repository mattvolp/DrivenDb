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
using System.Data;
using System.Linq;

namespace Fastlite.DrivenDb.Data.Utility
{
   class DataRecord : IDataRecord
   {
      private readonly Dictionary<string,object> _dictionary = new Dictionary<string, object>();
      private readonly string[] _names;
      private readonly object[] _values;

      public DataRecord(IEnumerable<string> names, IEnumerable<object> values)
      {
         _names = names.ToArray();
         _values = values.ToArray();

         for(var i = 0; i < _names.Length; i++)
         {
            _dictionary.Add(_names[i], _values[i]);
         }
      }

      public string GetName(int i)
      {
         return _names[i];
      }

      public string GetDataTypeName(int i)
      {
         return _values.GetType().Name;
      }

      public Type GetFieldType(int i)
      {
         return _values.GetType();
      }

      public object GetValue(int i)
      {
         return _values[i];
      }

      public int GetValues(object[] values)
      {
         _values.CopyTo(values, 0);
         return _values.Length;
      }

      public int GetOrdinal(string name)
      {         
         for(var i = 0; i < _names.Length; i++)
         {
            if (_names[i] == name)
            {
               return i;
            }
         }

         return -1;
      }

      public bool GetBoolean(int i)
      {
         return Convert.ToBoolean(_values[i]);
      }

      public byte GetByte(int i)
      {
         return Convert.ToByte(_values[i]);
      }

      public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
      {
         throw new NotSupportedException();
      }

      public char GetChar(int i)
      {
         return Convert.ToChar(_values[i]);
      }

      public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
      {
         throw new NotSupportedException();
      }

      public Guid GetGuid(int i)
      {
         return Guid.Parse((string) _values[i]);
      }

      public short GetInt16(int i)
      {
         return Convert.ToInt16(_values[i]);
      }

      public int GetInt32(int i)
      {
         return Convert.ToInt32(_values[i]);
      }

      public long GetInt64(int i)
      {
         return Convert.ToInt64(_values[i]);
      }

      public float GetFloat(int i)
      {
         return Convert.ToSingle(_values[i]);
      }

      public double GetDouble(int i)
      {
         return Convert.ToDouble(_values[i]);
      }

      public string GetString(int i)
      {
         return Convert.ToString(_values[i]);
      }

      public decimal GetDecimal(int i)
      {
         return Convert.ToDecimal(_values[i]);
      }

      public DateTime GetDateTime(int i)
      {
         return Convert.ToDateTime(_values[i]);
      }

      public IDataReader GetData(int i)
      {
         throw new NotSupportedException();
      }

      public bool IsDBNull(int i)
      {
         return DBNull.Value.Equals(_values[i]);
      }

      public int FieldCount
      {
         get { return _names.Length; }
      }

      object IDataRecord.this[int i]
      {
         get { return _values[i]; }
      }

      object IDataRecord.this[string name]
      {
         get { return _dictionary[name]; }
      }
   }
}
