﻿using System.Collections.Generic;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Models;
using Newtonsoft.Json;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Alexa.NET.Management.UWPApp
{
    public sealed partial class CertificationList : UserControl
    {
        public CertificationList()
        {
            this.InitializeComponent();
            this.DataContextChanged += CertificationList_DataContextChanged;
        }

        private async void CertificationList_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue == null)
            {
                return;
            }

            var summary = args.NewValue as SkillSummaryViewModel;
            try
            {
                await summary.Certification.UpdateList();
            }
            catch (Exception)
            {

            }
        }
    }
}
