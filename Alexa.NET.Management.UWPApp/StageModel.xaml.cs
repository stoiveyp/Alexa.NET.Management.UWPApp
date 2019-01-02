using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Alexa.NET.Management.Manifest;
using Alexa.NET.Management.UWPApp.Models;
using Newtonsoft.Json;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Alexa.NET.Management.UWPApp
{
    public sealed partial class StageModel : UserControl
    {
        public StageModel()
        {
            this.InitializeComponent();
            this.DataContextChanged += StageModel_DataContextChanged;
        }

        private async void StageModel_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue == null)
            {
                return;
            }

            var summary = args.NewValue as SkillSummaryViewModel;
            var skill = await summary.GetSkill();
            Manifest.Text = JsonConvert.SerializeObject(skill.Manifest,Formatting.Indented);
        }

        private async void PublishModel(object sender, RoutedEventArgs e)
        {
            var events = await ((SkillSummaryViewModel) DataContext).GetSkill();
            var manifest = JsonConvert.DeserializeObject<SkillManifest>(Manifest.Text);
            events.Manifest = manifest;
            await ((SkillSummaryViewModel) this.DataContext).SetSkill(events);
        }
    }
}
