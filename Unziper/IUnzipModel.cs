using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public delegate void FileUnzippedEventHandler(string sender);

    public delegate void UnzipFinishedEventHandler(string sender);

    public delegate void CopyingFinisedEventHandler();

    public interface IUnzipModel
    {
        string TargetFolder { set; get; }

        string UnzippedFile { set; get; }

        event FileUnzippedEventHandler ActionData;

        event UnzipFinishedEventHandler UnzipFinished;

        event CopyingFinisedEventHandler CopyingFinised;

        void Unzip();

        void Copy(List<FileCheck> sourceFiles);
    }
}
