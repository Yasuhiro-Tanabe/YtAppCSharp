using MemorieDeFleurs.Logging;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFLearningRelationshipTest
    {
        #region EFCore エンティティ定義
        [Table("PRODUCTS")]
        public class Product
        {
            [Column("ID"), Key]
            public int ID { get; set; }
            [Column("NAME")]
            public string Name { get; set; }

            public IList<BOM> UsingParts { get; } = new List<BOM>(); 

        }

        [Table("PARTS")]
        public class Parts
        {
            [Column("ID"), Key]
            public int ID { get; set; }
            [Column("NAME")]
            public string Name { get; set; }

            public IList<BOM> ProductsBeingUsed { get; } = new List<BOM>();
        }

        [Table("BOM")]
        public class BOM
        {
            [Column("PRODUCT_ID")]
            public int ProductID { get; set; }
            [Column("PARTS_ID")]
            public int PartsID { get; set; }
            [Column("QUANTITY")]
            public int Quantity { get; set; }

            [ForeignKey("ProductID")]
            public Product Product { get; set; }
            [ForeignKey("PartsID")]
            public Parts Parts { get; set; }
        }
        #endregion // EFCore エンティティ定義

        #region DbContext
        public class TestDbContext : DbContext
        {
            private DbConnection _conn;
            public TestDbContext(DbConnection conn)
            {
                _conn = conn;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder builder)
            {
                builder.UseSqlite(_conn);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<BOM>()
                    .HasKey("ProductID", "PartsID");

                // ①： 1対多の関連を Product 側に反映する
                modelBuilder
                    .Entity<BOM>()
                    .HasOne(b => b.Product)
                    .WithMany(p => p.UsingParts)
                    .HasForeignKey(p => p.ProductID);

                // ②： 1対多の関連を Parts 側に反映する
                modelBuilder
                    .Entity<BOM>()
                    .HasOne(b => b.Parts)
                    .WithMany(p => p.ProductsBeingUsed)
                    .HasForeignKey(p => p.PartsID);
            }

            public DbSet<Product> Products { get; set; }
            public DbSet<Parts> Parts { get; set; }
            public DbSet<BOM> Bom { get; set; }
        }
        #endregion // DbContext

        #region データベース (SQLite)
        private IList<string> DDLs = new List<string>()
        {
            "CREATE TABLE PRODUCTS (" +
            "        ID   INT  NOT NULL PRIMARY KEY," +
            "        NAME TEXT NOT NULL" +
            "    )",
            "CREATE TABLE PARTS (" +
            "        ID   INT  NOT NULL PRIMARY KEY," +
            "        NAME TEXT NOT NULL" +
            "    )",
            "CREATE TABLE BOM (" +
            "        PRODUCT_ID INT NOT NULL REFERENCES PRODUCTS(ID)," +
            "        PARTS_ID   INT NOT NULL REFERENCES PARTS(ID)," +
            "        QUANTITY   INT NOT NULL DEFAULT 0," +
            "      PRIMARY KEY ( PRODUCT_ID, PARTS_ID )" +
            "    )"
        };

        private SqliteConnection InMemoryDB { get; set; }

        private SqliteConnection CreateInMemoryDB()
        {
            return new SqliteConnection("Data Source=:memory:;Foreign Keys=True");
        }

        private void SaveCurrentDatabaseTo(SqliteConnection connection, string dbFileName)
        {
            LogUtil.DEBUGLOG_BeginMethod(dbFileName);
            try
            {
                if (File.Exists(dbFileName))
                {
                    LogUtil.Debug($"Database {dbFileName} is alerdy exists. removed.");
                    File.Delete(dbFileName);
                }

                var builder = new SqliteConnectionStringBuilder();
                builder.DataSource = dbFileName;
                builder.Mode = SqliteOpenMode.ReadWriteCreate;
                builder.ForeignKeys = true;

                var backupDb = new SqliteConnection(builder.ToString());
                connection.BackupDatabase(backupDb);
                LogUtil.Debug($"Database backuped to: {dbFileName}");

                LogUtil.DEBUGLOG_EndMethod(dbFileName, $"Saved successfully.");
            }
            catch (Exception ex)
            {
                LogUtil.DEBUGLOG_EndMethod(dbFileName, $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        private string GetTempFileName(string baseDir)
        {
            var tmpName = Path.GetTempFileName();

            return Path.Combine(baseDir, Path.GetFileName(tmpName));
        }
        #endregion データベース

        #region TestInitialize/TestTerminate
        [TestInitialize]
        public void Initialize()
        {
            InMemoryDB = CreateInMemoryDB();

            InMemoryDB.Open();

            foreach(var ddl in DDLs)
            {
                using (var cmd = InMemoryDB.CreateCommand())
                {
                    cmd.CommandText = ddl;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Terminate()
        {
            
            SaveCurrentDatabaseTo(InMemoryDB, GetTempFileName("./testdata/db"));

            InMemoryDB.Close();
        }
        #endregion // TestInitialize/TestTerminate

        #region シーケンス
        private class Sequence
        {
            private int _index = 0;

            public int Next { get { return ++_index; } }
        }

        private Sequence SEQ_PARTS { get; } = new Sequence();
        private Sequence SEQ_PRODCUTS { get; } = new Sequence();
        #endregion // シーケンス

        #region エンティティ操作
        private int AddProduct(string name)
        {
            using (var context = new TestDbContext(InMemoryDB))
            {
                var product = new Product() { ID = SEQ_PRODCUTS.Next, Name = name };

                context.Products.Add(product);
                context.SaveChanges();

                return product.ID;
            }
        }

        private int AddParts(string name)
        {
            using (var context = new TestDbContext(InMemoryDB))
            {
                var parts = new Parts() { ID = SEQ_PARTS.Next, Name = name };

                context.Parts.Add(parts);
                context.SaveChanges();

                return parts.ID;
            }
        }

        private void AddToBom(int productID, int partsID, int quantity)
        {
            using (var context = new TestDbContext(InMemoryDB))
            {
                var bom = new BOM() { ProductID = productID, PartsID = partsID, Quantity = quantity };

                context.Bom.Add(bom);
                context.SaveChanges();
            }
        }
        #endregion // エンティティ操作

        [TestMethod]
        public void Rerationship_CanReference()
        {
            var productID = AddProduct("製品1");
            var parts = new List<int>()
            {
                AddParts("部品1"),
                AddParts("部品2"),
                AddParts("部品3")
            };
            AddToBom(productID, parts[0], 2);
            AddToBom(productID, parts[1], 5);
            AddToBom(productID, parts[2], 3);

            using (var context = new TestDbContext(InMemoryDB))
            {
                var actual = context.Products.Find(productID);
                var actualParts = context.Parts.OrderBy(p => p.ID).ToList();

                var list = context.Bom.Where(b => b.ProductID == productID).ToList();
                Assert.AreEqual(3, list.Count());

                Assert.AreEqual("製品1", actual.Name);
                Assert.AreEqual(3, actual.UsingParts.Count());


                // DbContext.OnModelCreating() で ① の関連を追加することでテストが Green になる
                Assert.IsNotNull(actual.UsingParts[0].Product);
                Assert.AreEqual(actual.Name, actual.UsingParts[0].Product.Name);

                // DbContext.OnModelCreating() で ② の関連を追加するだけではNG。
                // 反対側の Parts も取得しないとテストは Green にならない。
                Assert.IsNotNull(actual.UsingParts[0].Parts);
                Assert.AreEqual("部品1", actual.UsingParts[0].Parts.Name);
            }

        }

    }
}
