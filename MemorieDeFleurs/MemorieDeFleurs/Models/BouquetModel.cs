using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 商品(花束)と単品(花)の管理モデル
    /// </summary>
    public class BouquetModel
    {
        private MemorieDeFleursDbContext DbContext { get; set; }
        private MemorieDeFleursModel Parent { get; set; }

        private SequenceUtil Sequence { get; set; }

        /// <summary>
        /// (パッケージ内限定)コンストラクタ
        /// 
        /// モデルのプロパティとして参照できるので、外部でこのオブジェクトを作成することは想定しない。
        /// </summary>
        /// <param name="parent"></param>
        internal BouquetModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
            DbContext = parent.DbContext;
            Sequence = new SequenceUtil(DbContext.Database.GetDbConnection() as SqliteConnection);
        }

        /// <summary>
        /// 単品オブジェクト生成器
        /// 
        /// 単品の各プロパティはフルーエントインタフェース形式で入力する。
        /// </summary>
        public class BouquetPartProcessor
        {
            private BouquetModel _model;

            private string _code;
            private string _name;
            private int _leadTime;
            private int _parLot;
            private int _expire;

            internal static BouquetPartProcessor GetInstance(BouquetModel parent)
            {
                return new BouquetPartProcessor(parent);
            }

            private BouquetPartProcessor(BouquetModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 花コードを登録/変更する
            /// </summary>
            /// <param name="code">花コード</param>
            /// <returns>花コード変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartProcessor PartCodeIs(string code)
            {
                _code = code;
                return this;
            }

            /// <summary>
            /// 単品名称を登録/変更する
            /// </summary>
            /// <param name="name">単品名称</param>
            /// <returns>単品名称変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartProcessor PartNameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// 発注リードタイムを登録/変更する
            /// </summary>
            /// <param name="days">発注リードタイム、単位は日</param>
            /// <returns>発注リードタイム変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartProcessor LeadTimeIs(int days)
            {
                _leadTime = days;
                return this;
            }

            /// <summary>
            /// 購入単位数を登録/変更する
            /// </summary>
            /// <param name="quantity">購入単位数：1発注ロットあたりの単品数、単位は本</param>
            /// <returns>購入単位数更新後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartProcessor QauntityParLotIs(int quantity)
            {
                _parLot = quantity;
                return this;
            }
            /// <summary>
            /// 品質維持可能日数を登録/変更する
            /// </summary>
            /// <param name="days">品質維持可能日数、単位は日</param>
            /// <returns>品質維持可能日数変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartProcessor ExpiryDateIs(int days)
            {
                _expire = days;
                return this;
            }

            /// <summary>
            /// 現在の内容で単品オブジェクトを生成、データベースに登録する
            /// </summary>
            /// <returns>生成/データベース登録された単品オブジェクト</returns>
            public BouquetPart Create()
            {
                var p = new BouquetPart()
                {
                    Code = _code,
                    Name = _name,
                    LeadTime = _leadTime,
                    QuantitiesPerLot = _parLot,
                    ExpiryDate = _expire,
                    Status = 0
                };

                _model.DbContext.BouquetParts.Add(p);
                _model.DbContext.SaveChanges();
                return p;
            }
        }

        /// <summary>
        /// DB登録オブジェクト生成器を取得する
        /// </summary>
        /// <typeparam name="BouquetPart">DB登録オブジェクト生成器が生成するオブジェクト：単品</typeparam>
        /// <returns>単品オブジェクト生成器</returns>
        public BouquetPartProcessor Entity<BouquetPart>()
        {
            return BouquetPartProcessor.GetInstance(this);
        }

        /// <summary>
        /// 条件を満たす仕入先オブジェクトを取得する
        /// </summary>
        /// <param name="condition">取得条件</param>
        /// <returns>単品オブエクとの列挙(<see cref="IEnumerable{BouquetPart}"/>)。
        /// 条件を満たす仕入先が1つしかない場合でも列挙として返すので注意</returns>
        public IEnumerable<BouquetPart> Find(Func<BouquetPart,bool> condition)
        {
            return DbContext.BouquetParts.Where(condition);
        }

        /// <summary>
        /// 花コードをキーに単品オブジェクトを取得する
        /// </summary>
        /// <param name="partCode">花コード</param>
        /// <returns>単品オブジェクト。花コードに該当する単品がないときはnull。</returns>
        public BouquetPart Find(string partCode)
        {
            return DbContext.BouquetParts.SingleOrDefault(p => p.Code == partCode);
        }

        /// <summary>
        /// 更新した単品オブジェクトでデータベースを更新する
        /// </summary>
        /// <param name="p">単品オブジェクト。nullの場合は更新処理を行わずに処理を抜ける。</param>
        /// <remarks>呼び出し前に花コードを書き換えないこと。意図しない単品の内容が変更されることがある。</remarks>
        public void Replace(BouquetPart p)
        {
            if(p == null) { return; }
            DbContext.BouquetParts.Update(p);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 単品オブジェクトをデータベースから削除する
        /// </summary>
        /// <param name="p">単品オブジェクト。nullの場合は削除処理を行わずに処理を抜ける。</param>
        /// <remarks>呼び出し前に花コードを書き換えないこと。意図しない単品が削除されることがある。</remarks>
        public void Remove(BouquetPart p)
        {
            if (p == null) { return; }
            DbContext.BouquetParts.Remove(p);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 単品コード指定で単品をデータベースから削除する。
        /// 該当する単品が見つからないときは何もしない。
        /// </summary>
        /// <param name="partCode">花コード</param>
        /// <remarks>内部で <see cref="Find(string)"/> を呼び出し、
        /// 見つかった単品オブジェクトを <see cref="Remove(BouquetPart)"/> で破棄する。</remarks>
        public void Remove(string partCode)
        {
            Remove(Find(partCode));
        }
    }
}
