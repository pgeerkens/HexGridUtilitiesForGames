#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>enumerationof known debugging trace flags.</summary>
 [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
  [Flags]public enum Traces  {
    /// <summary>TODO</summary>
    None            = 0x00000,
    /// <summary>TODO</summary>
    Caching         = 0x00001,
    /// <summary>TODO</summary>
    FieldOfView     = 0x00002,
    /// <summary>TODO</summary>
    Mouse           = 0x00004,
    /// <summary>TODO</summary>
    MouseMove       = 0x00008,
    /// <summary>TODO</summary>
    MainForm        = 0x00010,
    /// <summary>TODO</summary>
    Initialization  = 0x00020,
    /// <summary>TODO</summary>
    Docking         = 0x00040,
    /// <summary>TODO</summary>
    MenuEvents      = 0x00080,
    /// <summary>TODO</summary>
    KeyEvents       = 0x00100,
    /// <summary>TODO</summary>
    Sizing          = 0x00200,
    /// <summary>TODO</summary>
    ScrollEvents    = 0x00400,
    /// <summary>TODO</summary>
    ToolTipEvents   = 0x00800,
    /// <summary>TODO</summary>
    Paint           = 0x01000,
    /// <summary>TODO</summary>
    PaintMap        = 0x02000,
    /// <summary>TODO</summary>
    PaintDetail     = 0x04000,
    /// <summary>TODO</summary>
    FindPathEnqueue = 0x08000,
    /// <summary>TODO</summary>
    FindPathDequeue = 0x10000,
    /// <summary>TODO</summary>
    FindPathDetail  = 0x20000,
    /// <summary>TODO</summary>
    FindPathShortcut= 0x40000
  }

  internal static partial class NativeMethods {
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class SystemTime {
      public ushort year;
      public ushort month;
      public ushort weekday;
      public ushort day;
      public ushort hour;
      public ushort minute;
      public ushort second;
      public ushort millisecond;

      public override string ToString() {
        return string.Format(CultureInfo.InvariantCulture,"{0,2}:{1,2}:{2,2}.{3,3}", 
          this.hour, this.minute, this.second, this.millisecond);
      }
    }
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [DllImport("Kernel32.dll")]
    internal static extern void GetSystemTime([In,Out] SystemTime st);
  }

  /// <summary>TODO</summary>
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
  public static partial class DebugTracing {
    /// <summary>TODO</summary>
    public static Traces EnabledTraces { get; set; }

    /// <summary>TODO</summary>
    public static void Trace(Traces traces, string format, params object[] args) {
      Trace(traces, false, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    public static void Trace(Traces traces, bool newLine, string format, params object[] args) {
      Trace(traces, newLine, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "traces")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
    public static void Trace(Traces traces, bool newLine, string description) {
      TraceDetail(traces, newLine, description);
    }

    /// <summary>TODO</summary>
    public static void LogTime(Traces traces, string format, params object[] args) {
      LogTime(traces, false, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    public static void LogTime(Traces traces, bool newLine, string format, params object[] args) {
      LogTime(traces, newLine, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    public static void LogTime(Traces traces, string description) {
      LogTime(traces, false, description);
    }
    /// <summary>TODO</summary>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "traces")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
    public static void LogTime(Traces traces, bool newLine, string description) {
      LogTimeDetail(traces, newLine, description);
    }

    static partial void TraceDetail(Traces traces, bool newLine, string description);
    [Conditional("TRACE")]
    static partial void TraceDetail(Traces traces, bool newLine, string description) {
      if (EnabledTraces.HasFlag(traces)) {
        if(newLine) System.Diagnostics.Trace.WriteLine("");
        System.Diagnostics.Trace.WriteLine(description);
      }
    }

    static partial void LogTimeDetail(Traces traces, bool newLine, string description);
    [Conditional("TRACE")]
    static partial void LogTimeDetail(Traces traces, bool newLine, string description) {
      if (EnabledTraces.HasFlag(traces)) {
        if(newLine) System.Diagnostics.Trace.WriteLine("");
        var st = new NativeMethods.SystemTime();
        NativeMethods.GetSystemTime(st);

        System.Diagnostics.Trace.Write("{0} - ", st.ToString());
        Trace(traces, false, description);
      }
    }
  }

    /// <summary>TODO</summary>
  public static partial class Extensions {
    #if TRACE
      public static void Trace(this Traces @this, string format, params object[] args) {
        DebugTracing.Trace(@this,format,args);
      }
      public static void Trace(this Traces @this, bool newLine, string format, params object[] args) {
        DebugTracing.Trace(@this,newLine,format,args);
      }
      public static void Trace(this Traces @this, bool newLine, string description) {
        DebugTracing.Trace(@this,newLine,description);
      }

      public static void LogTime(this Traces @this, string format, params object[] args) {
        DebugTracing.LogTime(@this,format,args);
      }
      public static void LogTime(this Traces @this, bool newLine, string format, params object[] args) {
        DebugTracing.LogTime(@this,newLine,format,args);
      }
      public static void LogTime(this Traces @this, string description) {
        DebugTracing.LogTime(@this,description);
      }
      public static void LogTime(this Traces @this, bool newLine, string description) {
        DebugTracing.LogTime(@this,newLine,description);
      }
    #else
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      public static void Trace(this Traces @this, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void Trace(this Traces @this, bool newLine, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void Trace(this Traces @this, bool newLine, string description) {}

      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      public static void LogTime(this Traces @this, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void LogTime(this Traces @this, bool newLine, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      public static void LogTime(this Traces @this, string description) {}
      /// <summary>TODO</summary>
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
      public static void LogTime(this Traces @this, bool newLine, string description) {}
    #endif
  }
}
