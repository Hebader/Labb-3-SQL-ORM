using System;
using System.Collections.Generic;

namespace Labb_3_SQL___ORM.Models
{
    public partial class staff
    {
        public int StaffId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? FkpositionId { get; set; }

        
       
    }
}
