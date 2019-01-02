using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Alexa.NET.Management.UWPApp.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Alexa.NET.Management.UWPApp
{
    public sealed partial class StageOverview : UserControl
    {
        public StageOverview()
        {
            this.DataContextChanged += OnDataContextChanged;
            this.InitializeComponent();
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var summary = ((SkillSummaryViewModel) DataContext);
                await UpdateSkill(summary);
            }
        }

        private async Task UpdateSkill(SkillSummaryViewModel summary)
        {
            await summary.ExportPackage.Update();
        }

        private async void ExportPackageClick(object sender, RoutedEventArgs e)
        {
            var summary = (SkillSummaryViewModel)DataContext;
            await summary.ExportPackage.RequestExport();
        }

        private void DownloadPackageClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
