using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nalice.smallWindow
{
    /// <summary>
    /// FileMove.xaml の相互作用ロジック
    /// </summary>
    public partial class FileMove : Window
    {
        /// <summary>
        /// 移動後のインデックス
        /// </summary>
        public int moveNum { get; set; }

        /// <summary>
        /// ウィンドウが正しく閉じられたか
        /// </summary>
        public bool trueClose { get; set; } = false;

        /// <summary>
        /// 最大の範囲(0..)
        /// </summary>
        public int maxIndex { get; set; }

        /// <summary>
        /// 指定されたインデックス
        /// </summary>
        public int selectedIndex { get; set; }

        public FileMove()
        {
            InitializeComponent();
        }

        private void submit_button_Click(object sender, RoutedEventArgs e)  //送信ボタン
        {
            try
            {
                moveNum = int.Parse(moveToIndex_textbox.Text);

                if (moveNum > maxIndex + 1 || moveNum < 1 || moveNum == selectedIndex + 1)
                {
                    AleartWindow aleartWindow = new AleartWindow();
                    aleartWindow.windowName = "Aleart";
                    aleartWindow.aleartDetail = "Please set the valid number.";

                    aleartWindow.ShowDialog();

                    return;
                }

                trueClose = true;
                this.Close();
            }
            catch
            {
                AleartWindow aleartWindow = new AleartWindow();
                aleartWindow.windowName = "Aleart";
                aleartWindow.aleartDetail = "Please set the valid number.";

                aleartWindow.ShowDialog();
            }
        }

        private void text_input(object sender, TextCompositionEventArgs e)  //数字しか入力できないようにする
        {
            e.Handled = !new Regex("[0-9]+").IsMatch(e.Text);
        }
    }
}
