#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace PGNapoleonics.HexUtilities {
    using HexSizeF = System.Drawing.SizeF;
    using HexSize  = System.Drawing.Size;

    /// <summary>TODO</summary>
    public static class SizeExtensions {
        /// <summary>TODO</summary>
        public static HexSize Scale(this HexSize @this,int value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexSize Scale(this HexSize @this,int valueX,int valueY)
        => new HexSize(@this.Width * valueX,@this.Height * valueY);

        /// <summary>TODO</summary>
        public static HexSizeF Scale(this HexSize @this,float value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexSizeF Scale(this HexSize @this,float valueX,float valueY)
        => new HexSizeF(@this).Scale(valueX,valueY);

        /// <summary>TODO</summary>
        public static HexSizeF Scale(this HexSizeF @this,float value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexSizeF Scale(this HexSizeF @this,float valueX,float valueY)
        => new HexSizeF(@this.Width * valueX,@this.Height * valueY);
    }
}
