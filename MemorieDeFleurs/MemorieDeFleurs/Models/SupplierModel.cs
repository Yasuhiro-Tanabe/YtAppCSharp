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
        private MemorieDeFleursDbContext DbContext { get; set; }
        private MemorieDeFleursModel Parent { get; set; }

        private SequenceUtil Sequences { get { return Parent.Sequences; } }

        /// <summary>
        /// (パッケージ内限定)コンストラクタ
        /// 
        /// モデルのプロパティとして参照できるので、外部でこのオブジェクトを作成することは想定しない。
        /// </summary>
        /// <param name="parent"></param>
        internal SupplierModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
            DbContext = parent.DbContext;
        }

        /// <summary>
        /// 仕入先オブジェクトの生成器
        /// 
        /// 仕入先の各プロパティは、フルーエントインタフェース形式で入力する。
        /// 必要なプロパティを入力後、Create() を実行することでオブジェクトが生成されDBに登録される。
        /// </summary>
        public class SupplierProcesser
        {
            private SupplierModel _model;
            private string _name;
            private string _address1;
            private string _address2;
            private string _tel;
            private string _fax;
            private string _email;

            internal static SupplierProcesser GetInstance(SupplierModel parent)
            {
                return new SupplierProcesser(parent);
            }

            private SupplierProcesser(SupplierModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 仕入先名称を登録/変更する
            /// </summary>
            /// <param name="name">仕入先名称</param>
            /// <returns>仕入先名称変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierProcesser NameIs(string name)
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
            public SupplierProcesser AddressIs(string address1, string address2 = null)
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
            public SupplierProcesser PhoneNumberIs(string tel)
            {
                _tel = tel;
                return this;
            }

            /// <summary>
            /// 仕入先FAX番号を登録/変更する
            /// </summary>
            /// <param name="fax">FAX番号</param>
            /// <returns>FAX番号変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierProcesser FaxNumberIs(string fax)
            {
                _fax = fax;
                return this;
            }

            /// <summary>
            /// 仕入先e-メールアドレスを登録/変更する
            /// </summary>
            /// <param name="email">e-メールアドレス</param>
            /// <returns>e-メールアドレス変更後の仕入先オブジェクト生成器(自分自身)</returns>
            public SupplierProcesser EmailIs(string email)
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
                var s = new Supplier()
                {
                    Code = _model.NextSequenceCode,
                    Name = _name,
                    Address1 = _address1,
                    Address2 = _address2,
                    Telephone = _tel,
                    Fax = _fax,
                    EmailAddress = _email
                };

                _model.DbContext.Suppliers.Add(s);
                _model.DbContext.SaveChanges();
                return s;
            }
        }

        /// <summary>
        /// 仕入先コードの採番：次の未使用コードを返す。
        /// </summary>
        internal int NextSequenceCode { get { return Sequences.SEQ_SUPPLIERS.Next; } }


        /// <summary>
        /// DB登録オブジェクト生成器を取得する
        /// </summary>
        /// <typeparam name="Sypplier">DB登録オブジェクト生成器が生成するオブジェクト：仕入先</typeparam>
        /// <returns>仕入先オブジェクト生成器</returns>
        public SupplierProcesser Entity<Sypplier>()
        {
            return SupplierProcesser.GetInstance(this);
        }

        #region Supplier の生成・更新・削除
        /// <summary>
        /// 条件を満たす仕入先オブジェクトを取得する
        /// </summary>
        /// <param name="condition">取得条件</param>
        /// <returns>仕入先オブジェクトの列挙(<see cref="IEnumerable{Supplier}()"/>)。
        /// 条件を満たす仕入先が1つしかない場合でも列挙として返すので注意。</returns>
        public IEnumerable<Supplier> Find(Func<Supplier,bool> condition)
        {
            return DbContext.Suppliers.Where(condition);
        }

        /// <summary>
        /// 仕入先コードをキーに仕入先オブジェクトを取得する
        /// </summary>
        /// <param name="supplierCode">仕入先コード</param>
        /// <returns>仕入先オブジェクト、仕入先コードに該当する仕入先が存在しないときはnull。</returns>
        public Supplier Find(int supplierCode)
        {
            return DbContext.Suppliers.SingleOrDefault(s => s.Code == supplierCode);
        }

        /// <summary>
        /// 更新した仕入先オブジェクトでデータベースを更新する
        /// </summary>
        /// <param name="s">仕入先オブジェクト。nullの場合は更新処理を行わずに処理を抜ける。</param>
        /// <remarks>呼び出し前に仕入先コードを書き換えないこと。意図しない仕入先の内容が変更されることがある。</remarks>
        public void Replace(Supplier s)
        {
            if(s == null) { return; }
            DbContext.Suppliers.Update(s);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 仕入先オブジェクトをデータベースから削除する
        /// </summary>
        /// <param name="s">仕入先オブジェクト。nullの場合は削除処理を行わずに処理を抜ける。</param>
        /// <remarks>呼び出し前に仕入先コードを書き換えないこと。意図しない仕入先が削除されることがある。</remarks>
        public void Remove(Supplier s)
        {
            if(s == null) { return; }
            DbContext.Suppliers.Remove(s);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 仕入先コード指定で仕入先をデータベースから削除する。
        /// 該当する仕入先が見つからないときは何もしない。
        /// </summary>
        /// <param name="supplierCode">仕入先コード</param>
        /// <remarks>内部で <see cref="Find(int)"/> を呼び出し、
        /// 見つかった仕入先オブジェクトを <see cref="Remove(Supplier)"/> で破棄する。</remarks>
        public void Remove(int supplierCode)
        {
            var found = DbContext.Suppliers.SingleOrDefault(s => s.Code == supplierCode);
            Remove(found);
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
            DEBUGLOG_BeginMethod(new StringBuilder()
                .AppendFormat("order={0:yyyyMMdd}", orderDate)
                .Append(", part=").Append(part.Code)
                .AppendFormat(", quantity={0}[lot(s)]({1}[parts])", quantityOfLot, quantityOfLot * part.QuantitiesPerLot)
                .AppendFormat(", arrival={0:yyyyMMdd}", arrivalDate)
                .ToString());

            // [TODO] 発注ロット番号=在庫ロット番号は発注時に採番する。
            var lotNo = Sequences.SEQ_STOCK_LOT_NUMBER.Next;
            var quantity = quantityOfLot * part.QuantitiesPerLot;

            var param = new StockActionParameterToOrder(arrivalDate, part, lotNo, quantityOfLot);

            // 追加発注分の在庫アクションを登録する
            AddScheduledToArriveStockAction(param);
            AddScheduledToDiscardStockAction(param);
            AddScheduledToUseStockAction(param);
            DbContext.SaveChanges();

            // 解消できる在庫不足アクションを解消する：
            // 見つけた StockAction を削除しながら進めるので、ループを回す用のリストは DbContext 抽出内容のコピーを使う
            foreach(var outOfStock in DbContext.StockActions
                .Where(act => act.Action == StockActionType.OUT_OF_STOCK)
                .Where(act => act.ActionDate >= arrivalDate)
                .Where(act => act.ActionDate <= arrivalDate.AddDays(part.ExpiryDate))
                .OrderBy(act => act.ActionDate)
                .ToList())
            {
                var action = DbContext.StockActions
                    .Where(act => act.StockLotNo == lotNo)
                    .Where(act => act.ActionDate == outOfStock.ActionDate)
                    .Single(act => act.Action == StockActionType.SCHEDULED_TO_USE);

                DEBUGLOG_ComparationOfStockRemainAndQuantity(action, outOfStock.Quantity);
                if(action.Remain >= outOfStock.Quantity)
                {
                    // 不足分全量をこの在庫ロットから払い出す
                    AddQuantityToStockLot(part, lotNo, outOfStock.ActionDate, outOfStock.Quantity);
                }
                else
                {
                    throw new NotImplementedException(new StringBuilder()
                        .Append("在庫不足：発注量では在庫不足を賄えない:")
                        .Append(" 品目=").Append(part.Code)
                        .Append(", 発注ロット：").Append(lotNo)
                        .AppendFormat(", 発注日={0:yyyyMMdd}", orderDate)
                        .AppendFormat(", 不足日={0:yyyyMMdd", action.ActionDate)
                        .Append(", 不足数=").Append(outOfStock.Quantity)
                        .Append(", 当日残=").Append(action.Remain)
                        .ToString());
                }

                DbContext.StockActions.Remove(outOfStock);
            }
            DbContext.SaveChanges();

            // arrivalDate 以降の既存納品予定から払い出していた加工分を今回発注分に振替する
            foreach (var d in Enumerable.Range(0, part.ExpiryDate + 1).Select(i => arrivalDate.AddDays(i)))
            {
                var stocks = DbContext.StockActions
                    .Where(act => act.PartsCode == part.Code)
                    .Where(act => act.ActionDate == d)
                    .Where(act => act.ArrivalDate > arrivalDate)
                    .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                    .Where(act => act.Quantity > 0);
                
                if(stocks.Count() == 0) { continue; }

                var sum = stocks.Sum(act => act.Quantity);
                if (sum <= quantity)
                {
                    // 今回発注分ですべて賄える
                    AddQuantityToStockLot(part, lotNo, d, sum);
                    foreach (var stock in stocks)
                    {
                        // 加工分を在庫に戻す
                        var back = stock.Quantity;
                        AddQuantityToStockLot(part, stock.StockLotNo, d, -back);
                    }
                }
                else
                {
                    throw new NotImplementedException(new StringBuilder()
                        .Append("今回発注分では賄えない： ")
                        .AppendFormat("発注日={0:yyyyMMdd", orderDate)
                        .AppendFormat(", 到着予定日={0:yyyyMMdd}", arrivalDate)
                        .AppendFormat(", 在庫不足発生日={0:yyyyMMDD}", d)
                        .Append(", 数量=").Append(quantity)
                        .Append(", 当日分残数合計=").Append(sum)
                        .ToString());
                }
            }

            LogUtil.Info($"{orderDate.ToString("yyyyMMdd")}: {part.Code} x {quantityOfLot}[Lot(s)] ordered. arrive at {arrivalDate.ToString("yyyyMMdd")}, OrderLot#={lotNo}.");
            DEBUGLOG_EndMethod($"{part.Code}, {arrivalDate.ToString("yyyyMMdd")}", $"Lot#={lotNo}");
            return lotNo;
        }

        private void AddScheduledToUseStockAction(StockActionParameterToOrder param)
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
                DbContext.StockActions.Add(toUse);
                DEBUGLOG_StockActionCreated(toUse);
            }
        }

        private void AddScheduledToDiscardStockAction(StockActionParameterToOrder param)
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
            DbContext.StockActions.Add(discard);
            DEBUGLOG_StockActionCreated(discard);
        }

        private void AddScheduledToArriveStockAction(StockActionParameterToOrder param)
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
            DbContext.StockActions.Add(arrive);
            DEBUGLOG_StockActionCreated(arrive);
        }
        #endregion // 発注

        #region 発注取消
        public void CancelOrder(int lotNo)
        {
            DEBUGLOG_BeginMethod($"lotNo={lotNo}");

            var lot = DbContext.StockActions.Where(a => a.StockLotNo == lotNo);
            if (lot.Count() == 0)
            {
                LogUtil.Warn($"CancelOrder: Lot {lotNo} was not found in stock actions.");
                return;
            }

            // コピーを取ってコピー元(データベースの中身)を削除
            var theLot = lot.ToList();
            DbContext.StockActions.RemoveRange(theLot);
            DbContext.SaveChanges();

            var partCode = theLot.First().PartsCode;
            var part = Parent.BouquetModel.Find(partCode);
            if(part == null)
            {
                throw new NotImplementedException($"単品 {partCode} が見つからない： Lot No. {lotNo}");
            }

            foreach(var action in theLot
                .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                .Where(act => act.Quantity > 0)
                .OrderBy(act => act.ActionDate))
            {
                // このロットで払い出されている加工数量を他のロットに移動する
                var outOfStock = TransferToOtherLot(part, action.ActionDate, action.ArrivalDate, action.Quantity);
                if(outOfStock.Quantity > 0)
                {
                    LogUtil.Warn($"Out of stock : {part.Code}, {action.ActionDate.ToString("yyyyMMdd")}, Lacked={outOfStock} at lot {action.StockLotNo}");
                    var outOfStockLot = DbContext.StockActions
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
                    DbContext.StockActions.Add(lacked);
                    DbContext.SaveChanges();
                }
            }

            LogUtil.Info($"Lot {lotNo} removed.");
            DEBUGLOG_EndMethod($"lotNo={lotNo}");
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
        private OutOfStock TransferToOtherLot(BouquetPart part, DateTime actionDate, DateTime arrivalDate, int quantity)
        {
            DEBUGLOG_BeginMethod(string.Format("part={0}, date={1:yyyyMMdd}, arrived={2:yyyyMMdd}, quantity={3}", part.Code, actionDate, arrivalDate, quantity));

            var outOfStock = new OutOfStock() { StockLotNo = 0, Quantity = quantity };

            // 基準日が等しい加工予定在庫アクションのうち、残数＞0のロットの加工予定在庫アクションに振り替える
            foreach (var action in DbContext.StockActions
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
                    AddQuantityToStockLot(part, action.StockLotNo, action.ActionDate, outOfStock.Quantity);
                    outOfStock.Quantity = 0;
                    break;
                }
                else
                {
                    // 払い出せる分だけこの在庫アクションから払い出す
                    var usedFromThisStock = action.Remain;
                    AddQuantityToStockLot(part, action.StockLotNo, action.ActionDate, usedFromThisStock);

                    outOfStock.Quantity -= usedFromThisStock;
                    outOfStock.StockLotNo = action.StockLotNo;

                    LogUtil.Debug($"{Indent}outOfStock: {outOfStock.Quantity + usedFromThisStock}->{outOfStock.Quantity}");
                }
            }

            DEBUGLOG_EndMethod(msg: $"lacked={outOfStock}, at {actionDate.ToString("yyyyMMdd")}");
            return outOfStock;
        }

        /// <summary>
        /// 日付方向に払出予定を展開する：同一ロットのactionDate、翌日、その翌日…と破棄予定日までの残数から quantity を引く
        /// </summary>
        /// <param name="part">払出対象の単品</param>
        /// <param name="lotNo">払出を行うロット番号</param>
        /// <param name="actionDate">基準日</param>
        /// <param name="quantity">払出数量</param>
        private void AddQuantityToStockLot(BouquetPart part, int lotNo, DateTime actionDate, int quantity)
        {
            DEBUGLOG_BeginMethod(args: $"part={part.Code}, lot={lotNo}, date={actionDate.ToString("yyyyMMdd")}, quantity={quantity}");

            // ロット lotNo の、actionDate 以降の加工予定アクションから quantity 本の単品を払い出す
            var theLotStocks = DbContext.StockActions
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
                DEBUGLOG_StockActionQuantityChanged(toUse, usedFromTheLot, -usedFromTheLot);
                toUse.Quantity += usedFromTheLot;
                toUse.Remain -= usedFromTheLot;
                DbContext.StockActions.Update(toUse);
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
                    DEBUGLOG_StockActionQuantityChangedDirectly(action, action.Quantity, previousRemain - action.Quantity);
                    action.Remain = previousRemain - action.Quantity;
                    DbContext.StockActions.Update(action);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    // このロットだけでは払い出せない：前日残分はこのロットから、残りは別のロットから払い出す
                    DEBUGLOG_StockActionQuantityChangedDirectly(action, previousRemain, 0);
                    var outOfStockQuantity = action.Quantity - previousRemain;
                    action.Quantity = previousRemain;
                    action.Remain = 0;
                    DbContext.StockActions.Update(action);

                    var outOfStock = TransferToOtherLot(part, action.ActionDate, action.ArrivalDate, outOfStockQuantity);
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
            DEBUGLOG_StockActionQuantityChangedDirectly(toDiscard, previousRemain, 0);
            toDiscard.Quantity = previousRemain;
            DbContext.StockActions.Update(toDiscard);

            DbContext.SaveChanges();
            DEBUGLOG_EndMethod();
        }
        #endregion // 払い出し予定の振替


#if DEBUG
#region デバッグ用

        private class IndentString
        {
            private int Depth { get; set; }
            public IndentString(int depth = 0)
            {
                Depth = depth;
            }

            public static IndentString operator++(IndentString i)
            {
                i.Depth++;
                return i;
            }

            public static IndentString operator--(IndentString i)
            {
                i.Depth--;
                return i;
            }

            public override string ToString()
            {
                return string.Join("", Enumerable.Range(0, Depth).Select(i => "  "));
            }
        }

        private IndentString Indent { get; set; } = new IndentString();


        [Conditional("DEBUG")]
        private void DEBUGLOG_BeginMethod(string args = "", string msg = "", [CallerMemberName] string caller = "")
        {
            var b = new StringBuilder().Append($"{Indent}[BEGIN] ").Append(caller)
                .Append(string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )");

            if(!string.IsNullOrWhiteSpace(msg))
            {
                b.Append(' ').Append(msg);
            }

            LogUtil.Debug(b.ToString());
            Indent++;
        }

        [Conditional("DEBUG")]
        private void DEBUGLOG_EndMethod(string args = "", string msg = "", [CallerMemberName] string caller = "")
        {
            var b = new StringBuilder().Append($"{Indent}[END] ").Append(caller)
                .Append(string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )");

            if (!string.IsNullOrWhiteSpace(msg))
            {
                b.Append(' ').Append(msg);
            }

            Indent--;
            LogUtil.Debug(b.ToString());
        }

        /// <summary>
        /// 生成/登録された在庫アクションをデバッグログ出力する
        /// </summary>
        /// <param name="action">出力対象在庫アクション</param>
        /// <param name="calledFrom">このメソッドの呼び出し元：通常は指定不要。直接の呼び出し元ではなく、さらにその呼び出し元をログに残したいとき指定する</param>
        /// <param name="line">このメソッドの呼び出し位置：ソースファイル中の行番号。calledFrom と同様通常は指定不要、呼び出し元の呼び出し元をログに残したいときのみ指定する</param>
        [Conditional("DEBUG")]
        private void DEBUGLOG_StockActionCreated(StockAction action, [CallerMemberName] string calledFrom = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.Debug(new StringBuilder()
                .Append(Indent).Append("Created: ").Append(action.Action)
                .AppendFormat("[ day={0:yyyyMMdd}", action.ActionDate)
                .Append(", part=").Append(action.PartsCode)
                .AppendFormat(", arrived={0:yyyyMMdd}", action.ArrivalDate)
                .Append(", quantity=").Append(action.Quantity)
                .Append(", remain=").Append(action.Remain)
                .Append(" ]")
                .AppendFormat(" ({0}:{1})", calledFrom, line)
                .ToString()); ;
        }

        /// <summary>
        /// 在庫アクションの数量変更をデバッグログ出力する
        /// </summary>
        /// <param name="action">出力対象在庫アクション</param>
        /// <param name="diffOfQuantity">数量の変更量：変更後の数量が変更前＋αのとき＋α、変更前－αのとき－αを指定する</param>
        /// <param name="diffOfRemain">残数の変更量：変更後の残数が変更前＋αのとき＋α、変更前－αのとき－αを指定する</param>
        /// <param name="calledFrom">このメソッドの呼び出し元：通常は指定不要。直接の呼び出し元ではなく、さらにその呼び出し元をログに残したいとき指定する</param>
        /// <param name="line">このメソッドの呼び出し位置：ソースファイル中の行番号。calledFrom と同様通常は指定不要、呼び出し元の呼び出し元をログに残したいときのみ指定する</param>
        [Conditional("DEBUG")]
        private void DEBUGLOG_StockActionQuantityChanged(StockAction action, int diffOfQuantity, int diffOfRemain, [CallerMemberName] string calledFrom = "", [CallerLineNumber] int line = 0)
        {
            var builder = new StringBuilder()
                .Append(Indent).Append(action.PartsCode).Append(".").Append(action.Action)
                .Append("[lot=").Append(action.StockLotNo)
                .AppendFormat(", day={0:yyyyMMdd}", action.ActionDate);

            if(diffOfQuantity == 0)
            {
                builder.AppendFormat(", quantity={0} (same)", action.Quantity);
            }
            else
            {
                builder.AppendFormat(", quantity={0}->{1}", action.Quantity, action.Quantity + diffOfQuantity);
            }
            
            if(diffOfRemain == 0)
            {
                builder.AppendFormat(", remain={0} (same)", action.Remain);
            }
            else
            {
                builder.AppendFormat(", remain={0}->{1}", action.Remain, action.Remain + diffOfRemain);
            }
            
            if(string.IsNullOrWhiteSpace(calledFrom))
            {
                builder.Append(" ]");
            }
            else
            {
                builder.AppendFormat(" ] ({0}:{1})", calledFrom, line);
            }

            LogUtil.Debug(builder.ToString());
        }

        /// <summary>
        /// 在庫アクションの数量変更をデバッグログ出力する
        /// </summary>
        /// <param name="action">出力対象在庫アクション</param>
        /// <param name="newQuantity">変更後の数量</param>
        /// <param name="newRemain">変更後の単品</param>
        /// <param name="calledFrom">このメソッドの呼び出し元：通常は指定不要。直接の呼び出し元ではなく、さらにその呼び出し元をログに残したいとき指定する</param>
        /// <param name="line">このメソッドの呼び出し位置：ソースファイル中の行番号。calledFrom と同様通常は指定不要、呼び出し元の呼び出し元をログに残したいときのみ指定する</param>
        [Conditional("DEBUG")]
        private void DEBUGLOG_StockActionQuantityChangedDirectly(StockAction action, int newQuantity, int newRemain, [CallerMemberName] string calledFrom = "", [CallerLineNumber] int line = 0)
        {
            var builder = new StringBuilder()
                .Append(Indent).Append(action.PartsCode).Append('.').Append(action.Action)
                .Append("[lot=").Append(action.StockLotNo)
                .AppendFormat(", day={0:yyyyMMdd}", action.ActionDate);

            if (newQuantity == action.Quantity)
            {
                builder.AppendFormat(", quantity={0} (same)", action.Quantity);
            }
            else
            {
                builder.AppendFormat(", quantity={0}->{1}", action.Quantity, newQuantity);
            }

            if (newRemain == action.Remain)
            {
                builder.AppendFormat(", remain={0} (same)", action.Remain);
            }
            else
            {
                builder.AppendFormat(", remain={0}->{1}", action.Remain, newRemain);
            }

            builder.AppendFormat(" ] ({0}:{1})", calledFrom, line);

            LogUtil.Debug(builder.ToString());
        }

        [Conditional("DEBUG")]
        private void DEBUGLOG_ComparationOfStockRemainAndQuantity(StockAction action, int quantity, [CallerMemberName] string calledFrom = "", [CallerLineNumber] int line = 0)
        {
            if(action.Remain >= quantity)
            {
                LogUtil.DebugFormat("{0}Lot{1}.Remain({2:yyyyMMdd}) = {3} >= {4} ({5}:{6})",
                    Indent, action.StockLotNo, action.ActionDate, action.Remain, quantity, calledFrom, line);
            }
            else
            {
                LogUtil.DebugFormat("{0}Lot{1}.Remain({2:yyyyMMdd}) = {3} < {4} ({5}:{6})",
                    Indent, action.StockLotNo, action.ActionDate, action.Remain, quantity, calledFrom, line);
            }

        }

        [Conditional("DEBUG")]
        private void DEBUGLOG_ComparisonOfStockQuantityAndPreviousRemain(StockAction action, int remain, [CallerMemberName] string calledFrom = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.Debug(new StringBuilder()
                .Append(Indent).Append(action.Action).Append(": ")
                .AppendFormat("Lot{0}.Quantity({1:yyyyMMdd}) = {2}", action.StockLotNo, action.ActionDate, action.Quantity)
                .Append(action.Quantity <= remain ? " <= " : " > ")
                .AppendFormat("PreviousRemain({0})", remain)
                .AppendFormat(" ({0}:{1}", calledFrom, line)
                .ToString());
        }
#endregion // デバッグ用
#endif
    }
}
