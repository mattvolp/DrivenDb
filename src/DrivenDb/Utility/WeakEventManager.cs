/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb    
 *  
 * This source is subject to the Microsoft Public License.
 * Link: http://www.microsoft.com/en-us/openness/licenses.aspx
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace DrivenDb.Utility
{
    internal class WeakEventManager<T>        
        where T : EventArgs
    {
        private readonly HashSet<Weak<EventHandler<T>>> m_Handlers = new HashSet<Weak<EventHandler<T>>>(new WeakEqualityComparer<EventHandler<T>>());

        public void Add(EventHandler<T> handler)
        {
            m_Handlers.Add(new Weak<EventHandler<T>>(handler));
        }

        public void Remove(EventHandler<T> handler)
        {
            m_Handlers.Remove(new Weak<EventHandler<T>>(handler));
        }

        public void Invoke(object sender, T args)
        {
            foreach (var handler in m_Handlers.ToArray())
            {
                if (!handler.IsAlive)
                {
                    m_Handlers.Remove(handler);
                }
                else
                {
                    handler.Target.Invoke(sender, args);
                }
            }
        }
    }
}
