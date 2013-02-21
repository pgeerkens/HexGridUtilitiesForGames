#region License - Copyright (C) 2012-2013 Pieter Geerkens, all rights reserved.
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
//
// Use of this software is permitted only as described in the attached file: license.txt.
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
    None         = 0x0000,
    Painting     = 0x0001,
    FieldOfView  = 0x0002,
    Mouse        = 0x0004,
    MouseMove    = 0x0008,
    MainForm     = 0x0010,
    FindPath     = 0x0020,
    Docking      = 0x0040,
    MenuEvents   = 0x0080,
    KeyEvents    = 0x0100,
    Sizing       = 0x0200,
    ScrollEvents = 0x0400,
    ToolTipEvents= 0x0800,
    Caching      = 0x1000
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

    private static string GetTimeString() {
      var st = new _SystemTime();
      GetSystemTime(st); 
      return st.ToString();
    }
    private static _SystemTime GetTime() {
      var st = new _SystemTime();
      GetSystemTime(st); 
      return st;;
    }
  }
}
