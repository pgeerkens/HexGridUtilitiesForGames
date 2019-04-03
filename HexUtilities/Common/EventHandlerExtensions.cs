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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace PGNapoleonics.HexUtilities.Common {
  /// <summary>TODO</summary>
  public static class EventHandlerExtensions {
    /// <summary>Raises an event in a standard (mostly thread-safe) manner, for a basic EventHandler.</summary>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
      Justification = "Not an event, just an event raiser.")]
    public static void Raise<T>(this EventHandler @this, object sender, T e)
      where T : EventArgs {
      var handler = @this;
      if (handler != null) handler(sender, e);
    }
    /// <summary>Raises an event in a standard (mostly thread-safe) manner, for a generic EventHandler.</summary>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
      Justification = "Not an event, just an event raiser.")]
    public static void Raise<T>(this EventHandler<T> @this, object sender, T e)
      where T : EventArgs {
      var handler = @this;
      if (handler != null) handler(sender, e);
    }
    /// <summary>Raises an event in a standard (mostly thread-safe) manner.</summary>
    /// <remarks>Seems redundant, but needed for ViewModelBase.cs.</remarks>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", 
      Justification="Not an event, just an event raiser.")]
    public static void Raise<T>(this PropertyChangedEventHandler @this, object sender, T e)  
      where T : PropertyChangedEventArgs {
      var handler = @this;
      if( handler != null ) handler(sender, e);
    }
  }
}
