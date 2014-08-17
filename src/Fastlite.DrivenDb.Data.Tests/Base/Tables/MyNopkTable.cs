using System.ComponentModel;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   [DataContract]
   [DbTable(Name = "MyNopkTable")]
   internal class MyNopkTable : DbEntity<MyNopkTable>, INotifyPropertyChanged
   {
      [DataMember]
      private string m_MyString;

      [DataMember]
      private long m_MyNumber;

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
