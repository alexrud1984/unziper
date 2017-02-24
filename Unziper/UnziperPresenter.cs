using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            model.ActionData += Model_FileUnzipped;
        }

        private void Model_FileUnzipped(string sender)
        {
            view.UnzippedFile = sender;
        }

        private void AttachUnziperView(IUnziperView view)
        {
            view.SourceFolderSelected += View_SourceFolderSelected;
            view.TargetFolderSelected += View_TargetFolderSelected;
            view.UnzippedClick += View_Unzip;
            view.CopyClick += View_CopyClick;
            view.ItemCheckChanged += View_ItemCheckChanged;
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
        }

        private void View_CopyClick()
        {
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
        }

        private void View_TargetFolderSelected(string targetFolder)
        {
            DirectoryInfo di = new DirectoryInfo(targetFolder);
            if (di.Exists)
            {
                model.TargetFolder = targetFolder;
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
                    {
                        using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(item.FullName))
                        {
                            fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                      sysicon.Handle,
                                      System.Windows.Int32Rect.Empty,
                                      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
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
                        sourceFilesList.Add(new FileCheck(i, di.FullName, di.Name, false, true));
                        i++;
                    }
                }
                foreach (var item in Directory.GetFiles(sourceFolder))
                {
                    if (System.IO.File.Exists(item))
                    {
                        FileInfo fi = new FileInfo(item);
                        sourceFilesList.Add(new FileCheck(i, fi.FullName, fi.Name, false, false));
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
            model.Unzip();
        }
    }
}
