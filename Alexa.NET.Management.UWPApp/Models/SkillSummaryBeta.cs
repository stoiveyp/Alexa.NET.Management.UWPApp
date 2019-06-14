using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alexa.NET.Management.UWPApp.Annotations;

namespace Alexa.NET.Management.UWPApp.Models
{
    public class SkillSummaryBeta:INotifyPropertyChanged
    {
        private readonly SkillSummaryViewModel _summary;

        public SkillSummaryBeta(SkillSummaryViewModel skillSummaryViewModel)
        {
            _summary = skillSummaryViewModel;
        }

        private Beta.BetaTest BetaTest { get; set; }

        public async Task UpdateList()
        {
            try
            {
                var certificationList = await _summary.Api.Beta.Get(_summary.SkillId);
                BetaTest = certificationList;
            }
            catch (WebException ex) when(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                
            }

            OnPropertyChanged(nameof(BetaTest));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
