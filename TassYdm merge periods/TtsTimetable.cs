using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    [ObservableObject]
    internal partial class TtsTimetable
    {
        internal ObservableCollection<TtsActivity> Schedule { get; } = new();

        [ObservableProperty]
        int maxDayNumber;
        [ObservableProperty]
        int maxPeriodNumber;

        internal void LoadFrom(string[] lines)
        {
            Schedule.Clear();

            int maxDay = 0;
            int maxPeriod = 0;

            foreach (var line in lines)
            {
                if (line.Length > 0)
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 6
                        && int.TryParse(fields[0], out int day)
                        && int.TryParse(fields[1], out int period))
                    {
                        Schedule.Add(new TtsActivity(day, period, fields[2], fields[3], fields[4], fields[5]));
                        if (day > maxDay)
                            maxDay = day;
                        if (period > maxPeriod)
                            maxPeriod = period;
                    }
                }
            }

            MaxDayNumber = maxDay;
            MaxPeriodNumber = maxPeriod;
        }
    }
}
