using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PartyCompParser
{
    internal static class GoogleSheetsProxy
    {
        private const string SheetDataUrl =
            "https://script.google.com/macros/s/AKfycbzoFHtHU9sTztKd9zrbAzJdKtc72aDgAruTmjpkRwpPi5vjyCa7XVtpCiIxfwbPMcFa/exec";


        public static async Task<SheetData> GetSheetDataAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetFromJsonAsync<SheetData>(SheetDataUrl);

            return response;
        }

        public static async Task PostResultsAsync(string[][] results)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(SheetDataUrl, results);
            response.EnsureSuccessStatusCode();
        }
    }
}