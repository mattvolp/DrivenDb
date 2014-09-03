
namespace Fastlite.DrivenDb
{
   public sealed class DbRecordSet<T>
   {      
      public DbRecordSet(
         DbRecordCollection<T> records
         )
      {
         Records = records;
      }

      public readonly DbRecordCollection<T> Records;
   }

   internal sealed class DbRecordSet<T1, T2>
   {
      public DbRecordSet(
         DbRecordCollection<T1> result1,
         DbRecordCollection<T2> result2
         )
      {
         Record1 = result1;
         Record2 = result2;
      }

      public readonly DbRecordCollection<T1> Record1;
      public readonly DbRecordCollection<T2> Record2;
   }

   internal sealed class DbRecordSet<T1, T2, T3>
   {
      public DbRecordSet(
         DbRecordCollection<T1> result1,
         DbRecordCollection<T2> result2,
         DbRecordCollection<T3> result3
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
      }

      public readonly DbRecordCollection<T1> Record1;
      public readonly DbRecordCollection<T2> Record2;
      public readonly DbRecordCollection<T3> Record3;
   }

   internal sealed class DbRecordSet<T1, T2, T3, T4>
   {
      public DbRecordSet(
         DbRecordCollection<T1> result1,
         DbRecordCollection<T2> result2,
         DbRecordCollection<T3> result3,
         DbRecordCollection<T4> result4
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
      }

      public readonly DbRecordCollection<T1> Record1;
      public readonly DbRecordCollection<T2> Record2;
      public readonly DbRecordCollection<T3> Record3;
      public readonly DbRecordCollection<T4> Record4;
   }

   internal sealed class DbRecordSet<T1, T2, T3, T4, T5>
   {
      public DbRecordSet(
         DbRecordCollection<T1> result1,
         DbRecordCollection<T2> result2,
         DbRecordCollection<T3> result3,
         DbRecordCollection<T4> result4,
         DbRecordCollection<T5> result5
         )
      {
         Record1 = result1;
         Record2 = result2;
         Record3 = result3;
         Record4 = result4;
         Record5 = result5;
      }

      public readonly DbRecordCollection<T1> Record1;
      public readonly DbRecordCollection<T2> Record2;
      public readonly DbRecordCollection<T3> Record3;
      public readonly DbRecordCollection<T4> Record4;
      public readonly DbRecordCollection<T5> Record5;
   }
}
