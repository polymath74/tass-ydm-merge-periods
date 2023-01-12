using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    [ObservableObject]
    internal partial class SelectableDay
    {
        public int DayNumber { get; init; }
        [ObservableProperty]
        bool isSelected = true;
    }
}
