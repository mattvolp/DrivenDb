using System;
using System.Collections.Generic;
using System.Data;

namespace Fastlite.DrivenDb
{
   public class DbQuery 
   {
      private readonly IDbConnectionFactory _connections;
      private readonly string _sql;
      private readonly object[] _arguments;

      public DbQuery(IDbConnectionFactory connections, string sql, params object[] arguments)
      {
         _connections = connections;
         _sql = sql;
         _arguments = arguments;
      }

      public string Sql
      {
         get { return _sql; }
      }

      public DbRecordSet<T> Execute<T>()
         where T : new()
      {
         var recordsets = Execute().GetEnumerator();

         if (!recordsets.MoveNext()) throw new MissingResultException(1, 1);
         var recordset = CreateRecords<T>(recordsets.Current, () => new T());

         // this is required in order to end the enumeration and dispose of the connection
         if (recordsets.MoveNext()) throw new ExtraResultException(5, 6);

         return new DbRecordSet<T>(recordset);
      }

      public DbRecordSet<T1, T2> Execute<T1, T2>()
         where T1 : new()
         where T2 : new()
      {
         var recordsets = Execute().GetEnumerator();

         if (!recordsets.MoveNext()) throw new MissingResultException(1, 1);
         var recordset1 = CreateRecords<T1>(recordsets.Current, () => new T1());
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current, () => new T2());

         // this is required in order to end the enumeration and dispose of the connection
         if (recordsets.MoveNext()) throw new ExtraResultException(5, 6);

         return new DbRecordSet<T1, T2>(recordset1, recordset2);
      }

      public DbRecordSet<T1, T2, T3> Execute<T1, T2, T3>()
         where T1 : new()
         where T2 : new()
         where T3 : new()
      {
         var recordsets = Execute().GetEnumerator();

         if (!recordsets.MoveNext()) throw new MissingResultException(1, 1);
         var recordset1 = CreateRecords<T1>(recordsets.Current, () => new T1());
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current, () => new T2());
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current, () => new T3());

         // this is required in order to end the enumeration and dispose of the connection
         if (recordsets.MoveNext()) throw new ExtraResultException(5, 6);

         return new DbRecordSet<T1, T2, T3>(recordset1, recordset2, recordset3);
      }

      public DbRecordSet<T1, T2, T3, T4> Execute<T1, T2, T3, T4>()
         where T1 : new()
         where T2 : new()
         where T3 : new()
         where T4 : new()
      {
         var recordsets = Execute().GetEnumerator();

         if (!recordsets.MoveNext()) throw new MissingResultException(1, 1);
         var recordset1 = CreateRecords<T1>(recordsets.Current, () => new T1());
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current, () => new T2());
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current, () => new T3());
         if (!recordsets.MoveNext()) throw new MissingResultException(4, 3);
         var recordset4 = CreateRecords<T4>(recordsets.Current, () => new T4());

         // this is required in order to end the enumeration and dispose of the connection
         if (recordsets.MoveNext()) throw new ExtraResultException(5, 6);

         return new DbRecordSet<T1, T2, T3, T4>(recordset1, recordset2, recordset3, recordset4);
      }

      public DbRecordSet<T1, T2, T3, T4, T5> Execute<T1, T2, T3, T4, T5>()
         where T1 : new()
         where T2 : new()
         where T3 : new()
         where T4 : new()
         where T5 : new()
      {
         var recordsets = Execute().GetEnumerator();

         if (!recordsets.MoveNext()) throw new MissingResultException(1, 1);
         var recordset1 = CreateRecords<T1>(recordsets.Current, () => new T1());
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current, () => new T2());
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current, () => new T3());
         if (!recordsets.MoveNext()) throw new MissingResultException(4, 3);
         var recordset4 = CreateRecords<T4>(recordsets.Current, () => new T4());
         if (!recordsets.MoveNext()) throw new MissingResultException(5, 4);
         var recordset5 = CreateRecords<T5>(recordsets.Current, () => new T5());

         // this is required in order to end the enumeration and dispose of the connection
         if (recordsets.MoveNext()) throw new ExtraResultException(5, 6);
         
         return new DbRecordSet<T1, T2, T3, T4, T5>(recordset1, recordset2, recordset3, recordset4, recordset5);
      }

      public IEnumerable<IDataReader> Execute()
      {
         using (var connection = _connections.Create())
         {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
               command.CommandText = _sql;

               BuildParameters(command, _arguments);

               using (var reader = command.ExecuteReader())
               {
                  do
                  {
                     yield return reader;
                  } while (reader.NextResult());   
               }
            }
         }
      }

      private static DbRecordList<T> CreateRecords<T>(IDataReader reader, Func<T> factory)
      {
         var records = new List<DbRecord<T>>();
         var names = ExtractNames(reader);

         while (reader.Read())
         {
            var values = ExtractValues(reader);
            var entity = factory();
            var record = new DbRecord<T>(entity, names, values);

            records.Add(record);
         }

         return new DbRecordList<T>(records);
      }

      private static void BuildParameters(IDbCommand command, object[] arguments)
      {
         for (var i = 0; i < arguments.Length; i++)
         {
            var parameter = command.CreateParameter();

            parameter.ParameterName = "@" + i;
            parameter.Value = arguments[i];
         }
      }

      private static string[] ExtractNames(IDataReader reader)
      {
         var names = new List<string>();

         for (var i = 0; i < reader.FieldCount; i++)
         {
            var name = reader.GetName(i);

            names.Add(name);
         }

         return names.ToArray();
      }

      private static object[] ExtractValues(IDataReader reader)
      {
         var values = new object[reader.FieldCount];

         reader.GetValues(values);

         return values;
      }
   }
}
