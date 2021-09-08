﻿using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 商品(花束)と単品(花)の管理モデル
    /// </summary>
    public class BouquetModel
    {
        private MemorieDeFleursDbContext DbContext { get; set; }
        private MemorieDeFleursModel Parent { get; set; }

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
        }

        #region BouquetPartBuilder
        /// <summary>
        /// 単品オブジェクト生成器
        /// 
        /// 単品の各プロパティはフルーエントインタフェース形式で入力する。
        /// </summary>
        public class BouquetPartBuilder
        {
            private BouquetModel _model;

            private string _code;
            private string _name;
            private int _leadTime;
            private int _parLot;
            private int _expire;

            internal static BouquetPartBuilder GetInstance(BouquetModel parent)
            {
                return new BouquetPartBuilder(parent);
            }

            private BouquetPartBuilder(BouquetModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 花コードを登録/変更する
            /// </summary>
            /// <param name="code">花コード</param>
            /// <returns>花コード変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartBuilder PartCodeIs(string code)
            {
                _code = code;
                return this;
            }

            /// <summary>
            /// 単品名称を登録/変更する
            /// </summary>
            /// <param name="name">単品名称</param>
            /// <returns>単品名称変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartBuilder PartNameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// 発注リードタイムを登録/変更する
            /// </summary>
            /// <param name="days">発注リードタイム、単位は日</param>
            /// <returns>発注リードタイム変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartBuilder LeadTimeIs(int days)
            {
                _leadTime = days;
                return this;
            }

            /// <summary>
            /// 購入単位数を登録/変更する
            /// </summary>
            /// <param name="quantity">購入単位数：1発注ロットあたりの単品数、単位は本</param>
            /// <returns>購入単位数更新後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartBuilder QauntityParLotIs(int quantity)
            {
                _parLot = quantity;
                return this;
            }
            /// <summary>
            /// 品質維持可能日数を登録/変更する
            /// </summary>
            /// <param name="days">品質維持可能日数、単位は日</param>
            /// <returns>品質維持可能日数変更後の単品オブジェクト生成器(自分自身)</returns>
            public BouquetPartBuilder ExpiryDateIs(int days)
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
        /// 単品オブジェクト生成器を取得する
        /// </summary>
        /// <typeparam name="BouquetPart">単品オブジェクト生成器が生成するオブジェクト：単品</typeparam>
        /// <returns>単品オブジェクト生成器</returns>
        public BouquetPartBuilder GetBouquetPartBuilder()
        {
            return BouquetPartBuilder.GetInstance(this);
        }
        #endregion // BouquetPartBuilder

        #region BouquetBuilder
        /// <summary>
        /// 商品オブジェクト生成期
        /// </summary>
        public class BouquetBuilder
        {
            private BouquetModel _model;

            private string _code;
            private string _name;
            private string _image;
            private IDictionary<string, int> _partsList = new Dictionary<string, int>();

            public static BouquetBuilder GetInstance(BouquetModel parent)
            {
                return new BouquetBuilder(parent);
            }

            private BouquetBuilder(BouquetModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 花束コードを登録/変更する
            /// </summary>
            /// <param name="code">花束コード</param>
            /// <returns>花束コード変更後の商品オブジェクト生成器（自分自身）</returns>
            public BouquetBuilder CodeIs(string code)
            {
                _code = code;
                return this;
            }

            /// <summary>
            /// 名称を登録/変更する
            /// </summary>
            /// <param name="name">名称</param>
            /// <returns>名称変更後の商品オブジェクト生成器（自分自身）</returns>
            public BouquetBuilder NameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// イメージファイルへのパスを登録/変更する
            /// </summary>
            /// <param name="image">イメージファイルへのパス</param>
            /// <returns>イメージファイルへのパス変更後の商品オブジェクト生成器（自分自身）</returns>
            public BouquetBuilder ImageIs(string image)
            {
                _image = image;
                return this;
            }

            /// <summary>
            /// 現在の内容で商品オブジェクトを生成、データベースに登録する
            /// </summary>
            /// <returns>データベースに登録された商品オブジェクト</returns>
            public Bouquet Create()
            {
                var ret = new Bouquet()
                {
                    Code = _code,
                    Name = _name,
                    Image = _image,
                    LeadTime = 0,
                    Status = 0
                };

                foreach (var p in _partsList)
                {
                    var item = new BouquetPartsList() { BouquetCode = _code, PartsCode = p.Key, Quantity = p.Value };
                    ret.PartsList.Add(item);
                }
                _model.DbContext.Bouquets.Add(ret);

                _model.DbContext.SaveChanges();
                return ret;
            }

            /// <summary>
            /// 商品構成を追加する(単品コード指定版)
            /// 
            /// 既存単品が登録済みで単品コードがわかっているとき用
            /// </summary>
            /// <param name="partCode">単品コード</param>
            /// <param name="quantity">数量</param>
            /// <returns>商品構成追加後の商品オブジェクト生成器（自分自身)</returns>
            public BouquetBuilder Uses(string partCode, int quantity)
            {
                _partsList.Add(partCode, quantity);
                return this;
            }

            /// <summary>
            /// 商品構成を追加する(単品指定版)
            /// 
            /// 単品作成と同時に商品構成に追加するとき用
            /// </summary>
            /// <param name="part">単品</param>
            /// <param name="quantity">数量</param>
            /// <returns>商品構成追加後の商品オブジェクト生成器（自分自身)</returns>
            public BouquetBuilder Uses(BouquetPart part, int quantity)
            {
                return Uses(part.Code, quantity);
            }
        }

        /// <summary>
        /// 商品オブジェクト生成器を取得する
        /// </summary>
        /// <returns>商品オブジェクト生成器</returns>
        public BouquetBuilder GetBouquetBuilder()
        {
            return BouquetBuilder.GetInstance(this);
        }
        #endregion // BouquetBuilder

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
        public BouquetPart FindBouquetPart(string partCode)
        {
            return DbContext.BouquetParts.SingleOrDefault(p => p.Code == partCode);
        }

        /// <summary>
        /// 花束コードをキーに商品オブジェクトを取得する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <returns>商品オブジェクト。花束コードに該当する単品がないときは null。</returns>
        public Bouquet FindBouquet(string bouquetCode)
        {
            return DbContext.Bouquets.SingleOrDefault(b => b.Code == bouquetCode);
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
        /// <remarks>内部で <see cref="FindBouquetPart(string)"/> を呼び出し、
        /// 見つかった単品オブジェクトを <see cref="Remove(BouquetPart)"/> で破棄する。</remarks>
        public void Remove(string partCode)
        {
            Remove(FindBouquetPart(partCode));
        }

        #region UseBouquetPart
        /// <summary>
        /// 在庫から使用した単品を指定個数取り去る
        /// </summary>
        /// <param name="part">対象となる単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantity">取り去る数量</param>
        /// <returns>取り去った後の在庫数量：当日分複数ロットの合計値</returns>
        public int UseBouquetPart(BouquetPart part, DateTime date, int quantity)
        {
            try
            {
                var stock = DbContext.StockActions
                    .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE)
                    .Where(a => 0 == a.ActionDate.CompareTo(date))
                    .Where(a => a.Remain > 0)
                    .OrderBy(a => a.ArrivalDate)
                    .FirstOrDefault();

                if (stock == null)
                {
                    throw new NotImplementedException($"該当ストックなし：基準日={date.ToString("yyyyMMdd")}, 花コード{part.Code}, 数量={quantity}");
                }
                else
                {
                    UseBouquetPartFromTheStockAction(stock, quantity);
                }

                DbContext.SaveChanges();

                var remain = DbContext.StockActions
                    .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE || a.Action == StockActionType.OUT_OF_STOCK)
                    .Where(a => 0 == a.ActionDate.CompareTo(date))
                    .Sum(a => a.Remain);
                LogUtil.Info($"UseBouQuetPart(part={part.Code}, date={date.ToString("yyyyMMdd")}, quantity={quantity}) returns remain={remain}");
                return remain;
            }
            catch(NotImplementedException ei)
            {
                LogUtil.Fatal($"★未実装★ {ei.Message}");
                throw;
            }
            catch(Exception e)
            {
                LogUtil.Error($"UseBouquetPart(part={part.Code}, date={date.ToString("yyyyMMdd")}, quantity={quantity}) failed. {e.GetType().Name}: {e.Message}");
                LogUtil.Error(e.StackTrace);
                throw;
            }
        }

        private void UseBouquetPartFromTheStockAction(StockAction stock, int quantity)
        {
            LogUtil.Debug($"CurrentStock: date={stock.ActionDate.ToString("yyyyMMdd")}, part={stock.PartsCode}, lot={stock.StockLotNo}, remain={stock.Remain}, used={quantity}");
            if (stock.Remain >= quantity)
            {
                // この在庫アクションで全量加工できる
                stock.Quantity += quantity;
                stock.Remain -= quantity;
                DbContext.StockActions.Update(stock);
                DbContext.SaveChanges();

                UpdateRemainAndDiscardQuantitiesOfThisStockLot(stock, quantity);

                // 在庫アクションの更新終了
                quantity = 0;
            }
            else // s.Remain < quantity
            {
                // この在庫アクションだけでは全量を加工できない
                var outOfStock = quantity - stock.Remain;
                var useFromThisLot = stock.Remain;

                stock.Quantity += useFromThisLot;
                stock.Remain = 0;
                DbContext.StockActions.Update(stock);

                UpdateRemainAndDiscardQuantitiesOfThisStockLot(stock, useFromThisLot);

                LogUtil.Debug($"OutOfStock:   date={stock.ActionDate.ToString("yyyyMMdd")}, part={stock.PartsCode}, lot={stock.StockLotNo}, remain={-outOfStock}, used={useFromThisLot}");
                // outOfStock 分を同日分で入荷予定日が一番若い在庫アクションから引く
                var nextStock = DbContext.StockActions
                    .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE)
                    .Where(a => 0 == a.ActionDate.CompareTo(stock.ActionDate))
                    .Where(a => a.Remain > 0)
                    .Where(a => a.StockLotNo != stock.StockLotNo)
                    .OrderBy(a => a.ArrivalDate)
                    .FirstOrDefault();

                if(nextStock == null)
                {
                    LogUtil.Debug($"NextStock is null: date={stock.ActionDate}, part={stock.PartsCode}");
                    // 在庫不足レコード追加
                    var outOfStockAction = new StockAction()
                    {
                        Action = StockActionType.OUT_OF_STOCK,
                        ActionDate = stock.ActionDate,
                        ArrivalDate = stock.ArrivalDate,
                        PartsCode = stock.PartsCode,
                        StockLotNo = stock.StockLotNo,
                        Quantity = outOfStock,
                        Remain = -outOfStock
                    };
                    DbContext.StockActions.Add(outOfStockAction);
                    DbContext.SaveChanges();
                    LogUtil.Debug($"OutOfStockAction : date={outOfStockAction.ActionDate.ToString("yyyyMMdd")}, quantity");
                }
                else
                {
                    // nextStock を対象に自分自身を再帰呼び出し
                    UseBouquetPartFromTheStockAction(nextStock, outOfStock);
                }
            }
        }

        private void UpdateRemainAndDiscardQuantitiesOfThisStockLot(StockAction stock, int quantity)
        {
            // 同一ロットの翌日以降の残数更新
            var daysAfter = DbContext.StockActions
                .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(a => a.StockLotNo == stock.StockLotNo)
                .Where(a => a.ActionDate > stock.ActionDate)
                .OrderBy(a => a.ActionDate)
                .ToList();
            foreach (var a in daysAfter)
            {
                if(a.Remain < quantity)
                {
                    // [TODO] 残数が足りなかったら不足分を別ロットから削除する
                    throw new NotImplementedException($"残数<必要数：ロット={stock.StockLotNo}, 基準日={stock.ActionDate.ToString("yyyyMMdd")}, 不足日={a.ActionDate.ToString("yyyyMMdd")},");
                }
                a.Remain -= quantity;
                DbContext.StockActions.Update(a);
                DbContext.SaveChanges();

            }

            // 同一ロットの破棄数更新
            var discarding = DbContext.StockActions
                .Where(a => a.Action == StockActionType.SCHEDULED_TO_DISCARD)
                .Where(a => a.StockLotNo == stock.StockLotNo)
                .Single();
            discarding.Quantity -= quantity;
            DbContext.StockActions.Update(discarding);
            DbContext.SaveChanges();
        }
        #endregion // UseBouquetPart

        public void CreatePartsList(string bouquet, string part, int quantity)
        {
            var item = new BouquetPartsList() { BouquetCode = bouquet, PartsCode = part, Quantity = quantity };
            DbContext.PartsList.Add(item);
            DbContext.SaveChanges();
        }
    }
}
