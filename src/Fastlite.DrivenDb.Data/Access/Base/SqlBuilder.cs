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
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Base
{
   public abstract class SqlBuilder : ISqlBuilder
   {
      protected readonly List<Where> _wheres = new List<Where>();
      protected readonly List<Setter> _setters = new List<Setter>();
      protected readonly HashSet<string> _columns = new HashSet<string>();
      protected readonly List<GroupedWhere> _grouped = new List<GroupedWhere>();
      private readonly string _idOpen;
      private readonly string _idClose;
      private readonly string _parameter;
      protected readonly string _terminator;

      protected SqlBuilder(string idOpen, string idClose, string parameter, string terminator)
      {
         _idOpen = idOpen;
         _idClose = idClose;
         _parameter = parameter;
         _terminator = terminator;
      }

      public string Schema
      {
         get;
         set;
      }

      public string Table
      {
         get;
         set;
      }

      public void AddColumn(string name)
      {
         _columns.Add(name);
      }

      public void AddSetter(string column, int parameter)
      {
         _setters.Add(new Setter(column, parameter));
      }

      public void AddWhere(string column, int parameter)
      {
         _wheres.Add(new Where(column, parameter));
      }

      public void GroupWhere()
      {
         _grouped.Add(new GroupedWhere(_wheres.ToArray()));
         _wheres.Clear();
      }

      public abstract string ToInsert<T>(T entity, int index, bool returnId) where T : IDbEntity;

      public string ToSelect()
      {
         if (String.IsNullOrWhiteSpace(Table))
         {
            throw new InvalidOperationException("No table specified for select");
         }

         return String.Format("SELECT {0} FROM {1}{2}{3}", GetFormattedColumns(), GetFormattedTable(), GetWhere(), _terminator);
      }

      public string ToUpdate()
      {
         if (String.IsNullOrWhiteSpace(Table))
         {
            throw new InvalidOperationException("No table specified for update");
         }

         if (!_setters.Any())
         {
            throw new InvalidOperationException("No columns specified for update");
         }

         return String.Format("UPDATE {0} SET {1}{2}{3}", GetFormattedTable(), GetFormattedSetters(), GetWhere(), _terminator);
      }

      public string ToDelete()
      {
         if (String.IsNullOrWhiteSpace(Table))
         {
            throw new InvalidOperationException("No table specified for delete");
         }

         return String.Format("DELETE FROM {0}{1}{2}", GetFormattedTable(), GetWhere(), _terminator);
      }

      public virtual DateTime CorrectDateTime(DateTime dateTime)
      {
         return dateTime;
      }

      protected string GetWhere()
      {
         if (!_wheres.Any() && !_grouped.Any())
         {
            return null;
         }

         if (_wheres.Any() && !_grouped.Any())
         {
            return String.Format(" WHERE {0}", GetFormattedWheres());
         }

         if (!_wheres.Any() && _grouped.Any())
         {
            return String.Format(" WHERE {0}", GetFormattedGroupedWheres());
         }

         return String.Format(" WHERE {0} AND ({1})", GetFormattedWheres(), GetFormattedGroupedWheres());
      }

      protected string GetFormattedColumns()
      {
         if (_columns.Count == 0)
         {
            return "*";
         }

         return String.Join(", ", _columns.Select(c => String.Format("{0}{1}{2}", _idOpen, c, _idClose)));
      }

      protected string GetFormattedSetterColumns()
      {
         return String.Join(", ", _setters.Select(s => String.Format("{0}{1}{2}", _idOpen, s.Column, _idClose)));
      }

      protected string GetFormattedSetterValues()
      {
         return String.Join(", ", _setters.Select(s => String.Format("{0}{1}", _parameter, s.Parameter)));
      }

      protected string GetFormattedTable()
      {
         if (String.IsNullOrWhiteSpace(Schema))
         {
            return String.Format("{0}{2}{1}", _idOpen, _idClose, Table);
         }

         return String.Format("{0}{2}{1}.{0}{3}{1}", _idOpen, _idClose, Schema, Table);
      }

      protected string GetFormattedWheres()
      {
         return String.Join(" AND ", _wheres.Select(c => String.Format("{0}{1}{2} = {3}{4}", _idOpen, c.Column, _idClose, _parameter, c.Parameter)));
      }

      protected string GetFormattedGroupedWheres()
      {
         var groups = _grouped.Select(
             g => "(" +
                 String.Join(" AND ",
                     g.Wheres.Select(w => String.Format("{0}{1}{2} = {3}{4}", _idOpen, w.Column, _idClose, _parameter, w.Parameter))
                     ) + ")"
             );

         return String.Join(" OR ", groups);
      }

      protected string GetFormattedSetters()
      {
         return String.Join(", ", _setters.Select(c => String.Format("{0}{1}{2} = {3}{4}", _idOpen, c.Column, _idClose, _parameter, c.Parameter)));
      }

      protected class GroupedWhere
      {
         public GroupedWhere(IEnumerable<Where> wheres)
         {
            Wheres = wheres;
         }

         public IEnumerable<Where> Wheres
         {
            get;
            private set;
         }
      }

      protected class Where
      {
         public Where(string column, int parameter)
         {
            Column = column;
            Parameter = parameter;
         }

         public string Column
         {
            get;
            private set;
         }

         public int Parameter
         {
            get;
            private set;
         }
      }

      protected class Setter
      {
         public Setter(string column, int parameter)
         {
            Column = column;
            Parameter = parameter;
         }

         public string Column
         {
            get;
            private set;
         }

         public int Parameter
         {
            get;
            private set;
         }
      }
   }
}
