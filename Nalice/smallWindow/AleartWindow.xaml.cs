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
using System.Windows.Shapes;

namespace Nalice.smallWindow
{
    /// <summary>
    /// AleartWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AleartWindow : Window
    {
        /// <summary>
        /// ウィンドウの名前
        /// </summary>
        public string windowName { get; set; }

        /// <summary>
        /// 警告内容文
        /// </summary>
        public string aleartDetail { get; set; }

        public AleartWindow()
        {
            InitializeComponent();
            ContentRendered += (s, e) => {
                Console.WriteLine(aleartDetail);
                aleart_label.Content = aleartDetail;
                this.Title = windowName;
            };
        }

        private void submit_button_Click(object sender, RoutedEventArgs e)  //OKボタン
        {
            this.Close();
        }
    }
}
