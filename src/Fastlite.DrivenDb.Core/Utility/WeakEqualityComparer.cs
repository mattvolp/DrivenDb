/**************************************************************************************
 * Original Author : Anthony Leatherwood (fastlite@outlook.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb     
 *  
 * This source is subject to the Mozilla Public License, version 2.0.
 * Link: https://github.com/Fastlite/DrivenDb/blob/master/LICENSE
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System.Collections.Generic;

namespace Fastlite.DrivenDb.Core.Utility
{
    class WeakEqualityComparer<T> : IEqualityComparer<Weak<T>>
    {
        public bool Equals(Weak<T> x, Weak<T> y)
        {            
            if (x.IsAlive && x.Target.Equals(y.Target))
            {
                return true;
            }

            if (y.IsAlive && y.Target.Equals(x.Target))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(Weak<T> obj)
        {
            return obj.IsAlive
                ? obj.Target.GetHashCode()
                : 0;
        }
    }
}
