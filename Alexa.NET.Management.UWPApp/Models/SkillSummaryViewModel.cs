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

        public async Task<Skill> GetSkill()
        {
            var status = await Api.InteractionModel.Get(SkillId, Stage.ToString().ToLower(), "en-GB");
            return await Api.Skills.Get(SkillId, Stage.ToString().ToLower());
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

        public SkillSummaryExport ExportPackage => _export ?? (_export = new SkillSummaryExport(this));

        public SkillSummaryCertification Certification => _certification ?? (_certification = new SkillSummaryCertification(this));

        public SkillSummaryBeta Beta => _beta ?? (_beta = new SkillSummaryBeta(this));

        public SkillSummarySettings Settings { get; set; }
        private SkillSummaryCertification _certification;
        private SkillSummaryExport _export;
        private SkillSummaryBeta _beta;
    }
}
