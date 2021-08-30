
using MemorieDeFleurs;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using System;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class DateUtilTest : MemorieDeFleursTestBase
    {
        private DateUtil TestDateMaster { get; set; }
        private DateUtil EmptyDateMaster { get; set; }

        public DateUtilTest() : base()
        {
            TestDateMaster = new DateUtil(TestDB);
            EmptyDateMaster = new DateUtil(EmptyDB);

            BeforeTestBaseCleaningUp += CleanupDateMaster;
        }

        public void CleanupDateMaster(object sender, EventArgs unused)
        {
            TestDateMaster.Clear();
        }

        [TestMethod]
        public void NoDatesInEmptyDb()
        {
            Assert.AreEqual(DateUtil.InvalidDate, EmptyDateMaster.FirstDate);
            Assert.AreEqual(DateUtil.InvalidDate, EmptyDateMaster.LastDate);
        }

        [TestMethod]
        public void AddAndRemoveDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20221231;

            TestDateMaster.Fill(startDate, endDate);
            Assert.AreEqual(startDate, TestDateMaster.FirstDate);
            Assert.AreEqual(endDate, TestDateMaster.LastDate);

            TestDateMaster.Clear();
            Assert.AreEqual(DateUtil.InvalidDateIndex, TestDateMaster.FirstDate);
            Assert.AreEqual(DateUtil.InvalidDateIndex, TestDateMaster.LastDate);
        }

        [TestMethod]
        public void IsValidDate()
        {
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20200101));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20211231));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20990630));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20200229)); // �[�N�ł���
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20000229)); // �[�N�ł���A������N%400=0�Ȃ̂ŉ[��������
        }

        [TestMethod]
        public void IsInvalidDate()
        {
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20200100));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20211301));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20231131));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20210229));// �[�N�ł͂Ȃ�
            Assert.IsFalse(EmptyDateMaster.IsValidDate(21000229));// �[�N�����[�����Ȃ�(400�N��3��A����N%400=100,200,300�̔N)
        }

        [TestMethod]
        public void NextDayOf20200331Is20200401()
        {
            TestDateMaster.Fill(20200101, 20201231);

            Assert.AreEqual(20200401, TestDateMaster.Add(20200331, 1));
        }

        [TestMethod]
        public void PreviousDayOf20200401Is20200331()
        {
            TestDateMaster.Fill(20200101, 20201231);

            Assert.AreEqual(20200331, TestDateMaster.Add(20200401, -1));
        }

        public void PreviousDayIsNotRegisteredDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20201231;
            TestDateMaster.Fill(startDate, endDate);

            Assert.AreEqual(DateUtil.InvalidDate, TestDateMaster.Add(startDate, -1));
        }

        public void NextDayIsNotRegisteredInDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20201231;
            TestDateMaster.Fill(startDate, endDate);

            Assert.AreEqual(DateUtil.InvalidDate, TestDateMaster.Add(endDate, 1));
        }
    }
}