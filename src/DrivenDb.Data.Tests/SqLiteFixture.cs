using DrivenDb.Data.Internal;
using DrivenDb.Testing;

namespace DrivenDb.Data.Tests
{
   internal class SqLiteFixture
      
   {
      private readonly DbWriter _writer;
      private readonly SqLitePublisher _publisher;

      public SqLiteFixture()
      {
         _writer = new DbWriter();
         _publisher = new SqLitePublisher(_writer);
      }
   }
}
