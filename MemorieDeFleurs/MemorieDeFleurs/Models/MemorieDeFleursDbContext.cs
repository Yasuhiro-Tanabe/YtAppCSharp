using MemorieDeFleurs.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 花束問題の Entity Framework Core 用データベースコンテキスト
    /// </summary>
    /// <remarks>
    /// (過去に別件でEF6を試行したときの作業メモ：今回はCode-FirstではなくDB-Firstなので方向が違うかもしれない。)
    /// Code-First で DB アクセスするアプリケーションを作るときは、
    /// エンティティオブジェクトやDBコンテキスト（DbContextを継承したクラス）を定義し、
    /// Nuget パッケージマネージャーコンソールからDBマイグレーションを実行する。
    /// 具体的には、
    /// Visual Studio のメニューから [ツール]
    /// →[NuGetパッケージマネージャー]
    /// →[パッケージマネージャーコンソール] を選択、以下のコマンドを入力する
    /// ("Xxxx" はデータベースの名称として適当な文字列)
    /// <code>
    /// PM> add-migration CreateXxxxDB
    /// </code>
    /// 
    /// またDBコンテキストやエンティティオブジェクトの定義を変更したら、
    /// Nuget パッケージマネージャを起動してデータベース更新を行うこと。
    /// 具体的には、
    /// Visual Studio のメニューから [ツール]
    /// →[NuGetパッケージマネージャー]
    /// →[パッケージマネージャーコンソール] を選択、以下のコマンドを入力する
    /// ("マイグレーション名" 部分は適当な名前で良いが、 
    ///  CreateXxxxDB で作成したなら UpdateXxxxDB が良いと思う。)
    /// <code>
    /// PM> add-migration マイグレーション名
    /// PM> update-database -verbose
    /// </code>
    /// 生成されるマイグレーションクラス名は適宜日付が入るので、同じ名前でもかまわない（かもしれない）。
    /// </remarks>
    public class MemorieDeFleursDbContext : DbContext
    {
        private DbConnection Connection { get; set; }

        public MemorieDeFleursDbContext(DbConnection conn)
        {
            Connection = conn;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if(Connection is SqliteConnection)
            {
                builder.UseSqlite(Connection);
            }
        }

        /// <summary>
        /// 得意先
        /// </summary>
        public DbSet<Customer> Customers { get; set; }
    }
}
