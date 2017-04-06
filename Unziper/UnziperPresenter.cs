using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Unziper
{
    class UnziperPresenter
    {
        private IUnziperView view;
        private IUnzipModel model;
        private List<FileCheck> sourceFilesList = new List<FileCheck>();
        private List<FileListView> sourceFilesView = new List<FileListView>();

        public UnziperPresenter(IUnziperView view)
        {
            this.AttachUnziperView(view);
            this.view = view;
            model = new UnzipModel();
            AttachUziperModel(model);
        }

        private void AttachUziperModel(IUnzipModel model)
        {
            model.ActionData += Model_ActionData;
            model.UnzipFinished += Model_UnzipFinished;
            model.CopyingFinised += Model_CopyingFinised;
            model.FileCopied += Model_FileCopied;
            model.FileUnzipped += Model_FileUnzipped;
        }

        private void Model_FileUnzipped(string fileName)
        {
            view.UnzippedFile = String.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] File unzipped {1}\r\n", DateTime.Now, fileName);
            view.ProgressBarCurrent = model.UnsippedListSize;
        }

        private void Model_FileCopied(string fileName)
        {
            view.UnzippedFile = String.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] File copied {1}\r\n", DateTime.Now, fileName);
            view.ProgressBarCurrent = model.CopiedListSize;
        }

        private void Model_CopyingFinised()
        {
            view.Status="Finished to copy";
            view.IsProgressBarEnabled = false;
            if (view.AutoUnzip)
            {
                View_Unzip(view);
            }
        }

        private void Model_UnzipFinished(string sender)
        {
            view.Status = "Finished to unzip";
            view.ShowMessage("Done! Put the SQL package in relevant folder.");
            view.IsProgressBarEnabled = false;
        }

        private void Model_ActionData(string sender)
        {
            view.UnzippedFile = String.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n", DateTime.Now, sender);
        }

        private void AttachUnziperView(IUnziperView view)
        {
            view.SourceFolderSelected += View_SourceFolderSelected;
            view.TargetFolderSelected += View_TargetFolderSelected;
            view.UnzippedClick += View_Unzip;
            view.CopyClick += View_CopyClick;
            view.ItemCheckChanged += View_ItemCheckChanged;
            view.CancelClick += View_CancelClick;
            view.AutodeleteChanged += View_AutodeleteChanged;
        }

        private void View_AutodeleteChanged(bool isChecked)
        {
            model.Autodelete=isChecked;
        }

        private void View_CancelClick()
        {

            if (model.UnzipCancelTokenSrc != null)
            {
                if (model.UnzipCancelTokenSrc.IsCancellationRequested != true)
                {
                    view.Status = "Cancelling...";
                    model.UnzipCancelTokenSrc.Cancel();
                }
            }
            if (model.CopyCancelTokenSrc != null)
            {
                if (model.CopyCancelTokenSrc.IsCancellationRequested != true)
                {
                    view.Status = "Cancelling...";
                    model.CopyCancelTokenSrc.Cancel();
                }
            }
        }
        private void View_ItemCheckChanged(int id, bool isChecked)
        {
            foreach (var item in sourceFilesList)
            {
                if (item.Id == id)
                {
                    item.IsChecked = isChecked;
                    break;
                }
            }
            foreach (var item in sourceFilesView)
            {
                if (item.Id == id)
                {
                    item.IsChecked = isChecked;
                    break;
                }
            }
            CopyButtonReflect();
        }
        private void CopyButtonReflect()
        {
            bool _isCopyEnabled = false;
            foreach (var item in sourceFilesList)
            {
                if (item.IsChecked == true)
                {
                    _isCopyEnabled = true;
                }
            }
            view.IsCopyEnabled = _isCopyEnabled;
        }
        private void View_CopyClick()
        {
            view.Status = "Copying...";
            if (!System.IO.Directory.Exists(view.SourceFolder))
            {
                view.ShowMessage("Source folder doesn't exsists or field is empty.");
                return;
            }
            if (!System.IO.Directory.Exists(view.TargetFolder))
            {
                view.ShowMessage("Traget folder doesn't exsists or field is empty.");
                return;
            }
            model.TargetFolder = view.TargetFolder;
            model.Copy(sourceFilesList);
            view.ProgressBarMax = model.ToCopyListSize;
            view.ProgressBarCurrent = 0;
            view.IsProgressBarEnabled = true;
        }
        private void View_TargetFolderSelected(string targetFolder)
        {
            if (!String.IsNullOrEmpty(targetFolder))
            {
                DirectoryInfo di = new DirectoryInfo(targetFolder);
                if (di.Exists)
                {
                    model.TargetFolder = targetFolder;
                    view.IsUnzipEnabled = true;
                }
                else
                {
                    view.IsUnzipEnabled = false;
                }
            }
            else
            {
                view.ShowMessage("Selected target folder doesn't exist!");
            }
        }
        private void View_SourceFolderSelected(IUnziperView sender)
        {
            sourceFilesList.Clear();
            SourceFilesListLoad(sender.SourceFolder);
            sourceFilesView.Clear();
            SourceFilesViewLoad();
            sender.SourceList = sourceFilesView;
        }
        private void SourceFilesViewLoad()
        {
            if (sourceFilesList != null)
            {
                foreach (var item in sourceFilesList)
                {
                    System.Windows.Media.ImageSource fileIcon;
                    if (item.IsDirectory)
                    {
                        using (System.Drawing.Icon sysicon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FolderClosed.ico")))
                        {
                            fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                      sysicon.Handle,
                                      System.Windows.Int32Rect.Empty,
                                      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }
                    }
                    else
                    if (String.Equals(item.Ext, ".zip"))
                    {
                        using (System.Drawing.Icon sysicon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zip.ico")))
                        {
                            fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                      sysicon.Handle,
                                      System.Windows.Int32Rect.Empty,
                                      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }

                    }
                    else
                    {
                        try
                        {
                            using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(item.FullName))
                            {
                                fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                          sysicon.Handle,
                                          System.Windows.Int32Rect.Empty,
                                          System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                            }
                        }
                        catch (Exception ex)
                        {
                            using (System.Drawing.Icon sysicon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default.ico")))
                            {
                                fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                          sysicon.Handle,
                                          System.Windows.Int32Rect.Empty,
                                          System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                            }
                        }
                    }
                    sourceFilesView.Add(new FileListView(item.Id, item.Name, fileIcon, item.IsChecked));
                }
            }
        }
        private void SourceFilesListLoad(string sourceFolder)
        {
            if (String.IsNullOrWhiteSpace(sourceFolder))
            {
                return;
            }
            try
            {
                int i = 0;
                foreach (var item in Directory.GetDirectories(sourceFolder))
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    if (System.IO.Directory.Exists(item))
                    {
                        sourceFilesList.Add(new FileCheck(i, di.FullName, di.Name, false, true, di.Extension));
                        i++;
                    }
                }
                foreach (var item in Directory.GetFiles(sourceFolder))
                {
                    if (System.IO.File.Exists(item))
                    {
                        FileInfo fi = new FileInfo(item);
                        sourceFilesList.Add(new FileCheck(i, fi.FullName, fi.Name, false, false, fi.Extension));
                        i++;
                    }
                }
            }
            catch (Exception)
            {
                view.ShowMessage("Something went wrong. Please try again.");
            }
        }
        private void View_Unzip(IUnziperView sender)
        {
            view.Status = "Unzipping...";
            View_TargetFolderSelected(view.TargetFolder);
            model.Unzip();
            view.ProgressBarMax = model.ToUnzipListSize;
            view.ProgressBarCurrent = 0;
            view.IsProgressBarEnabled = true;
        }
    }
}
