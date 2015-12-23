using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal
{
   internal class ScriptLines 
      : IEnumerable<ScriptLine>
   {      
      private readonly List<ScriptLine> _lines = new List<ScriptLine>();
      
      public void Add(string line) 
      {
         _lines.Add(new ScriptLine(line));
      }
      
      public void Add(string line, params ScriptingOptions[] options)
      {         
         _lines.Add(new ScriptLine(line, options)); 
      }
      
      public IEnumerator<ScriptLine> GetEnumerator()
      {
         return _lines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   internal class SegmentCollection
      : IEnumerable<ScriptSegment>
   {
      private readonly IEnumerable<ScriptSegment> _segments;
      
      public SegmentCollection()
      {
         _segments = new ScriptSegment[0];
      }

      public SegmentCollection(IEnumerable<ScriptSegment> segments)
      {       
         _segments = segments;
      }

      public SegmentCollection Append(ScriptSegment segment)
      {
         return new SegmentCollection(_segments.Concat(new[] { segment }));
      }

      public SegmentCollection AppendForEach<T>(IEnumerable<T> items, ValueExtractor<T> values, ScriptSegment segment)
      {
         items.Select()
         return new SegmentCollection(_segments.Concat(new[] { segment }));
      }

      public IEnumerator<ScriptSegment> GetEnumerator()
      {
         return _segments.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   internal class ScriptSegment
      : IEnumerable<ScriptLine>
   {
      private readonly List<ScriptLine> _lines = new List<ScriptLine>();
      public readonly string[] Args;

      public ScriptSegment(params string[] args)
      {
         Args = args;
      }
      
      public void Add(string line)
      {
         _lines.Add(new ScriptLine(line));
      }

      public void Add(string line, params ScriptingOptions[] options)
      {
         _lines.Add(new ScriptLine(line, options));
      }
      public IEnumerator<ScriptLine> GetEnumerator()
      {
         return _lines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   internal delegate Script<R> Scripter<T, R>(T t, ScriptingOptions so, SegmentCollection sc);

   internal class Script<T>
   {
      private readonly T _t;
      private readonly ScriptingOptions _options;
      public readonly SegmentCollection Segments; // TODO: public?
      
      public Script(T t, ScriptingOptions options, SegmentCollection segments)
      {
         _t = t;
         _options = options;
         Segments = segments;
      }

      public Script<T> Bind(IScripter<T> scripter)
      {
         return scripter.Script(_t, _options, Segments);
      }

      public Script<R> Bind<R>(Scripter<T,R> func)
      {
         return func(_t, _options, Segments);
      }

      public Script<T> BindIf(Func<T, ScriptingOptions, SegmentCollection, bool> condition, Scripter<T,T> func)
      {
         return condition(_t, _options, Segments)
            ? func(_t, _options, Segments)
            : this;
      }
   }
}