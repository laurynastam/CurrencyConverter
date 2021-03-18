using Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Readers.Interfaces
{
    public interface IReader
    {
        void ReadCurrentData();
        List<CurencyDataViewModel> ReadHistoricalData(string date);
        void ReadAllHistoricDates();

        List<string> GetAllHistoricalDates();
        List<CurencyDataViewModel> GetCurrencyList();
    }
}
