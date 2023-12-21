using System;
using System.Collections.Generic;

namespace Labb_3_SQL___ORM.Models
{
    public partial class Course
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int? FkstaffId { get; set; }
    }
}
