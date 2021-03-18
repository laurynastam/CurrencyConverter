using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO
{
    public class CurencyDataViewModel
    {
        public DateTime? DateTimeOf { get; set; }

        public List<CurencyViewModel> Curencies { get; set; }
    }
}
