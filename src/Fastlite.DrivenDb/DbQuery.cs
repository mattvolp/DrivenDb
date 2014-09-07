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
      {
         var recordsets = Execute().GetEnumerator();
         var recordset = CreateRecords<T>(recordsets.Current);

         return new DbRecordSet<T>(recordset);
      }

      public DbRecordSet<T1, T2> Execute<T1, T2>()
      {
         var recordsets = Execute().GetEnumerator();

         var recordset1 = CreateRecords<T1>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current);

         return new DbRecordSet<T1, T2>(recordset1, recordset2);
      }

      public DbRecordSet<T1, T2, T3> Execute<T1, T2, T3>()
      {
         var recordsets = Execute().GetEnumerator();

         var recordset1 = CreateRecords<T1>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current);

         return new DbRecordSet<T1, T2, T3>(recordset1, recordset2, recordset3);
      }

      public DbRecordSet<T1, T2, T3, T4> Execute<T1, T2, T3, T4>()
      {
         var recordsets = Execute().GetEnumerator();

         var recordset1 = CreateRecords<T1>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(4, 3);
         var recordset4 = CreateRecords<T4>(recordsets.Current);

         return new DbRecordSet<T1, T2, T3, T4>(recordset1, recordset2, recordset3, recordset4);
      }

      public DbRecordSet<T1, T2, T3, T4, T5> Execute<T1, T2, T3, T4, T5>()
      {
         var recordsets = Execute().GetEnumerator();

         var recordset1 = CreateRecords<T1>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(2, 1);
         var recordset2 = CreateRecords<T2>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(3, 2);
         var recordset3 = CreateRecords<T3>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(4, 3);
         var recordset4 = CreateRecords<T4>(recordsets.Current);
         if (!recordsets.MoveNext()) throw new MissingResultException(5, 4);
         var recordset5 = CreateRecords<T5>(recordsets.Current);

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

      private static DbRecordList<T> CreateRecords<T>(IDataReader reader)
      {
         var records = new List<DbRecord<T>>();
         var names = ExtractNames(reader);

         while (reader.Read())
         {
            var values = ExtractValues(reader);
            var record = new DbRecord<T>(names, values);

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
