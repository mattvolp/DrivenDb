using System.ComponentModel;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   [DataContract]
   [DbTable(Name = "MyChildren")]
   internal class MyChildren : DbEntity<MyChildren>, INotifyPropertyChanged
   {
      [DataMember]
      private long m_HisIdentity;

      [DataMember]
      private string m_MyString;

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = true, Name = "HisIdentity")]
      public long HisIdentity
      {
         get { return m_HisIdentity; }
         set
         {
            m_HisIdentity = value;
            PropertyChanged(this, new PropertyChangedEventArgs("HisIdentity"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
      public string MyString
      {
         get { return m_MyString; }
         set
         {
            m_MyString = value;
            PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
