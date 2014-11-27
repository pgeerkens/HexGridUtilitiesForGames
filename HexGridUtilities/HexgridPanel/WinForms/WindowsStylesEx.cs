#region The MIT License - Copyright (C) 2012-2014 Pieter Geerkens
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
using System.Diagnostics.CodeAnalysis;

namespace  PGNapoleonics.WinForms {
  /// <summary>TODO</summary>
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",Justification="Hex-parsing failure")]
  [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
  [Flags]
  public enum WindowStylesEx
  {
    /// <summary>TODO</summary>
    None            = 0x00000000,
    /// <summary>Specifies that a window created with this style accepts drag-drop files.</summary>
    AcceptFiles     = 0x00000010,
    /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
    AppWindow       = 0x00040000,
    /// <summary>Specifies that a window has a border with a sunken edge.</summary>
    ClientEdge      = 0x00000200,
    /// <summary>Windows XP: Paints all descendants of a window in bottom-to-top painting order w/ double-buffering.</summary>
    /// <remarks>
    /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
    /// </remarks>
    Composited      = 0x02000000,
    /// <summary>Includes a question mark in the title bar of the window.</summary>
    /// <remarks>
    /// When the user clicks the question mark, the cursor changes to a question mark 
    /// with a pointer. If the user then clicks a child window, the child receives a 
    /// WM_HELP message. The child window should pass the message to the parent window 
    /// procedure, which should call the WinHelp function using the HELP_WM_HELP command. 
    /// The Help application displays a pop-up window that typically contains help for
    /// the child window.  
    /// CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
    /// </remarks>
    ContextHelp     = 0x00000400,
    /// <summary>The window itself contains child windows that should take part in dialog box navigation. </summary>
    /// <remarks>
    /// If this style is specified, the dialog manager recurses into children of this 
    /// window when performing navigation operations such as handling the TAB key, 
    /// an arrow key, or a keyboard mnemonic.
    /// </remarks>
    ControlParent   = 0x00010000,
    /// <summary>Creates a window that has a double border.</summary>
    /// <remarks>
    /// The window can, optionally, be created with a title bar by specifying the 
    /// WS_CAPTION style in the dwStyle parameter.
    /// </remarks>
    DlgModalFrame   = 0x00000001,
    /// <summary>
    /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
    /// </summary>
    Layered         = 0x00080000,
    /// <summary>
    /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left. 
    /// </summary>
    LayoutRtl       = 0x00400000,
    /// <summary>
    /// Creates a window that has generic left-aligned properties. This is the default.
    /// </summary>
    Left            = 0x00000000,
    /// <summary>
    /// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
    /// </summary>
    LeftScrollBar   = 0x00004000,
    /// <summary>The window text is displayed using left-to-right reading-order properties.</summary>
    /// <remarks>
    /// This is the default.
    /// </remarks>
    LtrReading      = 0x00000000,
    /// <summary>Creates a multiple-document interface (MDI) child window.</summary>
    MdiChild        = 0x00000040,
    /// <summary>The window doesn't become the foreground window when the user clicks it.</summary>
    /// <remarks>
    /// Windows 2000/XP: A top-level window created with this style does not become the 
    /// foreground window when the user clicks it. 
    /// 
    /// The system does not bring this window to the foreground when the user minimizes 
    /// or closes the foreground window.  To activate the window, use the SetActiveWindow
    /// or SetForegroundWindow function.  The window does not appear on the taskbar by 
    /// default. To force the window to appear on the taskbar, use the APPWINDOW 
    /// style.
    /// </remarks>
    NoActivate      = 0x08000000,
    /// <summary>
    /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
    /// </summary>
    NoInheritLayout = 0x00100000,
    /// <summary>
    /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
    /// </summary>
    NoParentNotify  = 0x00000004,
    /// <summary>Combines the CLIENTEDGE and WINDOWEDGE styles.</summary>
    OverlappedWindow = WindowEdge | ClientEdge,
    /// <summary>Combines the WINDOWEDGE, TOOLWINDOW, and TOPMOST styles.</summary>
    PaletteWindow   = WindowEdge | ToolWindow | Topmost,
    /// <summary>Gives the window has generic "right-aligned" properties.</summary>
    /// <remarks>
    /// This depends on the window class. This style has an effect only if the 
    /// shell language is Hebrew, Arabic, or another language that supports 
    /// reading-order alignment; otherwise, the style is ignored.
    /// 
    /// Using the RIGHT style for static or edit controls has the same 
    /// effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this 
    /// style with button controls has the same effect as using BS_RIGHT and 
    /// BS_RIGHTBUTTON styles.
    /// </remarks>
    Right           = 0x00001000,
    /// <summary>
    /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
    /// </summary>
    RightScrollBar  = 0x00000000,
    /// <summary>
    /// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
    /// </summary>
    RtlReading      = 0x00002000,
    /// <summary>
    /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
    /// </summary>
    StaticEdge      = 0x00020000,
    /// <summary>Creates a tool window; that is, a window intended to be used as a floating toolbar. </summary>
    /// <remarks>
    /// A tool window has a title bar that is shorter than a normal title bar, and the 
    /// window title is drawn using a smaller font. A tool window does not appear in 
    /// the taskbar or in the dialog that appears when the user presses ALT+TAB. If a 
    /// tool window has a system menu, its icon is not displayed on the title bar. 
    /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
    /// </remarks>
    ToolWindow      = 0x00000080,
    /// <summary>Specifies that a window should be, and remain, topmost.</summary>
    /// <remarks>
    /// Specifies that a window created with this style should be placed above all 
    /// non-topmost windows and should stay above them, even when the window is 
    /// deactivated. To add or remove this style, use the SetWindowPos function.
    /// </remarks>
    Topmost         = 0x00000008,
    /// <summary>Specifies that a window should be painted after all siblings.</summary>
    /// <remarks>
    /// Specifies that a window created with this style should not be painted until 
    /// siblings beneath the window (that were created by the same thread) have been 
    /// painted. The window appears transparent because the bits of underlying sibling 
    /// windows have already been painted.
    /// To achieve transparency w/o these restrictions, use the SetWindowRgn function.
    /// </remarks>
    Transparent     = 0x00000020,
    /// <summary>Specifies that a window has a border with a raised edge.</summary>
    WindowEdge      = 0x00000100
  }
}
