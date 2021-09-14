﻿using MemorieDeFleurs;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [TestMethod]
        public void Transaction_CanCommit()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(0, context.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
                    CreateStockAction(context, 1);
                    Assert.AreEqual(1, context.StockActions.Count(), "登録されている在庫アクション数は１つのはず");
                    transaction.Commit();

                }
                Assert.AreEqual(1, context.StockActions.Count(), "コミットにより在庫アクションが保存されるはず");
            }
        }

        [TestMethod]
        public void Transaction_CanRollback()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(0, context.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
                    CreateStockAction(context, 1);
                    Assert.AreEqual(1, context.StockActions.Count(), "登録されている在庫アクション数は１つのはず");
                    transaction.Rollback();
                }
                Assert.AreEqual(0, context.StockActions.Count(), "ロールバック後も在庫アクションが残っている");
            }
        }

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

        [TestMethod]
        public void LearnEFCoreTransaction_CannotUseMultipeTransactionsInSingleDbContext()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var tansaction1 = context.Database.BeginTransaction())
            {
                Assert.ThrowsException<InvalidOperationException>(() => context.Database.BeginTransaction());
            }
        }

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
