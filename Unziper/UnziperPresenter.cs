using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    class UnziperPresenter
    {
        private IUnziperView view;
        private IUnzipModel model;
        public UnziperPresenter(IUnziperView view)
        {
            this.AttachUziperView(view);
            this.view = view;
            model = new UnzipModel();
            AttachUziperModelI(model);
        }

        private void AttachUziperModelI(IUnzipModel model)
        {
            model.FileUnzipped += Model_FileUnzipped;
        }

        private void Model_FileUnzipped(string sender)
        {
            view.UnzippedFile = sender;
        }

        private void AttachUziperView(IUnziperView view)
        {
            view.FolderSelected += View_FolderSelected;
            view.Unzipped += View_Unzip;
        }

        private void View_FolderSelected(IUnziperView sender)
        {
            model.TargetFolder = sender.TargetFolder;
        }

        private void View_Unzip(IUnziperView sender)
        {
            model.Unzip();
        }
    }
}
