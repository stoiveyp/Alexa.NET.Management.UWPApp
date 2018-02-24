using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Alexa.NET.Management.Api;
using Alexa.NET.Management.Manifest;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Utility;
using Alexa.NET.Management.Vendors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Alexa.NET.Management.UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ManagementApi Api { get; set; }
        public Skill CurrentSkill { get; set; }

        private JsonSerializer Serializer { get; } = JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Internals.ApiConverter() }
        });

        public MainPage()
        {
            SkillItemTitleConverter.Locale = "en-GB";
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var tokenAccess = await AmazonLogin.TokenAuthorizer();
            Api = new ManagementApi(tokenAccess);
            var vendorResponse = await Api.Vendors.Get();

            var firstVendor = vendorResponse.Vendors.FirstOrDefault();
            if (firstVendor == null)
            {
                return;
            }

            SetVendor(firstVendor);
            await GetAndSetSkills(firstVendor.Id);
        }

        private async Task GetAndSetSkills(string vendorId)
        {
            var skillList = await Api.Skills.List(vendorId);
            SkillNav.MenuItemsSource = skillList.Skills.OrderBy(s => s.NameByLocale[SkillItemTitleConverter.Locale]).ThenBy(s => s.Stage.ToString());
        }

        private void SetVendor(Vendor vendor)
        {
            VendorName.Content = $"Vendor: {vendor.Name}";
            VendorName.Tag = vendor.Id;
        }

        private async void SkillNav_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            await SetSkill(args.SelectedItem as SkillSummary);
        }

        private async void RefreshSkill(object sender, RoutedEventArgs e)
        {
            await SetSkill(SkillNav.SelectedItem as SkillSummary);
        }

        public async void UpdateSkill(object sender, RoutedEventArgs e)
        {
            if (!(SkillNav.SelectedItem is SkillSummary summary))
            {
                return;
            }

            if (summary.Stage == SkillStage.LIVE)
            {
                var dialog = new MessageDialog("This will update the development version of this skill, is this okay?",
                    "Unable to edit live skill");

                var result = false;
                dialog.Commands.Add(new UICommand("OK",a => result = true));
                dialog.Commands.Add(new UICommand("Cancel",a=>result = false));
                await dialog.ShowAsync().AsTask();
                if (!result)
                {
                    return;
                }
            }

            var text = ((ScrollViewer) InfoTab.Content).Content as TextBox;
            Skill skill;
            using (var reader = new JsonTextReader(new StringReader(text.Text)))
            {
                skill = Serializer.Deserialize<Skill>(reader);
            }

            try
            {
                await Api.Skills.Update(summary.SkillId, SkillStage.DEVELOPMENT.ToString(),skill);
                await SetSkill(summary);
            }
            catch (Exception)
            {
            }
        }

        private async Task SetSkill(SkillSummary summary)
        {
            var skill = await Api.Skills.Get(summary.SkillId, summary.Stage.ToString().ToLower());
            CurrentSkill = skill;
            var output = new ScrollViewer();

            var textview = new TextBox {AcceptsReturn = true,TextWrapping = TextWrapping.Wrap};
            output.Content = textview;

            var osb = new StringBuilder();
            using (var textWriter = new JsonTextWriter(new StringWriter(osb)))
            {
                Serializer.Serialize(textWriter, skill);
            }

            textview.Text = osb.ToString();

            InfoTab.Content = output;
        }
    }
}
