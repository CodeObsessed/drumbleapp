﻿#pragma checksum "C:\Users\chris\Desktop\Drumble\DrumbleApp.8\Views\TripDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "932DBCCF4185050C68342FA2416E2624"
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
using Microsoft.Phone.Maps.Controls;
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
    
    
    public partial class TripDetails : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Maps.Controls.Map RouteMap;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Drumble;component/Views/TripDetails.xaml", System.UriKind.Relative));
            this.RouteMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("RouteMap")));
        }
    }
}
