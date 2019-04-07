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
using System.Globalization;
using System.Linq;

using PGNapoleonics.HexUtilities.FastList;

namespace PGNapoleonics.HexUtilities {
    /// <summary>Enumeration of the six hexagonal directions.</summary>
    public class Hexside {
        #region Enum Constants & Constructor
        /// <summary>The hexside on the top of the hex.</summary>
        public static Hexside North     { get; } = new Hexside(0, "North", Hexsides.North);

        /// <summary>The hexside on the upper-right of the hex.</summary>
        public static Hexside Northeast { get; } = new Hexside(1, "Northeast", Hexsides.Northeast);

        /// <summary>The hexside on the lower-right of the hex</summary>
        public static Hexside Southeast { get; } = new Hexside(2, "Southeast", Hexsides.Southeast);

        /// <summary>The hexside on the bottom of the hex.</summary>
        public static Hexside South     { get; } = new Hexside(3, "South", Hexsides.South);

        /// <summary>The hexside on the lower-left of the hex.</summary>
        public static Hexside Southwest { get; } = new Hexside(4, "Southwest", Hexsides.Southwest);

        /// <summary>The hexside on the upper-left of the hex.</summary>
        public static readonly Hexside Northwest = new Hexside(5, "Northwest", Hexsides.Northwest);

        private Hexside(int value, string name, Hexsides hexsides) {
            AsHexsides = hexsides;
            Name     = name;
            Value    = value;
            _reversed = (Value + 3) % 6;
        }
        #endregion

        #region Static members
        /// <summary><c>Static List {Hexside}</c> for enumerations.</summary>
        public static IFastList<Hexside> HexsideList { get; } = new Hexside[] {
            North,Northeast,Southeast,South,Southwest,Northwest }.ToFastList();

        /// <summary>Performs <c>action</c> for each Enum value of <c>Hexside</c>.</summary>
        /// <param name="action"></param>
        public static void ForEach(Action<Hexside> action) => HexsideList.ForEach(action);

        /// <summary>Perform the Invoke() method of <c>functor</c> for each value of Enum <c>Hexside</c>.</summary>
        /// <param name="functor"></param>
        public static void ForEach(FastIteratorFunctor<Hexside> functor) => HexsideList.ForEach(functor);

        /// <summary>Returns the <see cref="Hexside"/> with this <paramref name="name"/>.</summary>
        /// <param name="name">The <see cref="Hexside"/> string to be parsed and recognized.</param>
        /// <param name="ignoreCase">Specifies whether or not a case-insensitive parse is desired.</param>
        public static Hexside ParseEnum(string name, bool ignoreCase) {
            var index = ignoreCase ? _namesUncased.IndexOf(name.ToUpper(CultureInfo.InvariantCulture))
                                   : _namesCased.IndexOf(name);
            if (index == -1) throw new ArgumentOutOfRangeException("name",name,"Enum type: " + typeof(Hexside).Name);

            return HexsideList[index];;
        }

        /// <summary>Returns the single <see cref="Hexsides"/> value corresponding to this <paramref name="hexside"/>.</summary>
        /// <param name="hexside">The supplied <see cref="Hexside"/>.</param>
        public static implicit operator Hexsides(Hexside hexside) => hexside.AsHexsides;

        /// <summary>Enables implicit casting of a <see cref="Hexside"/> as a <see cref="int"/>.</summary>
        /// <param name="hexside">The supplied <see cref="Hexside"/>.</param>
        public static implicit operator int(Hexside hexside) => hexside.Value;

        private static readonly IList<string> _namesCased   = (from hexside in HexsideList select hexside.Name).ToList();
        private static readonly IList<string> _namesUncased = (from name in _namesCased select name.ToUpper(CultureInfo.InvariantCulture)).ToList();
        #endregion

        #region Instance members
        /// <summary>The <c>Hexsides</c> bit corresponding to this <c>Hexside</c>.</summary>
        public Hexsides AsHexsides { get; }

        /// <summary>The name of this enumeration constant.</summary>
        public string   Name       { get; }

        /// <summary>Returns the reversed, or opposite, <see cref="Hexside"/> to this one.</summary>
        public Hexside  Reversed   => HexsideList[_reversed]; private readonly int _reversed;

        /// <summary>The integer value for this enumeration constant.</summary>
        public int      Value      { get; }

        /// <inheritdoc/>
        public  override string ToString() => Name;
        #endregion
    }
}
