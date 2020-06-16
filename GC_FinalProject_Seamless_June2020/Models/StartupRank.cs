using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class StartupRank
    {
        public Record Startup { get; set; }

        public int Rank { get; set; }

        public StartupRank(Record startup, int rank)
        {
            Startup = startup;
            Rank = rank;
        }

    }
}
