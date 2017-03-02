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
                GetFilesLsit();
            }
        }

        public string UnzippedFile { set; get; }

        public UnzipModel()
        {
            filesList = new List<FileInfo>();
        }

        public event FileUnzippedEventHandler ActionData;
        public event UnzipFinishedEventHandler UnzipFinished;
        public event CopyingFinisedEventHandler CopyingFinised;

        public async void Unzip()
        {
            foreach (var item in filesList)
            {

                if (String.Equals(item.Extension, ".zip"))
                {
                    DirectoryInfo di = new DirectoryInfo(item.FullName);
                    ZipArchive za = ZipFile.OpenRead(item.FullName);
                    ActionData("Start to unzip: " + item.FullName);
                    foreach (var entry in za.Entries)
                    {
                        try
                        {
                            string toFolder = Path.Combine(targetFolder, entry.FullName.Remove(entry.FullName.Length - entry.Name.Length - 1));
                            if (!Directory.Exists(toFolder))
                            {
                                Directory.CreateDirectory(toFolder);
                                ActionData("Folder created" + toFolder);
                            }
                            if (!String.IsNullOrEmpty(entry.Name))
                            {
                                ActionData("Extracting file: " + entry.FullName + " ...");
                                await Task.Run(() => entry.ExtractToFile(targetFolder + "\\" + entry.FullName, true));
                            }
                        }
                        catch (Exception ex)
                        {
                            ActionData("ERROR " + ex.Message);
                        }
                    }
                    ActionData("Finish to unzip: " + item.FullName);
                }
            }
            OnUnzipFinished(targetFolder);
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
            if (ActionData != null)
            {
                ActionData(file);
            }
        }

        private void OnUnzipFinished(string targetFolder)
        {
            if (UnzipFinished != null)
            {
                UnzipFinished(targetFolder);
            }
        }

        private void OnCopyingFinised()
        {
            if (CopyingFinised != null)
            {
                CopyingFinised();
            }
        }

        public async void Copy(List<FileCheck> sourceFiles)
        {
            foreach (var item in sourceFiles)
            {
                if (item.IsChecked)
                {
                    if (item.IsDirectory)
                    {
                        DirectoryInfo di = new DirectoryInfo(item.FullName);
                        if (!System.IO.Directory.Exists(Path.Combine(targetFolder, di.Name)))
                        {
                            try
                            {
                                ActionData("Copy folder: " + di.FullName);
                                Directory.CreateDirectory(Path.Combine(targetFolder, di.Name));
                            }
                            catch (Exception ex)
                            {
                                ActionData("ERROR " + ex.Message);
                            }
                        }

                        DirectoryCopy(item.FullName, Path.Combine(targetFolder, di.Name));
                    }
                    else
                    {
                        try
                        {
                            ActionData("Start copy: " + item.FullName + "...");
                            await CopyFileAsync(item.FullName, Path.Combine(targetFolder, item.Name));
                            ActionData("File copied: " + item.FullName);
                        }
                        catch (Exception ex)
                        {
                            ActionData("ERROR " + ex.Message);
                        }
                    }
                }
            }
            OnCopyingFinised();
            ActionData("Copying finished!");
        }

        private async void DirectoryCopy(string source, string target)
        {
            //files copy
            foreach (var item in Directory.GetFiles(source))
            {
                try
                {
                    ActionData("Start copy: " + item + "...");
                    FileInfo fi = new FileInfo(item);
                    await CopyFileAsync(item, Path.Combine(target, fi.Name));
                    ActionData("File copied: " + item);
                }
                catch (Exception ex)
                {
                    ActionData("ERROR " + ex.Message);
                }
            }

            //subdirs copy
            foreach (var item in Directory.GetDirectories(source))
            {
                DirectoryInfo di = new DirectoryInfo(item);
                if (!System.IO.Directory.Exists(Path.Combine(target, di.Name)))
                {
                    try
                    {
                        ActionData("Copy folder: " + di.FullName);
                        Directory.CreateDirectory(Path.Combine(target, di.Name));
                    }
                    catch (Exception ex)
                    {
                        ActionData("ERROR " + ex.Message);
                    }
                }
                DirectoryCopy(item, Path.Combine(target, di.Name));
            }
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.OpenRead(sourcePath))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
    }
}
