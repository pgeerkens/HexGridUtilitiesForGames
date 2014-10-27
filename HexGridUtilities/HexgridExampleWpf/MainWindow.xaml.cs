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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using PGNapoleonics.HexgridExamples;
using PGNapoleonics.HexgridPanel;
using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;

using MyMapDisplay = PGNapoleonics.HexgridPanel.MapDisplay<PGNapoleonics.HexUtilities.MapGridHex>;

namespace PGNapoleonics.HexgridExampleWpf {
  /// <summary>TODO</summary>
  public partial class MainWindow : Window {
    /// <summary>TODO</summary>
    public MainWindow() {
      InitializeComponent();
      this.DataContext = this;
    }

    void RefreshCmdExecuted(object target, ExecutedRoutedEventArgs e) { 
      if (e.Parameter != null) HexgridPanel.SetMapDirty();
      HexgridPanel.Refresh();  
    }
    void RefreshCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }

    private void Window_Loaded (object sender, RoutedEventArgs e) {
      HexgridPanel = (HexgridScrollable) _host.Child;
      _host.Child.Focus();

      HexgridPanel.DataContext.SetScales(_scales);
      HexgridPanel.ScaleIndex   = _scales.Select((f,i) => new {value=f, index=i})
                                         .Where(s => s.value==1.0F)
                                         .Select(s => s.index).FirstOrDefault(); 
      HexgridPanel.MouseMove   += this.hexgridPanel_MouseMove;

      var sink = sender as System.Windows.Interop.IKeyboardInputSink;
      if (sink != null) 
        ((System.Windows.Interop.IKeyboardInputSink)sender).TabInto
              (new System.Windows.Input.TraversalRequest(FocusNavigationDirection.First));
      SelectedMapIndex     = 0;
    }

    /// <summary>TODO</summary>
    public   HexgridScrollable HexgridPanel { get; private set; }
    /// <summary>TODO</summary>
    public   IMapDisplayWinForms       Model        { get { return HexgridPanel.DataContext.Model; } }

    /// <summary>TODO</summary>
    public   int               SelectedMapIndex  { 
      get { return _selectedMapIndex; } 
      set {
        _selectedMapIndex = value;
        var mapName = ((ListBoxItem)comboBoxMapSelection.Items[_selectedMapIndex]).Name;
        switch (mapName) {
          case "MazeMap":    HexgridPanel.SetModel(SetMapBoard(new MazeMap(),    Model.FovRadius)); break;
          case "TerrainMap": HexgridPanel.SetModel(SetMapBoard(new TerrainMap(), Model.FovRadius)); break;
          default:            break;
        }
        sliderFovRadius.Value = Model.FovRadius;
        HexgridPanel.Refresh();
      }
    } int _selectedMapIndex = 0;

    static readonly List<float> _scales = new List<float> {0.707F,  0.841F, 1.000F, 1.189F, 1.414F};

    MyMapDisplay SetMapBoard(MyMapDisplay mapBoard, int fovRadius) {
      mapBoard.FovRadius  = fovRadius;
      RefreshLandmarkMenu(mapBoard);

      CustomCoords.SetMatrices(new IntMatrix2D(2,0, 0,-2, 0,2*mapBoard.MapSizeHexes.Height-1, 2));
      return mapBoard;
    }

    /// <summary>TODO</summary>
    public ObservableCollection<ListBoxItem> LandmarkItems {
      get { return _landmarkItems; }
    } ObservableCollection<ListBoxItem> _landmarkItems 
      = new ObservableCollection<ListBoxItem>() { new ListBoxItem(){Name="None", Content="None"} }; 

    void RefreshLandmarkMenu(MyMapDisplay model) {
      Model.LandmarkToShow = 0;
      while(LandmarkItems.Count > 1) LandmarkItems.RemoveAt(1);

      foreach(var item in 
        model.Landmarks.Select((l,i) => new ListBoxItem {
            Name=String.Format("No_{0}",i),
            Content=String.Format(CultureInfo.InvariantCulture, "{0}", l.Coords)
        } ) ) {
        LandmarkItems.Add(item);
      }
      HexgridPanel.SetMapDirty();
    }

    void hexgridPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
      var hotHex       = Model.HotspotHex;
      statusLabel.Content = string.Format(CultureInfo.InvariantCulture,
        "Hotspot Hex: {0:gi3} / {0:uI4} / {0:c5}; {1:r6}; Path Length = {2}",
        hotHex, Model.StartHex - hotHex, (Model.Path==null ? 0 : Model.Path.TotalCost));
    }
  }
}

#pragma warning disable 1587
/// <summary>Extensions to the System.Windows.Forms technologies used by namespace PGNapoleonics.HexgridPanel.</summary>
#pragma warning restore 1587
namespace PGNapoleonics.WinForms {
  using System;
  using System.Runtime.InteropServices;

  /// <summary>Extern declarations from the Win32 API.</summary>
  internal static partial class NativeMethods {
    /// <summary>P/Invoke declaration for user32.dll.WindowFromPoint</summary>
    /// <remarks><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(v=vs.85).aspx"></a></remarks>
    /// <param name="pt">(Sign-extended) screen coordinates as a Point structure.</param>
    /// <returns>Window handle (hWnd).</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability",
      "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0",
      Justification = "Research suggests the Code Analysis message is incorrect.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode")]
    [DllImport("user32.dll")]
    internal static extern IntPtr WindowFromPoint(System.Drawing.Point pt);

    /// <summary>P/Invoke declaration for user32.dll.SendMessage</summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="msg">Windows message</param>
    /// <param name="wParam">WParam</param>
    /// <param name="lParam">LParam</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
  }
}
