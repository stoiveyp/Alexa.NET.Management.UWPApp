using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Alexa.NET.Management.Api;
using Alexa.NET.Management.Skills;
using Alexa.NET.Management.UWPApp.Annotations;

namespace Alexa.NET.Management.UWPApp
{
    public class SkillSet : INotifyPropertyChanged
    {
        public SkillSummary[] Summaries { get; }

        public SkillSummary ActiveSummary => SummaryForStage(CurrentStage);

        public bool HasLiveStage => Summaries.Any(s => s.Stage == SkillStage.LIVE);

        public string SkillId { get; set; }

        public string CurrentStage { get; set; }

        public string ApiTypes => string.Join(",", Summaries.SelectMany(s => s.Apis).Distinct());
        public bool UpdateStage(SkillStage stageName)
        {
            var newSummary = SummaryForStage(stageName.ToString());

            if (newSummary == null)
            {
                return false;
            }

            if (newSummary == ActiveSummary)
            {
                return true;
            }

            CurrentStage = stageName.ToString();
            OnPropertyChanged(nameof(ActiveSummary));
            return true;
        }

        public void SwitchStage(object sender, RoutedEventArgs e)
        {
            UpdateStage(CurrentStage == SkillStage.DEVELOPMENT.ToString() ? SkillStage.LIVE : SkillStage.DEVELOPMENT);
        }

        public SkillSet(IGrouping<string, SkillSummary> summaryById)
        {
            Summaries = summaryById.ToArray();
            SkillId = summaryById.First().SkillId;
            UpdateStage(SkillStage.DEVELOPMENT);
        }

        private SkillSummary SummaryForStage(string stage)
        {
            return Summaries.FirstOrDefault(s =>
                string.Equals(s.Stage.ToString(), stage, StringComparison.InvariantCultureIgnoreCase));
        }

        public string GetTitle(string locale)
        {
            var titles = Summaries.SelectMany(s => s.NameByLocale);
            return titles.FirstOrDefault(kvp => kvp.Key == locale).Value ??
                   titles.FirstOrDefault().Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
