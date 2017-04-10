using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;

namespace Unziper
{
    public static class AsyncExtension
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<double> progress, CancellationToken cancellationToken, int bufferSize = 0x1000)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;
            long totalRead = 0;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                totalRead += bytesRead;
                progress.Report(totalRead);
            }
        }
        public static async Task ExtractToAsync(ZipEntry source, string destinationPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            using (Ionic.Crc.CrcCalculatorStream src = source.OpenReader())
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await CopyToAsync(src, destination, progress, cancellationToken);
                }
            }
        }
        public static async Task CopyFileAsync(string sourcePath, string destinationPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            using (Stream source = File.OpenRead(sourcePath))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await CopyToAsync(source, destination, progress, cancellationToken);
                }
            }
        }

    }
}
