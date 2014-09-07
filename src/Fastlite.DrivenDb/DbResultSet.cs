
namespace Fastlite.DrivenDb
{
   public sealed class DbResultSet<T>
   {
      public DbResultSet(
         DbResultList<T> result
         )
      {
         Result = result;
      }

      public readonly DbResultList<T> Result;
   }

   internal sealed class DbResultSet<T1, T2>
   {
      public DbResultSet(
         DbResultList<T1> result1,
         DbResultList<T2> result2
         )
      {
         Result1 = result1;
         Result2 = result2;
      }

      public readonly DbResultList<T1> Result1;
      public readonly DbResultList<T2> Result2;
   }

   internal sealed class DbResultSet<T1, T2, T3>
   {
      public DbResultSet(
         DbResultList<T1> result1,
         DbResultList<T2> result2,
         DbResultList<T3> result3
         )
      {
         Result1 = result1;
         Result2 = result2;
         Result3 = result3;
      }

      public readonly DbResultList<T1> Result1;
      public readonly DbResultList<T2> Result2;
      public readonly DbResultList<T3> Result3;
   }

   internal sealed class DbResultSet<T1, T2, T3, T4>
   {
      public DbResultSet(
         DbResultList<T1> result1,
         DbResultList<T2> result2,
         DbResultList<T3> result3,
         DbResultList<T4> result4
         )
      {
         Result1 = result1;
         Result2 = result2;
         Result3 = result3;
         Result4 = result4;
      }

      public readonly DbResultList<T1> Result1;
      public readonly DbResultList<T2> Result2;
      public readonly DbResultList<T3> Result3;
      public readonly DbResultList<T4> Result4;
   }

   internal sealed class DbResultSet<T1, T2, T3, T4, T5>
   {
      public DbResultSet(
         DbResultList<T1> result1,
         DbResultList<T2> result2,
         DbResultList<T3> result3,
         DbResultList<T4> result4,
         DbResultList<T5> result5
         )
      {
         Result1 = result1;
         Result2 = result2;
         Result3 = result3;
         Result4 = result4;
         Result5 = result5;
      }

      public readonly DbResultList<T1> Result1;
      public readonly DbResultList<T2> Result2;
      public readonly DbResultList<T3> Result3;
      public readonly DbResultList<T4> Result4;
      public readonly DbResultList<T5> Result5;
   }
}
