﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Odyssey.Controls.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls.Ribbon.Interfaces
{
    public interface IRibbonButton:IRibbonControl,IKeyTipControl
    {
        object Content { get; set; }

        ImageSource LargeImage { get; set; }

        ImageSource SmallImage { get; set; }

        CornerRadius CornerRadius { get; set; }
    }
}
