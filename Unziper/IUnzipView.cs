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
        string TargetFolder { set; get; }

        string UnzippedFile { set; get; }

        event FolderSelectedEventHandler FolderSelected;

        event UnzipEventHandler Unzipped;
    }
}
