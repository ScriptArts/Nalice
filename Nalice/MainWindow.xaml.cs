using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using Nalice.AdminServices;
using Nalice.smallWindow;

namespace Nalice
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Start待機中か?
        /// </summary>
        public bool isToggleStart = true;

        /// <summary>
        /// 
        /// </summary>
        public static FileSystemWatcher Watcher = new FileSystemWatcher();

        public MainWindow()
        {
            InitializeComponent();

            NKernel.observeDirectory = directory_textbox.Text;
        }

        /// <summary>
        /// ファイル一覧リストとNKernelを初期化します
        /// </summary>
        public void List_init()
        {
            files_listview.Items.Clear();

            NKernel.observeDirectory = directory_textbox.Text;
            string[] files = Directory.GetFiles(NKernel.observeDirectory, "*.wav");
            //NKernel.fileName = files;
            foreach (var i in files)
            {
                string fileName = i.Replace(NKernel.observeDirectory + @"\", "");
                fileName = fileName.Replace(".wav", "");
                NKernel.fileName.Add(fileName);
                files_listview.Items.Add(fileName);
            }
        }

        /// <summary>
        /// 監視スタート
        /// </summary>
        public void StartWatching()
        {
            
            Watcher.Path = NKernel.observeDirectory;
            Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            Watcher.Filter = "*.txt"; //テキストファイルが作成されるのが後だからこっちを監視
            Watcher.IncludeSubdirectories = false;

            // EVENTS
            Watcher.Created += FileUpdata; // NEW CREATION
            Watcher.Changed += FileUpdata; // CHANGE
            Watcher.Deleted += FileUpdata; // DELETE
            Watcher.Renamed += FileUpdata; // RENAME

            // 監視開始
            Watcher.EnableRaisingEvents = true;
            Console.WriteLine("START WATCHING");
        }

        /// <summary>
        /// ファイル変更トリガー処理
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void FileUpdata(object source, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    string _fileName = e.FullPath.Replace(NKernel.observeDirectory + @"\", "");
                    if (_fileName == "nalice.txt")
                    {
                        string newFileName = (NKernel.fileName.Count + 1).ToString("000") + "_" + ReadTextFile(e.FullPath);
                        string newWavFileDirectory = NKernel.observeDirectory + @"\" + newFileName + ".wav";
                        string newTxtFileDirectory = NKernel.observeDirectory + @"\" + newFileName + ".txt";
                        bool j = true;
                        while (j)
                        {
                            try
                            {
                                File.Move(e.FullPath.Replace(".txt", ".wav"), newWavFileDirectory);
                                File.Move(e.FullPath, newTxtFileDirectory);
                                j = false;
                            }
                            catch
                            {

                            }
                        }
                        NKernel.fileName.Add(newFileName);
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            files_listview.Items.Add(newFileName);
                        }));
                    }
                    Console.WriteLine($"NEW CREATION: {e.FullPath}");
                    break;
                case WatcherChangeTypes.Deleted:
                    Console.WriteLine($"DELETE: {e.FullPath}");
                    break;
                case WatcherChangeTypes.Changed:
                    Console.WriteLine($"CHANGE: {e.FullPath}");
                    break;
                case WatcherChangeTypes.Renamed:
                    var renameEventArgs = e as RenamedEventArgs;
                    Console.WriteLine($"RENAME: {renameEventArgs.OldFullPath} => {renameEventArgs.FullPath}");
                    break;
            }
        }

        /// <summary>
        /// 監視停止
        /// </summary>
        public static void StopWatching()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Dispose();
            }
        }

        /// <summary>
        /// テキストファイルの中身を返します
        /// </summary>
        /// <param name="dir">フルディレクトリ</param>
        /// <returns>テキスト本文</returns>
        public static string ReadTextFile(string dir)
        {
            bool j = true;
            string text = "";
            while (j)
            {
                try
                {
                    StreamReader sr2 = new StreamReader(dir, Encoding.GetEncoding("Shift_JIS"));
                    text = sr2.ReadToEnd();
                    sr2.Close();
                    j = false;
                }
                catch
                {

                }
            }
            return text;
        }

        /// <summary>
        /// ファイル名を変更します
        /// </summary>
        /// <param name="index">NKernel.fileName のインデックス</param>
        /// <param name="extension">拡張子</param>
        /// <param name="isUp">上に移動か</param>
        public void RenameFile(int index, string extension, bool isUp)
        {
            string _upData = NKernel.fileName[index];
            string _upDir = NKernel.observeDirectory + @"\" + _upData + "." + extension;
            string _upedDir;

            if (isUp)
            {
                _upedDir = NKernel.observeDirectory + @"\" + index.ToString("000") + _upData.Remove(0, 3) + "." + extension;
            }
            else
            {
                _upedDir = NKernel.observeDirectory + @"\" + (index + 2).ToString("000") + _upData.Remove(0, 3) + "." + extension;
            }

            try
            {
                File.Move(_upDir, _upedDir);
            }
            catch
            {
                AleartWindow aleartWindow = new AleartWindow
                {
                    windowName = "Error",
                    aleartDetail = "The required files could not be found."
                };

                aleartWindow.ShowDialog();
                this.Close(); //重大なエラー
            }
        }

        private void FolderSearch_button_Click(object sender, RoutedEventArgs e) //フォルダ選択
        {
            var dialog = new CommonOpenFileDialog("Select Folder")
            {
                IsFolderPicker = true
            };
            var ret = dialog.ShowDialog();
            if (ret == CommonFileDialogResult.Ok)
            {
                directory_textbox.Text = dialog.FileName;
            }
        }

        private void Toggle_button_Click(object sender, RoutedEventArgs e) //スタート・ストップボタン
        {
            if (isToggleStart) //Startボタン押されたら
            {
                try
                {
                    Console.WriteLine("START");
                    List_init();
                    StartWatching();
                    Toggle_button.Content = "Stop";
                    BrushConverter bc = new BrushConverter();
                    Toggle_button.Background =  (Brush)bc.ConvertFromString("#FFF59CF5");
                    isToggleStart = false;

                    sortUp_button.IsEnabled = true;
                    sortDown_button.IsEnabled = true;
                    sortSelect_button.IsEnabled = true;
                    delete_button.IsEnabled = true;

                    FolderSearch_button.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("STOP");
                StopWatching();
                Toggle_button.Content = "Start";
                BrushConverter bc = new BrushConverter();
                Toggle_button.Background = (Brush)bc.ConvertFromString("#FF9DA4A4");
                isToggleStart = true;

                sortUp_button.IsEnabled = false;
                sortDown_button.IsEnabled = false;
                sortSelect_button.IsEnabled = false;
                delete_button.IsEnabled = false;

                FolderSearch_button.IsEnabled = true;
            }
        }

        private void sortUp_button_Click(object sender, RoutedEventArgs e)  //上ボタン
        {
            int selectedIndex = files_listview.SelectedIndex;
            if (selectedIndex < 0)
            {
                AleartWindow aleartWindow = new AleartWindow
                {
                    windowName = "Aleart",
                    aleartDetail = "Please select arbitrary file."
                };

                aleartWindow.ShowDialog();
                return;
            }

            // ファイル名変更
            RenameFile(selectedIndex, "wav", true);
            RenameFile(selectedIndex, "txt", true);

            RenameFile(selectedIndex - 1, "wav", false);
            RenameFile(selectedIndex - 1, "txt", false);

            // DB入れ替え処理
            NKernel.fileName.Clear();
            List_init();
        }

        private void sortDown_button_Click(object sender, RoutedEventArgs e)  //下ボタン
        {
            int selectedIndex = files_listview.SelectedIndex;
            if (selectedIndex < 0)
            {
                AleartWindow aleartWindow = new AleartWindow
                {
                    windowName = "Aleart",
                    aleartDetail = "Please select arbitrary file."
                };

                aleartWindow.ShowDialog();
                return;
            }

            if (selectedIndex < files_listview.Items.Count - 1) //一番下を選択中は無効
            {
                // ファイル名変更
                RenameFile(selectedIndex + 1, "wav", true);
                RenameFile(selectedIndex + 1, "txt", true);

                RenameFile(selectedIndex, "wav", false);
                RenameFile(selectedIndex, "txt", false);

                // DB入れ替え処理
                NKernel.fileName.Clear();
                List_init();
            }
        }

        private void sortSelect_button_Click(object sender, RoutedEventArgs e)  //ファイル移動ボタン
        {
            int selectedIndex = files_listview.SelectedIndex;
            if (selectedIndex < 0)
            {
                AleartWindow aleartWindow = new AleartWindow
                {
                    windowName = "Aleart",
                    aleartDetail = "Please select arbitrary file."
                };

                aleartWindow.ShowDialog();
                return;
            }

            FileMove fileMove = new FileMove();
            fileMove.maxIndex = NKernel.fileName.Count - 1;
            fileMove.selectedIndex = selectedIndex;
            fileMove.ShowDialog();

            if (fileMove.trueClose) //ウィンドウが正しく閉じられた場合
            {
                int toMoveIndex = fileMove.moveNum;

                if (toMoveIndex < selectedIndex)
                {
                    for (int i = selectedIndex; i > toMoveIndex - 1; i--)
                    {
                        RenameFile(i - 1, "wav", false);
                        RenameFile(i - 1, "txt", false);
                    }

                    File.Move(NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".wav", NKernel.observeDirectory + @"\" + toMoveIndex.ToString("000") + NKernel.fileName[selectedIndex].Remove(0, 3) + ".wav");
                    File.Move(NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".txt", NKernel.observeDirectory + @"\" + toMoveIndex.ToString("000") + NKernel.fileName[selectedIndex].Remove(0, 3) + ".txt");
                }
                else if (toMoveIndex > selectedIndex)
                {
                    for (int i = selectedIndex; i < toMoveIndex - 1; i++)
                    {
                        RenameFile(i + 1, "wav", true);
                        RenameFile(i + 1, "txt", true);
                    }

                    File.Move(NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".wav", NKernel.observeDirectory + @"\" + toMoveIndex.ToString("000") + NKernel.fileName[selectedIndex].Remove(0, 3) + ".wav");
                    File.Move(NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".txt", NKernel.observeDirectory + @"\" + toMoveIndex.ToString("000") + NKernel.fileName[selectedIndex].Remove(0, 3) + ".txt");
                }

                NKernel.fileName.Clear();
                List_init();
            }
        }

        private void delete_button_Click(object sender, RoutedEventArgs e)  //削除ボタン
        {
            int selectedIndex = files_listview.SelectedIndex;

            if (selectedIndex < 0)
            {
                AleartWindow aleartWindow = new AleartWindow
                {
                    windowName = "Aleart",
                    aleartDetail = "Please select arbitrary file."
                };

                aleartWindow.ShowDialog();
                return;
            }

            string deleteWavDir = NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".wav";
            string deleteTxtDir = NKernel.observeDirectory + @"\" + NKernel.fileName[selectedIndex] + ".txt";
            File.Delete(deleteWavDir);
            File.Delete(deleteTxtDir);

            for (int i = selectedIndex + 1; i < NKernel.fileName.Count; i++)
            {
                RenameFile(i, "wav", true);
                RenameFile(i, "txt", true);
            }

            NKernel.fileName.Clear();
            List_init();
        }
    }
}
