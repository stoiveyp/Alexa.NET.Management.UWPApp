using System.Collections;
using System.Threading.Tasks;
using Alexa.NET.Management.Api;
using Alexa.NET.Management.Skills;

namespace Alexa.NET.Management.UWPApp.Models
{
    public class SkillSummaryViewModel
    {
        public SkillSummaryViewModel(SkillSummary summary, ManagementApi api)
        {
            Api = api;
            SkillSummary = summary;
            Settings = new SkillSummarySettings(summary);
        }

        public ManagementApi Api { get; }

        public Task<Skill> GetSkill()
        {
            return Api.Skills.Get(SkillId, Stage.ToString().ToLower());
        }

        public Task<SkillId> SetSkill(Skill skill)
        {
            return Api.Skills.Update(SkillId, Stage.ToString(), skill);
        }

        public SkillSummary SkillSummary { get; }

        public string SkillId => SkillSummary.SkillId;

        public SkillStage Stage => SkillSummary.Stage;

        public string GetTitle(string locale)
        {
            return SkillSet.GetTitle(new[] { SkillSummary }, locale);
        }

        private SkillSummaryExport _export;

        public SkillSummaryExport ExportPackage => _export ?? (_export = new SkillSummaryExport(this));
        public SkillSummarySettings Settings { get; set; }
    }
}
