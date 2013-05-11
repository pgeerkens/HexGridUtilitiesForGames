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
using System.Diagnostics;
using System.Runtime.InteropServices;     // For StructLayout, DllImport

namespace PG_Napoleonics.Utilities {
  [StructLayout(LayoutKind.Sequential)]
  public sealed class _SystemTime {
    public ushort year;
    public ushort month;
    public ushort weekday;
    public ushort day;
    public ushort hour;
    public ushort minute;
    public ushort second;
    public ushort millisecond;

    public override string ToString() {
      return string.Format("{0,2}:{1,2}:{2,2}.{3,3}", 
        this.hour, this.minute, this.second, this.millisecond);
    }
  }
  [Flags]
  public enum TraceFlag  {
     None           = 0x0000
    ,Painting       = 0x0001
    ,FieldOfView    = 0x0002
    ,Mouse          = 0x0004
    ,MouseMove      = 0x0008
    ,MainForm       = 0x0010
    ,FindPath       = 0x0020
    ,Docking        = 0x0040
    ,MenuEvents     = 0x0080
    ,KeyEvents      = 0x0100
    ,Sizing         = 0x0200
    ,ScrollEvents   = 0x0400
    ,ToolTipEvents  = 0x0800
    ,Caching        = 0x1000
    ,Initialization = 0x2000
    ,Solo          = Int32.MinValue // 0x40000000
  }

  public sealed partial class DebugTracing {
    [DllImport("Kernel32.dll")]
    private static extern void GetSystemTime([In,Out] _SystemTime st);

    public static TraceFlag EnabledFags { get; set; }

    public static string GetTimeString() {
      var st = new _SystemTime();
      GetSystemTime(st); 
      return st.ToString();
    }

  #if DEBUG
      public static void Trace(TraceFlag flags, string format, params object[] args) {
        Trace(flags, false, string.Format(format,args));
      }
      public static void Trace(TraceFlag flags, bool newline, string format, params object[] args) {
        Trace(flags, newline, string.Format(format,args));
      }
      public static void Trace(TraceFlag flags, bool newline, string description) {
        if (EnabledFags.HasFlag(flags)) {
          if(newline) Debug.WriteLine("");
          Debug.WriteLine(description);
        }
      }

      public static void LogTime(TraceFlag flags, string format, params object[] args) {
        LogTime(flags, false, string.Format(format,args));
      }
      public static void LogTime(TraceFlag flags, bool newline, string format, params object[] args) {
        LogTime(flags, newline, string.Format(format,args));
      }
      public static void LogTime(TraceFlag flags, string description) {
        LogTime(flags, false, description);
      }
      public static void LogTime(TraceFlag flags, bool newline, string description) {
        if (EnabledFags.HasFlag(flags)) {
          if(newline) Debug.WriteLine("");
          Debug.Write("{0} - ", GetTimeString());
          Trace(flags, false, description);
        }
      }
    #else
      public static void Trace(TraceFlag flags, string format, params object[] args) {}
      public static void Trace(TraceFlag flags, bool newline, string format, params object[] args) {}
      public static void Trace(TraceFlag flags, bool newline, string description) {}

      public static void LogTime(TraceFlag flags, string format, params object[] args) {}
      public static void LogTime(TraceFlag flags, bool newline, string format, params object[] args) {}
      public static void LogTime(TraceFlag flags, string description) {}
      public static void LogTime(TraceFlag flags, bool newline, string description) {}
    #endif
  }

  public static partial class Extensions {
    #if DEBUG
      public static void Trace(this TraceFlag @this, string format, params object[] args) {
        DebugTracing.Trace(@this,format,args);
      }
      public static void Trace(this TraceFlag @this, bool newline, string format, params object[] args) {
        DebugTracing.Trace(@this,newline,format,args);
      }
      public static void Trace(this TraceFlag @this, bool newline, string description) {
        DebugTracing.Trace(@this,newline,description);
      }

      public static void LogTime(this TraceFlag @this, string format, params object[] args) {
        DebugTracing.LogTime(@this,format,args);
      }
      public static void LogTime(this TraceFlag @this, bool newline, string format, params object[] args) {
        DebugTracing.LogTime(@this,newline,format,args);
      }
      public static void LogTime(this TraceFlag @this, string description) {
        DebugTracing.LogTime(@this,description);
      }
      public static void LogTime(this TraceFlag @this, bool newline, string description) {
        DebugTracing.LogTime(@this,newline,description);
      }
    #else
      public static void Trace(this TraceFlag @this, string format, params object[] args) {}
      public static void Trace(this TraceFlag @this, bool newline, string format, params object[] args) {}
      public static void Trace(this TraceFlag @this, bool newline, string description) {}

      public static void LogTime(this TraceFlag @this, string format, params object[] args) {}
      public static void LogTime(this TraceFlag @this, bool newline, string format, params object[] args) {}
      public static void LogTime(this TraceFlag @this, string description) {}
      public static void LogTime(this TraceFlag @this, bool newline, string description) {}
    #endif
  }
}
