using System;
using Windows.Storage;
using Alexa.NET.Management.Skills;

namespace Alexa.NET.Management.UWPApp.Models
{
    public class SkillSummarySettings
    {
        private readonly SkillSummary _summary;
        private readonly ApplicationDataContainer _local = Windows.Storage.ApplicationData.Current.LocalSettings;

        public SkillSummarySettings(SkillSummary summary)
        {
            _summary = summary;
        }

        public bool HasValue(string key)
        {
            return CurrentContainer().Values.ContainsKey(key);
        }

        private ApplicationDataContainer CurrentContainer()
        {
            return _local.CreateContainer(_summary.SkillId + "_" + _summary.Stage.ToString().ToLower(),ApplicationDataCreateDisposition.Always);
        }

        public T GetValue<T>(string key)
        {
            return (T)CurrentContainer().Values[key];
        }

        public void SetValue<T>(string key, T value)
        {
            var container = CurrentContainer();
            if (container.Values.ContainsKey(key))
            {
                container.Values.Remove(key);
            }

            container.Values.Add(key,value);
        }

        public void RemoveValue(string key)
        {
            CurrentContainer().Values.Remove(key);
        }
    }
}