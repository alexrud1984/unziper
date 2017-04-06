﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public delegate void SourceFolderSelectedEventHandler(IUnziperView sender);
    public delegate void TargetFolderSelectedEventHandler(string targetFolder);
    public delegate void UnzipClickEventHandler(IUnziperView sender);
    public delegate void CopyClickEventHandler();
    public delegate void ItemCheckEventHandler(int id, bool isChecked);
    public delegate void CancelClickEventHandler();

    public interface IUnziperView
    {
        string SourceFolder { set; get; }
        string TargetFolder { set; get; }
        string UnzippedFile { set; get; }
        string Status { set; get; }
        bool AutoUnzip { set; get; }
        List<FileListView> SourceList { set; }

        event SourceFolderSelectedEventHandler SourceFolderSelected;
        event TargetFolderSelectedEventHandler TargetFolderSelected;
        event UnzipClickEventHandler UnzippedClick;
        event CopyClickEventHandler CopyClick;
        event ItemCheckEventHandler ItemCheckChanged;
        event CancelClickEventHandler CancelClick;

        void ShowMessage(string msg);
    }
}
