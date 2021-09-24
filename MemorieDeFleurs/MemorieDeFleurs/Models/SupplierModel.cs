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
        /// 次の在庫ロット番号
        /// 
        /// 発注のタイミングで生成し、発注ロット番号と在庫ロット番号を兼用する。
        /// </summary>
        internal SequenceUtil.SequenceValueManager SEQ_INVENTORY_LOT_NUMBER { get { return Parent.Sequences.SEQ_INVENTORY_LOT_NUMBER; } }

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
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var supplier = Create(context);
                        transaction.Commit();
                        return supplier;
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            
            private Supplier Create(MemorieDeFleursDbContext context)
            {
                var s = new Supplier()
                {
                    Code = _model.SEQ_SUPPLIERS.Next(context),
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

        #region OrderToSupplierBuilder
        public class OrderToSupplierBuilder
        {
            private class OrderDetail
            {
                internal int LotCount { get; set; }
                internal int LotNo { get; set; }
            }
            private SupplierModel _model;
            private Supplier _supplier;
            private DateTime _orderDate;
            private DateTime _delivaryDate;
            private IDictionary<string, OrderDetail> _details = new Dictionary<string, OrderDetail>();
            internal static OrderToSupplierBuilder GetInstance(SupplierModel model)
            {
                return new OrderToSupplierBuilder(model);
            }

            private OrderToSupplierBuilder(SupplierModel model)
            {
                _model = model;
            }

            public OrderToSupplierBuilder SupplierTo(Supplier supplier)
            {
                _supplier = supplier;
                return this;
            }

            public OrderToSupplierBuilder OrderAt(DateTime orderDate)
            {
                _orderDate = orderDate;
                return this;
            }

            public OrderToSupplierBuilder DerivalyAt(DateTime derivalyDate)
            {
                _delivaryDate = derivalyDate;
                return this;
            }

            public OrderToSupplierBuilder Order(BouquetPart part, int count, int lotNo = 0)
            {
                if(_details.ContainsKey(part.Code))
                {
                    _details[part.Code].LotCount += count;
                }
                else
                {
                    _details.Add(part.Code, new OrderDetail() { LotCount = count, LotNo = lotNo });
                }
                return this;
            }

            public OrdersToSupplier Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var order = Create(context);
                        transaction.Commit();
                        return order;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            public OrdersToSupplier Create(MemorieDeFleursDbContext context)
            {
                var currentOrders = context.OrdersToSuppliers
                    .Count(o => o.OrderDate == _orderDate);

                var order = new OrdersToSupplier()
                {
                    ID = $"{_orderDate.ToString("yyyyMMdd")}-{currentOrders + 1:000000}",
                    DeliveryDate = _delivaryDate,
                    OrderDate = _orderDate,
                    Supplier = _supplier.Code
                };

                var i = 1;
                foreach (var detail in _details)
                {
                    var od = new OrderDetailsToSupplier()
                    {
                        OrderToSupplierID = order.ID,
                        OrderIndex = i++,
                        PartsCode = detail.Key,
                        LotCount = detail.Value.LotCount,
                        InventoryLotNo = detail.Value.LotNo == 0 ? _model.SEQ_INVENTORY_LOT_NUMBER.Next(context) : detail.Value.LotNo
                    };

                    context.OrderDetailsToSuppliers.Add(od);
                    order.Details.Append(od);
                }

                context.OrdersToSuppliers.Add(order);
                context.SaveChanges();

                return order;
            }
        }

        public OrderToSupplierBuilder GetOrderToSupplierBuilder()
        {
            return OrderToSupplierBuilder.GetInstance(this);
        }
        #endregion // OrderToSupplierBuilder

        #region InventoryActionLotBuilder
        /// <summary>
        /// 発注時に登録する在庫アクションの共通パラメータ
        /// </summary>
        private class InventoryActionLotBuilder
        {
            private BouquetPart _part;
            private DateTime _arrivalDate;
            private int _lotNo;
            private int _quantity;

            private InventoryActionLotBuilder() { }

            public static InventoryActionLotBuilder GetInstance()
            {
                return new InventoryActionLotBuilder();
            }

            public InventoryActionLotBuilder OrderPartIs(BouquetPart part, int num)
            {
                _part = part;
                _quantity = num * part.QuantitiesPerLot;
                return this;
            }

            public InventoryActionLotBuilder ArriveAt(DateTime date)
            {
                _arrivalDate = date;
                return this;
            }

            public InventoryActionLotBuilder LotNumberIs(int no)
            {
                _lotNo = no;
                return this;
            }

            public void Create(MemorieDeFleursDbContext context)
            {
                AddScheduledToArriveInventoryAction(context);
                AddScheduledToDiscardInventoryAction(context);
                AddScheduledToUseInventoryAction(context);
                context.SaveChanges();
            }

            private void AddScheduledToUseInventoryAction(MemorieDeFleursDbContext context)
            {
                var list = new List<InventoryAction>();

                // 品質維持可能日数＋1日分 (入荷日+0日目、入荷日+1日目、…、入荷日+品質維持可能日数日目)を生成
                foreach (var d in Enumerable.Range(0, _part.ExpiryDate + 1).Select(i => _arrivalDate.AddDays(i)))
                {
                    var toUse = new InventoryAction()
                    {
                        ActionDate = d,
                        Action = InventoryActionType.SCHEDULED_TO_USE,
                        PartsCode = _part.Code,
                        InventoryLotNo = _lotNo,
                        ArrivalDate = _arrivalDate,
                        Quantity = 0,
                        Remain = _quantity
                    };
                    context.InventoryActions.Add(toUse);
                    LogUtil.DEBUGLOG_InventoryActionCreated(toUse);
                    list.Add(toUse);
                }
            }

            private void AddScheduledToDiscardInventoryAction(MemorieDeFleursDbContext context)
            {
                var discard = new InventoryAction()
                {
                    ActionDate = _arrivalDate.AddDays(_part.ExpiryDate),
                    Action = InventoryActionType.SCHEDULED_TO_DISCARD,
                    PartsCode = _part.Code,
                    InventoryLotNo = _lotNo,
                    ArrivalDate = _arrivalDate,
                    Quantity = _quantity,
                    Remain = 0
                };
                context.InventoryActions.Add(discard);
                LogUtil.DEBUGLOG_InventoryActionCreated(discard);
            }

            private void AddScheduledToArriveInventoryAction(MemorieDeFleursDbContext context)
            {
                var arrive = new InventoryAction()
                {
                    ActionDate = _arrivalDate,
                    Action = InventoryActionType.SCHEDULED_TO_ARRIVE,
                    PartsCode = _part.Code,
                    InventoryLotNo = _lotNo,
                    ArrivalDate = _arrivalDate,
                    Quantity = _quantity,
                    Remain = _quantity
                };
                context.InventoryActions.Add(arrive);
                LogUtil.DEBUGLOG_InventoryActionCreated(arrive);
            }

        }

        private InventoryActionLotBuilder GetLotBuilder()
        {
            return InventoryActionLotBuilder.GetInstance();
        }
        #endregion // InventoryActionLotBuilder

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
            return context.Suppliers.Find(supplierCode);
        }
        #endregion // Supplier の生成・更新・削除

        #region 発注

        /// <summary>
        /// 発注する
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="supplier">発注先</param>
        /// <param name="derivalyDate">納品予定日</param>
        /// <param name="orderParts">発注明細</param>
        /// <returns>発注番号</returns>
        public string Order(DateTime orderDate, Supplier supplier, DateTime derivalyDate, IList<Tuple<BouquetPart,int>> orderParts)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                var orderList = string.Join(", ", orderParts.Select(t => $"({t.Item1.Code} x{t.Item2})"));
                LogUtil.DEBUGLOG_BeginMethod($"{orderDate.ToString("yyyyMMdd")}, {supplier.Code}, {derivalyDate.ToString("yyyyMMdd")}, [{orderList}]");
                try
                {
                    var orderNo = Order(context, orderDate, supplier, derivalyDate, orderParts);
                    transaction.Commit();
                    return orderNo;
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod();
                }
            }
        }

        /// <summary>
        /// 発注する：トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日</param>
        /// <param name="supplier">発注先</param>
        /// <param name="derivalyDate">納品予定日</param>
        /// <param name="orderParts">発注明細</param>
        /// <returns>発注番号</returns>
        public string Order(MemorieDeFleursDbContext context, DateTime orderDate, Supplier supplier, DateTime derivalyDate, IList<Tuple<BouquetPart, int>> orderParts)
        {
            var builder = GetOrderToSupplierBuilder()
                .OrderAt(orderDate)
                .SupplierTo(supplier)
                .DerivalyAt(derivalyDate);

            foreach(var item in orderParts)
            {
                var part = item.Item1;
                var lotcount = item.Item2; 

                var lotNo = Order(context, orderDate, part, lotcount, derivalyDate);
                builder.Order(part, lotcount, lotNo);
            }

            var order = builder.Create(context);

            var orderDetails = orderParts.Select(t => $"({t.Item1.Code} x{t.Item2})");
            LogUtil.Info($"{order.ID} ordered: {supplier.Code}, {derivalyDate.ToString("yyyyMMdd")}, [{string.Join(", ", orderDetails)}]");
            return order.ID;
        }

        /// <summary>
        /// 注文処理に伴う在庫アクション登録
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="part">単品</param>
        /// <param name="quantityOfLot">注文ロット数</param>
        /// <param name="arrivalDate">納品予定日</param>
        /// <returns>発注ロット番号(＝在庫ロット番号)</returns>
        public int Order(DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod($"{orderDate.ToString("yyyyMMdd")}, {part.Code}, {quantityOfLot} Lot(s), {arrivalDate.ToString("yyyyMMdd")}");
                try
                {
                    var orderNo = Order(context, orderDate, part, quantityOfLot, arrivalDate);
                    transaction.Commit();
                    return orderNo;
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod($"{orderDate.ToString("yyyyMMdd")}, {part.Code}, {quantityOfLot} Lot(s), {arrivalDate.ToString("yyyyMMdd")}");
                }
            }
        }

        /// <summary>
        /// 発注処理に伴う在庫アクション登録、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日</param>
        /// <param name="part">単品</param>
        /// <param name="quantityOfLot">注文ロット数</param>
        /// <param name="arrivalDate">納品予定日</param>
        /// <returns>発注ロット番号(＝在庫ロット番号)</returns>
        public int Order(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod(new StringBuilder()
                .AppendFormat("order={0:yyyyMMdd}", orderDate)
                .Append(", part=").Append(part.Code)
                .AppendFormat(", quantity={0}[lot(s)]({1}[parts])", quantityOfLot, quantityOfLot * part.QuantitiesPerLot)
                .AppendFormat(", arrival={0:yyyyMMdd}", arrivalDate)
                .ToString());

            // [TODO] 発注ロット番号=在庫ロット番号は発注時に採番する。
            var lotNo = SEQ_INVENTORY_LOT_NUMBER.Next(context);
            var usedLot = new Stack<int>();

            // 発注分の在庫アクションを登録する
            InventoryActionLotBuilder.GetInstance()
                .OrderPartIs(part, quantityOfLot)
                .ArriveAt(arrivalDate)
                .LotNumberIs(lotNo)
                .Create(context);


            // 当日以降の入荷予定分と在庫不足分をこのロットに振り替える
            foreach (var currentOrder in context.InventoryActions
                .Where(act => act.InventoryLotNo == lotNo)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .OrderBy(act => act.ActionDate))
            {
                // 在庫不足の解消: ループイテレータを都度 DBContext から削除するので、ループ対象のコピーを取る。
                foreach (var shortageAction in context.InventoryActions
                    .Where(act => act.PartsCode == currentOrder.PartsCode)
                    .Where(act => act.Action == InventoryActionType.SHORTAGE)
                    .Where(act => act.ActionDate == currentOrder.ActionDate)
                    .OrderBy(act => act.ArrivalDate).ToList())
                {
                    context.InventoryActions.Remove(shortageAction);
                    context.SaveChanges();

                    LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(currentOrder, shortageAction.Quantity);
                    if (currentOrder.Remain >= shortageAction.Quantity)
                    {
                        // 不足分全量をこの在庫ロットから払い出す
                        AddQuantityToInventoryLot(context, part, lotNo, shortageAction.ActionDate, shortageAction.Quantity);
                    }
                    else
                    {
                        throw new NotImplementedException(new StringBuilder()
                            .Append("在庫不足：発注量では在庫不足を賄えない:")
                            .Append(" 品目=").Append(part.Code)
                            .Append(", 発注ロット：").Append(lotNo)
                            .AppendFormat(", 発注日={0:yyyyMMdd}", orderDate)
                            .AppendFormat(", 不足日={0:yyyyMMdd", currentOrder.ActionDate)
                            .Append(", 不足数=").Append(shortageAction.Quantity)
                            .Append(", 当日残=").Append(currentOrder.Remain)
                            .ToString());
                    }
                    LogUtil.Info($"Inventory shortage was eliminated. Date={shortageAction.ActionDate.ToString("yyyyMMdd")}, lot={shortageAction.InventoryLotNo}, lacked={shortageAction.Quantity}");
                }

                // 納品予定が翌日以降の在庫ロットからの振り替え
                foreach (var usedFromOthers in context.InventoryActions
                        .Where(act => act.PartsCode == currentOrder.PartsCode)
                        .Where(act => act.ActionDate == currentOrder.ActionDate)
                        .Where(act => act.ArrivalDate > currentOrder.ArrivalDate)
                        .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                        .Where(act => act.Quantity > 0)
                        .OrderBy(act => act.ArrivalDate))
                {
                    LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(currentOrder, usedFromOthers.Quantity);
                    if (currentOrder.Remain >= usedFromOthers.Quantity)
                    {
                        // 全量をこの在庫ロットから払い出す
                        AddQuantityToInventoryLot(context, part, lotNo, usedFromOthers.ActionDate, usedFromOthers.Quantity);
                        AddQuantityToInventoryLot(context, part, usedFromOthers.InventoryLotNo, usedFromOthers.ActionDate, -usedFromOthers.Quantity);
                    }
                    else
                    {
                        var shortageQuantity = usedFromOthers.Quantity - currentOrder.Remain;

                        // 再振替のため、振替元の加工数を一旦ゼロに戻す
                        AddQuantityToInventoryLot(context, part, usedFromOthers.InventoryLotNo, usedFromOthers.ActionDate, -usedFromOthers.Quantity);

                        // 振替元で加工していた分をこのロット＋他ロットで再度振替なおす
                        AddQuantityToInventoryLot(context, part, lotNo, usedFromOthers.ActionDate, currentOrder.Remain);
                        var inventoryShortageAction = TransferToOtherLot(context, part, currentOrder.ActionDate, usedFromOthers.ArrivalDate, shortageQuantity);

                        if (inventoryShortageAction.Quantity > 0)
                        {
                            throw new NotImplementedException(new StringBuilder()
                                .Append("在庫不足：発注量では既存在庫ロットを全量振替できない:")
                                .Append(" 品目=").Append(part.Code)
                                .Append(", 発注ロット：").Append(lotNo)
                                .AppendFormat(", 発注日={0:yyyyMMdd}", orderDate)
                                .AppendFormat(", 不足日={0:yyyyMMdd}", currentOrder.ActionDate)
                                .Append("最終振替対象ロット").Append(inventoryShortageAction.InventoryLotNo)
                                .Append(", 振替残数=").Append(shortageQuantity)
                                .ToString());
                        }
                    }

                }
            }

            LogUtil.Info($"{orderDate.ToString("yyyyMMdd")}: {part.Code} x {quantityOfLot}[Lot(s)] ordered. arrive at {arrivalDate.ToString("yyyyMMdd")}, OrderLot#={lotNo}.");
            LogUtil.DEBUGLOG_EndMethod($"{part.Code}, {arrivalDate.ToString("yyyyMMdd")}", $"Lot#={lotNo}");
            return lotNo;
        }

        #endregion // 発注

        #region 発注取消
        /// <summary>
        /// 発注を取り消す
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        public void CancelOrder(string orderNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod(orderNo);
                try
                {
                    CancelOrder(context, orderNo);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(orderNo);
                }

            }
        }

        /// <summary>
        /// 発注を取り消す、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderNo">発注番号</param>
        public void CancelOrder(MemorieDeFleursDbContext context, string orderNo)
        {
            var order = context.OrdersToSuppliers.Find(orderNo);
            var details = context.OrderDetailsToSuppliers.Where(d => d.OrderToSupplierID == order.ID);

            foreach(var d in details)
            {
                CancelOrder(context, d.InventoryLotNo);
            }

            context.OrderDetailsToSuppliers.RemoveRange(details);
            context.OrdersToSuppliers.Remove(order);
            context.SaveChanges();
        }

        public void CancelOrder(int lotNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod($"LotNo.{lotNo}");
                try
                {
                    CancelOrder(context, lotNo);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod($"LotNo.{lotNo}");
                }
            }
        }

        public void CancelOrder(MemorieDeFleursDbContext context, int lotNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"lotNo={lotNo}");
            var lot = context.InventoryActions.Where(a => a.InventoryLotNo == lotNo);
            if (lot.Count() == 0)
            {
                LogUtil.Warn($"CancelOrder: Lot {lotNo} was not found in inventory actions.");
                return;
            }

            // コピーを取ってコピー元(データベースの中身)を削除
            var theLot = lot.ToList();
            context.InventoryActions.RemoveRange(theLot);
            context.SaveChanges();

            var partCode = theLot.First().PartsCode;
            var part = context.BouquetParts.Find(partCode);
            if (part == null)
            {
                throw new NotImplementedException($"単品 {partCode} が見つからない： Lot No. {lotNo}");
            }

            foreach (var action in theLot
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.Quantity > 0)
                .OrderBy(act => act.ActionDate))
            {
                // このロットで払い出されている加工数量を他のロットに移動する
                var shortageInventory = TransferToOtherLot(context, part, action.ActionDate, action.ArrivalDate, action.Quantity);
                if (shortageInventory.Quantity > 0)
                {
                    LogUtil.Warn($"Inventory shortage : {part.Code}, {action.ActionDate.ToString("yyyyMMdd")}, Lacked={shortageInventory} at lot {action.InventoryLotNo}");
                    var inventoryShortageAction = context.InventoryActions
                        .Where(act => act.InventoryLotNo == shortageInventory.InventoryLotNo)
                        .Single(act => act.ActionDate == action.ActionDate);

                    // 在庫不足アクションの生成
                    var lacked = new InventoryAction()
                    {
                        Action = InventoryActionType.SHORTAGE,
                        ActionDate = inventoryShortageAction.ActionDate,
                        BouquetPart = inventoryShortageAction.BouquetPart,
                        InventoryLotNo = inventoryShortageAction.InventoryLotNo,
                        ArrivalDate = inventoryShortageAction.ArrivalDate,
                        PartsCode = inventoryShortageAction.PartsCode,
                        Quantity = shortageInventory.Quantity,
                        Remain = -shortageInventory.Quantity
                    };
                    context.InventoryActions.Add(lacked);
                    context.SaveChanges();

                    LogUtil.DEBUGLOG_InventoryActionCreated(lacked);
                }
            }

            LogUtil.Info($"Lot {lotNo} removed.");
            LogUtil.DEBUGLOG_EndMethod($"lotNo={lotNo}");
        }
        #endregion // 発注取消

        #region 払い出し予定の振替
        private class ShortageInventory
        {
            public int InventoryLotNo { get; set; }
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
        private ShortageInventory TransferToOtherLot(MemorieDeFleursDbContext context, BouquetPart part, DateTime actionDate, DateTime arrivalDate, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod(string.Format("part={0}, date={1:yyyyMMdd}, arrived={2:yyyyMMdd}, quantity={3}", part.Code, actionDate, arrivalDate, quantity));

            var inventory = new ShortageInventory() { InventoryLotNo = 0, Quantity = quantity };

            // 基準日が等しい加工予定在庫アクションのうち、残数＞0のロットの加工予定在庫アクションに振り替える
            foreach (var action in context.InventoryActions
                .Where(a => a.PartsCode == part.Code)
                .Where(a => a.ActionDate == actionDate)
                .Where(a => a.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(a => a.ArrivalDate >= arrivalDate)
                .Where(a => a.Remain > 0)
                .OrderBy(a => a.ArrivalDate))
            {
                LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(action, inventory.Quantity);
                if(action.Remain >= inventory.Quantity)
                {
                    // 全量をこの在庫アクションから払い出す
                    AddQuantityToInventoryLot(context, part, action.InventoryLotNo, action.ActionDate, inventory.Quantity);
                    inventory.Quantity = 0;
                    break;
                }
                else
                {
                    // 払い出せる分だけこの在庫アクションから払い出す
                    var usedFromThisInventory = action.Remain;
                    AddQuantityToInventoryLot(context, part, action.InventoryLotNo, action.ActionDate, usedFromThisInventory);

                    inventory.Quantity -= usedFromThisInventory;
                    inventory.InventoryLotNo = action.InventoryLotNo;

                    LogUtil.Debug($"{LogUtil.Indent}Inventory shortage: {inventory.Quantity + usedFromThisInventory}->{inventory.Quantity}");
                }
            }

            LogUtil.DEBUGLOG_EndMethod(msg: $"lacked={inventory.Quantity}, at {actionDate.ToString("yyyyMMdd")}");
            return inventory;
        }

        /// <summary>
        /// 日付方向に払出予定を展開する：同一ロットのactionDate、翌日、その翌日…と破棄予定日までの残数から quantity を引く
        /// </summary>
        /// <param name="part">払出対象の単品</param>
        /// <param name="lotNo">払出を行うロット番号</param>
        /// <param name="actionDate">基準日</param>
        /// <param name="quantity">払出数量</param>
        private void AddQuantityToInventoryLot(MemorieDeFleursDbContext context, BouquetPart part, int lotNo, DateTime actionDate, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod(args: $"part={part.Code}, lot={lotNo}, date={actionDate.ToString("yyyyMMdd")}, quantity={quantity}");

            // ロット lotNo の、actionDate 以降の加工予定アクションから quantity 本の単品を払い出す
            var theLotInventories = context.InventoryActions
                .Where(act => act.PartsCode == part.Code)
                .Where(act => act.InventoryLotNo == lotNo);
            var usedFromTheLot = quantity;

            // actionDate 当日分：加工数に quantity を加算する
            var toUse
                = theLotInventories
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Single(act => act.ActionDate == actionDate);
            LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(toUse, usedFromTheLot);
            if (toUse.Remain >= usedFromTheLot)
            {
                // 全量払出する
                LogUtil.DEBUGLOG_InventoryActionQuantityChanged(toUse, toUse.Quantity + usedFromTheLot, toUse.Remain - usedFromTheLot);
                toUse.Quantity += usedFromTheLot;
                toUse.Remain -= usedFromTheLot;
                context.InventoryActions.Update(toUse);
            }
            else
            {
                throw new NotImplementedException(new StringBuilder()
                    .Append("在庫払い出しできない：残数が要求された使用量より小さい. ")
                    .AppendFormat(" 払出開始日：{0:yyyyMMdd}", actionDate)
                    .AppendFormat(" 不足発生日={0:yyyyMMdd}", toUse.ActionDate)
                    .Append(", 花コード=").Append(toUse.PartsCode)
                    .Append(", ロット番号").Append(toUse.InventoryLotNo)
                    .AppendFormat(", 納品日={0:yyyyMMdd}", toUse.ArrivalDate)
                    .Append(", 要求数量=").Append(usedFromTheLot)
                    .Append(", 在庫数量=").Append(toUse.Remain)
                    .ToString());
            }

            var previousRemain = toUse.Remain;

            // actionDate 翌日以降：加工数は変更せず残数から usedFromTheLot を引く
            foreach (var action in theLotInventories
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate > actionDate)
                .OrderBy(act => act.ActionDate))
            {
                LogUtil.DEBUGLOG_ComparisonOfInventoryQuantityAndPreviousRemain(action, 0);
                if(action.Quantity <= previousRemain)
                {
                    // このロットから全量払い出せる
                    LogUtil.DEBUGLOG_InventoryActionQuantityChanged(action, action.Quantity, previousRemain - action.Quantity);
                    action.Remain = previousRemain - action.Quantity;
                    context.InventoryActions.Update(action);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    // このロットだけでは払い出せない：前日残分はこのロットから、残りは別のロットから払い出す
                    LogUtil.DEBUGLOG_InventoryActionQuantityChanged(action, previousRemain, 0);
                    var inventoryShortageQuantity = action.Quantity - previousRemain;
                    action.Quantity = previousRemain;
                    action.Remain = 0;
                    context.InventoryActions.Update(action);

                    var shortageInventory = TransferToOtherLot(context, part, action.ActionDate, action.ArrivalDate, inventoryShortageQuantity);
                    if(shortageInventory.Quantity > 0)
                    {
                        throw new NotImplementedException(new StringBuilder()
                            .Append("在庫振替不可：当日分在庫不足")
                            .AppendFormat(" 払出開始日：{0:yyyyMMdd}", actionDate)
                            .AppendFormat(" 不足発生日={0:yyyyMMdd}", action.ActionDate)
                            .Append(", 花コード=").Append(action.PartsCode)
                            .Append(", ロット番号").Append(action.InventoryLotNo)
                            .AppendFormat(", 納品日={0:yyyyMMdd}", action.ArrivalDate)
                            .Append(", 要求数量=").Append(usedFromTheLot)
                            .Append(", 在庫数量=").Append(action.Remain)
                            .ToString());
                    }
                    previousRemain = 0;
                }
            }

            // 破棄アクションの更新
            var toDiscard = theLotInventories.Single(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
            LogUtil.DEBUGLOG_InventoryActionQuantityChanged(toDiscard, previousRemain, 0);
            toDiscard.Quantity = previousRemain;
            context.InventoryActions.Update(toDiscard);

            context.SaveChanges();
            LogUtil.DEBUGLOG_EndMethod();
        }
#endregion // 払い出し予定の振替
    }
}
