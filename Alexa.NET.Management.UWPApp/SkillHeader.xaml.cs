using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Alexa.NET.Management.UWPApp.Models;

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
