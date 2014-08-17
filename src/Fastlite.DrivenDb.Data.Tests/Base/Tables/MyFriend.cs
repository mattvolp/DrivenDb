using System.ComponentModel;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   [DataContract]
   [DbTable(Name = "MyFriend")]
   internal class MyFriend : DbEntity<MyFriend>, INotifyPropertyChanged
   {
      [DataMember]
      private int m_MyIdentity;

      [DataMember]
      private string m_MyString;

      [DataMember]
      private int m_MyNumber;

      [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
      public int MyIdentity
      {
         get { return m_MyIdentity; }
         set
         {
            m_MyIdentity = value;
            PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
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

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
      public int MyNumber
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
