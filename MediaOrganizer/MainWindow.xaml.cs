using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using MediaOrganizer.Core;

namespace MediaOrganizer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<DuplicateRow> DuplicateRows { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DuplicatesGrid.ItemsSource = DuplicateRows;
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using var dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourceTextBox.Text = dlg.SelectedPath;
            }
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            using var dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TargetTextBox.Text = dlg.SelectedPath;
            }
        }

        private async void OrganizeButton_Click(object sender, RoutedEventArgs e)
        {
            string source = SourceTextBox.Text;
            string target = TargetTextBox.Text;

            if (string.IsNullOrWhiteSpace(source) || !Directory.Exists(source))
            {
                MessageBox.Show("Please choose a valid source folder.");
                return;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                MessageBox.Show("Please choose a target folder.");
                return;
            }

            bool copy = CopyInsteadOfMoveCheckBox.IsChecked == true;

            try
            {
                IsEnabled = false;
                var organizer = new Organizer(source, target, copy);
                await System.Threading.Tasks.Task.Run(() => organizer.Organize());
                MessageBox.Show("Organizing completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private async void ScanDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            string root = TargetTextBox.Text;
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                MessageBox.Show("Please choose a valid target folder to scan.");
                return;
            }

            try
            {
                IsEnabled = false;
                DuplicateRows.Clear();

                var finder = new DuplicateFinder(root);
                var groups = await System.Threading.Tasks.Task
                    .Run(() => finder.FindDuplicates());

                int groupId = 1;
                foreach (var group in groups)
                {
                    bool first = true;
                    foreach (string path in group.Paths)
                    {
                        long sizeBytes = new FileInfo(path).Length;
                        DuplicateRows.Add(new DuplicateRow
                        {
                            GroupId = groupId,
                            Path = path,
                            Hash = group.Hash,
                            SizeKB = sizeBytes / 1024,
                            SelectedForDelete = !first
                        });
                        first = false;
                    }
                    groupId++;
                }

                MessageBox.Show("Duplicate scan completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void MarkAllExceptFirst_Click(object sender, RoutedEventArgs e)
        {
            var groups = DuplicateRows.GroupBy(r => r.GroupId);
            foreach (var group in groups)
            {
                bool first = true;
                foreach (var row in group)
                {
                    row.SelectedForDelete = !first;
                    first = false;
                }
            }
            DuplicatesGrid.Items.Refresh();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = DuplicateRows.Where(r => r.SelectedForDelete).ToList();
            if (!toDelete.Any())
            {
                MessageBox.Show("No duplicates selected for deletion.");
                return;
            }

            if (MessageBox.Show(
                    $"Delete {toDelete.Count} files? This cannot be undone.",
                    "Confirm delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var row in toDelete)
            {
                try
                {
                    if (File.Exists(row.Path))
                    {
                        File.Delete(row.Path);
                    }
                    DuplicateRows.Remove(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete: " + row.Path + "\n" + ex.Message);
                }
            }
        }
    }
}
