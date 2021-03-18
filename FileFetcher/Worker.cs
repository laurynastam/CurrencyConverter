using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileFetcher
{
    public class Worker : BackgroundService
    {

        private readonly string tempCurrentCsvFileZipName = "\\tempCsvZipFile.zip";
        private readonly string tempHistoricalCsvFileZipName = "\\tempHistoricalCsvZipFile.zip";

        private string CurrentFilePath;
        private string HistoricalFilePath;

        private string currentCSVFileUrl;
        private string historicalCSVFileUrl;

        private string currentXMLFileUrl;
        private string historicalXMLFileUrl;

        private string currentPDFFileUrl;

        private string temporaryStorageLocation;
        private int secondsBetweenFetches = 3600; // Default time - 1 hour

        public Worker(IConfiguration config)
        {
            CurrentFilePath = config.GetValue<string>("DataSourceOPtions:Current_FilePath");
            HistoricalFilePath = config.GetValue<string>("DataSourceOPtions:Historical_FilePath");
            currentCSVFileUrl = config.GetValue<string>("DataSourceOPtions:URL_CurentCsv");
            temporaryStorageLocation = config.GetValue<string>("DataSourceOPtions:TemporaryStoring_FilePath");
            historicalCSVFileUrl = config.GetValue<string>("DataSourceOPtions:URL_HistoricalCsv");
            currentXMLFileUrl = config.GetValue<string>("DataSourceOPtions:URL_CurrentXml");
            historicalXMLFileUrl = config.GetValue<string>("DataSourceOPtions:URL_HistoricalXml");
            currentPDFFileUrl = config.GetValue<string>("DataSourceOPtions:URL_CurrentPdf");
            try
            {
                secondsBetweenFetches = config.GetValue<int>("TimeInSecondsBetweenFileFetches");

            }
            catch { }

        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DownloadZipedItemsAsync();
                DownloadXmlFiles();
                DownloadPdfFiles();
                await Task.Delay(secondsBetweenFetches * 1000, stoppingToken);
            }
        }

        public async Task DownloadPdfFiles() {
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(currentPDFFileUrl))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(new Uri(currentPDFFileUrl), CurrentFilePath + "\\eurofxref.pdf");
                    }
                }
            }
        }

        public async Task DownloadXmlFiles() {
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(currentXMLFileUrl))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(new Uri(currentXMLFileUrl), CurrentFilePath + "\\eurofxref-daily.xml");
                        webClient.DownloadFile(new Uri(historicalXMLFileUrl), HistoricalFilePath + "\\eurofxref-hist.xml");
                    }
                }
            }
        }

        public async Task DownloadZipedItemsAsync()
        {

            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(currentCSVFileUrl))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        WebClient webClient = new WebClient();
                        await webClient.DownloadFileTaskAsync(new Uri(historicalCSVFileUrl), temporaryStorageLocation + tempHistoricalCsvFileZipName);
                        await webClient.DownloadFileTaskAsync(new Uri(currentCSVFileUrl), temporaryStorageLocation + tempCurrentCsvFileZipName);
                        ExtractFiles();

                    }

                }
            }
        }

        private void ExtractFiles() { 
            DirectoryInfo di = new DirectoryInfo(temporaryStorageLocation);
            try
            {
                var allZipFiles = di.GetFiles("*.zip")
                    .Where(file => file.Name.EndsWith(".zip"))
                    .Select(file => file.Name).ToList();
                for (var i = 0; i < allZipFiles.Count; i++) { 
                    ZipFile.ExtractToDirectory((temporaryStorageLocation + "\\" + allZipFiles[i]), (temporaryStorageLocation ));
                
                }
                var allCSVFiles = di.GetFiles("*.csv")
                                    .Where(file => file.Name.EndsWith(".csv"))
                                    .Select(file => file.Name).ToList();

                foreach (var item in allCSVFiles) {
                    string path = "";
                    if (item.Contains("hist"))
                    {
                        path = HistoricalFilePath;
                    }
                    else {
                        path = CurrentFilePath;
                    }
                    File.Copy(temporaryStorageLocation + "\\" + item, path + "\\" + item, true);
                    File.Delete((temporaryStorageLocation + "\\" + item));
                }

            }
            catch (Exception ex) {
                foreach (var item in di.GetFiles())
                {
                    File.Delete(item.ToString());
                }
                DownloadZipedItemsAsync();
                return;
            }
        
        }

        private void ZipFileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Unzip, if fails clen temp folder and try again

            //try
            //{

            //    foreach (var item in allCSVFiles) {
            //        File.Copy(temporaryStorageLocation + "\\" + item, CurrentFilePath + "\\" + item);
            //        File.Delete((temporaryStorageLocation + "\\" + tempCurrentCsvFileZipName));
            //    }

            //}
            //catch (Exception ex) {
            //    DownloadCurrentFilesAsync();
            //    return;
            //}
        }
    }
}
