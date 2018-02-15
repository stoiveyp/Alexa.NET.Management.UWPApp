using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Alexa.NET.Management.Skills;

namespace Alexa.NET.Management.UWPApp.Utility
{
    public class SkillItemTitleConverter:IValueConverter
    {
        public static string Locale { get; set; } = "en-GB";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is SkillSummary))
            {
                return "[No Skill Selected]";
            }

            return ((SkillSummary) value).NameByLocale[Locale];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
