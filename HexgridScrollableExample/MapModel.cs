#region The MIT License - Copyright (C) 2012-2019 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Hex-Grid Utilities
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
using System.Windows.Forms;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexgridPanel;
namespace PGNapoleonics.HexgridScrollableExample {
    using HexSize = System.Drawing.Size;

    public abstract class MapModel : MapDisplayBlocked<Hex> {
        protected MapModel( HexSize sizeHexes, HexSize gridSize, InitializeHex initializeHex, IMapViewModel viewModel)
        : base(sizeHexes, gridSize, initializeHex){
            ViewModel = ViewModel;
            AttachViewModel();
        }

        IMapViewModel ViewModel { get; }

        void AttachViewModel() {
            ViewModel.GoalHexChanged            += GoalHexChanged;
            ViewModel.StartHexChanged           += StartHexChange;
            ViewModel.HotSpotHexChanged         += HotSpotHexChange;

            //ViewModel.TransposeMapToggled       += TransposeMapToggled;
            ViewModel.ShowRangeLineToggled      += ShowRangeLineToggled;
            ViewModel.ShowPathArrowToggled      += ShowPathArrowToggled;
            ViewModel.ShowFieldOfViewToggled    += ShowFieldOfViewToggled;

            ViewModel.LandmarkSelected          += LandmarkSelected;

            ViewModel.MouseMoved                += MouseMoved;
        }

        void GoalHexChanged(object sender, HexEventArgs e)   => RefreshAfter(()=>{GoalHex = e.Coords;});
        void StartHexChange(object sender, HexEventArgs e)   => RefreshAfter(()=>{StartHex = e.Coords;});
        void HotSpotHexChange(object sender, HexEventArgs e) => RefreshAfter(()=>{HotspotHex = e.Coords;});

        //void TransposeMapToggled(object sender, bool isChecked)  => ViewModel.IsTransposed = isChecked;
        void ShowRangeLineToggled(object sender, bool isChecked) { }
        void ShowPathArrowToggled(object sender, bool isChecked) { }
        void ShowFieldOfViewToggled(object sender, bool isChecked) { }

        void LandmarkSelected(object sender, int value) { }

        void MouseMoved(object sender, MouseEventArgs value) { }

        void RefreshAfter(Action action) { action?.Invoke(); ViewModel.Refresh(); }
    }
}
