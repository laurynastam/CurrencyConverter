using Data.DTO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Readers.Base;
using Readers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Readers.Readers
{
    public class PDFReader : BaseReader, IReader
    {
        public PDFReader(string FullFilePath, string HistoricalFileName) : base(FullFilePath, HistoricalFileName)
        {
            ReadAllHistoricDates();
            ReadCurrentData();
        }
        public List<string> GetAllHistoricalDates()
        {
            return null;
        }

        public List<CurencyDataViewModel> GetCurrencyList()
        {
            return CurencyList;
            //throw new NotImplementedException();
        }

        public void ReadAllHistoricDates()
        {
            
        }

        public void ReadCurrentData()
        {
            PdfReader reader = new PdfReader(FileName);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                string[] currencies = PdfTextExtractor.GetTextFromPage(reader, page).Split("\n");
                if (currencies != null && currencies.Length > 0) {
                    List<CurencyViewModel> currenciesList = new List<CurencyViewModel>();
                    for (var i = 3; i < currencies.Length - 1; i++) {
                        string[] curencyString = currencies[i].Split(" ");
                        string currencyName = curencyString[0];
                        double? rateAsDouble = null;
                        try
                        {
                            rateAsDouble = double.Parse(curencyString[curencyString.Length-1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            rateAsDouble = null;
                        }
                        currenciesList.Add(new CurencyViewModel { NameOfCurency = currencyName, Rate = rateAsDouble });
                    }
                    CurencyList = new List<CurencyDataViewModel> { new CurencyDataViewModel { Curencies = currenciesList } };
                }
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
        }

        public List<CurencyDataViewModel> ReadHistoricalData(string date)
        {
            throw new NotImplementedException();
        }
    }
}
