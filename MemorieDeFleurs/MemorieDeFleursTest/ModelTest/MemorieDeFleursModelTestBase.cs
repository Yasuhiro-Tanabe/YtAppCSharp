using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;

using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MemorieDeFleursTest.ModelTest
{
    public class MemorieDeFleursModelTestBase : MemorieDeFleursTestBase
    {
        protected MemorieDeFleursModel Model { get; private set; }

        public MemorieDeFleursModelTestBase() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);
        }

        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }

        /// <summary>
        /// 現在の在庫アクション一覧をログ出力する。出力対象は引数指定可能。
        /// </summary>
        /// <param name="connection">対象DB</param>
        /// <param name="partsCode">出力対象単品</param>
        /// <param name="lots">(任意)出力対象ロットを絞りたいとき、対象ロットを指定する</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        protected void DEBUGLOG_ShowInventoryActions(DbConnection connection, string partsCode, int[] lots = null, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.DEBUGLOG_BeginMethod(partsCode, $"in {caller},{Path.GetFileName(path)}:{line}");
            LogUtil.Indent++;

            using(var context = new MemorieDeFleursDbContext(connection))
            {
                if(lots == null)
                {
                    foreach (var action in context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .OrderBy(act => act.ArrivalDate)
                        .ThenBy(act => act.InventoryLotNo)
                        .ThenBy(act => act.Action))
                    {
                        LogUtil.DebugFormat("{0}{1}", LogUtil.Indent, action.ToString("DB"));
                    }
                }
                else
                {
                    foreach (var action in context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .OrderBy(act => act.ArrivalDate)
                        .ThenBy(act => act.InventoryLotNo)
                        .ThenBy(act => act.Action))
                    {
                        if (lots.Contains(action.InventoryLotNo))
                        {
                            LogUtil.DebugFormat("{0}{1}", LogUtil.Indent, action.ToString("DB"));
                        }
                    }
                }
            }

            LogUtil.Indent--;
            LogUtil.DEBUGLOG_EndMethod(partsCode);
        }
    }
}
