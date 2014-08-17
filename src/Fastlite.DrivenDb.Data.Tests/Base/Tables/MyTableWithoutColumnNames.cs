using System.ComponentModel;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   [DataContract]
   [DbTable(Name = "MyTable")]
   internal class MyTableWithoutColumnNames : DbEntity<MyTableWithoutColumnNames>, INotifyPropertyChanged
   {
      [DataMember]
      private long m_MyIdentity;

      [DataMember]
      private string m_MyString;

      [DataMember]
      private long m_MyNumber;

      [DbColumn(IsDbGenerated = true, IsPrimaryKey = true)]
      public long MyIdentity
      {
         get { return m_MyIdentity; }
         set
         {
            m_MyIdentity = value;
            PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false)]
      public string MyString
      {
         get { return m_MyString; }
         set
         {
            m_MyString = value;
            PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false)]
      public long MyNumber
      {
         get { return m_MyNumber; }
         set
         {
            m_MyNumber = value;
            PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
