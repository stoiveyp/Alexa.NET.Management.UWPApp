using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Annotations;

namespace Alexa.NET.Management.UWPApp.Models
{
    public class SkillSummaryCertification:INotifyPropertyChanged
    {
        private SkillSummaryViewModel _summary;
        public CertificationSummary[] Summaries { get; private set; }

        public SkillSummaryCertification(SkillSummaryViewModel summary)
        {
            _summary = summary;
        }

        public async Task UpdateList()
        {
            var certificationList = await _summary.Api.Skills.ListCertification(_summary.SkillId);
            Summaries = certificationList.Items;
            OnPropertyChanged(nameof(Summaries));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
