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
    public class UnzipModel : IUnzipModel
    {
        #region Fields
        private List<FileInfo> filesList;
        private string targetFolder;
        private double toCopyListSize;
        private double copiedListSize;
        private double toUnzipListSize;
        private double unzippedListSize;
        #endregion

        #region Properties
        public CancellationTokenSource UnzipCancelTokenSrc { set; get; }
        public CancellationTokenSource CopyCancelTokenSrc { set; get; }
        public double ToCopyListSize
        {
            get
            {
                return toCopyListSize;
            }
        }
        public double CopiedListSize
        {
            get
            {
                return copiedListSize;
            }
        }
        public string TargetFolder
        {
            get
            {
                return targetFolder;
            }

            set
            {
                targetFolder = value;
            }
        }
        public double ToUnzipListSize
        {
            get
            {
                return toUnzipListSize;
            }
        }
        public double UnsippedListSize
        {
            get
            {
                return unzippedListSize;
            }
        }
        public string UnzippedFile { set; get; }
        public bool Autodelete { set; get; }
        public UnzipModel()
        {
            filesList = new List<FileInfo>();
        }
        #endregion

        #region Events
        public event ActionDataEventHandler ActionData;
        public event UnzipFinishedEventHandler UnzipFinished;
        public event CopyingFinisedEventHandler CopyingFinised;
        public event FileCopiedEventHandler FileCopied;
        public event FileUnzippedEventHandler FileUnzipped;
        #endregion

        #region Public methods
        public async void Unzip()
        {
            unzippedListSize = 0;
            GetZipListSize();
            UnzipCancelTokenSrc = new CancellationTokenSource();
            DirectoryInfo diTarget = new DirectoryInfo(targetFolder);
            foreach (var item in diTarget.GetFiles("*.zip"))
            {
                ZipFile zf = ZipFile.Read(item.FullName);
                ActionData("Start to unzip: " + item.FullName);
                foreach (var entry in zf.Entries)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            UnzipCancelTokenSrc.Token.ThrowIfCancellationRequested();
                            entry.Extract(targetFolder, ExtractExistingFileAction.OverwriteSilently);
                        });
                        OnFileUnzipped(entry.FileName);
                        unzippedListSize += entry.Info.Length;
                    }
                    catch (OperationCanceledException ex)
                    {
                        ActionData(ex.Message);
                        UnzipCancelTokenSrc.Dispose();
                        return;
                    }
                    catch (Exception ex)
                    {
                        ActionData("ERROR " + ex.Message);
                    }
                }
                ActionData("Finish to unzip: " + item.FullName);
                if (Autodelete)
                {
                    try
                    {
                        zf.Dispose();
                        item.Delete();
                        ActionData(item.Name + " - deleted");
                    }
                    catch(Exception ex)
                    {
                        ActionData(ex.Message);
                    }
                }
            }
            OnUnzipFinished(targetFolder);
        }
        public async void Copy(List<FileCheck> sourceFiles)
        {
            CopyCancelTokenSrc = new CancellationTokenSource();
            FillItemsListSize(sourceFiles);
            copiedListSize = 0;
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
                            CopyCancelTokenSrc.Token.ThrowIfCancellationRequested();
                            ActionData("Start copy: " + item.FullName + "...");
                            await CopyFileAsync(item.FullName, Path.Combine(targetFolder, item.Name));
                            copiedListSize += (new FileInfo(item.FullName)).Length;
                            FileCopied(item.FullName);
                        }
                        catch (OperationCanceledException ex)
                        {
                            ActionData(ex.Message);
                            CopyCancelTokenSrc.Dispose();
                            return;
                        }
                        catch (Exception ex)
                        {
                            ActionData("ERROR " + ex.Message);
                        }
                    }
                }
            }
            ActionData("Copying finished!");
            OnCopyingFinised();
        }
        public void Cancel()
        {
            if (UnzipCancelTokenSrc != null)
            {
                UnzipCancelTokenSrc.Cancel();
            }
            if (CopyCancelTokenSrc != null)
            {
                CopyCancelTokenSrc.Cancel();
            }
        }
        #endregion

        #region Private mathods
        private void FillItemsListSize(List<FileCheck> _sourceList)
        {
            toCopyListSize = 0;
            foreach (var item in _sourceList)
            {
                if (item.IsDirectory && item.IsChecked)
                {
                    toCopyListSize += GetDirectorySize(item.FullName);
                }
                else if (item.IsChecked)
                {
                    toCopyListSize += (new FileInfo(item.FullName)).Length;
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
        private async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.OpenRead(sourcePath))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
        private double GetDirectorySize(string _path)
        {
            double _size = 0;
            _size = GetFilesSizeInDir(_path);
            foreach (var item in Directory.GetDirectories(_path))
            {
                _size += GetDirectorySize(item);
            }
            return _size;
        }
        private double GetFilesSizeInDir(string _path)
        {
            double _size = 0;
            foreach (var item in Directory.GetFiles(_path))
            {
                FileInfo fi = new FileInfo(item);
                _size += fi.Length;
            }
            return _size;
        }
        private void GetZipListSize()
        {
            toUnzipListSize = 0;
            DirectoryInfo diTarget = new DirectoryInfo(targetFolder);
            foreach (var item in diTarget.GetFiles("*.zip"))
            {
                ZipFile zf = ZipFile.Read(item.FullName);
                toUnzipListSize += zf.Info.Length;
            }
        }
        #endregion

        #region On events methods
        private void OnActionData(string file)
        {
            if (ActionData != null)
            {
                ActionData(file);
            }
        }
        private void OnFileCopied(string file)
        {
            if (FileCopied != null)
            {
                FileCopied(file);
            }
        }
        private void OnFileUnzipped(string file)
        {
            if (FileUnzipped != null)
            {
                FileUnzipped(file);
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
        #endregion
    }
}
