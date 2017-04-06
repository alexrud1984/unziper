using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Unziper
{
    public delegate void FileUnzippedEventHandler(string sender);
    public delegate void UnzipFinishedEventHandler(string sender);
    public delegate void CopyingFinisedEventHandler();

    public interface IUnzipModel
    {
        CancellationTokenSource UnzipCancelTokenSrc { set; get; }
        CancellationTokenSource CopyCancelTokeSrc { set; get; }
        string TargetFolder { set; get; }
        string UnzippedFile { set; get; }

        event FileUnzippedEventHandler ActionData;
        event UnzipFinishedEventHandler UnzipFinished;
        event CopyingFinisedEventHandler CopyingFinised;

        void Unzip();
        void Copy(List<FileCheck> sourceFiles);
        void Cancel();
    }
}
