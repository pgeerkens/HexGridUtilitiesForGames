#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>enumerationof known debugging trace flags.</summary>
  public partial struct Tracing {
    #region Named TraceFlags
    private static int _bits;
    /// <summary>TODO</summary>
    private static readonly IDictionary<string,Tracing> Collection = new Dictionary<string,Tracing>() {
      { "None",             new Tracing(0) },
      { "Caching",          new Tracing(_bits  = 1) },
      { "FieldOfView",      new Tracing(_bits *= 2) },
      { "Mouse",            new Tracing(_bits *= 2) },
      { "MouseMove",        new Tracing(_bits *= 2) },
      { "MainForm",         new Tracing(_bits *= 2) },
      { "Initialization",   new Tracing(_bits *= 2) },
      { "Docking",          new Tracing(_bits *= 2) },
      { "MenuEvents",       new Tracing(_bits *= 2) },
      { "KeyEvents",        new Tracing(_bits *= 2) },
      { "Sizing",           new Tracing(_bits *= 2) },
      { "ScrollEvents",     new Tracing(_bits *= 2) },
      { "ToolTipEvents",    new Tracing(_bits *= 2) },
      { "Paint",            new Tracing(_bits *= 2) },
      { "PaintMap",         new Tracing(_bits *= 2) },
      { "PaintDetail",      new Tracing(_bits *= 2) },
      { "FindPathEnqueue",  new Tracing(_bits *= 2) },
      { "FindPathDequeue",  new Tracing(_bits *= 2) },
      { "FindPathDetail",   new Tracing(_bits *= 2) },
      { "FindPathShortcut", new Tracing(_bits *= 2) }
    };

    /// <summary>TODO</summary>
    public static readonly Tracing None             = Item("None");
    /// <summary>TODO</summary>
    public static readonly Tracing Caching          = Item("Caching");
    /// <summary>TODO</summary>
    public static readonly Tracing FieldOfView      = Item("FieldOfView");
    /// <summary>TODO</summary>
    public static readonly Tracing Mouse            = Item("Mouse");
    /// <summary>TODO</summary>
    public static readonly Tracing MouseMove        = Item("MouseMove");
    /// <summary>TODO</summary>
    public static readonly Tracing MainForm         = Item("MainForm");
    /// <summary>TODO</summary>
    public static readonly Tracing Initialization   = Item("Initialization");
    /// <summary>TODO</summary>
    public static readonly Tracing Docking          = Item("Docking");
    /// <summary>TODO</summary>
    public static readonly Tracing MenuEvents       = Item("MenuEvents");
    /// <summary>TODO</summary>
    public static readonly Tracing KeyEvents        = Item("KeyEvents");
    /// <summary>TODO</summary>
    public static readonly Tracing Sizing           = Item("Sizing");
    /// <summary>TODO</summary>
    public static readonly Tracing ScrollEvents     = Item("ScrollEvents");
    /// <summary>TODO</summary>
    public static readonly Tracing ToolTipEvents    = Item("ToolTipEvents");
    /// <summary>TODO</summary>
    public static readonly Tracing Paint            = Item("Paint");
    /// <summary>TODO</summary>
    public static readonly Tracing PaintMap         = Item("PaintMap");
    /// <summary>TODO</summary>
    public static readonly Tracing PaintDetail      = Item("PaintDetail");
    /// <summary>TODO</summary>
    public static readonly Tracing FindPathEnqueue  = Item("FindPathEnqueue");
    /// <summary>TODO</summary>
    public static readonly Tracing FindPathDequeue  = Item("FindPathDequeue");
    /// <summary>TODO</summary>
    public static readonly Tracing FindPathDetail   = Item("FindPathDetail");
    /// <summary>TODO</summary>
    public static readonly Tracing FindPathShortcut = Item("FindPathShortcut");
    #endregion

    /// <summary>TODO</summary>
    public static void ForEachKey(Action<string> action, Func<string,bool> predicate) {
      foreach(var item in Tracing.Collection.Select(t => t.Key).Where(t => predicate(t))) action(item);
    }
    /// <summary>TODO</summary>
    public static void ForEachValue(Action<Tracing> action, Func<Tracing,bool> predicate) {
      foreach(var item in Tracing.Collection.Select(t => t.Value).Where(t => predicate(t))) action(item);
    }
    /// <summary>TODO</summary>
    public static Tracing Item(string name) {
      return Collection[name];
    }
    /// <summary>TODO</summary>
    public static Tracing EnabledTraces { get; set; }

    private Tracing(int enumValue) : this() { Value = enumValue; }

    /// <summary>TODO</summary>
    private bool HasFlag(Tracing bits) { return (this & bits) != Tracing.None; }

    /// <summary>TODO</summary>
    public  int  Value { get; private set; }

    /// <summary>TODO</summary>
    [Conditional("TRACE")]
    public  void Trace(string format) => Trace(false, format);

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
    [Conditional("TRACE")]
    public  void Trace(string format, params object[] args)
    =>  Trace(false, string.Format(CultureInfo.CurrentCulture,format,args));
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
    [Conditional("TRACE")]
    public  void Trace(bool newLine, string format, params object[] args)
     => Trace(newLine, string.Format(CultureInfo.CurrentCulture,format,args));

    #region Operators
    /// <summary>TODO</summary>
    public static Tracing operator & (Tracing lhs, Tracing rhs) {return new Tracing(lhs.Value & rhs.Value);}
    /// <summary>TODO</summary>
    public static Tracing operator | (Tracing lhs, Tracing rhs) {return new Tracing(lhs.Value | rhs.Value);}
    /// <summary>TODO</summary>
    public static Tracing operator ~ (Tracing lhs)            {return new Tracing(~lhs.Value);}

    /// <summary>TODO</summary>
    public static Tracing operator + (Tracing lhs, Tracing rhs) {return new Tracing((lhs |  rhs).Value);}
    /// <summary>TODO</summary>
    public static Tracing operator - (Tracing lhs, Tracing rhs) {return new Tracing((lhs & ~rhs).Value);}

    /// <summary>TODO</summary>
    public static Tracing BitwiseAnd    (Tracing lhs, Tracing rhs) { return (lhs & rhs); }
    /// <summary>TODO</summary>
    public static Tracing BitwiseOr     (Tracing lhs, Tracing rhs) { return (lhs | rhs); }
    /// <summary>TODO</summary>
    public static Tracing OnesComplement(Tracing lhs)                { return ~lhs; }

    /// <summary>TODO</summary>
    public static Tracing Add           (Tracing lhs, Tracing rhs) { return (lhs + rhs); }
    /// <summary>TODO</summary>
    public static Tracing Subtract      (Tracing lhs, Tracing rhs) { return (lhs - rhs); }
    #endregion

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
    [Conditional("TRACE")]
    public void Trace(bool newLine, string description) =>  TraceDetail(newLine, description);

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
    [Conditional("TRACE")]
    public void LogTime(string format, params object[] args)
    =>  LogTime(false, string.Format(CultureInfo.CurrentCulture,format,args));

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
    [Conditional("TRACE")]
    public void LogTime(bool newLine, string format, params object[] args)
    =>  LogTime(newLine, string.Format(CultureInfo.CurrentCulture,format,args));

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
    [Conditional("TRACE")]
    public void LogTime(string description) =>  LogTime(false, description);

    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
    [Conditional("TRACE")]
    public void LogTime(bool newLine, string description)
    =>  LogTimeDetail(newLine, description);

    partial void TraceDetail(bool newLine, string description);
    partial void LogTimeDetail(bool newLine, string description);

    [Conditional("TRACE")]
    partial void TraceDetail(bool newLine, string description) {
      if (HasFlag(Tracing.EnabledTraces)) {
        if(newLine) System.Diagnostics.Trace.WriteLine("");
        System.Diagnostics.Trace.WriteLine(description);
      }
    }

    [Conditional("TRACE")]
    partial void LogTimeDetail(bool newLine, string description) {
      if (HasFlag(Tracing.EnabledTraces)) {
        if(newLine) System.Diagnostics.Trace.WriteLine("");
        var st = new NativeMethods.SystemTime();
        NativeMethods.GetSystemTime(st);

        System.Diagnostics.Trace.Write("{0} - ", st.ToString());
        Trace(false, description);
      }
    }

    #region Value Equality
    /// <inheritdoc/>
    public override bool Equals(object obj) { 
      var other = obj as Tracing?;
      return other.HasValue  &&  this == other.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() { return Value.GetHashCode(); }

    /// <inheritdoc/>
    public bool Equals(Tracing other) { return this == other; }

    /// <summary>Tests value-inequality.</summary>
    public static bool operator != (Tracing lhs, Tracing rhs) { return ! (lhs == rhs); }

    /// <summary>Tests value-equality.</summary>
    public static bool operator == (Tracing lhs, Tracing rhs) { return (lhs.Value == rhs.Value); }
    #endregion
  }
}
