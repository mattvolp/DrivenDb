
namespace Fastlite.DrivenDb
{
   public sealed class DbResultSet<T>
   {
      public DbResultSet(
         DbResultCollection<T> records
         )
      {
         Records = records;
      }

      public readonly DbResultCollection<T> Records;
   }

   internal sealed class DbResultSet<T1, T2>
   {
      public DbResultSet(
         DbResultCollection<T1> result1,
         DbResultCollection<T2> result2
         )
      {
         Record1 = result1;
         Record2 = result2;
      }

      public readonly DbResultCollection<T1> Record1;
      public readonly DbResultCollection<T2> Record2;
   }

   internal sealed class DbResultSet<T1, T2, T3>
   {
      public DbResultSet(
         DbResultCollection<T1> result1,
         DbResultCollection<T2> result2,
         DbResultCollection<T3> result3
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
      }

      public readonly DbResultCollection<T1> Record1;
      public readonly DbResultCollection<T2> Record2;
      public readonly DbResultCollection<T3> Record3;
   }

   internal sealed class DbResultSet<T1, T2, T3, T4>
   {
      public DbResultSet(
         DbResultCollection<T1> result1,
         DbResultCollection<T2> result2,
         DbResultCollection<T3> result3,
         DbResultCollection<T4> result4
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
      }

      public readonly DbResultCollection<T1> Record1;
      public readonly DbResultCollection<T2> Record2;
      public readonly DbResultCollection<T3> Record3;
      public readonly DbResultCollection<T4> Record4;
   }

   internal sealed class DbResultSet<T1, T2, T3, T4, T5>
   {
      public DbResultSet(
         DbResultCollection<T1> result1,
         DbResultCollection<T2> result2,
         DbResultCollection<T3> result3,
         DbResultCollection<T4> result4,
         DbResultCollection<T5> result5
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
         Record5 = result5;
      }

      public readonly DbResultCollection<T1> Record1;
      public readonly DbResultCollection<T2> Record2;
      public readonly DbResultCollection<T3> Record3;
      public readonly DbResultCollection<T4> Record4;
      public readonly DbResultCollection<T5> Record5;
   }
}
