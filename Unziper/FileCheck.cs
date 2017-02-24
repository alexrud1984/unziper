using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;


namespace Unziper
{
    public class FileCheck:FileList
    {
        public string FullName { set; get; }

        public FileCheck (int id, string fullName, string name, bool isChecked, bool isDir)
        {
            this.Id = id;
            this.FullName = fullName;
            this.Name = name;
            this.IsChecked = isChecked;
            this.IsDirectory = isDir;
        }
    }
}
