
namespace Fastlite.DrivenDb
{
   public sealed class DbRecordSet<T>
   {      
      public DbRecordSet(
         DbRecordList<T> records
         )
      {
         Records = records;
      }

      public readonly DbRecordList<T> Records;
   }

   public sealed class DbRecordSet<T1, T2>
   {
      public DbRecordSet(
         DbRecordList<T1> result1,
         DbRecordList<T2> result2
         )
      {
         Record1 = result1;
         Record2 = result2;
      }

      public readonly DbRecordList<T1> Record1;
      public readonly DbRecordList<T2> Record2;
   }

   public sealed class DbRecordSet<T1, T2, T3>
   {
      public DbRecordSet(
         DbRecordList<T1> result1,
         DbRecordList<T2> result2,
         DbRecordList<T3> result3
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
      }

      public readonly DbRecordList<T1> Record1;
      public readonly DbRecordList<T2> Record2;
      public readonly DbRecordList<T3> Record3;
   }

   public sealed class DbRecordSet<T1, T2, T3, T4>
   {
      public DbRecordSet(
         DbRecordList<T1> result1,
         DbRecordList<T2> result2,
         DbRecordList<T3> result3,
         DbRecordList<T4> result4
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
      }

      public readonly DbRecordList<T1> Record1;
      public readonly DbRecordList<T2> Record2;
      public readonly DbRecordList<T3> Record3;
      public readonly DbRecordList<T4> Record4;
   }

   public sealed class DbRecordSet<T1, T2, T3, T4, T5>
   {
      public DbRecordSet(
         DbRecordList<T1> result1,
         DbRecordList<T2> result2,
         DbRecordList<T3> result3,
         DbRecordList<T4> result4,
         DbRecordList<T5> result5
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
         Record5 = result5;
      }

      public readonly DbRecordList<T1> Record1;
      public readonly DbRecordList<T2> Record2;
      public readonly DbRecordList<T3> Record3;
      public readonly DbRecordList<T4> Record4;
      public readonly DbRecordList<T5> Record5;
   }
}
