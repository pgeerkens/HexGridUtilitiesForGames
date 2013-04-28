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
  public enum TraceFlag {
    None           = 0x0000,
    Painting       = 0x0001,
    FieldOfView    = 0x0002,
    Mouse          = 0x0004,
    MouseMove      = 0x0008,
    MainForm       = 0x0010,
    FindPath       = 0x0020,
    Docking        = 0x0040,
    MenuEvents     = 0x0080,
    KeyEvents      = 0x0100,
    Sizing         = 0x0200,
    ScrollEvents   = 0x0400,
    ToolTipEvents  = 0x0800,
    Caching        = 0x1000,
    Initialization = 0x2000
  }

  public sealed class DebugTracing {
    [DllImport("Kernel32.dll")]
    private static extern void GetSystemTime([In,Out] _SystemTime st);

    public static TraceFlag EnabledFags { get; set; }

    public static void Trace(TraceFlag flags, string format, params object[] args) {
      #if DEBUG
        Trace(flags, false, string.Format(format,args));
      #endif
    }
    public static void Trace(TraceFlag flags, bool newline, string format, params object[] args) {
      #if DEBUG
        Trace(flags, newline, string.Format(format,args));
      #endif
    }
    public static void Trace(TraceFlag flags, bool newline, string description) {
      #if DEBUG
        if (EnabledFags.HasFlag(flags)) {
          if(newline) Debug.WriteLine("");
          Debug.WriteLine(description);
        }
      #endif
    }

    public static void LogTime(TraceFlag flags, string description) {
      #if DEBUG
        LogTime(flags, false, description);
      #endif
    }
    public static void LogTime(TraceFlag flags, string format, params object[] args) {
      #if DEBUG
        LogTime(flags, false, string.Format(format,args));
      #endif
    }
    public static void LogTime(TraceFlag flags, bool newline, string description) {
      #if DEBUG
        if (EnabledFags.HasFlag(flags)) {
          if(newline) Debug.WriteLine("");
          Debug.Write("{0} - ", GetTimeString());
          Trace(flags, false, description);
        }
      #endif
    }

    public static void LogTime(TraceFlag flags, bool newline, string format, params object[] args) {
      #if DEBUG
        LogTime(flags, newline, string.Format(format,args));
      #endif
    }

    public static string GetTimeString() {
      var st = new _SystemTime();
      GetSystemTime(st); 
      return st.ToString();
    }
    //private static _SystemTime GetTime() {
    //  var st = new _SystemTime();
    //  GetSystemTime(st); 
    //  return st;
    //}
  }
}
