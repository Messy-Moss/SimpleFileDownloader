using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader {
    public class Downlaoder {
        private readonly string _url;
        private readonly string _path;
        private readonly int _bufferSize = 8192;

        private string _mediaType = string.Empty;
        private long _contentLength = 0;
        private string _fileName;

        private HttpClient _client;

        public Downlaoder(string url, string path) {
            _url = url;
            _path = path;
        }

        public async Task Start(Action<double> onDownloadingCallback, Action onFinishedCallback) {
            _client = new HttpClient();

            await setDownloadMetadata();

            using Stream stream = await _client.GetStreamAsync(_url);
            using Stream fStream = new FileStream(_path + _fileName, FileMode.Create, FileAccess.Write, FileShare.None, _bufferSize, true);

            byte[] buffer = new byte[_bufferSize];
            int bytesRead = 0;
            double downloaded = 0;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, _bufferSize)) > 0) {
                await fStream.WriteAsync(buffer, 0, bytesRead);
                downloaded += bytesRead;
                onDownloadingCallback(downloaded / _contentLength);
            }
            onFinishedCallback();
        }

        private async Task setDownloadMetadata() {
            var response = await _client.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);

            _contentLength = (long)response.Content.Headers.ContentLength;
            _mediaType = response.Content.Headers.ContentType.MediaType.Split('/')[1];
            _fileName = $"/file.{_mediaType}";
                
        }
    }
}
