using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POLICLINIKA
{
    class Schedule
    {
        public int Id { get; set; }
        public string WorkingDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DoctorName { get; set; }
    }
}
