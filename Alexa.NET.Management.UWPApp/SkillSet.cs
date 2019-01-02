using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alexa.NET.Management.Api;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Annotations;
using Alexa.NET.Management.UWPApp.Models;

namespace Alexa.NET.Management.UWPApp
{
    public class SkillSet:INotifyPropertyChanged
    {
        public SkillSet(IGrouping<string, SkillSummary> summaryById, ManagementApi api)
        {
            Summaries = summaryById.ToArray();
            Api = api;
        }

        private ManagementApi Api { get; }

        public SkillSummary[] Summaries { get; }

        public bool HasLiveStage => Summaries.Any(s => s.Stage == SkillStage.LIVE);

        public string ApiTypes => string.Join(",", Summaries.SelectMany(s => s.Apis).Distinct());

        public string GetTitle(string locale)
        {
            return GetTitle(Summaries, locale);
        }

        public static string GetTitle(IEnumerable<SkillSummary> summaries, string locale)
        {
            var titles = summaries.SelectMany(s => s.NameByLocale);
            return titles.FirstOrDefault(kvp => kvp.Key == locale).Value ??
                   titles.FirstOrDefault().Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SkillSummaryViewModel _activeSummary;
        public SkillSummaryViewModel ActiveSummary
        {
            get => _activeSummary;
            private set
            {
                if (_activeSummary != value)
                {
                    _activeSummary = value;
                    OnPropertyChanged(nameof(ActiveSummary));
                }
            }
        }

        public async Task<bool> UpdateStage(SkillStage stage)
        {
            var newSummary = Summaries.FirstOrDefault(s => s.Stage == stage);

            if (newSummary == null)
            {
                return false;
            }

            if (newSummary == ActiveSummary?.SkillSummary)
            {
                return true;
            }

            ActiveSummary = new SkillSummaryViewModel(newSummary,Api);
            //DO STUFF
            return true;
        }
    }
}
 