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
using System.Windows.Threading;
using System.ComponentModel;

namespace Unziper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUnziperView
    {
        private List<FileListView> sourceList;
        public List<FileListView> SourceList
        {
            set
            {
                sourceList = value;
                FillSourceView();
            }

            get
            {
                return sourceList;
            }
        }

        private void FillSourceView()
        {
            sourceListView.Items.Clear();
            foreach (var item in sourceList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = item;
                sourceListView.Items.Add(lvi);
            }
            if (sourceListView.Items.Count != 0)
            {
                sourceListView.Focus();
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

        public string TargetFolder
        {
            get
            {
                return targetTextBox.Text;
            }

            set
            {
                targetTextBox.Text = value;
            }
        }

        public string Status
        {
            get
            {
                return actionColumn.Header.ToString();
            }

            set
            {
                actionColumn.Header=value;
            }
        }

        public bool AutoUnzip 
        {
            get
            {
                return (bool)autoUnzipCheckBox.IsChecked;
            }

            set
            {
                autoUnzipCheckBox.IsChecked=value;
            }
        }

        public event SourceFolderSelectedEventHandler SourceFolderSelected;
        public event UnzipClickEventHandler UnzippedClick;
        public event TargetFolderSelectedEventHandler TargetFolderSelected;
        public event CopyClickEventHandler CopyClick;
        public event ItemCheckEventHandler ItemCheckChanged;
        public event CancelClickEventHandler CancelClick;

        private void OnCancelClick()
        {
            if (CancelClick != null)
            {
                CancelClick();
            }
        }
        private void OnCopyClick()
        {
            if (CopyClick != null)
            {
                CopyClick();
            }
        }

        private void OnItemCheckChange(int id, bool isChecked)
        {
            if (ItemCheckChanged != null)
            {
                ItemCheckChanged(id, isChecked);
            }
        }

        private void OnSourceSelected()
        {
            if (SourceFolderSelected != null)
            {
                SourceFolderSelected(this);
            }
        }

        private void OnUnzipped()
        {
            if (UnzippedClick != null)
            {
                UnzippedClick(this);
            }
        }

        private void OnTargetSelected()
        {
            if (TargetFolderSelected != null)
            {
                TargetFolderSelected(TargetFolder);
            }
        }

        private void AddFileToList(string filePath)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Content = filePath;
            lvi.Height = 25;
            filesListView.Items.Add(lvi);
            filesListView.SelectedIndex = filesListView.Items.Count - 1;
            actionColumn.Width = filesListView.ActualWidth;
            filesListView.ScrollIntoView(filesListView.SelectedItem);
        }

        private void unzipButton_Click(object sender, RoutedEventArgs e)
        {
            OnUnzipped();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            sourceList[(int)chk.Tag].IsChecked = true;
            OnItemCheckChange((int)chk.Tag, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            sourceList[(int)chk.Tag].IsChecked = false;
            OnItemCheckChange((int)chk.Tag, false);
        }

        private void sourceListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Space)
            {
                ChangeCheckedState();
            }
        }

        private void sourceListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            if(lv != null)
            {
                ChangeCheckedState();
            }

        }

        private void ChangeCheckedState()
        {
            int index = 0;
            index = sourceListView.SelectedIndex;
            if (index >= 0 && index < sourceList.Count)
            {
                sourceList[index].IsChecked ^= true;
                ListViewItem lvi = new ListViewItem();
                lvi.Content = sourceList[index];
                lvi.IsSelected = true;
                sourceListView.Items[index] = lvi;
                ListViewItem item = sourceListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
                item.Focus();
                OnItemCheckChange(sourceList[index].Id, sourceList[index].IsChecked);
            }
        }

        private void Header_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sourceList != null)
            {
                foreach (var item in sourceList)
                {
                    item.IsChecked = true;
                    OnItemCheckChange(item.Id, item.IsChecked);
                }
                sourceListView.Items.Clear();
                FillSourceView();
            }

        }

        private void Header_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sourceList != null)
            {
                foreach (var item in sourceList)
                {
                    item.IsChecked = false;
                    OnItemCheckChange(item.Id, item.IsChecked);
                }
                sourceListView.Items.Clear();
                FillSourceView();
            }
        }

        private void targetButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPickerDialog fpd = new FolderPickerDialog();
            fpd.ShowDialog();
            if ((bool)fpd.DialogResult)
            {
                TargetFolder = fpd.SelectedPath;
            }
            OnTargetSelected();
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            OnCopyClick();
        }

        private void getListButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(SourceFolder))
            {
                OnSourceSelected();
            }
            else
            {
                MessageBox.Show("Folder not exists");
            }
        }

        private void sourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancelClick();
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender,
                                                RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked =
                  e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header  
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(sourceListView.Items);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

    }
}
