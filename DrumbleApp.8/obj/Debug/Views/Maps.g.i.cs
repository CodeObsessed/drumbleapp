﻿#pragma checksum "C:\Users\chris\Desktop\Drumble\DrumbleApp.8\Views\Maps.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0D3EE8E4DD388B986DD084ECA5817851"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Drumble.Views {
    
    
    public partial class Maps : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Primitives.ViewportControl ImageViewPort;
        
        internal System.Windows.Controls.Canvas canvas;
        
        internal System.Windows.Controls.Image SystemMapImage;
        
        internal System.Windows.Media.ScaleTransform xform;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Drumble;component/Views/Maps.xaml", System.UriKind.Relative));
            this.ImageViewPort = ((System.Windows.Controls.Primitives.ViewportControl)(this.FindName("ImageViewPort")));
            this.canvas = ((System.Windows.Controls.Canvas)(this.FindName("canvas")));
            this.SystemMapImage = ((System.Windows.Controls.Image)(this.FindName("SystemMapImage")));
            this.xform = ((System.Windows.Media.ScaleTransform)(this.FindName("xform")));
        }
    }
}

