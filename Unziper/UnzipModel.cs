using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public class UnzipModel : IUnzipModel
    {
        private List<FileInfo> filesList;

        private string targetFolder;

        public string TargetFolder
        {
            get
            {
                return targetFolder;
            }

            set
            {
                targetFolder = value;
                watcher.Path = value;
                GetFilesLsit();
            }
        }

        public string UnzippedFile { set; get; }

        public UnzipModel()
        {
            filesList = new List<FileInfo>();
        }

        public event FileUnzippedEventHandler FileUnzipped;
        public event FileExistsEventHandler FileExists;

        public void Unzip()
        {
            foreach (var item in filesList)
            {
                if (String.Equals(item.Extension, ".zip"))
                {
                    ZipArchive za = ZipFile.OpenRead(item.FullName);
                    FileUnzipped("Start to unzip: " + item.FullName);
                    foreach (var entry in za.Entries)
                    {
                        entry.ExtractToFile(targetFolder+"\\"+entry.Name, true);
                    }
                    FileUnzipped("Finish to unzip: " + item.FullName);
                }
            }
        }

        private void GetFilesLsit()
        {
            if (String.IsNullOrWhiteSpace(targetFolder))
            {
                return;
            }
            try
            {

                foreach (var item in Directory.GetFiles(targetFolder))
                {
                    filesList.Add(new FileInfo (item));
                }
            }
            catch(Exception)
            {
                filesList.Clear();
                GetFilesLsit();
            }
        }

        private void OnFileUnzipped(string file)
        {
            if (FileUnzipped != null)
            {
                FileUnzipped(file);
            }
        }

        private void OnFileExists(string file)
        {
            if (FileExists != null)
            {
                FileExists(file);
            }
        }

    }
}
