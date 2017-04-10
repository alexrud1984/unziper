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
        public double UnzippedListSize
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
        public event UpdateProgressEventHandler UpdateProgress;
        public event OperationCanceledEventHandler OperationCanceled;
        #endregion

        #region Public methods
        public async void Unzip()
        {
            unzippedListSize = 0;
            var progressHandler = new Progress<double>(value =>
            {
                OnUpdateProgress(unzippedListSize + value);
            });
            FillZipListSize();
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
                        var progress = progressHandler as IProgress<double>;
                        UnzipCancelTokenSrc.Token.ThrowIfCancellationRequested();
                        await AsyncExtension.ExtractToAsync(entry, Path.Combine(targetFolder, entry.FileName), progress, UnzipCancelTokenSrc.Token);
                        unzippedListSize += entry.Info.Length;
                        OnFileUnzipped(entry.FileName);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ActionData(ex.Message);
                        UnzipCancelTokenSrc.Dispose();
                        OperationCanceled();
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
            var progressHandler = new Progress<double>(value =>
            {
                OnUpdateProgress(copiedListSize + value);
            });
            foreach (var item in sourceFiles)
            {
                if (item.IsChecked)
                {
                    if (item.IsDirectory)
                    {
                        try
                        {
                            DirectoryInfo di = new DirectoryInfo(item.FullName);
                            CreateDirectory(di);
                            await DirectoryCopy(item.FullName, Path.Combine(targetFolder, di.Name), progressHandler);
                        }
                        catch (OperationCanceledException ex)
                        {
                            ActionData(ex.Message);
                            CopyCancelTokenSrc.Dispose();
                            OperationCanceled();
                            return;
                        }
                        catch (Exception ex)
                        {
                            ActionData("ERROR " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            await FileCopy(item.FullName, targetFolder, progressHandler);
                        }
                        catch (OperationCanceledException ex)
                        {
                            ActionData(ex.Message);
                            CopyCancelTokenSrc.Dispose();
                            OperationCanceled();
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
        private void FillZipListSize()
        {
            toUnzipListSize = 0;
            DirectoryInfo diTarget = new DirectoryInfo(targetFolder);
            foreach (var item in diTarget.GetFiles("*.zip"))
            {
                try
                {
                    ZipFile zf = ZipFile.Read(item.FullName);
                    toUnzipListSize += zf.Info.Length;
                }
                catch (Exception ex)
                {
                    ActionData(ex.Message);
                }
            }
        }
        private async Task DirectoryCopy(string source, string target, Progress<double> progress)
        {
            //files copy
                foreach (var item in Directory.GetFiles(source))
                {
                    await FileCopy(item, target, progress);
                }
                //subdirs copy
                foreach (var item in Directory.GetDirectories(source))
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    if (!System.IO.Directory.Exists(Path.Combine(target, di.Name)))
                    {
                        ActionData("Copy folder: " + di.FullName);
                        Directory.CreateDirectory(Path.Combine(target, di.Name));
                    }
                    await DirectoryCopy(item, Path.Combine(target, di.Name), progress);
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
        private void CreateDirectory(DirectoryInfo di)
        {
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
        }
        private async Task FileCopy(string sourceItem, string target, IProgress<double> progress)
        {
            ActionData("Start copy: " + sourceItem + "...");
            FileInfo fi = new FileInfo(sourceItem);
            await AsyncExtension.CopyFileAsync(sourceItem, Path.Combine(target, fi.Name), progress, CopyCancelTokenSrc.Token);
            copiedListSize += (new FileInfo(sourceItem)).Length;
            FileCopied(sourceItem);
        }
        #endregion

        #region On events methods
        private void OnOperationCanceled()
        {
            if (OperationCanceled != null)
            {
                OperationCanceled();
            }
        }
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
        private void OnUpdateProgress(double value)
        {
            if (UpdateProgress != null)
            {
                UpdateProgress(value);
            };
        }
        #endregion
    }
}
