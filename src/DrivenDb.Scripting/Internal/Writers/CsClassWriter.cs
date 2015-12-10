using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsClassWriter
   {
      private readonly CsConstructorWriter _constructor;
      private readonly CsFieldWriter _fields;
      private readonly CsKeyPropertyWriter _keyProperty;
      private readonly CsPropertyWriter _properties;
      private readonly CsPartialWriter _partials;
      private readonly CsPropertyChangingWriter _propertyChanging;
      private readonly CsPropertyChangedWriter _propertyChanged;
      private readonly CsValidationWriter _validation;

      public CsClassWriter(
           CsConstructorWriter constructor
         , CsFieldWriter fields
         , CsKeyPropertyWriter keyProperty
         , CsPropertyWriter properties
         , CsPartialWriter partials
         , CsPropertyChangingWriter propertyChanging
         , CsPropertyChangedWriter propertyChanged
         , CsValidationWriter validation
         )
      {
         _constructor = constructor;
         _fields = fields;
         _keyProperty = keyProperty;
         _properties = properties;
         _partials = partials;
         _propertyChanging = propertyChanging;
         _propertyChanged = propertyChanged;
         _validation = validation;
      }

      public void Write(ScriptTarget target, TableMap table)
      {
         target.WriteLines(new ScriptLines()
            {
               {"                                                                   "},
               {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
               {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
               {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
               {"    public partial class $1 $2                                     "},
               {"    {                                                              "},
               {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking},
               {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
            }
            , table.Detail.Schema
            , table.Detail.Name
            , target.Options.ScriptAsDelimitedImpliedInterfaces()
            );

         _constructor.Write(target, table);
         _fields.Write(target, table.Columns);
         _keyProperty.Write(target, table);
         _properties.Write(target, table.Columns);
         _partials.Write(target, table.Columns);
         _propertyChanging.Write(target);
         _propertyChanged.Write(target);
         _validation.Write(target, table);

         target.WriteLineAndContinue("    }");
      }

      //public static void WriteClassClose(ScriptTarget target)
      //{
      //   target.WriteLineAndContinue("    }");
      //}
   }
}
