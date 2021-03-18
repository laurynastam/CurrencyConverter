using Data.DTO;
using Readers.Base;
using Readers.Extensions;
using Readers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Readers.Readers
{
    public class XmlReader : BaseReader, IReader
    {

        public XmlReader(string FullFilePath, string HistoricalFileName) : base(FullFilePath, HistoricalFileName)
        {
            ReadAllHistoricDates();
            ReadCurrentData();
        }

        public List<string> GetAllHistoricalDates()
        {
            return AllHistoricDates;
        }

        public List<CurencyDataViewModel> GetCurrencyList()
        {
            return CurencyList;
        }

        public void ReadAllHistoricDates()
        {
            try
            {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                string str = null;
                FileStream fs = new FileStream(HistoricalFileName, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("Cube");
                List<string> allDates = new List<string>();
                for (var j = 0; j < xmlnode.Count; j++ ) {
                    var timeNode = xmlnode[j].Attributes.GetNamedItem("time");
                    if (timeNode != null) { 
                        allDates.Add(timeNode.Value);
                
                    }
                }
                AllHistoricDates = allDates;

            }
            catch (Exception ex){
                AllHistoricDates = null;
            }
        }

        public void ReadCurrentData()
        {
            try {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                string str = null;
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("Cube");
                List<string> allDates = new List<string>();
                CurencyDataViewModel currencyData = new CurencyDataViewModel();
                currencyData.Curencies = new List<CurencyViewModel>();
                for (var j = 0; j < xmlnode.Count; j++)
                {
                    var time = xmlnode[1].Attributes.GetNamedItem("time").Value;
                    if (time != null && time != "") {
                        currencyData.DateTimeOf = time.GetDateTimeFromString();
                    }
                    if (xmlnode[j].Attributes.GetNamedItem("currency") != null && xmlnode[j].Attributes.GetNamedItem("rate") != null) { 
                        var rate = xmlnode[j].Attributes.GetNamedItem("rate").Value;
                        var currency = xmlnode[j].Attributes.GetNamedItem("currency").Value;
                        if (currency != null && currency != "" && rate != null && rate != "")
                        {
                            double? rateAsDouble = null;
                            try
                            {
                                rateAsDouble = double.Parse(rate.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception ex)
                            {
                                rateAsDouble = null;
                            }
                            currencyData.Curencies.Add(new CurencyViewModel { NameOfCurency = currency, Rate = rateAsDouble });
                        }                    
                    }
                }
                CurencyList = new List<CurencyDataViewModel> { currencyData };
            }
            catch (Exception ex) {
                CurencyList = null;
            }
}

        public List<CurencyDataViewModel> ReadHistoricalData(string date)
        {
            try
            {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                string str = null;
                FileStream fs = new FileStream(HistoricalFileName, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("Cube");
                List<string> allDates = new List<string>();
                for (var j = 0; j < xmlnode.Count; j++)
                {
                    var timeNode = xmlnode[j].Attributes.GetNamedItem("time");
                    if (timeNode != null)
                    {
                        if (timeNode.Value == date) {
                            var allCurrencies = xmlnode[j].ChildNodes;
                            List<CurencyDataViewModel> response = new List<CurencyDataViewModel>();
                            List<CurencyViewModel> currencyList = new List<CurencyViewModel>();
                            for (var i=0; i< allCurrencies.Count; i++) {
                                var currency = allCurrencies[i]?.Attributes.GetNamedItem("currency")?.Value;
                                var rate = allCurrencies[i]?.Attributes.GetNamedItem("rate")?.Value;
                                if (currency != null && rate != null) {
                                    double? rateAsDouble = null;
                                    try
                                    {
                                        rateAsDouble = double.Parse(rate.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception ex)
                                    {
                                        rateAsDouble = null;
                                    }
                                    currencyList.Add(new CurencyViewModel { NameOfCurency = currency.ToString(), Rate = rateAsDouble });
                                }
                            }
                            response.Add(new CurencyDataViewModel { Curencies = currencyList, DateTimeOf = date.GetDateTimeFromString() });
                            return response;
                        }
                        allDates.Add(timeNode.Value);

                    }
                }

            }
            catch (Exception ex) { 
            
            }
            return null;
        }
    }
}
