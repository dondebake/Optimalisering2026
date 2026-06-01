#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/Controls/Colorpicker/SelectableColor.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OptimaliseRing.Controls.ColorPicker
{
    internal class SelectableColor
    {
        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        private bool _hotTrack = false;

        public bool HotTrack
        {
            get { return _hotTrack; }
            set { _hotTrack = value; }
        }

        public SelectableColor(Color color)
        {
            _color = color;
        }
    }
}
