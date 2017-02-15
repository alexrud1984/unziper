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

        private FileSystemWatcher watcher;

        private string targetFolder;

        public string TargetFolder
        {
            get
            {
                return targetFolder;
            }

            set
            {
 //               WindowsEventsDetach();
                targetFolder = value;
                watcher.Path = value;
 //               WindowsEventsAttach();
                watcher.EnableRaisingEvents=true;
                GetFilesLsit();
            }
        }

        public string UnzippedFile { set; get; }

        public UnzipModel()
        {
            filesList = new List<FileInfo>();
            watcher = new FileSystemWatcher();
        }

        public event FileUnzippedEventHandler FileUnzipped;

        public void Unzip()
        {
            foreach (var item in filesList)
            {
                if (String.Equals(item.Extension,".zip"))
                {
                    ZipFile.ExtractToDirectory(item.FullName, targetFolder);
                    FileUnzipped(item.FullName);
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

        private void WindowsEventsAttach()
        {
            if (watcher != null)
            {
                watcher.Created += Watcher_Created;
            }
        }


        private void WindowsEventsDetach()
        {
            if (watcher != null)
            {
                watcher.Created -= Watcher_Created;
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            OnFileUnzipped(e.FullPath);
        }
    }
}
