using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    internal class Config
    {
        public List<PeriodRule> Rules { get; set; }
        public List<SelectableDay> Days { get; set; }
    }
}
