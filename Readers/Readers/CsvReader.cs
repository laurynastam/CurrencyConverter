using Data.DTO;
using Readers.Base;
using Readers.Extensions;
using Readers.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Readers.Readers
{
    public class CsvReader : BaseReader, IReader
    {

        public CsvReader(string FullFilePath, string HistoricalFileName) : base(FullFilePath, HistoricalFileName) {
            ReadAllHistoricDates();
            ReadCurrentData();
        }

        public void ReadCurrentData()
        {
            try
            {
                using (var reader = new StreamReader(FileName))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = new List<CurencyDataViewModel>();
                    csv.Read();
                    csv.ReadHeader();
                    string[] headerRows = csv.HeaderRecord;
                    while (csv.Read())
                    {
                        var dateAsString = csv.GetField(0);
                        DateTime? dateOfCurrencyRate = dateAsString.GetDateTimeFromString();
                        List<CurencyViewModel> currencies = GetAllCurencies(headerRows, csv);
                        records.Add( new CurencyDataViewModel { DateTimeOf = dateOfCurrencyRate, Curencies = currencies });
                    }
                    CurencyList = records;
                }

            }
            catch (Exception ex) {
                CurencyList = null;
            }
        }

        private List<CurencyViewModel> GetAllCurencies(string[] headerRows, CsvHelper.CsvReader csv) {
            List<CurencyViewModel> currencies = new List<CurencyViewModel>();
            for (int i = 1; i < headerRows.Length - 1; i++)
            {
                double? rate = null;
                try
                {
                    rate = double.Parse(csv.GetField(i), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    rate = null;
                }
                currencies.Add(new CurencyViewModel { NameOfCurency = headerRows[i], Rate = rate });
            }
            return currencies;
        }

        public List<CurencyDataViewModel> ReadHistoricalData(string date)
        {
            try
            {
                using (var reader = new StreamReader(HistoricalFileName))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = new List<CurencyDataViewModel>();
                    csv.Read();
                    csv.ReadHeader();
                    string[] headerRows = csv.HeaderRecord;
                    while (csv.Read())
                    {
                        var dateAsString = csv.GetField(0);
                        if (dateAsString == date) {
                            List<CurencyViewModel> currencies = GetAllCurencies(headerRows,csv);
                            return  new List<CurencyDataViewModel>
                            {
                                new CurencyDataViewModel{ 
                                    DateTimeOf = dateAsString.GetDateTimeFromString(),
                                    Curencies = currencies                                
                                }
                            };
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex) {
                return null;
            }
        }

        public void ReadAllHistoricDates()
        {
            try
            {
                using (var reader = new StreamReader(HistoricalFileName))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = new List<CurencyDataViewModel>();
                    csv.Read();
                    csv.ReadHeader();
                    List<string> allDates = new List<string>();
                    while (csv.Read())
                    {
                        var dateAsString = csv.GetField(0);
                        allDates.Add(dateAsString);
                    }
                    AllHistoricDates = allDates;
                }
            }
            catch (Exception ex) {
                AllHistoricDates = null;
            }
        }

        public List<string> GetAllHistoricalDates() {  return AllHistoricDates; }

        public List<CurencyDataViewModel> GetCurrencyList() { return CurencyList; }
    }
}
