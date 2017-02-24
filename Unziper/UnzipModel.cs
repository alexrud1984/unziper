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

        public void Unzip()
        {
            foreach (var item in filesList)
            {
                if (String.Equals(item.Extension, ".zip"))
                {
                    ZipArchive za = ZipFile.OpenRead(item.FullName);
                    ActionData("Start to unzip: " + item.FullName);
                    foreach (var entry in za.Entries)
                    {
                        entry.ExtractToFile(targetFolder+"\\"+entry.Name, true);
                    }
                    ActionData("Finish to unzip: " + item.FullName);
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
            if (ActionData != null)
            {
                ActionData(file);
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
                                ActionData(ex.Message);
                            }
                        }

                        DirectoryCopy(item.FullName, Path.Combine(targetFolder, di.Name));
                    }
                    else
                    {
                        try
                        {
                            ActionData("Start copy: " + item.FullName);
                            await CopyFileAsync(item.FullName, Path.Combine(targetFolder, item.Name));
                            //                           File.Copy(item.FullName, Path.Combine(targetFolder,item.Name), true);
                            ActionData("File copied: " + item.FullName);
                        }
                        catch (Exception ex)
                        {
                            ActionData(ex.Message);
                        }
                    }
                }
            }
        }

        private async void DirectoryCopy(string source, string target)
        {
            //files copy
            foreach (var item in Directory.GetFiles(source))
            {
                try
                {
                    ActionData("Start copy: " + item);
                    FileInfo fi = new FileInfo(item);
                    await CopyFileAsync(item, Path.Combine(target, fi.Name));
                    ActionData("File copied: " + item);
                }
                catch (Exception ex)
                {
                    ActionData(ex.Message);
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
                        ActionData(ex.Message);
                    }
                }
                DirectoryCopy(item, Path.Combine(target, di.Name));
            }
        }

        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
    }
}
