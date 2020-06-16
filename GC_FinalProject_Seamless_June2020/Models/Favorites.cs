using System;
using System.Collections.Generic;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public partial class Favorites
    {
        public int Id { get; set; }
        public int? ApiId { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
