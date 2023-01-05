using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TassYdm_merge_periods
{
    internal record class TtsActivity(
        int DayNumber,
        int PeriodNumber,
        string YearGroup,
        string ClassCode,
        string RoomCode,
        string TeacherCode
    );
}
