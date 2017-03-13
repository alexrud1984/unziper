using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unziper
{
    abstract public class FileList
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public bool IsChecked { set; get; }
        public bool IsDirectory { set; get; }
        public string Ext { set; get; }
    }
}
