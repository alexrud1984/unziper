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
using System.IO;

namespace Unziper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window, IUnziperView
    {
        private int index = 0;
        private List<FileCheck> sourceList;
        FolderPickerDialog fpd = new FolderPickerDialog();
        public List<FileCheck> SourceList
        {
            set
            {
                sourceList = value;
                sourceListView.Items.Clear();
                sourceListView.ItemsSource=sourceList;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            UnziperPresenter presenter = new UnziperPresenter(this);
        }

        public string SourceFolder
        {
            set
            {
                sourceTextBox.Text = value;
            }
            get
            {
                return sourceTextBox.Text;
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

        public event FolderSelectedEventHandler SourceFolderSelected;
        public event UnzipEventHandler Unzipped;

        private void OnFolderSelected()
        {
            if (SourceFolderSelected != null)
            {
                SourceFolderSelected(this);
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
            /*            fpd.ShowDialog();
                        if ((bool)fpd.DialogResult)
                        {
                            TargetFolder = fpd.SelectedPath;
                        }*/
            if (Directory.Exists(SourceFolder))
            {
                OnFolderSelected();
            }
            else
            {
                MessageBox.Show("Folder not exists");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            sourceList[(int)chk.Tag].IsChecked = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            sourceList[(int)chk.Tag].IsChecked = false;
        }

        private void sourceListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Space)
            {
                index = sourceListView.SelectedIndex;
                if (index >=0 && index<sourceList.Count)
                {
                    Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.Auto;
                    sourceList[index].IsChecked ^= true;
                    sourceListView.Items.Refresh();
                    sourceListView.SelectedIndex = index;
                }
            }
        }

    }
}
