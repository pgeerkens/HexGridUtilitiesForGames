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
using System.Diagnostics;

namespace PGNapoleonics.HexUtilities.FieldOfView {
    /// <summary>TODO</summary>
    [DebuggerDisplay("RiseRun: ({Rise} over {Run})")]
    public struct RiseRun : IEquatable<RiseRun>, IComparable<RiseRun> {
        /// <summary>Creates a new instance of the RiseRUn struct.</summary>
        /// <param name="rise"></param>
        /// <param name="run"></param>
        internal RiseRun(int rise, int run) : this() {
            Rise = rise;
            Run  = run;
        }

        /// <summary>Delta-height in units of elevation: feet.</summary>
        public int Rise { get; }
        /// <summary>Delta-width in units of distance:  hexes.</summary>
        public int Run  { get; }

        #region Operators and  IComparable<RiseRun> implementations: 
        /// <summary>Less Than operator</summary>
        public static bool operator <  (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) <  0;

        /// <summary>Less Than or Equals operator</summary>
        public static bool operator <= (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) <= 0;

        /// <summary>Greater Thanoperator</summary>
        public static bool operator >  (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) >  0;

        /// <summary>Greater Than or Equals operator</summary>
        public static bool operator >= (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) >= 0;

        /// <summary>Less-Than comparaator.</summary>
        public int CompareTo(RiseRun other) => (Rise * other.Run).CompareTo(other.Rise * Run);
        
        #endregion

        #region Value Equality with IEquatable<T>
        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is RiseRun other) && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(RiseRun other) => Rise == other.Rise && Run == other.Run;

        /// <inheritdoc/>
        public override int GetHashCode() => Run != 0 ? (Rise / Run).GetHashCode() 
                                                      : int.MaxValue.GetHashCode();

        /// <summary>Tests value-inequality.</summary>
        public static bool operator != (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) != 0;

        /// <summary>Tests value-equality.</summary>
        public static bool operator == (RiseRun lhs, RiseRun rhs) => lhs.CompareTo(rhs) == 0;
        #endregion

        /// <inheritdoc/>
        public override string ToString() => $"Rise={Rise}; Run={Run}";
    }
}
