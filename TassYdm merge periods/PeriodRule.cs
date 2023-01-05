using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    [ObservableObject]
    internal partial class PeriodRule
    {
        [ObservableProperty]
        string label;
        [ObservableProperty]
        int ttsPeriod;
        [ObservableProperty]
        bool merge;
        [ObservableProperty]
        bool duplicate;
        [ObservableProperty]
        int tassPeriod;
    }
}
