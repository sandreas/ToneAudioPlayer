using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ToneAudioPlayer.Services;

public static class StreamExtensions
{
    public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress,
        int bufferSize = 81920, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[bufferSize];
        int bytesRead;
        long totalRead = 0;
        while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            totalRead += bytesRead;
            progress.Report(totalRead);
        }
    }
}

public sealed class SynchronousProgress<T> : IProgress<T>
{
    private readonly Action<T> _callback;

    public SynchronousProgress(Action<T> callback)
    {
        _callback = callback;
    }

    void IProgress<T>.Report(T data) => _callback(data);
}

public class Download
{
    public readonly string Id;
    public readonly DownloadPackage Package;

    public readonly Uri Source;
    public readonly Uri Destination;


    // public readonly IProgress<Download>? ProgressHandler;
    public CancellationToken CancelToken;
    

    public float Progress;
    public bool IsFinished => Progress >= 1;

    public Download(string id, DownloadPackage package, Uri source, Uri destination, CancellationToken? ct = null)
    {
        Id = id;
        Package = package;
        Source = source;
        Destination = destination;
        // ProgressHandler = progressHandler;
        CancelToken = ct ?? CancellationToken.None;
    }

}

public enum DownloadPackageStatus
{
    None,
    Enqueued,
    Downloading,
    Paused,
    Finished
}

public delegate void StatusUpdated(DownloadPackage package);

public class DownloadPackage
{
    public readonly string Id;
    public readonly object? Context;
    public readonly List<Download> Downloads = new();

    public DownloadPackageStatus Status = DownloadPackageStatus.None;
    
    
    public event StatusUpdated? StatusUpdated;
    
    public float Progress => Downloads.Count == 0 ? -1 : Downloads.Sum(d => d.Progress) / Downloads.Count;
    // 100, 100, 100
    // 0.1, 0.25, 0
    // 10, 50
    // 10%
    // 
    
    // 150 => 100%
    // 150 * 0.1 => ?%
    // count * sum(Progress) / 100 
    // sum(download.progress) / downloads.Count
    
    
    // public float Progress => Downloads.Select(d => d.Progress)
    
    public DownloadPackage(string id, object? context = null)
    {
        Id = id;
        Context = context;
    }

    /*
    public float CalculateProgress()
    {
        var sumProgress = Downloads.Sum(d => d.Progress);
        var count = Downloads.Count;
        var totalProgress = sumProgress / Downloads.Count;
        return totalProgress;
    }
    */
    
    public void UpdateStatus(DownloadPackageStatus status)
    {
        // ProgressHandler?.Report(this);
        Status = status;
        StatusUpdated?.Invoke(this);
    }

}

public class DownloadService
{
    private readonly HttpClient _http;
    private static readonly BufferBlock<Download> Queue = new();
    private static readonly List<Download> ActiveDownloads = new();

    private static readonly int MaxConcurrentDownloads = 1;


    public DownloadService(HttpClient http)
    {
        _http = http;
    }


    public void Download(DownloadPackage downloadPackage, CancellationToken? ct = null)
    {
        foreach (var download in downloadPackage.Downloads)
        {
            download.CancelToken = ct ?? CancellationToken.None;
            Queue.Post(download);
        }
        Task.Run(ConsumeDownloads);
    }

    private async Task? ConsumeDownloads()
    {
        if (ActiveDownloads.Count > 0)
        {
            return;
        }

        while (await Queue.OutputAvailableAsync() && Queue.TryReceive(out var download))
        {
            if (ActiveDownloads.Count >= MaxConcurrentDownloads)
            {
                await Task.Delay(10);
            }

            _ = Task.Run(async () => await DownloadNext(download));
        }
    }

    private async Task DownloadNext(Download download)
    {
        ActiveDownloads.Add(download);
        // IProgress<float>? progress = null;
        using var response = await _http.GetAsync(download.Source, HttpCompletionOption.ResponseHeadersRead);
        var contentLength = response.Content.Headers.ContentLength ?? -1;

        var destination = File.OpenWrite(download.Destination.LocalPath);

        await using var dl = await response.Content.ReadAsStreamAsync();

        // var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
        // Use extension method to report progress while downloading
        var relativeProgress = new SynchronousProgress<long>(progressBytes =>
        {
            // 1234 => 1
            // 345 => 0.27
            // 1/totalBytes * downloadedBytes => 0.27
            download.Progress = CalculateByteProgress(progressBytes, contentLength);
            download.Package.UpdateStatus(DownloadPackageStatus.Downloading);
        });
        await dl.CopyToAsync(destination, relativeProgress, 81920, download.CancelToken); // 81920,

        // await dl.CopyToAsync(destination);
        ActiveDownloads.Remove(download);
    }

    private float CalculateByteProgress(long progressBytes, long contentLength)
    {
        if (contentLength <= 0)
        {
            return -1;
        }

        if (progressBytes >= contentLength)
        {
            return 1f;
        }

        return 1f / contentLength * progressBytes;
    }

    /*
    private void BtnDownload_Click()
    {
        using (WebClient wc = new WebClient())
        {
            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileAsync (
                // Param1 = Link of file
                new System.Uri("http://www.sayka.com/downloads/front_view.jpg"),
                // Param2 = Path to save
                "D:\\Images\\front_view.jpg"
            );
        }
    }
    */
    /*

        public static bool DownloadFile(string url, string filepath)
        {
            WebClient client = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
                { return true; };

            // Add a user agent header in case the
            // requested URI contains a query.
            int count = 1;
            bool result = true;
            Retry.On<Exception>().For(3).With(context =>
                {
                    try
                    {
                        client.Headers.Add("user-agent",
                                           "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                        client.OpenRead(url);

                        FileInfo finfo = null;

                        if (File.Exists(filepath))
                        {
                            finfo = new FileInfo(filepath);

                            if (client.ResponseHeaders != null &&
                                finfo.Length >= Convert.ToInt64(client.ResponseHeaders["Content-Length"]))
                            {
                                File.Delete(filepath);
                            }
                        }

                        DownloadFileWithResume(url, filepath);


                        // Refresh the file content size
                        finfo = finfo ?? new FileInfo(destinationFilePath);

                        if (finfo.Length == Convert.ToInt64(client.ResponseHeaders["Content-Length"]))
                        {
                            result = true;
                            Console.WriteLine("Download success!!!");
                        }
                        else
                        {
                            result = false;
                            throw new WebException("Download interrupted. Retrying download.. ");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0} {1}", ex.Message, Environment.NewLine);
                        Console.WriteLine("Download inturrpted retry({0}).. in 5 seconds", count++);
                        System.Threading.Thread.Sleep(5000);
                        throw;
                    }
                });

            return false;
        }

        private static void DownloadFileWithResume(string sourceUrl, string destinationPath)
        {
            long existLen = 0;
            System.IO.FileStream saveFileStream;
            if (System.IO.File.Exists(destinationPath))
            {
                System.IO.FileInfo fINfo =
                    new System.IO.FileInfo(destinationPath);
                existLen = fINfo.Length;
            }
            if (existLen > 0)
                saveFileStream = new System.IO.FileStream(destinationPath,
                                                          System.IO.FileMode.Append, System.IO.FileAccess.Write,
                                                          System.IO.FileShare.ReadWrite);
            else
                saveFileStream = new System.IO.FileStream(destinationPath,
                                                          System.IO.FileMode.Create, System.IO.FileAccess.Write,
                                                          System.IO.FileShare.ReadWrite);

            System.Net.HttpWebRequest httpWebRequest;
            System.Net.HttpWebResponse httpWebResponse;
            httpWebRequest = (System.Net.HttpWebRequest) System.Net.HttpWebRequest.Create(sourceUrl);
            httpWebRequest.AddRange((int) existLen);
            System.IO.Stream smRespStream;
            httpWebResponse = (System.Net.HttpWebResponse) httpWebRequest.GetResponse();
            smRespStream = httpWebResponse.GetResponseStream();
            var abc = httpWebRequest.Timeout;

            smRespStream.CopyTo(saveFileStream);
            saveFileStream.Close();
        }
*/
    /*
    public static void DoSomething()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        if (UseProxy)
        {
            request.Proxy = new WebProxy(ProxyServer + ":" + ProxyPort.ToString());
            if (ProxyUsername.Length > 0)
                request.Proxy.Credentials = new NetworkCredential(ProxyUsername, ProxyPassword);
        }
//HttpWebRequest hrequest = (HttpWebRequest)request;
//hrequest.AddRange(BytesRead); ::TODO: Work on this
        if (BytesRead > 0) request.AddRange(BytesRead);

        WebResponse response = request.GetResponse();
//result.MimeType = res.ContentType;
//result.LastModified = response.LastModified;
        if (!resuming)//(Size == 0)
        {
            //resuming = false;
            Size = (int)response.ContentLength;
            SizeInKB = (int)Size / 1024;
        }
        acceptRanges = String.Compare(response.Headers["Accept-Ranges"], "bytes", true) == 0;

//create network stream
        ns = response.GetResponseStream();
    }
    */
    /*
    static bool DownloadFileWithRange()
    {
        string link = "http://freelistenonline.com/"; //<- link to some big file
        string FilePath = @"C:\Test\1.zip";

        if (File.Exists(FilePath))
            File.Delete(FilePath);

        long totalBytesRead = 0;
        long MaxContentLength = 0;
        long RequestContentLength = 0;
        int i = 0;
        while (MaxContentLength == 0 || totalBytesRead < MaxContentLength)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);

            if (totalBytesRead > 0) request.AddRange(totalBytesRead);

            WebResponse response = request.GetResponse();

            Console.WriteLine("=============== Request #{0} ==================", ++i);
            foreach (var header in response.Headers)
            {
                if (header.ToSaveString().Contains("Content-Length") || header.ToSaveString().Contains("Content-Range"))
                    Console.WriteLine("{0}: {1}", header, response.Headers[header.ToString()]);
            }

            if (response.ContentLength > MaxContentLength)
                MaxContentLength = response.ContentLength;

            var ns = response.GetResponseStream();
            RequestContentLength = 0;
            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (FileStream localFileStream = new FileStream(FilePath, FileMode.Append))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead += bytesRead;
                            RequestContentLength += bytesRead;
                            localFileStream.Write(buffer, 0, bytesRead);
                        }

                        Console.WriteLine("Got bytes: {0}", RequestContentLength);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got bytes: {0}", RequestContentLength);
            }
        }

        if (MaxContentLength == totalBytesRead)
            return true;

        return false;
    }
    */
}