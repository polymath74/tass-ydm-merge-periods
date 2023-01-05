using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    internal class AppSettings
    {
        internal static string SourcePath
        {
            get => Properties.Settings.Default.SourcePath;
            set
            {
                Properties.Settings.Default.SourcePath = value;
                Properties.Settings.Default.Save();
            }
        }

        internal static string DestinationPath
        {
            get => Properties.Settings.Default.DestinationPath;
            set
            {
                Properties.Settings.Default.DestinationPath = value;
                Properties.Settings.Default.Save();
            }
        }

        internal static string ConfigPath
        {
            get => Properties.Settings.Default.ConfigPath;
            set
            {
                Properties.Settings.Default.ConfigPath = value;
                Properties.Settings.Default.Save();
            }
        }


    }
}
