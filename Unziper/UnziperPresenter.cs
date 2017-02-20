using System;
using System.Collections.Generic;
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
        private List<FileCheck> sourceListView = new List<FileCheck>();
        private List<FileCheck> targetListView = new List<FileCheck>();

        public UnziperPresenter(IUnziperView view)
        {
            this.AttachUnziperView(view);
            this.view = view;
            model = new UnzipModel();
            AttachUziperModel(model);
        }

        private void AttachUziperModel(IUnzipModel model)
        {
            model.FileUnzipped += Model_FileUnzipped;
            model.FileExists += Model_FileExists;
        }

        private void Model_FileExists(string sender)
        {
            view.UnzippedFile = "File already exists "+sender;
        }

        private void Model_FileUnzipped(string sender)
        {
            view.UnzippedFile = sender;
        }

        private void AttachUnziperView(IUnziperView view)
        {
            view.SourceFolderSelected += View_SourceFolderSelected;
            view.Unzipped += View_Unzip;
        }

        private void View_SourceFolderSelected(IUnziperView sender)
        {
            sourceListView.Clear();
            if (String.IsNullOrWhiteSpace(sender.SourceFolder))
            {
                return;
            }
            try
            {
                int i = 0;
                foreach (var item in Directory.GetDirectories(sender.SourceFolder))
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    sourceListView.Add(new FileCheck(i, di.FullName, di.Name, true));
                    i++;
                }
                foreach (var item in Directory.GetFiles(sender.SourceFolder))
                {
                    FileInfo fi = new FileInfo(item);
                    sourceListView.Add(new FileCheck(i, fi.FullName, fi.Name, true));
                    i++;
                }
            }
            catch (Exception)
            {
                //to do
            }
            sender.SourceList = sourceListView;
            //            model.TargetFolder = sender.SourceFolder;
        }

        private void View_Unzip(IUnziperView sender)
        {
            model.Unzip();
        }
    }
}
