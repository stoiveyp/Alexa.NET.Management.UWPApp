using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Management.Api;
using Alexa.NET.Management.Skills;

namespace Alexa.NET.Management.UWPApp
{
    public class SkillSummaryViewModel
    {
        public SkillSummaryViewModel(SkillSummary summary)
        {
            SkillSummary = summary;
        }

        public SkillSummary SkillSummary { get; }

        public string SkillId => SkillSummary.SkillId;

        public SkillStage Stage => SkillSummary.Stage;

        public string GetTitle(string locale)
        {
            return SkillSet.GetTitle(new[] {SkillSummary}, locale);
        }
    }
}
