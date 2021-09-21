﻿using MemorieDeFleurs.Models.Converters;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System.Data.Common;

namespace MemorieDeFleurs.Databese.SQLite
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
                builder.EnableSensitiveDataLogging(true); // for Debug
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new DateTimeConverter();

            modelBuilder.Entity<StockAction>(ent =>
                {
                    ent.HasKey(nameof(StockAction.ActionDate), nameof(StockAction.Action), nameof(StockAction.PartsCode),
                        nameof(StockAction.ArrivalDate), nameof(StockAction.StockLotNo));
                    ent.Property(act => act.ActionDate).HasConversion(converter);
                    ent.Property(act => act.ArrivalDate).HasConversion(converter);
                });


            modelBuilder
                .Entity<PartSupplier>()
                .HasKey("SupplierCode", "PartCode");

            modelBuilder.Entity<BouquetPartsList>(ent =>
                {
                    ent.HasKey(nameof(BouquetPartsList.BouquetCode), nameof(BouquetPartsList.PartsCode));
                    ent.HasOne(p => p.Bouquet).WithMany(b => b.PartsList).HasForeignKey(p => p.BouquetCode);
                });

            modelBuilder
                .Entity<ShippingAddress>()
                .Property(a => a.LatestOrderDate)
                .HasConversion(converter);

            modelBuilder
                .Entity<OrderFromCustomer>()
                .Property(o => o.OrderDate)
                .HasConversion(converter);

        }

        /// <summary>
        /// 得意先
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// 在庫アクション
        /// </summary>
        public DbSet<StockAction> StockActions { get; set; }

        /// <summary>
        /// 仕入先
        /// </summary>
        public DbSet<Supplier> Suppliers { get; set; }

        /// <summary>
        /// 単品
        /// </summary>
        public DbSet<BouquetPart> BouquetParts { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public DbSet<Bouquet> Bouquets { get; set; }

        /// <summary>
        /// 商品構成
        /// </summary>
        public DbSet<BouquetPartsList> PartsList { get; set; }

        /// <summary>
        /// お届け先
        /// </summary>
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }

        /// <summary>
        /// 連番管理
        /// </summary>
        public DbSet<SequenceValue> Sequences { get; set; }

        /// <summary>
        /// 得意先からの受注履歴
        /// </summary>
        public DbSet<OrderFromCustomer> OrderFromCustomers { get; set; }
    }
}
