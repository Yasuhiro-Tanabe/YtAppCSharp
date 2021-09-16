using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 仕入先モデル：仕入先と、関連する情報の管理を行う。
    /// </summary>
    public class SupplierModel
    {
        private MemorieDeFleursModel Parent { get; set; }

        /// <summary>
        /// 次の未使用仕入先コード
        /// </summary>
        internal SequenceUtil.SequenceValueManager SEQ_SUPPLIERS { get { return Parent.Sequences.SEQ_SUPPLIERS; } }

        /// <summary>
        /// (パッケージ内限定)コンストラクタ
        /// 
        /// モデルのプロパティとして参照できるので、外部でこのオブジェクトを作成することは想定しない。
        /// </summary>
        /// <param name="parent"></param>
        internal SupplierModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
        }

        #region SupplierBuilder
        /// <summary>
        /// 仕入先オブジェクトの生成器
        /// 
        /// 仕入先の各プロパティは、フルーエントインタフェース形式で入力する。
        /// 必要なプロパティを入力後、Create() を実行することでオブジェクトが生成されDBに登録される。
        /// </summary>
        public class SupplierBuilder
        {
            private SupplierModel _model;
            private string _name;
            private string _address1;
            private string _address2;
            private string _tel;
            private string _fax;
            private string _email;

            internal static SupplierBuilder GetInstance(SupplierModel parent)
            {
                return new SupplierBuilder(parent);
            }

            private SupplierBuilder(SupplierModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 仕入先名称を登録/変更する
            /// </summary>
            /// <param name="name">仕入先名称</param>
            /// <returns>仕入先名称変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierBuilder NameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// 仕入先住所を登録/変更する
            /// 
            /// 仕入先住所1,2 を分けて入力する。
            /// </summary>
            /// <param name="address1">住所1</param>
            /// <param name="address2">住所2、省略可。省略したときは null が指定されたものと見なす。</param>
            /// <returns>住所変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierBuilder AddressIs(string address1, string address2 = null)
            {
                _address1 = address1;
                _address2 = address2;
                return this;
            }

            /// <summary>
            /// 仕入先電話番号を登録/変更する
            /// </summary>
            /// <param name="tel">電話番号</param>
            /// <returns>電話番号変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierBuilder PhoneNumberIs(string tel)
            {
                _tel = tel;
                return this;
            }

            /// <summary>
            /// 仕入先FAX番号を登録/変更する
            /// </summary>
            /// <param name="fax">FAX番号</param>
            /// <returns>FAX番号変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierBuilder FaxNumberIs(string fax)
            {
                _fax = fax;
                return this;
            }

            /// <summary>
            /// 仕入先e-メールアドレスを登録/変更する
            /// </summary>
            /// <param name="email">e-メールアドレス</param>
            /// <returns>e-メールアドレス変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierBuilder EmailIs(string email)
            {
                _email = email;
                return this;
            }

            /// <summary>
            /// 登録変更した内容で仕入先を登録する
            /// 
            /// 仕入先コードの採番はこのメソッド内で行われる。
            /// </summary>
            /// <returns>登録された仕入先オブジェクト</returns>
            public Supplier Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                {
                    var s = new Supplier()
                    {
                        Code = _model.SEQ_SUPPLIERS.Next(),
                        Name = _name,
                        Address1 = _address1,
                        Address2 = _address2,
                        Telephone = _tel,
                        Fax = _fax,
                        EmailAddress = _email
                    };

                    context.Suppliers.Add(s);
                    context.SaveChanges();

                    return s;
                }
            }
        }

        /// <summary>
        /// DB登録オブジェクト生成器を取得する
        /// </summary>
        /// <typeparam name="Sypplier">DB登録オブジェクト生成器が生成するオブジェクト：仕入先</typeparam>
        /// <returns>仕入先オブジェクト生成器</returns>
        public SupplierBuilder GetSupplierBuilder()
        {
            return SupplierBuilder.GetInstance(this);
        }
        #endregion // SupplierBuilder

        #region Supplier の生成・更新・削除
        /// <summary>
        /// 仕入先コードをキーに仕入先オブジェクトを取得する
        /// </summary>
        /// <param name="supplierCode">仕入先コード</param>
        /// <returns>仕入先オブジェクト、仕入先コードに該当する仕入先が存在しないときはnull。</returns>
        public Supplier Find(int supplierCode)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return Find(context, supplierCode);
            }
        }
        private Supplier Find(MemorieDeFleursDbContext context, int supplierCode)
        {
            return context.Suppliers.SingleOrDefault(s => s.Code == supplierCode);
        }
        #endregion // Supplier の生成・更新・削除

        #region 発注
        /// <summary>
        /// 発注時に登録する在庫アクションの共通パラメータ
        /// </summary>
        private class StockActionParameterToOrder
        {
            /// <summary>
            /// 到着予定日
            /// </summary>
            public DateTime ArrivalDate { get; private set; }

            /// <summary>
            /// 花コード
            /// </summary>
            public string PartsCode { get; private set; }

            /// <summary>
            /// 在庫ロット番号
            /// </summary>
            public int StockActionLotNo { get; private set; }

            /// <summary>
            /// 数量[本]：初期登録時は入荷時の数量を全量破棄する
            /// </summary>
            public int Quantity { get; private set; }

            /// <summary>
            /// 品質維持可能日数[日]：加工予定および破棄予定の在庫アクションで日付計算のために使用する。
            /// </summary>
            public int DaysToExpire { get; private set; }

            public StockActionParameterToOrder(DateTime arrival, BouquetPart part, int lotNo, int quantityOfLot)
            {
                ArrivalDate = arrival;
                PartsCode = part.Code;
                StockActionLotNo = lotNo;
                Quantity = quantityOfLot * part.QuantitiesPerLot;
                DaysToExpire = part.ExpiryDate;
            }
        }

        /// <summary>
        /// (試作) 注文処理に伴う在庫アクション登録
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="part">単品</param>
        /// <param name="quantityOfLot">注文ロット数</param>
        /// <param name="arrivalDate">納品予定日</param>
        /// <returns>発注ロット番号(＝在庫ロット番号)</returns>
        public int Order(DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return Order(context, orderDate, part, quantityOfLot, arrivalDate);
            }
        }

        public int Order(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod(new StringBuilder()
                .AppendFormat("order={0:yyyyMMdd}", orderDate)
                .Append(", part=").Append(part.Code)
                .AppendFormat(", quantity={0}[lot(s)]({1}[parts])", quantityOfLot, quantityOfLot * part.QuantitiesPerLot)
                .AppendFormat(", arrival={0:yyyyMMdd}", arrivalDate)
                .ToString());

            // [TODO] 発注ロット番号=在庫ロット番号は発注時に採番する。
            var lotNo = SEQ_SUPPLIERS.Next(context);
            var quantity = quantityOfLot * part.QuantitiesPerLot;

            var param = new StockActionParameterToOrder(arrivalDate, part, lotNo, quantityOfLot);

            // 追加発注分の在庫アクションを登録する
            AddScheduledToArriveStockAction(context, param);
            AddScheduledToDiscardStockAction(context, param);
            AddScheduledToUseStockAction(context, param);
            context.SaveChanges();

            // 当日以降の入荷予定分と在庫不足分をこのロットに振り替える
            foreach (var currentOrder in context.StockActions
                .Where(act => act.StockLotNo == lotNo)
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .OrderBy(act => act.ActionDate))
            {
                // 在庫不足の解消: ループイテレータを都度 DBContext から削除するので、ループ対象のコピーを取る。
                foreach (var outOfStock in context.StockActions
                    .Where(act => act.PartsCode == currentOrder.PartsCode)
                    .Where(act => act.Action == StockActionType.OUT_OF_STOCK)
                    .Where(act => act.ActionDate == currentOrder.ActionDate)
                    .OrderBy(act => act.ArrivalDate).ToList())
                {
                    context.StockActions.Remove(outOfStock);
                    context.SaveChanges();

                    DEBUGLOG_ComparationOfStockRemainAndQuantity(currentOrder, outOfStock.Quantity);
                    if (currentOrder.Remain >= outOfStock.Quantity)
                    {
                        // 不足分全量をこの在庫ロットから払い出す
                        AddQuantityToStockLot(context, part, lotNo, outOfStock.ActionDate, outOfStock.Quantity);
                    }
                    else
                    {
                        throw new NotImplementedException(new StringBuilder()
                            .Append("在庫不足：発注量では在庫不足を賄えない:")
                            .Append(" 品目=").Append(part.Code)
                            .Append(", 発注ロット：").Append(lotNo)
                            .AppendFormat(", 発注日={0:yyyyMMdd}", orderDate)
                            .AppendFormat(", 不足日={0:yyyyMMdd", currentOrder.ActionDate)
                            .Append(", 不足数=").Append(outOfStock.Quantity)
                            .Append(", 当日残=").Append(currentOrder.Remain)
                            .ToString());
                    }
                    LogUtil.Info($"Out of Stock was resolved. Date={outOfStock.ActionDate.ToString("yyyyMMdd")}, lot={outOfStock.StockLotNo}, lacked={outOfStock.Quantity}");
                }

                // 納品予定が翌日以降の在庫ロットからの振り替え
                foreach (var usedFromOthers in context.StockActions
                        .Where(act => act.PartsCode == currentOrder.PartsCode)
                        .Where(act => act.ActionDate == currentOrder.ActionDate)
                        .Where(act => act.ArrivalDate > currentOrder.ArrivalDate)
                        .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                        .Where(act => act.Quantity > 0)
                        .OrderBy(act => act.ArrivalDate))
                {
                    DEBUGLOG_ComparationOfStockRemainAndQuantity(currentOrder, usedFromOthers.Quantity);
                    if (currentOrder.Remain >= usedFromOthers.Quantity)
                    {
                        // 全量をこの在庫ロットから払い出す
                        AddQuantityToStockLot(context, part, lotNo, usedFromOthers.ActionDate, usedFromOthers.Quantity);
                        AddQuantityToStockLot(context, part, usedFromOthers.StockLotNo, usedFromOthers.ActionDate, -usedFromOthers.Quantity);
                    }
                    else
                    {
                        var outOfStock = usedFromOthers.Quantity - currentOrder.Remain;

                        // 再振替のため、振替元の加工数を一旦ゼロに戻す
                        AddQuantityToStockLot(context, part, usedFromOthers.StockLotNo, usedFromOthers.ActionDate, -usedFromOthers.Quantity);

                        // 振替元で加工していた分をこのロット＋他ロットで再度振替なおす
                        AddQuantityToStockLot(context, part, lotNo, usedFromOthers.ActionDate, currentOrder.Remain);
                        var outOfStockAction = TransferToOtherLot(context, part, currentOrder.ActionDate, usedFromOthers.ArrivalDate, outOfStock);

                        if (outOfStockAction.Quantity > 0)
                        {
                            throw new NotImplementedException(new StringBuilder()
                                .Append("在庫不足：発注量では既存在庫ロットを全量振替できない:")
                                .Append(" 品目=").Append(part.Code)
                                .Append(", 発注ロット：").Append(lotNo)
                                .AppendFormat(", 発注日={0:yyyyMMdd}", orderDate)
                                .AppendFormat(", 不足日={0:yyyyMMdd}", currentOrder.ActionDate)
                                .Append("最終振替対象ロット").Append(outOfStockAction.StockLotNo)
                                .Append(", 振替残数=").Append(outOfStock)
                                .ToString());
                        }
                    }

                }
            }

            LogUtil.Info($"{orderDate.ToString("yyyyMMdd")}: {part.Code} x {quantityOfLot}[Lot(s)] ordered. arrive at {arrivalDate.ToString("yyyyMMdd")}, OrderLot#={lotNo}.");
            LogUtil.DEBUGLOG_EndMethod($"{part.Code}, {arrivalDate.ToString("yyyyMMdd")}", $"Lot#={lotNo}");
            return lotNo;
        }

        private void AddScheduledToUseStockAction(MemorieDeFleursDbContext context, StockActionParameterToOrder param)
        {
            // 品質維持可能日数＋1日分 (入荷日+0日目、入荷日+1日目、…、入荷日+品質維持可能日数日目)を生成
            foreach (var d in Enumerable.Range(0, param.DaysToExpire + 1).Select(i => param.ArrivalDate.AddDays(i)))
            {
                var toUse = new StockAction()
                {
                    ActionDate = d,
                    Action = StockActionType.SCHEDULED_TO_USE,
                    PartsCode = param.PartsCode,
                    StockLotNo = param.StockActionLotNo,
                    ArrivalDate = param.ArrivalDate,
                    Quantity = 0,
                    Remain = param.Quantity
                };
                context.StockActions.Add(toUse);
                DEBUGLOG_StockActionCreated(toUse);
            }
        }

        private void AddScheduledToDiscardStockAction(MemorieDeFleursDbContext context, StockActionParameterToOrder param)
        {
            var discard = new StockAction()
            {
                ActionDate = param.ArrivalDate.AddDays(param.DaysToExpire),
                Action = StockActionType.SCHEDULED_TO_DISCARD,
                PartsCode = param.PartsCode,
                StockLotNo = param.StockActionLotNo,
                ArrivalDate = param.ArrivalDate,
                Quantity = param.Quantity,
                Remain = 0
            };
            context.StockActions.Add(discard);
            DEBUGLOG_StockActionCreated(discard);
        }

        private void AddScheduledToArriveStockAction(MemorieDeFleursDbContext context, StockActionParameterToOrder param)
        {
            var arrive = new StockAction()
            {
                ActionDate = param.ArrivalDate,
                Action = StockActionType.SCHEDULED_TO_ARRIVE,
                PartsCode = param.PartsCode,
                StockLotNo = param.StockActionLotNo,
                ArrivalDate = param.ArrivalDate,
                Quantity = param.Quantity,
                Remain = param.Quantity
            };
            context.StockActions.Add(arrive);
            DEBUGLOG_StockActionCreated(arrive);
        }
#endregion // 発注

        #region 発注取消
        public void CancelOrder(int lotNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                CancelOrder(context, lotNo);
            }
        }

        public void CancelOrder(MemorieDeFleursDbContext context, int lotNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"lotNo={lotNo}");
            var lot = context.StockActions.Where(a => a.StockLotNo == lotNo);
            if (lot.Count() == 0)
            {
                LogUtil.Warn($"CancelOrder: Lot {lotNo} was not found in stock actions.");
                return;
            }

            // コピーを取ってコピー元(データベースの中身)を削除
            var theLot = lot.ToList();
            context.StockActions.RemoveRange(theLot);
            context.SaveChanges();

            var partCode = theLot.First().PartsCode;
            var part = Parent.BouquetModel.FindBouquetPart(partCode);
            if (part == null)
            {
                throw new NotImplementedException($"単品 {partCode} が見つからない： Lot No. {lotNo}");
            }

            foreach (var action in theLot
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(act => act.Quantity > 0)
                .OrderBy(act => act.ActionDate))
            {
                // このロットで払い出されている加工数量を他のロットに移動する
                var outOfStock = TransferToOtherLot(context, part, action.ActionDate, action.ArrivalDate, action.Quantity);
                if (outOfStock.Quantity > 0)
                {
                    LogUtil.Warn($"Out of stock : {part.Code}, {action.ActionDate.ToString("yyyyMMdd")}, Lacked={outOfStock} at lot {action.StockLotNo}");
                    var outOfStockLot = context.StockActions
                        .Where(act => act.StockLotNo == outOfStock.StockLotNo)
                        .Single(act => act.ActionDate == action.ActionDate);

                    // 在庫不足アクションの生成
                    var lacked = new StockAction()
                    {
                        Action = StockActionType.OUT_OF_STOCK,
                        ActionDate = outOfStockLot.ActionDate,
                        BouquetPart = outOfStockLot.BouquetPart,
                        StockLotNo = outOfStockLot.StockLotNo,
                        ArrivalDate = outOfStockLot.ArrivalDate,
                        PartsCode = outOfStockLot.PartsCode,
                        Quantity = outOfStock.Quantity,
                        Remain = -outOfStock.Quantity
                    };
                    context.StockActions.Add(lacked);
                    context.SaveChanges();
                }
            }

            LogUtil.Info($"Lot {lotNo} removed.");
            LogUtil.DEBUGLOG_EndMethod($"lotNo={lotNo}");
        }
        #endregion // 発注取消

        #region 払い出し予定の振替
        private class OutOfStock
        {
            public int StockLotNo { get; set; }
            public int Quantity { get; set; }
        }
        /// <summary>
        /// ロット方向に払出予定を展開する：入荷日 arrivalDate 以降に納品されたロットの、
        /// 基準日 actionDate の加工予定在庫アクションを入荷日別に並べ、順次残数の許す限り quantity を引く
        /// </summary>
        /// <param name="part">展開対象の単品</param>
        /// <param name="actionDate">基準日</param>
        /// <param name="arrivalDate">入荷日</param>
        /// <param name="quantity">振替数量</param>
        private OutOfStock TransferToOtherLot(MemorieDeFleursDbContext context, BouquetPart part, DateTime actionDate, DateTime arrivalDate, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod(string.Format("part={0}, date={1:yyyyMMdd}, arrived={2:yyyyMMdd}, quantity={3}", part.Code, actionDate, arrivalDate, quantity));

            var outOfStock = new OutOfStock() { StockLotNo = 0, Quantity = quantity };

            // 基準日が等しい加工予定在庫アクションのうち、残数＞0のロットの加工予定在庫アクションに振り替える
            foreach (var action in context.StockActions
                .Where(a => a.PartsCode == part.Code)
                .Where(a => a.ActionDate == actionDate)
                .Where(a => a.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(a => a.ArrivalDate >= arrivalDate)
                .Where(a => a.Remain > 0)
                .OrderBy(a => a.ArrivalDate))
            {
                DEBUGLOG_ComparationOfStockRemainAndQuantity(action, outOfStock.Quantity);
                if(action.Remain >= outOfStock.Quantity)
                {
                    // 全量をこの在庫アクションから払い出す
                    AddQuantityToStockLot(context, part, action.StockLotNo, action.ActionDate, outOfStock.Quantity);
                    outOfStock.Quantity = 0;
                    break;
                }
                else
                {
                    // 払い出せる分だけこの在庫アクションから払い出す
                    var usedFromThisStock = action.Remain;
                    AddQuantityToStockLot(context, part, action.StockLotNo, action.ActionDate, usedFromThisStock);

                    outOfStock.Quantity -= usedFromThisStock;
                    outOfStock.StockLotNo = action.StockLotNo;

                    LogUtil.Debug($"{LogUtil.Indent}outOfStock: {outOfStock.Quantity + usedFromThisStock}->{outOfStock.Quantity}");
                }
            }

            LogUtil.DEBUGLOG_EndMethod(msg: $"lacked={outOfStock}, at {actionDate.ToString("yyyyMMdd")}");
            return outOfStock;
        }

        /// <summary>
        /// 日付方向に払出予定を展開する：同一ロットのactionDate、翌日、その翌日…と破棄予定日までの残数から quantity を引く
        /// </summary>
        /// <param name="part">払出対象の単品</param>
        /// <param name="lotNo">払出を行うロット番号</param>
        /// <param name="actionDate">基準日</param>
        /// <param name="quantity">払出数量</param>
        private void AddQuantityToStockLot(MemorieDeFleursDbContext context, BouquetPart part, int lotNo, DateTime actionDate, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod(args: $"part={part.Code}, lot={lotNo}, date={actionDate.ToString("yyyyMMdd")}, quantity={quantity}");

            // ロット lotNo の、actionDate 以降の加工予定アクションから quantity 本の単品を払い出す
            var theLotStocks = context.StockActions
                .Where(act => act.PartsCode == part.Code)
                .Where(act => act.StockLotNo == lotNo);
            var usedFromTheLot = quantity;

            // actionDate 当日分：加工数に quantity を加算する
            var toUse
                = theLotStocks
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Single(act => act.ActionDate == actionDate);
            DEBUGLOG_ComparationOfStockRemainAndQuantity(toUse, usedFromTheLot);
            if (toUse.Remain >= usedFromTheLot)
            {
                // 全量払出する
                LogUtil.DEBUGLOG_StockActionQuantityChanged(toUse, toUse.Quantity + usedFromTheLot, toUse.Quantity - usedFromTheLot);
                toUse.Quantity += usedFromTheLot;
                toUse.Remain -= usedFromTheLot;
                context.StockActions.Update(toUse);
            }
            else
            {
                throw new NotImplementedException(new StringBuilder()
                    .Append("在庫払い出しできない：残数が要求された使用量より小さい. ")
                    .AppendFormat(" 払出開始日：{0:yyyyMMdd}", actionDate)
                    .AppendFormat(" 不足発生日={0:yyyyMMdd}", toUse.ActionDate)
                    .Append(", 花コード=").Append(toUse.PartsCode)
                    .Append(", ロット番号").Append(toUse.StockLotNo)
                    .AppendFormat(", 納品日={0:yyyyMMdd}", toUse.ArrivalDate)
                    .Append(", 要求数量=").Append(usedFromTheLot)
                    .Append(", 在庫数量=").Append(toUse.Remain)
                    .ToString());
            }

            var previousRemain = toUse.Remain;

            // actionDate 翌日以降：加工数は変更せず残数から usedFromTheLot を引く
            foreach (var action in theLotStocks
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate > actionDate)
                .OrderBy(act => act.ActionDate))
            {
                DEBUGLOG_ComparisonOfStockQuantityAndPreviousRemain(action, 0);
                if(action.Quantity <= previousRemain)
                {
                    // このロットから全量払い出せる
                    LogUtil.DEBUGLOG_StockActionQuantityChanged(action, action.Quantity, previousRemain - action.Quantity);
                    action.Remain = previousRemain - action.Quantity;
                    context.StockActions.Update(action);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    // このロットだけでは払い出せない：前日残分はこのロットから、残りは別のロットから払い出す
                    LogUtil.DEBUGLOG_StockActionQuantityChanged(action, previousRemain, 0);
                    var outOfStockQuantity = action.Quantity - previousRemain;
                    action.Quantity = previousRemain;
                    action.Remain = 0;
                    context.StockActions.Update(action);

                    var outOfStock = TransferToOtherLot(context, part, action.ActionDate, action.ArrivalDate, outOfStockQuantity);
                    if(outOfStock.Quantity > 0)
                    {
                        throw new NotImplementedException(new StringBuilder()
                            .Append("在庫振替不可：当日分在庫不足")
                            .AppendFormat(" 払出開始日：{0:yyyyMMdd}", actionDate)
                            .AppendFormat(" 不足発生日={0:yyyyMMdd}", action.ActionDate)
                            .Append(", 花コード=").Append(action.PartsCode)
                            .Append(", ロット番号").Append(action.StockLotNo)
                            .AppendFormat(", 納品日={0:yyyyMMdd}", action.ArrivalDate)
                            .Append(", 要求数量=").Append(usedFromTheLot)
                            .Append(", 在庫数量=").Append(action.Remain)
                            .ToString());
                    }
                    previousRemain = 0;
                }
            }

            // 破棄アクションの更新
            var toDiscard = theLotStocks.Single(act => act.Action == StockActionType.SCHEDULED_TO_DISCARD);
            LogUtil.DEBUGLOG_StockActionQuantityChanged(toDiscard, previousRemain, 0);
            toDiscard.Quantity = previousRemain;
            context.StockActions.Update(toDiscard);

            context.SaveChanges();
            LogUtil.DEBUGLOG_EndMethod();
        }
#endregion // 払い出し予定の振替


#if DEBUG
#region デバッグ用

        /// <summary>
        /// 生成/登録された在庫アクションをデバッグログ出力する
        /// </summary>
        /// <param name="action">出力対象在庫アクション</param>
        /// <param name="caller">このメソッドの呼び出し元：通常は指定不要。直接の呼び出し元ではなく、さらにその呼び出し元をログに残したいとき指定する</param>
        /// <param name="line">このメソッドの呼び出し位置：ソースファイル中の行番号。calledFrom と同様通常は指定不要、呼び出し元の呼び出し元をログに残したいときのみ指定する</param>
        [Conditional("DEBUG")]
        private void DEBUGLOG_StockActionCreated(StockAction action, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.Debug($"{LogUtil.Indent}Created: {action.ToString("L")}", caller, path, line);
        }

        [Conditional("DEBUG")]
        private void DEBUGLOG_ComparationOfStockRemainAndQuantity(StockAction action, int quantity, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Remain >= quantity ? ">=" : "<";

            LogUtil.Debug($"{LogUtil.Indent}Compare: {action.ToString("h")}.Remain({action.Remain}) {operatorString} {quantity}", caller, path, line);

        }

        [Conditional("DEBUG")]
        private void DEBUGLOG_ComparisonOfStockQuantityAndPreviousRemain(StockAction action, int remain, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Quantity <= remain ? "<=" : ">";

            LogUtil.Debug($"{LogUtil.Indent}Compare: {action.ToString("h")}.Quantity({action.Quantity}) {operatorString} {remain}", caller, path, line);
        }
#endregion // デバッグ用
#endif
    }
}
