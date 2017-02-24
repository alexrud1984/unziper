using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public delegate void FileUnzippedEventHandler(string sender);

    public delegate void FileExistsEventHandler(string sender);

    public interface IUnzipModel
    {
        string TargetFolder { set; get; }

        string UnzippedFile { set; get; }

        event FileUnzippedEventHandler ActionData;

        void Unzip();

        void Copy(List<FileCheck> sourceFiles);
    }
}
