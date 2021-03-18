using System;
using System.Collections.Generic;
using System.Text;
using Data.DTO;

namespace Readers.Base
{
    public abstract class BaseReader
    {

        protected string FileName { get; set; }
        protected string HistoricalFileName { get; set; }
        public BaseReader(string FullFilePath, string HistoricalFileName) {
            this.FileName = FullFilePath;
            this.HistoricalFileName = HistoricalFileName;
        }

        protected List<string> AllHistoricDates { get; set; }

        protected List<CurencyDataViewModel> CurencyList { get; set; }

        



    }
}
