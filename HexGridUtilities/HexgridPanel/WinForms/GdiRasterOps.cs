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

namespace PGNapoleonics.WinForms {
  internal static partial class GdiRasterOps {
    public const int SrcCopy                 = 0x00CC0020; /* dest = source                   */ 
    public const int SrcPaint                = 0x00EE0086; /* dest = source OR dest           */
    public const int SrcAnd                  = 0x008800C6; /* dest = source AND dest          */
    public const int SrcInvert               = 0x00660046; /* dest = source XOR dest          */
    public const int SrcErase                = 0x00440328; /* dest = source AND (NOT dest )   */ 
    public const int NotSrcCopy              = 0x00330008; /* dest = (NOT source)             */
    public const int NotSrcErase             = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */ 
    public const int MergeCopy               = 0x00C000CA; /* dest = (source AND pattern)     */ 
    public const int MergePaint              = 0x00BB0226; /* dest = (NOT source) OR dest     */
    public const int PatCopy                 = 0x00F00021; /* dest = pattern                  */ 
    public const int PatPaint                = 0x00FB0A09; /* dest = DPSnoo                   */
    public const int PatInvert               = 0x005A0049; /* dest = pattern XOR dest         */
    public const int DstInvert               = 0x00550009; /* dest = (NOT dest)               */
    public const int Blackness               = 0x00000042; /* dest = BLACK                    */ 
    public const int Whiteness               = 0x00FF0062; /* dest = WHITE                    */
//    public const int CaptureBlt              = 0x40000000; /* Include layered windows */ 
  }
}
