using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;


namespace Unziper
{
    public class FileCheck
    {
        public int Id { set; get; }
        public string FullName { set; get; }
        public string Name { set; get; }
        public bool IsChecked { set; get; }
        private System.Windows.Media.ImageSource fileIcon;

        public System.Windows.Media.ImageSource FileIcon
        {
            get
            {
                return fileIcon;
            }
        }


        public FileCheck (int id, string fullName, string name, bool isChecked)
        {
            this.Id = id;
            this.FullName = fullName;
            this.Name = name;
            this.IsChecked = isChecked;
            if (System.IO.File.Exists(FullName))
            {
                using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(FullName))
                {
                    fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              sysicon.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
            }

            if (fileIcon == null && System.IO.Directory.Exists(FullName))
            {
                using (System.Drawing.Icon sysicon = new Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FolderClosed.ico")))
                {
                    fileIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              sysicon.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
            }

        }
    }
}
