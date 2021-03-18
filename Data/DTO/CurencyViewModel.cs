using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.DTO
{
    public class CurencyViewModel
    {
        [StringLength(20)]
        public string NameOfCurency { get; set; }
        public double? Rate { get; set; }
    }
}
