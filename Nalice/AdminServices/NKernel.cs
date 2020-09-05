using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nalice.AdminServices
{
    class NKernel
    {
        /// <summary>
        /// 監視フォルダのディレクトリ
        /// </summary>
        public static string observeDirectory { get; set; }

        /// <summary>
        /// 音声データ一覧
        /// </summary>
        public static List<string> fileName { get; set; } = new List<string>();

    }
}
