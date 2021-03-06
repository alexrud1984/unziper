﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Unziper
{
    public delegate void ActionDataEventHandler(string sender);
    public delegate void UnzipFinishedEventHandler(string sender);
    public delegate void CopyingFinisedEventHandler();
    public delegate void FileCopiedEventHandler(string fileName);
    public delegate void FileUnzippedEventHandler(string fileName);
    public delegate void UpdateProgressEventHandler(double buffer);
    public delegate void OperationCanceledEventHandler();

    public interface IUnzipModel
    {
        CancellationTokenSource UnzipCancelTokenSrc { set; get; }
        CancellationTokenSource CopyCancelTokenSrc { set; get; }
        string TargetFolder { set; get; }
        string UnzippedFile { set; get; }
        bool Autodelete { set; get;}
        double ToCopyListSize {get; }
        double CopiedListSize { get; }
        double ToUnzipListSize { get; }
        double UnzippedListSize { get; }

        event ActionDataEventHandler ActionData;
        event UnzipFinishedEventHandler UnzipFinished;
        event CopyingFinisedEventHandler CopyingFinised;
        event FileCopiedEventHandler FileCopied;
        event FileUnzippedEventHandler FileUnzipped;
        event UpdateProgressEventHandler UpdateProgress;
        event OperationCanceledEventHandler OperationCanceled;

        void Unzip();
        void Copy(List<FileCheck> sourceFiles);
        void Cancel();
    }
}
