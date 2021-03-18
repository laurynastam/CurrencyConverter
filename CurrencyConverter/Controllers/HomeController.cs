using CurrencyConverter.Models;
using Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Readers.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Readers.Interfaces.IReader currencyData;
        private IConfiguration configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            configuration = config;
            currencyData = GetCurrencyDataSource(); 
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.AwailableHistoricalDates = currencyData.GetAllHistoricalDates();
            List<CurencyViewModel> allCurencies = currencyData.GetCurrencyList().FirstOrDefault().Curencies;
            allCurencies.Add(new Data.DTO.CurencyViewModel { NameOfCurency = "EUR", Rate = 1 });
            ViewBag.CurrentCurencies = allCurencies;
            return View();
        }

        public JsonResult GetHistoricCurencies(string Date) {
            List<CurencyDataViewModel> historicalData = currencyData.ReadHistoricalData(Date);
            historicalData.FirstOrDefault().Curencies = historicalData.FirstOrDefault().Curencies.Where(w => w.Rate != null).ToList();
            historicalData.FirstOrDefault().Curencies.Add(new Data.DTO.CurencyViewModel { NameOfCurency = "EUR", Rate = 1 });
            return Json(new { HistoricalData = historicalData.FirstOrDefault().Curencies });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Readers.Interfaces.IReader GetCurrencyDataSource() {

            var preferredDS = configuration.GetValue<string>("DataSourceOPtions:DataSourceFile");
            switch (preferredDS) {
                case "CSV":
                    var csvFile = configuration.GetValue<string>("DataSourceOPtions:CSV_FilePath");
                    var csvHistoricalFile = configuration.GetValue<string>("DataSourceOPtions:CSV_HistoricalFilePath");
                    if (csvFile == null)
                        return null;
                    return new CsvReader(csvFile, csvHistoricalFile);
                case "XML":
                    var xmlFile = configuration.GetValue<string>("DataSourceOPtions:XML_FilePath");
                    var xmlHistoricalFile = configuration.GetValue<string>("DataSourceOPtions:XML_HistoricalFilePath");
                    if (xmlFile == null)
                        return null;
                    return new XmlReader(xmlFile, xmlHistoricalFile);
                case "PDF":
                    var pdfFile = configuration.GetValue<string>("DataSourceOPtions:PDF_FilePath");
                    var pdfHistoricalFile = configuration.GetValue<string>("DataSourceOPtions:PDF_HistoricalFilePath");
                    if (pdfFile == null)
                        return null;
                    return new PDFReader(pdfFile, pdfHistoricalFile);

                default:
                    return null;
            }
        }
    }
}
