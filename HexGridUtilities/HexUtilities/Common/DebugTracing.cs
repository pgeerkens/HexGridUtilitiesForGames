#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
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
#if DEBUG
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
#endif

namespace PGNapoleonics.HexUtilities.Common {
    /// <summary>TODO</summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
  public enum TraceFlags  {
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
#if DEBUG
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class SystemTime {
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort year;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort month;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort weekday;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort day;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort hour;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort minute;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort second;
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      public ushort millisecond;

      public override string ToString() {
        return string.Format(CultureInfo.InvariantCulture,"{0,2}:{1,2}:{2,2}.{3,3}", 
          this.hour, this.minute, this.second, this.millisecond);
      }
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), DllImport("Kernel32.dll")]
    internal static extern void GetSystemTime([In,Out] SystemTime st);
#endif
  }

  /// <summary>TODO</summary>
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
  public static partial class DebugTracing {
    /// <summary>TODO</summary>
    public static TraceFlags EnabledFags { get; set; }

    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void Trace(TraceFlags traceFlags, string format, params object[] args) {
      Trace(traceFlags, false, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void Trace(TraceFlags traceFlags, bool newLine, string format, params object[] args) {
      Trace(traceFlags, newLine, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "traceFlags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void Trace(TraceFlags traceFlags, bool newLine, string description) {
      TraceDetail(traceFlags, newLine, description);
    }

    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void LogTime(TraceFlags traceFlags, string format, params object[] args) {
      LogTime(traceFlags, false, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void LogTime(TraceFlags traceFlags, bool newLine, string format, params object[] args) {
      LogTime(traceFlags, newLine, string.Format(format,args));
    }
    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void LogTime(TraceFlags traceFlags, string description) {
      LogTime(traceFlags, false, description);
    }
    /// <summary>TODO</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "traceFlags"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public static void LogTime(TraceFlags traceFlags, bool newLine, string description) {
      LogTimeDetail(traceFlags, newLine, description);
    }

    static partial void TraceDetail(TraceFlags traceFlags, bool newLine, string description);

    static partial void LogTimeDetail(TraceFlags traceFlags, bool newLine, string description);
#if DEBUG
      static partial void TraceDetail(TraceFlags traceFlags, bool newLine, string description) {
        if (EnabledFags.HasFlag(traceFlags)) {
          if(newLine) Debug.WriteLine("");
          Debug.WriteLine(description);
        }
      }

    static string GetTimeString() {
      var st = new NativeMethods.SystemTime();
      NativeMethods.GetSystemTime(st); 
      return st.ToString();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
      static partial void LogTimeDetail(TraceFlags traceFlags, bool newLine, string description) {
        if (EnabledFags.HasFlag(traceFlags)) {
          if(newLine) Debug.WriteLine("");
          Debug.Write("{0} - ", GetTimeString());
          Trace(traceFlags, false, description);
        }
      }
#endif
  }

    /// <summary>TODO</summary>
  public static partial class Extensions {
    #if DEBUG
      public static void Trace(this TraceFlags @this, string format, params object[] args) {
        DebugTracing.Trace(@this,format,args);
      }
      public static void Trace(this TraceFlags @this, bool newLine, string format, params object[] args) {
        DebugTracing.Trace(@this,newLine,format,args);
      }
      public static void Trace(this TraceFlags @this, bool newLine, string description) {
        DebugTracing.Trace(@this,newLine,description);
      }

      public static void LogTime(this TraceFlags @this, string format, params object[] args) {
        DebugTracing.LogTime(@this,format,args);
      }
      public static void LogTime(this TraceFlags @this, bool newLine, string format, params object[] args) {
        DebugTracing.LogTime(@this,newLine,format,args);
      }
      public static void LogTime(this TraceFlags @this, string description) {
        DebugTracing.LogTime(@this,description);
      }
      public static void LogTime(this TraceFlags @this, bool newLine, string description) {
        DebugTracing.LogTime(@this,newLine,description);
      }
    #else
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      public static void Trace(this TraceFlags @this, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void Trace(this TraceFlags @this, bool newLine, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void Trace(this TraceFlags @this, bool newLine, string description) {}

      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format")]
      public static void LogTime(this TraceFlags @this, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "format"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newline")]
      public static void LogTime(this TraceFlags @this, bool newLine, string format, params object[] args) {}
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this")]
      public static void LogTime(this TraceFlags @this, string description) {}
      /// <summary>TODO</summary>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newLine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "this"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description")]
      public static void LogTime(this TraceFlags @this, bool newLine, string description) {}
    #endif
  }
}
