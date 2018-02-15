using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Alexa.NET.Management.Api;
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
        public SkillSummary CurrentSkill { get; set; }

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
            SkillNav.MenuItemsSource = skillList.Skills;
        }

        private void SetVendor(Vendor vendor)
        {
            VendorName.Content = $"Vendor: {vendor.Name}";
            VendorName.Tag = vendor.Id;
        }

        private async void SkillNav_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (!(args.SelectedItem is SkillSummary summary))
            {
                SkillNav.Content = null;
                return;
            }

            var skill = await Api.Skills.Get(summary.SkillId, summary.Stage.ToString().ToLower());

            var output = new ScrollViewer();

            var textview = new TextBlock {Text = JObject.FromObject(skill.Manifest).ToString(Formatting.Indented)};
            output.Content = textview;

            InfoTab.Content = output;
        }
    }
}
