using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

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

        #region 単品の登録改廃
        /// <summary>
        /// 花コードをキーに単品オブジェクトを取得する
        /// </summary>
        /// <param name="partCode">花コード</param>
        /// <returns>単品オブジェクト。花コードに該当する単品がないときはnull。</returns>
        public BouquetPart FindBouquetPart(string partCode)
        {
            return DbContext.BouquetParts.SingleOrDefault(p => p.Code == partCode);
        }
        #endregion // 単品の登録改廃

        #region 商品の登録改廃
        /// <summary>
        /// 花束コードをキーに商品オブジェクトを取得する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <returns>商品オブジェクト。花束コードに該当する単品がないときは null。</returns>
        public Bouquet FindBouquet(string bouquetCode)
        {
            return DbContext.Bouquets.SingleOrDefault(b => b.Code == bouquetCode);
        }
        #endregion // 商品の登録改廃

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
            return UseBouquetPart(DbContext, part, date, quantity);
        }

        private int UseBouquetPart(MemorieDeFleursDbContext context, BouquetPart part, DateTime date, int quantity)
        {
            try
            {
                var stock = context.StockActions
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
                    var usedLot = new Stack<int>();
                    UseFromThisLot(DbContext, stock, quantity, usedLot);
                }

                context.SaveChanges();

                var remain = DbContext.StockActions
                    .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE || a.Action == StockActionType.OUT_OF_STOCK)
                    .Where(a => 0 == a.ActionDate.CompareTo(date))
                    .Sum(a => a.Remain);
                LogUtil.Info($"UseBouquetPart(part={part.Code}, date={date.ToString("yyyyMMdd")}, quantity={quantity}) Total remain={remain}");
                return remain;
            }
            catch (NotImplementedException ei)
            {
                LogUtil.Fatal($"★未実装★ {ei.Message}");
                throw;
            }
            catch (Exception e)
            {
                LogUtil.Error($"UseBouquetPart(part={part.Code}, date={date.ToString("yyyyMMdd")}, quantity={quantity}) failed. {e.GetType().Name}: {e.Message}");
                LogUtil.Error(e.StackTrace);
                throw;
            }
        }

        private void UseFromThisLot(MemorieDeFleursDbContext context, StockAction today, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"today={today.ToString("s")}, quantity={quantity}, usedLot={string.Join(",", usedLot)}");

            var theLot = context.StockActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.StockLotNo == today.StockLotNo)
                .Where(act => act.ActionDate >= today.ActionDate)
                .ToList();

            var outOfStockAction = theLot.FirstOrDefault(act => act.Action == StockActionType.OUT_OF_STOCK);
            if (outOfStockAction != null)
            {
                // 在庫不足があるロットの対応は未考慮
                throw new NotImplementedException($"在庫不足がある：{outOfStockAction.ToString("L")}");
            }

            if (today.Remain >= quantity)
            {
                // 全量引き出せる
                today.Quantity += quantity;
                today.Remain -= quantity;
                context.StockActions.Update(today);

            }
            else
            {
                // 残数分はこのロットから、それ以外は他のロットから引き出す
                var useFromThisLot = today.Remain;
                var useFromOtherLot = quantity - today.Remain;
                today.Quantity += useFromThisLot;
                today.Remain -= useFromThisLot;
                context.StockActions.Update(today);

                usedLot.Push(today.StockLotNo);
                UseFromOtherLot(context, today, useFromOtherLot, usedLot);
                usedLot.Pop();
            }

            var previousRemain = today.Remain;
            foreach (var action in theLot
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate > today.ActionDate)
                .OrderBy(act => act.ActionDate))
            {
                if(previousRemain >= action.Quantity)
                {
                    // 全量引き出せる
                    action.Remain = previousRemain - action.Quantity;
                    context.StockActions.Update(action);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    // 前日残の分はこのロットから、それ以外は他のロットから引き出す
                    var usedFromThisLot = previousRemain;
                    var useFromOtherLot = action.Quantity - previousRemain;

                    action.Quantity = usedFromThisLot;
                    action.Remain = 0;
                    context.StockActions.Update(action);

                    usedLot.Push(action.StockLotNo);
                    UseFromOtherLot(context, action, useFromOtherLot, usedLot);
                    previousRemain = 0;
                }
            }

            var discard = theLot.Single(act => act.Action == StockActionType.SCHEDULED_TO_DISCARD);
            discard.Quantity = previousRemain;
            context.StockActions.Update(discard);

            LogUtil.DEBUGLOG_EndMethod();
        }

        private void UseFromOtherLot(MemorieDeFleursDbContext context, StockAction stock, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"stock={stock.ToString("s")}, quantity={quantity}, usedLot={string.Join(",", usedLot)}");

            var usableLots = context.StockActions
                .Where(act => act.PartsCode == stock.PartsCode)
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate == stock.ActionDate)
                //.Where(act => !usedLot.Contains(act.StockLotNo)) // Linq式の中で Stack<>.Contains() などのメソッド呼び出しはできない
                .Where(act => act.Remain > 0)
                .ToList();

            var useToThisLot = quantity;
            var previousLot = stock;

            foreach(var action in usableLots.OrderBy(act => act.ArrivalDate))
            {
                // すでに引当対象としたロットは除外：Linq式で usableLots を生成するタイミングでは除外できなかったため。
                if(usedLot.Contains(action.StockLotNo)) { continue; }

                if(action.Remain >= useToThisLot)
                {
                    // このロットから全量引き出す
                    UseFromThisLot(context, action, quantity, usedLot);
                    return;
                }
                else
                {
                    // 残数分はこのロットから、引き出せなかった分は次のロットから引き出す
                    useToThisLot = action.Remain;
                    UseFromThisLot(context, action, action.Remain, usedLot);
                    previousLot = action;
                }
            }

            if(useToThisLot > 0)
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
                    Quantity = useToThisLot,
                    Remain = -useToThisLot
                };
                DbContext.StockActions.Add(outOfStockAction);
                DbContext.SaveChanges();
                LogUtil.Debug($"OutOfStockAction : date={outOfStockAction.ActionDate.ToString("yyyyMMdd")}, quantity");
            }

            LogUtil.DEBUGLOG_EndMethod();
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
