using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrivenDb.Language.Interfaces
{
   public interface IParamConvertible
   {
      object ToParameterValue();
   }
}
