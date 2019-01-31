using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using PGNapoleonics.HexUtilities;

namespace PGNapoleonics.HexgridExampleCommon {
    /// <summary>Extension methods for <see Cref="Hex"/>.</summary>
    public static partial class HexExtensions {
        /// <summary>The <i>Manhattan</i> distance from this hex to that at <c>coords</c>.</summary>
        public static int Range(this IHex @this, IHex target)
        => @this.Coords.Range(target.Coords);

        /// <summary>TODO</summary>
        public static void Paint(this IHex @this, Graphics graphics, GraphicsPath path, Brush brush) {
            if (graphics==null) throw new ArgumentNullException("graphics");
            graphics.FillPath(brush, path);
        }
    }

    /// <summary>TODO</summary>
    public interface IPaintableHex<TSurface,TPath> {
        /// <summary>TODO</summary>
        void Paint(TSurface graphics, TPath path);
    }
}
