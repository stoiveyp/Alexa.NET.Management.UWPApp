using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<SkillSet> Skills = new ObservableCollection<SkillSet>();

        private async Task GetAndSetSkills(string vendorId)
        {
            SkillNav.MenuItemsSource = Skills;
            var skillList = await Api.Skills.List(vendorId);
            var skillSets = skillList.Skills.GroupBy(s => s.SkillId).Select(g => new SkillSet(g));
            foreach (var item in skillSets)
            {
                Skills.Add(item);
            }
        }

        private void SetVendor(Vendor vendor)
        {
            VendorName.Content = $"Vendor: {vendor.Name}";
            VendorName.Tag = vendor.Id;
        }

        private async void SkillNav_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var skillSet = args.SelectedItem as SkillSet;
            StageSwitch.IsOn = false;
            StageSwitch.IsEnabled = skillSet.Summaries.Length > 1;
            await SetSkill(skillSet);
        }

        private async Task SetSkill(SkillSet summary)
        {

            //var skill = await Api.Skills.Get(summary.SkillId, summary.CurrentStage);
            //CurrentSkill = skill;
            //var output = new ScrollViewer();

            //var textview = new TextBox {AcceptsReturn = true,TextWrapping = TextWrapping.Wrap};
            //output.Content = textview;

            //var osb = new StringBuilder();
            //using (var textWriter = new JsonTextWriter(new StringWriter(osb)))
            //{
            //    Serializer.Serialize(textWriter, skill);
            //}

            //textview.Text = osb.ToString();

            //InfoTab.Content = output;
        }

        private void StageSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var currentSet = SkillNav.SelectedItem as SkillSet;
            if (!currentSet.UpdateStage(StageSwitch.IsOn
                ? StageSwitch.OnContent.ToString()
                : StageSwitch.OffContent.ToString()))
            {
                StageSwitch.IsOn = !StageSwitch.IsOn;
            }
        }
    }
}
