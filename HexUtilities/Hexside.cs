#region The MIT License - Copyright (C) 2012-2015 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2015 Pieter Geerkens (email: pgeerkens@hotmail.com)
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.FastLists;

namespace PGNapoleonics.HexUtilities {
  /// <summary>Enumeration of the six hexagonal directions.</summary>
  public class Hexside {

    #region Enum Constants & Constructor
    /// <summary>The hexside on the top of the hex.</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside North     = new Hexside(0, "North", Hexsides.North);

    /// <summary>The hexside on the upper-right of the hex.</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside Northeast = new Hexside(1, "Northeast", Hexsides.Northeast);

    /// <summary>The hexside on the lower-right of the hex</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside Southeast = new Hexside(2, "Southeast", Hexsides.Southeast);

    /// <summary>The hexside on the bottom of the hex.</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside South     = new Hexside(3, "South", Hexsides.South);

    /// <summary>The hexside on the lower-left of the hex.</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside Southwest = new Hexside(4, "Southwest", Hexsides.Southwest);

    /// <summary>The hexside on the upper-left of the hex.</summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly Hexside Northwest = new Hexside(5, "Northwest", Hexsides.Northwest);

    private Hexside(int value, string name, Hexsides hexsides) {
    //  Contract.Requires(0 <= value  &&  value < 6);
      name.RequiredNotNull("name");

      _hexsides = hexsides;
      _name     = name;
      _value    = value;
      _reversed = (_value + 3) % 6;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [ContractInvariantMethod] [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void ObjectInvariant() {
    //  Contract.Invariant(Name != null);
    //  Contract.Invariant(0 <= Value  &&  Value < 6);
    //  Contract.Invariant(0 <= (int)this  &&  (int)this < 6);

    //  Contract.Invariant(Reversed != null);
    //  Contract.Invariant(0 <= Reversed.Value  &&  Reversed.Value < 6);
    //  Contract.Invariant(0 <= (int)Reversed  &&  (int)Reversed < 6);
    }
    #endregion

    #region Static members
    /// <summary><c>Static List {Hexside}</c> for enumerations.</summary>
    public static IFastList<Hexside> HexsideList { get {
    //  Contract.Ensures(Contract.Result<IFastList<Hexside>>() != null);
      return _hexsideList;
    } }

    /// <summary>Performs <c>action</c> for each Enum value of <c>Hexside</c>.</summary>
    /// <param name="action"></param>
    public static void ForEach(Action<Hexside> action) {
      action.RequiredNotNull("action");
      _hexsideList.ForEach(action);
    }
    /// <summary>Perform the Invoke() method of <c>functor</c> for each value of Enum <c>Hexside</c>.</summary>
    /// <param name="functor"></param>
    public static void ForEach(FastIteratorFunctor<Hexside> functor) {
      functor.RequiredNotNull("functor");
      _hexsideList.ForEach(functor);
    }

    /// <summary>Returns the <see cref="Hexside"/> with this <paramref name="name"/>.</summary>
    /// <param name="name">The <see cref="Hexside"/> string to be parsed and recognized.</param>
    /// <param name="ignoreCase">Specifies whether or not a case-insensitive parse is desired.</param>
    public static Hexside ParseEnum(string name, bool ignoreCase) {
      name.RequiredNotNull("Hexside");
    //  Contract.Ensures(Contract.Result<Hexside>() != null);
    //  Contract.Assume(_hexsideList.All(item => item != null));

      var index = ignoreCase ? _namesUncased.IndexOf(name.ToUpper(CultureInfo.InvariantCulture))
                             : _namesCased.IndexOf(name);
      if (index == -1)
              throw new ArgumentOutOfRangeException("name",name,"Enum type: " + typeof(Hexside).Name);
      return _hexsideList[index];;
    }

    /// <summary>Returns the single <see cref="Hexsides"/> value corresponding to this <paramref name="hexside"/>.</summary>
    /// <param name="hexside">The supplied <see cref="Hexside"/>.</param>
    [Pure]public static implicit operator Hexsides(Hexside hexside) {
      hexside.RequiredNotNull("hexside");
      hexside.AssumeInvariant();
      return hexside._hexsides;
    }

    /// <summary>Enables implicit casting of a <see cref="Hexside"/> as a <see cref="System.Int32"/>.</summary>
    /// <param name="hexside">The supplied <see cref="Hexside"/>.</param>
    [Pure]public static implicit operator int(Hexside hexside) {
      hexside.RequiredNotNull("hexside");
    //  Contract.Ensures(0 <= Contract.Result<int>()  &&  Contract.Result<int>() < HexsideList.Count);
      hexside.AssumeInvariant();
      return hexside.Value;
    }

    private static readonly IFastList<Hexside>  _hexsideList = new Hexside[] {
      North,Northeast,Southeast,South,Southwest,Northwest }.ToFastList();

    private static readonly IList<string> _namesCased   = (from hexside in _hexsideList select hexside.Name).ToList();
    private static readonly IList<string> _namesUncased = (from name in _namesCased select name.ToUpper(CultureInfo.InvariantCulture)).ToList();
    #endregion

    #region Instance members
    /// <summary>The <c>Hexsides</c> bit corresponding to this <c>Hexside</c>.</summary>
    public Hexsides AsHexsides { get { return _hexsides; } } private readonly Hexsides _hexsides;
    /// <summary>The name of this enumeration constant.</summary>
    public string   Name       { get {Contract.Ensures(Contract.Result<string>() != null); return _name;     } } private readonly string _name;
    /// <summary>Returns the reversed, or opposite, <see cref="Hexside"/> to this one.</summary>
    public Hexside  Reversed   { get {
    //  Contract.Ensures(Contract.Result<Hexside>() != null);
      return _hexsideList[_reversed];
    } } private readonly int _reversed;
    /// <summary>The integer value for this enumeration constant.</summary>
    public int      Value      { get { return _value;    } } private readonly int _value;

    /// <inheritdoc/>
    public  override string ToString() { return Name; }
    #endregion
  }
}
