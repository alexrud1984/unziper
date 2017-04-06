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
        private List<FileInfo> filesList;
        private string targetFolder;
        public CancellationTokenSource UnzipCancelTokenSrc { set; get; }
        public CancellationTokenSource CopyCancelTokeSrc { set; get; }

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
        public string UnzippedFile { set; get; }
        public bool Autodelete { set; get; }

        public UnzipModel()
        {
            filesList = new List<FileInfo>();
        }

        public event FileUnzippedEventHandler ActionData;
        public event UnzipFinishedEventHandler UnzipFinished;
        public event CopyingFinisedEventHandler CopyingFinised;

        public async void Unzip()
        {
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
                        ActionData("File extracted: " + entry.FileName);
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
            CopyCancelTokeSrc = new CancellationTokenSource();
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
                            CopyCancelTokeSrc.Token.ThrowIfCancellationRequested();
                            ActionData("Start copy: " + item.FullName + "...");
                            await CopyFileAsync(item.FullName, Path.Combine(targetFolder, item.Name));
                            ActionData("File copied: " + item.FullName);
                        }
                        catch (OperationCanceledException ex)
                        {
                            ActionData(ex.Message);
                            CopyCancelTokeSrc.Dispose();
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
            if (CopyCancelTokeSrc != null)
            {
                CopyCancelTokeSrc.Cancel();
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
    }
}
