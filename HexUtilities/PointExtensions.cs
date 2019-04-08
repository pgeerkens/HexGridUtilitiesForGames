#region Copyright (c) 2012-2019 Pieter Geerkens (email: pgeerkens@users.noreply.github.com)
///////////////////////////////////////////////////////////////////////////////////////////
// THis software may be used under the terms of attached file License.md (The MIT License).
///////////////////////////////////////////////////////////////////////////////////////////
#endregion

namespace PGNapoleonics.HexUtilities {
    using HexPointF = System.Drawing.PointF;
    using HexPoint  = System.Drawing.Point;

    /// <summary>TODO</summary>
    public static class PointExtensions {
        /// <summary>TODO</summary>
        public static HexPoint Scale(this HexPoint @this,int value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexPoint Scale(this HexPoint @this,int valueX,int valueY)
        => new HexPoint(@this.X * valueX,@this.Y * valueY);

        /// <summary>TODO</summary>
        public static HexPointF Scale(this HexPoint @this,float value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexPointF Scale(this HexPoint @this,float valueX,float valueY)
        => new HexPointF(@this.X,@this.Y).Scale(valueX,valueY);

        /// <summary>TODO</summary>
        public static HexPointF Scale(this HexPointF @this,float value)
        => @this.Scale(value,value);

        /// <summary>TODO</summary>
        public static HexPointF Scale(this HexPointF @this,float valueX,float valueY)
        => new HexPointF(@this.X * valueX,@this.Y * valueY);
    }
}
