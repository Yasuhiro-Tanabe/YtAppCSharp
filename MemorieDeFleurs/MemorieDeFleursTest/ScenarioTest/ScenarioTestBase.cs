using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;

using MemorieDeFleursTest.ModelTest;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MemorieDeFleursTest.ScenarioTest
{
    /// <summary>
    /// シナリオテスト用ベースクラス：
    /// 各テストケースを一続きのテストとして実行するため、テストケース毎のデータクリーンアップは実行しない。
    /// </summary>
    [TestClass]
    public class ScenarioTestBase
    {
        #region SQL
        private static string TableDefinitionFile = "./testdata/db/TableDefinitions.sql";
        private static IList<Tuple<string, string>> TableDefinitions = new List<Tuple<string, string>>();

        static ScenarioTestBase()
        {
            LoadTableDefinitionsFile();
        }
        #endregion // SQL

        #region ClassInitialize
        /// <summary>
        /// サブクラスの TestInitializeAttribute を付与したメソッドで呼び出す、
        /// 共通のデータベース初期化メソッド
        /// </summary>
        protected static SqliteConnection OpenDatabase()
        {
            LogUtil.DEBUGLOG_BeginMethod();

            var db = CreateInmeoryDBConnection();
            OpenDatabase(db);
            CreateTestBaseData(db);

            LogUtil.DEBUGLOG_EndMethod();
            return db;
        }

        private static SqliteConnection CreateInmeoryDBConnection()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.DataSource = ":memory:";
            builder.ForeignKeys = true;

            LogUtil.Debug($"CreateInmeoryDBConnection()=>DataSource={builder.ToString()}");
            return new SqliteConnection(builder.ToString());
        }

        private static void LoadTableDefinitionsFile()
        {
            using (var stream = new StreamReader(File.OpenRead(TableDefinitionFile)))
            {
                var ddls = stream.ReadToEnd().Split(";");
                var headerComment = new Regex("--[^\n]+\n");
                var tableComment = new Regex("/\\*[^/]*\\*/");
                var spaceUntilComma = new Regex("[ \t]+,");
                var whiteSpaces = new Regex("[\\s]+");
                foreach (var ddl in ddls)
                {
                    var s1 = headerComment.Replace(ddl.Trim(), string.Empty);
                    var s2 = tableComment.Replace(s1.Trim(), string.Empty);
                    var s3 = spaceUntilComma.Replace(s2, ","); // 行頭スペースは確保したいので Trim() は行わない
                    var s4 = whiteSpaces.Replace(s3, " ");
                    if (s4.StartsWith("CREATE TABLE", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var s5 = s4.Split()[2];
                        TableDefinitions.Add(Tuple.Create(s5, s4));
                    }
                }
            }
        }

        private static void OpenDatabase(DbConnection db)
        {
            db.Open();

            foreach (var ddl in TableDefinitions)
            {
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = ddl.Item2;
                    cmd.ExecuteNonQuery();
                    LogUtil.Debug($"Table {ddl.Item1} Created.");
                }
            }
        }

        private static void CreateTestBaseData(SqliteConnection db)
        {
            using (var context = new MemorieDeFleursDbContext(db))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var model = new MemorieDeFleursModel(db);
                    PrepareParts(model, context);
                    PrepareBouquets(model, context);
                    PrepareCustomers(model, context);
                    PrepareSuppliers(model, context);
                    PrepareInitialOrderToSupplier(model, context);
                    PrepareOrdersFromCustomer(model, context);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static void PrepareParts(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA002")
                .PartNameIs("薔薇(白)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA003")
                .PartNameIs("薔薇(ピンク)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(50)
                .ExpiryDateIs(2)
                .Create(context);

            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);

            model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN002")
                .PartNameIs("カーネーション(ピンク)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);
        }

        private static void PrepareBouquets(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses("BA001", 3)
                .Uses("BA003", 3)
                .Uses("GP001", 6)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT003")
                .NameIs("一輪挿し-A")
                .Uses("BA003", 1)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT004")
                .NameIs("結婚式用ブーケ")
                .Uses("BA002", 3)
                .Uses("BA003", 5)
                .Uses("GP001", 3)
                .Uses("CN002", 3)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT005")
                .NameIs("母の日感謝セット")
                .Uses("CN001", 6)
                .Uses("CN002", 6)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT006")
                .NameIs("花束-Cセット")
                .Uses("BA001", 6)
                .Uses("BA002", 4)
                .Uses("BA003", 3)
                .Uses("GP001", 3)
                .Uses("CN001", 2)
                .Uses("CN002", 5)
                .Create(context);

            model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT007")
                .NameIs("還暦祝い60本セット")
                .Uses("BA001", 40)
                .Uses("BA002", 10)
                .Uses("BA003", 10)
                .Create(context);
        }

        private static void PrepareCustomers(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .SendTo("ピアノ生徒1", "東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .SendTo("ピアノ生徒2", "茨城県水戸市城南2-1-20", "井門水戸ビル5階")
                .SendTo("友人A", "静岡県浜松市中区板屋町111-2", "浜松アクトタワー10階")
                .SendTo("ピアノ生徒3", "愛知県名古屋市西区上名古屋3-25-28", "第7猪村ビル4階")
                .SendTo("ピアノ生徒4", "大阪府吹田市垂水町3-9-10", "白川ビル3階")
                .SendTo("友人B", "福岡県福岡市博多区博多駅南2-8-16", "東洋マンション駅南スターオフィス2階")
                .Create(context);

            model.CustomerModel.GetCustomerBuilder()
                .NameIs("ユーザ2")
                .EmailAddressIs("user2@localdomain")
                .CardNoIs("1234123412341230")
                .SendTo("友人A", "住所2-1")
                .SendTo("友人B", "住所2-2", "建物2A")
                .SendTo("友人C", "住所2-3")
                .SendTo("生徒1", "住所2-4")
                .SendTo("生徒2", "住所2-5", "建物2B")
                .SendTo("生徒3", "住所2-6")
                .SendTo("生徒4", "住所2-7")
                .Create(context);

            model.CustomerModel.GetCustomerBuilder()
                .NameIs("ユーザ3")
                .EmailAddressIs("user3@localdomain")
                .CardNoIs("1234567890123450")
                .SendTo("友人V", "住所3-1-1")
                .SendTo("友人W", "住所3-1-2")
                .SendTo("友人X", "住所3-1-3")
                .SendTo("友人Y", "住所3-1-4")
                .SendTo("友人Z", "住所3-1-5")
                .SendTo("親戚1", "住所3-1-6")
                .Create(context);
        }

        private static void PrepareSuppliers(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .EmailIs("shinbashi@localdomain")
                .PhoneNumberIs("00012345678")
                .FaxNumberIs("00012345677")
                .SupplyParts("BA001", "BA002", "BA003", "GP001")
                .Create(context);

            model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .EmailIs("kobiki@localdomain")
                .PhoneNumberIs("09098765432")
                .FaxNumberIs("0120000000")
                .SupplyParts("GP001", "CN001", "CN002")
                .Create(context);
        }

        private static void PrepareInitialOrderToSupplier(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            var orderDate = new DateTime(DateConst.Year, 3, 10);
            var supplier1 = model.SupplierModel.Find(1);
            var supplier2 = model.SupplierModel.Find(2);
            var ba001 = model.BouquetModel.FindBouquetPart("BA001");
            var ba002 = model.BouquetModel.FindBouquetPart("BA002");
            var ba003 = model.BouquetModel.FindBouquetPart("BA003");
            var gp001 = model.BouquetModel.FindBouquetPart("GP001");
            var cn001 = model.BouquetModel.FindBouquetPart("CN001");
            var cn002 = model.BouquetModel.FindBouquetPart("CN002");

            // 仕入先1
            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.April30th,
                new[] {
                    Tuple.Create(ba001, 2),
                    Tuple.Create(ba002, 1),
                    Tuple.Create(ba003, 1),
                    Tuple.Create(gp001, 1),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May1st,
                new[] {
                    Tuple.Create(ba001, 3),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May2nd,
                new[] {
                    Tuple.Create(ba001, 2),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    Tuple.Create(ba001, 2),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    /* 同一日の発注が単品毎に分かれているパターン */
                    Tuple.Create(ba002, 1),
                    Tuple.Create(ba003, 2),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    /* 同一商品を同日複数回発注 : 在庫ロットが1日2ロットある状態を作る */
                    Tuple.Create(ba002, 1),
                });

            model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May6th,
                new[] {
                    Tuple.Create(ba001, 1),
                });

            // 仕入先2
            model.SupplierModel.Order(context, orderDate, supplier2, DateConst.April30th,
                new[]
                {
                    Tuple.Create(cn001, 1),
                    Tuple.Create(cn002, 1),
                });

            model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May1st,
                new[]
                {
                    Tuple.Create(cn002, 1),
                });


            model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May2nd,
                new[]
                {
                    Tuple.Create(gp001, 1),
                    Tuple.Create(cn001, 1),
                });

            model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May3rd,
                new[]
                {
                    Tuple.Create(cn002, 1),
                });

            model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May5th,
                new[]
                {
                    Tuple.Create(gp001, 1),
                    Tuple.Create(cn002, 1),
                });

        }

        private static void PrepareOrdersFromCustomer(MemorieDeFleursModel model, MemorieDeFleursDbContext context)
        {
            var orderDates = Enumerable.Range(0, 10).Select(i => new DateTime(DateConst.Year, 3, 1 + i)).ToArray();
            var rnd = new Random();

            var ht001 = model.BouquetModel.FindBouquet("HT001");
            var ht002 = model.BouquetModel.FindBouquet("HT002");
            var ht003 = model.BouquetModel.FindBouquet("HT003");
            var ht004 = model.BouquetModel.FindBouquet("HT004");
            var ht005 = model.BouquetModel.FindBouquet("HT005");
            var ht006 = model.BouquetModel.FindBouquet("HT006");
            var ht007 = model.BouquetModel.FindBouquet("HT007");

            var customer1 = model.CustomerModel.Find(1);
            var customer2 = model.CustomerModel.Find(2);
            var customer3 = model.CustomerModel.Find(3);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[0], DateConst.May1st, "Message1");
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[1], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[2], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[3], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[4], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer1.ShippingAddresses[5], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[0], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[1], DateConst.May1st, "Message2");
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer2.ShippingAddresses[2], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer2.ShippingAddresses[3], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer2.ShippingAddresses[4], DateConst.May1st);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer2.ShippingAddresses[5], DateConst.May1st);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[6], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[0], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[1], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[2], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[3], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer3.ShippingAddresses[4], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer3.ShippingAddresses[5], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer1.ShippingAddresses[0], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer1.ShippingAddresses[1], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer1.ShippingAddresses[2], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer1.ShippingAddresses[3], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[4], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[5], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer2.ShippingAddresses[0], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer2.ShippingAddresses[1], DateConst.May2nd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer2.ShippingAddresses[2], DateConst.May2nd);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[3], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[4], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[5], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[6], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer3.ShippingAddresses[0], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer3.ShippingAddresses[1], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[2], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[3], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[4], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[5], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[0], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[1], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[2], DateConst.May3rd);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[3], DateConst.May3rd);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[4], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer1.ShippingAddresses[5], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer2.ShippingAddresses[0], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer2.ShippingAddresses[1], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer2.ShippingAddresses[2], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht002, customer2.ShippingAddresses[3], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer2.ShippingAddresses[4], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer2.ShippingAddresses[5], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[6], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[0], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer3.ShippingAddresses[1], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer3.ShippingAddresses[2], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer3.ShippingAddresses[3], DateConst.May4th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht005, customer3.ShippingAddresses[4], DateConst.May4th);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer3.ShippingAddresses[5], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer1.ShippingAddresses[0], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht003, customer1.ShippingAddresses[1], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[2], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[3], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[4], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[5], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[0], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[1], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[2], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[3], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[4], DateConst.May5th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer2.ShippingAddresses[5], DateConst.May5th);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[6], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[0], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[1], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[2], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer3.ShippingAddresses[3], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer3.ShippingAddresses[4], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer3.ShippingAddresses[5], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[0], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[1], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[2], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[3], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[4], DateConst.May6th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht007, customer1.ShippingAddresses[5], DateConst.May6th);

            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[0], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[1], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[2], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht001, customer2.ShippingAddresses[3], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[4], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[5], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer2.ShippingAddresses[6], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[0], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[1], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht004, customer3.ShippingAddresses[2], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer3.ShippingAddresses[3], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer3.ShippingAddresses[4], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer3.ShippingAddresses[5], DateConst.May7th);
            model.CustomerModel.Order(context, orderDates[rnd.Next(0, 10)], ht006, customer1.ShippingAddresses[0], DateConst.May7th);
        }
        #endregion // TestInitialize

        #region ClassCleanup
        protected static void CloseDatabase(SqliteConnection db)
        {
            ClearModelData(db);
            db.Close();
            db.Dispose();
        }
        public static void ClearModelData(DbConnection connection)
        {
            // 削除対象テーブル、削除順
            var tables = new List<string>()
            {
                "INVENTORY_ACTIONS",
                "ORDER_DETAILS_FROM_CUSTOMER",
                "ORDER_DETAILS_TO_SUPPLIER",
                "BOUQUET_PARTS_LIST",
                "BOUQUET_SUPPLIERS",
                "ORDER_FROM_CUSTOMER",
                "ORDERS_TO_SUPPLIER",
                "SHIPPING_ADDRESS",
                "CUSTOMERS",
                "SUPPLIERS",
                "BOUQUET_SET",
                "BOUQUET_PARTS",
                "SEQUENCES",
                "DATE_MASTER",
            };

            using (var cmd = connection.CreateCommand())
            {
                foreach (var t in tables)
                {
                    cmd.CommandText = $"delete from {t}";
                    cmd.ExecuteNonQuery();
                }
            }

        }
        #endregion // ClassCleanup

        #region DEBUGLOG
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
            LogUtil.DEBUGLOG_BeginMethod(partsCode, "", caller, path, line);
            LogUtil.Indent++;

            using (var context = new MemorieDeFleursDbContext(connection))
            {
                if (lots == null)
                {
                    foreach (var action in context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .OrderBy(act => act.ArrivalDate)
                        .ThenBy(act => act.InventoryLotNo)
                        .ThenBy(act => act.ActionDate)
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
            LogUtil.DEBUGLOG_EndMethod(partsCode, "", caller, path, line);
        }

        #endregion // DEBUGLOG
    }
}
