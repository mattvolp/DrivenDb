using System.ComponentModel;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.MsSql.Tables
{
   [DataContract]
   [DbTable(Name = "TextTable")]
   public partial class TextTable : DbEntity<TextTable>, INotifyPropertyChanged
   {

      [DataMember]
      private int m_Id;
      [DataMember]
      private string m_Test;

      public TextTable()
      {
      }

      [DbColumn(Name = "Id", IsPrimaryKey = false, IsDbGenerated = true)]
      public int Id
      {
         get { return m_Id; }
         set { m_Id = value; }
      }

      [DbColumn(Name = "Test", IsPrimaryKey = false, IsDbGenerated = false)]
      public string Test
      {
         get { return m_Test; }
         set
         {
            m_Test = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Test"));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
