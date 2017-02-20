using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public delegate void FolderSelectedEventHandler(IUnziperView sender);

    public delegate void UnzipEventHandler(IUnziperView sender);

    public interface IUnziperView
    {
        string SourceFolder { set; get; }

        string UnzippedFile { set; get; }

        List<FileCheck> SourceList { set; }

        event FolderSelectedEventHandler SourceFolderSelected;

        event UnzipEventHandler Unzipped;
    }
}
