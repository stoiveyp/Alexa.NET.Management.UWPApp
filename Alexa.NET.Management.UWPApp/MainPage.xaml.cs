using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Utility;
using Alexa.NET.Management.Vendors;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Alexa.NET.Management.UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ManagementApi Api { get; set; }

        private JsonSerializer Serializer { get; } = JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Internals.ApiConverter() }
        });

        private ObservableCollection<SkillSet> Skills { get; }

        public static readonly DependencyProperty CurrentSkillSetProperty = DependencyProperty.Register(
            "CurrentSkillSet", typeof(SkillSet), typeof(MainPage), new PropertyMetadata(default(SkillSet)));

        public SkillSet CurrentSkillSet
        {
            get => (SkillSet) GetValue(CurrentSkillSetProperty);
            set => SetValue(CurrentSkillSetProperty, value);
        }

        public MainPage()
        {
            SkillItemTitleConverter.Locale = "en-GB";
            Skills = new ObservableCollection<SkillSet>();

            this.InitializeComponent();
            MasterDetails.ItemsSource = Skills;
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

        private void MasterDetails_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                CurrentSkillSet = e.AddedItems.Cast<SkillSet>().First();
            }
        }

        //var skill = await Api.Skills.Get(summary.SkillId, summary.CurrentStage);
        //if (!currentSet.UpdateStage(StageSwitch.IsOn
        //    ? StageSwitch.OnContent.ToString()
        //    : StageSwitch.OffContent.ToString()))
        //{
        //    StageSwitch.IsOn = !StageSwitch.IsOn;
        //}
    }
}
