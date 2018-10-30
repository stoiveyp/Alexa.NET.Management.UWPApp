using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FrameworkElement = Windows.UI.Xaml.FrameworkElement;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Alexa.NET.Management.UWPApp
{
    public sealed partial class SkillHeader : UserControl
    {
        public SkillHeader()
        {
            this.InitializeComponent();
        }

        private void CopySkillId(object sender, RoutedEventArgs e)
        {
            var toCopy = ((SkillSummaryViewModel)DataContext).SkillId;

            var package = new DataPackage();
            package.SetText(toCopy);

            Clipboard.SetContent(package);
        }
    }
}
