using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FolderPickerLib;

namespace Unziper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUnziperView
    {
        FolderPickerDialog fpd = new FolderPickerDialog();

        public MainWindow()
        {
            InitializeComponent();
            UnziperPresenter presenter = new UnziperPresenter(this);
        }

        public string TargetFolder
        {
            set
            {
                textBox.Text = value;
            }
            get
            {
                return textBox.Text;
            }
        }
        public string UnzippedFile
        {
            get
            {
                return UnzippedFile;
            }

            set
            {
                AddFileToList(value);
            }
        }

        public event FolderSelectedEventHandler FolderSelected;
        public event UnzipEventHandler Unzipped;

        private void OnFolderSelected()
        {
            if (FolderSelected != null)
            {
                FolderSelected(this);
            }
        }

        private void OnUnzipped()
        {
            if (Unzipped != null)
            {
                Unzipped(this);
            }
        }

        private void AddFileToList(string filePath)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Content = filePath;
            filesListView.Items.Add(lvi);
        }

        private void unzipButton_Click(object sender, RoutedEventArgs e)
        {
            OnUnzipped();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            fpd.ShowDialog();
            if ((bool)fpd.DialogResult)
            {
                TargetFolder = fpd.SelectedPath;
            }
            OnFolderSelected();
        }

    }
}
