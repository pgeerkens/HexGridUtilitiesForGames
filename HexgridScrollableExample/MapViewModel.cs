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
using PGNapoleonics.HexUtilities.Pathfinding;
using PGNapoleonics.HexgridPanel;
namespace PGNapoleonics.HexgridScrollableExample {
    public class MapViewModel : IMapViewModel {
        public MapViewModel(IMapView view) {
            View = view;
            AttachView();
        }

        public event EventHandler<HexEventArgs> GoalHexChanged;
        public event EventHandler<HexEventArgs> StartHexChanged;
        public event EventHandler<HexEventArgs> HotSpotHexChanged;

        public event EventHandler<bool> TransposeMapToggled;
        public event EventHandler<bool> ShowRangeLineToggled;
        public event EventHandler<bool> ShowPathArrowToggled;
        public event EventHandler<bool> ShowFieldOfViewToggled;

        public event EventHandler<int> LandmarkSelected;

        public event EventHandler<MouseEventArgs> MouseMoved;

        public void SetLandmarkMenu(ILandmarkCollection landmarks){ }

        public bool IsTransposed { get; set; }

        public void Refresh()=> View.Refresh();

        IMapView View { get; }

        void AttachView() {
            View.GoalHexChanged             += OnGoalHexChanged;
            View.StartHexChanged            += OnStartHexChanged;
            View.HotSpotHexChanged          += OnHotSpotHexChanged;

            View.TransposeMapToggled       += OnTransposeMapToggled;
            View.ShowRangeLineToggled      += OnShowRangeLineToggled;
            View.ShowPathArrowToggled      += OnShowPathArrowToggled;
            View.ShowFieldOfViewToggled    += OnShowFieldOfViewToggled;

            View.LandmarkSelected          += OnLandmarkSelected;

            View.MouseMoved                += OnMouseMoved;
        }

        void OnGoalHexChanged(object sender, HexEventArgs e)    => RefreshAfter(()=>{GoalHexChanged?.Invoke(sender,e);});
        void OnStartHexChanged(object sender, HexEventArgs e)   => RefreshAfter(()=>{StartHexChanged?.Invoke(sender,e);});
        void OnHotSpotHexChanged(object sender, HexEventArgs e) => RefreshAfter(()=>{HotSpotHexChanged?.Invoke(sender,e);});

        void OnTransposeMapToggled(object sender, bool isChecked)  => View.IsTransposed = isChecked;
        void OnShowRangeLineToggled(object sender, bool isChecked) { }
        void OnShowPathArrowToggled(object sender, bool isChecked) { }
        void OnShowFieldOfViewToggled(object sender, bool isChecked) { }

        void OnLandmarkSelected(object sender, int value) { }

        void OnMouseMoved(object sender, MouseEventArgs value) { }

        void RefreshAfter(Action action) { action?.Invoke(); View.Refresh(); }
    }
}
