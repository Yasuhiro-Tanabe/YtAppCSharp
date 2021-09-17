using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFCoreLearningTest : MemorieDeFleursTestBase
    {
        /// <summary>
        /// テストで使用する単品
        /// </summary>
        private BouquetPart ExpectedPart { get; set; }

        /// <summary>
        /// 検証対象モデル
        /// </summary>
        private MemorieDeFleursModel Model { get; set; }


        public EFCoreLearningTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupData;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);

            ExpectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
        }
        #endregion

        #region TestCleanup
        void CleanupData(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion

        #region DbContext.Database.CurrentTransaction の確認
        /// <summary>
        /// DbContext.Database.CurrentTransaction が BeginTransaction() の際にセットされ
        /// Commit() により null クリアされることの確認
        /// </summary>
        [TestMethod]
        public void CommitResetsCurrentTransactionInDbContext()
        {
            using(var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.IsNotNull(context.Database.CurrentTransaction);
                    transaction.Commit();
                }
                Assert.IsNull(context.Database.CurrentTransaction);
            }    
        }
        /// <summary>
        /// DbContext.Database.CurrentTransaction が BeginTransaction() の際にセットされ
        /// Rollback() により null クリアされることの確認
        /// </summary>
        public void RollbackResetsCurrentTransactionInDbCotenxt()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.IsNotNull(context.Database.CurrentTransaction);
                    transaction.Rollback();
                }
                Assert.IsNull(context.Database.CurrentTransaction);
            }
        }

        /// <summary>
        /// (SQLiteでは) 入れ子のトランザクションは作れない
        /// </summary>
        [TestMethod]
        public void LearnEFCoreTransaction_CannotUseMultipeTransactionsInSingleDbContext()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var tansaction1 = context.Database.BeginTransaction())
            {
                Assert.ThrowsException<InvalidOperationException>(() => context.Database.BeginTransaction());
            }
        }

        /// <summary>
        /// 同一 DbConnection から生成した DbContext だが、外側の DbContext の状態を継承するわけではない
        /// 
        /// また内側の DbContext では BeginTransaction() を呼び出せない
        /// </summary>
        [TestMethod]
        public void InnerAndOuterDbContextUsesSameDbConnection_ButNotInSameTransaction()
        {
            using(var outerContext = new MemorieDeFleursDbContext(TestDB))
            using (var outerTransaction = outerContext.Database.BeginTransaction())
            {
                Assert.IsNotNull(outerContext.Database.CurrentTransaction);
                Assert.AreEqual(outerTransaction.TransactionId, outerContext.Database.CurrentTransaction.TransactionId);
                using (var innerContext = new MemorieDeFleursDbContext(TestDB))
                {
                    Assert.IsNull(innerContext.Database.CurrentTransaction);
                    Assert.ThrowsException<InvalidOperationException>(() => innerContext.Database.BeginTransaction());
                }
            }

        }

        [TestMethod]
        public void CurrentTransactionIDOfDbContextAlwaysUnique()
        {
            var num = 10;
            var ids = new SortedSet<Guid>();
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                foreach(var i in Enumerable.Range(0, num))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        ids.Add(context.Database.CurrentTransaction.TransactionId);
                        transaction.Commit();
                    }
                }
            }

            Assert.AreEqual(num, ids.Count());
        }
        #endregion // CUrrentTransaction の確認

        [TestMethod]
        public void Transaction_CanCommit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(0, context.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    CreateStockAction(context, 1);
                    Assert.AreEqual(1, context.StockActions.Count(), "登録されている在庫アクション数が１つあるはず");
                    transaction.Commit();

                }
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, context.StockActions.Count(), "コミットにより在庫アクションが保存されるはず");
            }
        }

        [TestMethod]
        public void Transaction_CanRollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(0, context.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    CreateStockAction(context, 1);
                    Assert.AreEqual(1, context.StockActions.Count(), "ロールバック前なので、登録された在庫アクションが残っているはず");
                    transaction.Rollback();
                }
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(0, context.StockActions.Count(), "ロールバック後も在庫アクションが残っている");
            }
        }

        #region 在庫アクションの登録
        private StockAction CreateStockAction(MemorieDeFleursDbContext context, int lotNo)
        {
            StockAction action = CreateStockActionWithoutCallingSaveChanges(context, lotNo);
            context.SaveChanges();
            return action;
        }

        private StockAction CreateStockActionWithoutCallingSaveChanges(MemorieDeFleursDbContext context, int lotNo)
        {
            StockAction action = new StockAction()
            {
                ActionDate = new DateTime(2004, 03, 30),
                Action = StockActionType.SCHEDULED_TO_ARRIVE,
                PartsCode = ExpectedPart.Code,
                ArrivalDate = new DateTime(2004, 03, 30),
                StockLotNo = lotNo,
                Quantity = 200,
                Remain = 200
            };

            context.StockActions.Add(action);
            return action;
        }
        #endregion // 在庫アクションの登録

        #region SaveChanges をエンティティ登録の都度呼び出す
        #region Linq コレクションの操作方法により DB 登録できたりできなかったりする
        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_LinqSelect_Commit()
        {
            // お作法として、DBとDBContextおよびトランザクションとは同じタイミングで作るのが正しいらしい：
            //     using (var db = ...)
            //     using (var context = new MemorieDeFleursDbContext(TestDB))
            //     using(var transaction = context.DataBase.BeginTransaction() { ... }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    Enumerable.Range(1, numActions).Select(i => CreateStockAction(context, i));
                    transaction.Commit();
                }

                // ??? Linq の IEnumerable<T1>.Select<T1,T2>() 内で呼び出すと、Commit() してもトランザクションに反映されない？
                Assert.AreEqual(0, context.StockActions.Count());
            }

        }

        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_ListForEach_Commit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var list = Enumerable.Range(1, numActions).ToList();
                    list.ForEach(i => CreateStockAction(context, i));
                    transaction.Commit();
                }
                Assert.AreEqual(numActions, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_ForEachStatment_Commit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var list = Enumerable.Range(1, numActions);
                    foreach (var i in list)
                    {
                        CreateStockAction(context, i);
                    }
                    transaction.Commit();
                }
                Assert.AreEqual(numActions, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_LinqSelect_Rollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    Enumerable.Range(1, numActions).Select(i => CreateStockAction(context, i));
                    transaction.Rollback();
                }

                // Commit しても反映されていないので、現時点ではテストが無意味。
                // SaveChangesInTransaction_CallForEachCreatedEntity_LinqSelect_Commit() 参照。
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_ListForEach_Rollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var list = Enumerable.Range(1, numActions).ToList();
                    list.ForEach(i => CreateStockAction(context, i));
                    transaction.Rollback();
                }
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallForEachCreatedEntity_ForEachStatment_Rollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                int numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var list = Enumerable.Range(1, numActions);
                    foreach (var i in list)
                    {
                        CreateStockAction(context, i);
                    }
                    transaction.Rollback();
                }
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }
        #endregion // コレクションの操作方法により DB 登録できたりできなかったりする
        #endregion // SaveChanges をエンティティ登録の都度呼び出す

        #region SaveChanges を全エンティティ登録後に呼び出す
        #region 基本型：ループ内で呼び出す
        [TestMethod]
        public void SaveChangesInTransaction_CallOnceOnlyAfterAllEntityAdded_Commit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    // 一番確実な、 foreach ループによる方法で登録する
                    var list = Enumerable.Range(1, numActions);
                    foreach (var i in list)
                    {
                        CreateStockActionWithoutCallingSaveChanges(context, i);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
                Assert.AreEqual(numActions, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallOnceOnlyAfterAllEntityAdded_Rollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    // 一番確実な、 foreach ループによる方法で登録する
                    var list = Enumerable.Range(1, numActions);
                    foreach (var i in list)
                    {
                        CreateStockActionWithoutCallingSaveChanges(context, i);
                    }
                    context.SaveChanges();
                    transaction.Rollback();
                }
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }
        #endregion // 基本型

        #region Action 内、同期呼出
        [TestMethod]
        public void SaveChangesInTransaction_CallOnce_InsideAction_Commit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    Action action = () =>
                    {
                        var list = Enumerable.Range(1, numActions);
                        foreach (var i in list)
                        {
                            CreateStockActionWithoutCallingSaveChanges(context, i);
                            context.SaveChanges();
                        }
                    };

                    action();
                    transaction.Commit();
                }
                Assert.AreEqual(numActions, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallOnce_InsideAction_Rollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    Action action = () =>
                    {
                        var list = Enumerable.Range(1, numActions);
                        foreach (var i in list)
                        {
                            CreateStockActionWithoutCallingSaveChanges(context, i);
                        }
                        context.SaveChanges();
                    };

                    action();
                    transaction.Rollback();
                }
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }
        #endregion // Action 内、同期呼出

        #region // 別タスク
        [TestMethod]
        public void SaveChangesInTransaction_CallOnce_InsideTask_CommitInsideTask()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var task = Task.Run(() =>
                    {
                        var list = Enumerable.Range(1, numActions);
                        foreach (var i in list)
                        {
                            CreateStockActionWithoutCallingSaveChanges(context, i);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                    });

                    task.Wait();
                }
                Assert.AreEqual(numActions, context.StockActions.Count());
            }
        }

        [TestMethod]
        public void SaveChangesInTransaction_CallOnce_InsideTask_RollbackInsideTask()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var numActions = 10;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var task = Task.Run(() =>
                    {
                        var list = Enumerable.Range(1, numActions);
                        foreach (var i in list)
                        {
                            CreateStockActionWithoutCallingSaveChanges(context, i);
                        }
                        context.SaveChanges();
                        transaction.Rollback();
                    });

                    task.Wait();
                }
                Assert.AreEqual(0, context.StockActions.Count());
            }
        }
        #endregion // 別タスク
        #endregion // SaveChanges を全エンティティ登録後に呼び出す
    }
}
