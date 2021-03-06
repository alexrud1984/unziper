﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    public class FileListView : FileList,IComparable<FileListView>
    {
        protected System.Windows.Media.ImageSource fileIcon;
        public System.Windows.Media.ImageSource FileIcon
        {
            get
            {
                return fileIcon;
            }
        }

        public int CompareTo(FileListView other)
        {
            return (this.Name.CompareTo(other.Name));
        }

        public FileListView(int id, string name, System.Windows.Media.ImageSource icon,  bool isChecked)
        {
            this.Id = id;
            this.Name = name;
            this.IsChecked = isChecked;
            this.fileIcon = icon;
        }
    }
}
