using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace FileDownloader {
    public class Program {

        private static string _url = "";

        static async Task Main(string[] args) {

            if (args.Length == 0) {
                Console.WriteLine("specify a file url to download");
                return;
            }

            string pathToWriteTo = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();

            var downloader = new Downlaoder(_url, pathToWriteTo);

            await downloader.Start((double downloaded) => {
                Console.WriteLine($"Downloaded: {Math.Round(downloaded * 100)}%");
            }, () => {
                Console.WriteLine("Download Finished");
            });
        }
    }
}
