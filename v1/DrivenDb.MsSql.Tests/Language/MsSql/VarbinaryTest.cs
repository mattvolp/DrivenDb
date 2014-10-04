using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace DrivenDb.MsSql.Tests.Language.MsSql
{
   [DataContract]
   [Table(Name = "VarbinaryTest")]
   public partial class VarbinaryTest : DbEntity<VarbinaryTest>, INotifyPropertyChanged
   {
      [DataMember]
      private int m_Id;
      [DataMember]
      private byte[] m_Value1;
      [DataMember]
      private byte[] m_Value2;
      [DataMember]
      private string m_Value3;

      [Column(Name = "Id", DbType = "int", IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
      public int Id
      {
         get { return m_Id; }
         set { m_Id = value; }
      }

      [Column(Name = "Value1", DbType = "varbinary", IsPrimaryKey = false, IsDbGenerated = false, CanBeNull = false)]
      public byte[] Value1
      {
         get { return m_Value1; }
         set
         {
            if (value == m_Value1) return;
            m_Value1 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Value1"));
         }
      }

      [Column(Name = "Value2", DbType = "varbinary", IsPrimaryKey = false, IsDbGenerated = false, CanBeNull = true)]
      public byte[] Value2
      {
         get { return m_Value2; }
         set
         {
            if (value == m_Value2) return;
            m_Value2 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Value2"));
         }
      }

      [Column(Name = "Value3", DbType = "varchar", IsPrimaryKey = false, IsDbGenerated = false, CanBeNull = true)]
      public string Value3
      {
         get { return m_Value3; }
         set
         {
            if (value == m_Value3) return;
            m_Value3 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Value3"));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
