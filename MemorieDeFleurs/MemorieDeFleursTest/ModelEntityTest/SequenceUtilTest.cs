using MemorieDeFleurs;
using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class SequenceUtilTest : MemorieDeFleursTestBase
    {
        private SequenceUtil TestSequence { get; set; }

        private MemorieDeFleursModel Model { get; set; }
        public SequenceUtilTest() : base()
        {
            Model = new MemorieDeFleursModel(TestDB);
            TestSequence = Model.Sequences;

            BeforeTestBaseCleaningUp += ClearTestSequence;
        }

        private void ClearTestSequence(object sender, EventArgs unused)
        {
            TestSequence.Clear();

        }

        [TestMethod]
        public void GetSeqCostmoersFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_CUSTOMERS.Next());
        }

        [TestMethod]
        public void GetSeqCostomersNextValue()
        {
            var tmp = TestSequence.SEQ_CUSTOMERS.Next();
            Assert.AreEqual(2, TestSequence.SEQ_CUSTOMERS.Next());
        }

        [TestMethod]
        public void GetSeqShippingFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_SHIPPING.Next());
        }

        [TestMethod]
        public void GetSeqShippingNextValue()
        {
            var tmp = TestSequence.SEQ_SHIPPING.Next();
            Assert.AreEqual(2, TestSequence.SEQ_SHIPPING.Next());
        }

        [TestMethod]
        public void SequenceInTransaction_CommitAvailable()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, TestSequence.SEQ_SUPPLIERS.Next(context));

                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction.Commit();
                }
                using (var transaction2 = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(3, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction2.Commit();
                }
                Assert.AreEqual(4, TestSequence.SEQ_SUPPLIERS.Next(context));
            }
        }

        #region 【懸案】トランザクションロールバックのテスト：現在は RED になるためテスト対象外
        /// <summary>
        /// ロールバックの動作確認テスト：DbContext を使い回すのは間違い。
        /// </summary>
        [TestMethod]
        public void SequenceInTransaction_DoNotReuseContext()
        {
            // DbContext を「同一トランザクション内で処理する」目的以外で共用・使い回ししてはいけない。
            // DbContext 内のエンティティはキャッシュされ続けるため。
            // また、
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, TestSequence.SEQ_SUPPLIERS.Next(context));

                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction.Rollback();
                }
                using (var transaction2 = context.Database.BeginTransaction())
                {
                    // ロールバックしたつもりでもキャッシュ上の「変更された」SequenceValue (Value=2) を取得して処理を進めるため、
                    // ロールバックしたことにならない。
                    Assert.AreEqual(3, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction2.Rollback();
                }
                // ロールバックしたつもりでもキャッシュ上の「変更された」SequenceValue (Value=3) を取得して処理を進めるため、
                // ロールバックしたことにならない。
                Assert.AreEqual(4, TestSequence.SEQ_SUPPLIERS.Next(context));
            }
        }

        /// <summary>
        /// ロールバックの動作確認テスト：正しい作法で接続するバージョン
        /// </summary>
        [TestMethod]
        public void SequenceInTransaction_RollbackAvailable()
        {
            using (var context1 = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, TestSequence.SEQ_SUPPLIERS.Next(context1));
            }

            using (var context2 = new MemorieDeFleursDbContext(TestDB))
            using(var transaction = context2.Database.BeginTransaction())
            {
                Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context2));
                transaction.Rollback();
            }

            using (var context3 = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context3));
            }
        }

        /// <summary>
        /// 同一 DbContext 内でも、DBから再度エンティティを取り直せば、
        /// ロールバックでDbContext内のキャッシュとDBの内容が不整合になっていても
        /// 整合性を取り戻すことができる
        /// </summary>
        [TestMethod]
        public void UseSingleDbContext_ReloadIsNeccessary()
        {
            var seqValue = 0;
            var seqName = "SEQ_TEST";
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                {
                    var seq = new SequenceValue() { Name = seqName, Value = 1 };
                    context.Add(seq);
                    context.SaveChanges();
                    seqValue = seq.Value;
                }
                Assert.AreEqual(1, seqValue);

                using (var transaction = context.Database.BeginTransaction())
                {
                    {
                        var seq = context.Sequences.Find(seqName);
                        seq.Value++;
                        context.SaveChanges();
                        seqValue = seq.Value;
                    }
                    Assert.AreEqual(2, seqValue);
                    transaction.Rollback();
                }

                {
                    var seq = context.Sequences.Find(seqName);
                    context.Entry(seq).Reload(); // 【重要】ロールバックしたDBの内容をもう一度取り直す
                    seq.Value++;
                    context.SaveChanges();
                    seqValue = seq.Value;
                }
                Assert.AreEqual(2, seqValue);
            }
        }

        #endregion // 懸案
    }
}
