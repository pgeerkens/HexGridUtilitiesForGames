#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PGNapoleonics.HexUtilities.Common {
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

            public override string ToString()
            => string.Format(CultureInfo.InvariantCulture,
                    $"{hour,2}:{minute,2}:{second,2}.{millisecond,3}");
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("Kernel32.dll")]
        internal static extern void GetSystemTime([In,Out] SystemTime st);
    }
}
