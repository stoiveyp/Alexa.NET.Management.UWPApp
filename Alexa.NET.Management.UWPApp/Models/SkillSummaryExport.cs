using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Alexa.NET.Management.Package;
using Alexa.NET.Management.UWPApp.Annotations;
using Newtonsoft.Json;

namespace Alexa.NET.Management.UWPApp.Models
{
    public class SkillSummaryExport : INotifyPropertyChanged
    {
        private readonly SkillSummaryViewModel _summary;

        private bool _exportEnabled;
        private string _exportStatus = "Checking";
        private ExportMetadata _exportMetadata;

        public bool ExportEnabled
        {
            get => _exportEnabled;
            private set
            {
                _exportEnabled = value;
                OnPropertyChanged();
            }
        }
        public string ExportStatus
        {
            get => _exportStatus;
            private set
            {
                _exportStatus = value;
                OnPropertyChanged();
            }
        }

        public string DownloadStatus => _exportMetadata == null ? string.Empty : $"Download (available until {_exportMetadata.ExpiresAt:yyyy-MM-dd HH:mm}";
        public Visibility DownloadVisible => _exportMetadata != null && DateTime.Now < _exportMetadata.ExpiresAt ? Visibility.Visible : Visibility.Collapsed;

        public string DownloadUrl => _summary.Settings.GetValue<string>("exportPackageId");

        public SkillSummaryExport(SkillSummaryViewModel skillSummaryViewModel)
        {
            _summary = skillSummaryViewModel;
        }

        public async Task GetDownload()
        {
            if (_exportMetadata.Location.IsAbsoluteUri)
            {
                await Launcher.LaunchUriAsync(_exportMetadata.Location);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task RequestExport()
        {
            var request = await _summary.Api.Package.CreateExportRequest(_summary.SkillId, _summary.Stage);
            if (!request.IsAbsoluteUri)
            {
                request = new System.Uri(new System.Uri("https://test.com"), request);
            }

            var exportId = request.Segments.Last();
            _summary.Settings.SetValue("exportPackageId", exportId);
            await UpdateExportPackage();
        }

        private async Task UpdateExportPackage()
        {
            if (!_summary.Settings.HasValue("exportPackageId"))
            {
                ExportEnabled = true;
                ExportStatus = "Request";
                return;
            }

            var packageId = _summary.Settings.GetValue<string>("exportPackageId");
            var exportStatus = await _summary.Api.Package.ExportStatus(packageId);

            switch (exportStatus.Status)
            {
                case Package.ExportStatus.IN_PROGRESS:
                    ExportEnabled = false;
                    ExportStatus = "In Progress";
                    break;
                case Package.ExportStatus.FAILED:
                    ExportEnabled = true;
                    ExportStatus = "(Previous Failure) Request Again";
                    break;
                case Package.ExportStatus.SUCCEEDED:
                    ExportEnabled = true;
                    _summary.Settings.RemoveValue("exportPackageId");
                    _summary.Settings.SetValue("exportPackageData", JsonConvert.SerializeObject(exportStatus.Skill));
                    _exportMetadata = exportStatus.Skill;
                    await UpdateDownloadPackage();
                    break;
            }
        }

        private async Task UpdateDownloadPackage()
        {
            if (_exportMetadata == null && _summary.Settings.HasValue("exportPackageData"))
            {
                _exportMetadata =
                    JsonConvert.DeserializeObject<ExportMetadata>(
                        _summary.Settings.GetValue<string>("exportPackageData"));
            }

            if (_exportMetadata != null)
            {
                if (_exportMetadata.ExpiresAt < DateTime.Now)
                {
                    _summary.Settings.RemoveValue("exportPackageData");
                }
            }

            OnPropertyChanged(nameof(DownloadVisible));
            OnPropertyChanged(nameof(DownloadStatus));
        }

        public async Task Update()
        {
            var export = UpdateExportPackage();
            var download = UpdateDownloadPackage();
            await Task.WhenAll(export, download);
        }
    }
}