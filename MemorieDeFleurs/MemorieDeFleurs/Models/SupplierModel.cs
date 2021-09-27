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
                AddScheduledToUseInventoryAction(context);
                AddScheduledToDiscardInventoryAction(context);
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
            var lotNo = SEQ_INVENTORY_LOT_NUMBER.Next(context);
            Order(context, orderDate, part, quantityOfLot, arrivalDate, lotNo);
            return lotNo;
        }

        private void Order(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate, int lotNo)
        {
            LogUtil.DEBUGLOG_BeginMethod(new StringBuilder()
                .AppendFormat("order={0:yyyyMMdd}", orderDate)
                .Append(", part=").Append(part.Code)
                .AppendFormat(", quantity={0}[lot(s)]({1}[parts])", quantityOfLot, quantityOfLot * part.QuantitiesPerLot)
                .AppendFormat(", arrival={0:yyyyMMdd}", arrivalDate)
                .Append(", lotNo=").Append(lotNo)
                .ToString());

            // [TODO] 発注ロット番号=在庫ロット番号は発注時に採番する。
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
                    LogUtil.Debug($"Shortage Found:{shortageAction.ToString("s")}");

                    LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(currentOrder, shortageAction.Quantity);
                    if (currentOrder.Remain >= shortageAction.Quantity)
                    {
                        // 不足分全量をこの在庫ロットから払い出す
                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(currentOrder, shortageAction.Quantity);
                        LogUtil.Debug($"{LogUtil.Indent}Remove: {shortageAction.ToString("L")}");

                        Parent.BouquetModel.UseFromThisLot(context, currentOrder, shortageAction.Quantity, usedLot);
                        context.InventoryActions.Remove(shortageAction);
                    }
                    else
                    {
                        // 移し替えできる分だけ移し替え、残余は在庫不足のままとする
                        var quantity = currentOrder.Remain;

                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(currentOrder, quantity);
                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(shortageAction, -quantity);

                        Parent.BouquetModel.UseFromThisLot(context, currentOrder, quantity, usedLot);

                        shortageAction.Quantity -= quantity;
                        shortageAction.Remain += quantity;
                        context.InventoryActions.Update(shortageAction);
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
                        var quantity = usedFromOthers.Quantity;

                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(currentOrder, quantity);
                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(usedFromOthers, -quantity);

                        Parent.BouquetModel.UseFromThisLot(context, currentOrder, quantity, usedLot);
                        Parent.BouquetModel.UseFromThisLot(context, usedFromOthers, -quantity, usedLot);
                    }
                    else
                    {
                        // 振替可能な分は currentOrder に振り替え、残余は usedFromOthers に残す
                        var quantity = currentOrder.Remain;

                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(currentOrder, quantity);
                        LogUtil.DEBUGLOG_InventoryActionQuantityChanged(usedFromOthers, -quantity);

                        Parent.BouquetModel.UseFromThisLot(context, currentOrder, quantity, usedLot);
                        Parent.BouquetModel.UseFromThisLot(context, usedFromOthers, -quantity, usedLot);
                    }

                }
            }

            context.SaveChanges();

            LogUtil.Info($"{orderDate.ToString("yyyyMMdd")}: {part.Code} x {quantityOfLot}[Lot(s)] ordered. arrive at {arrivalDate.ToString("yyyyMMdd")}, OrderLot#={lotNo}.");
            LogUtil.DEBUGLOG_EndMethod($"{part.Code}, {arrivalDate.ToString("yyyyMMdd")}, Lot#={lotNo}");
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
                try
                {
                    var usedLot = new Stack<int>();
                    usedLot.Push(action.InventoryLotNo);
                    Parent.BouquetModel.UseFromOtherLot(context, action, action.Quantity, usedLot);
                }
                catch(InventoryShortageException eis)
                {
                    context.InventoryActions.Add(eis.InventoryShortageAction);
                    LogUtil.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                }
            }

            context.SaveChanges();

            LogUtil.Info($"Lot {lotNo} removed.");
            LogUtil.DEBUGLOG_EndMethod($"lotNo={lotNo}");
        }
        #endregion // 発注取消

        #region 納品予定日変更
        /// <summary>
        /// 納品予定日を変更する
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <param name="newArrivalDate">変更後の納品予定日</param>
        public void ChangeArrivalDate(string orderNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{orderNo}, {newArrivalDate.ToString("yyyyMMDD")}");

            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    ChangeArrivalDate(context, orderNo, newArrivalDate);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod($"{orderNo}, {newArrivalDate.ToString("yyyyMMDD")}");
                }

            }
        }

        /// <summary>
        /// 納品予定日を変更する：トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderNo">発注番号</param>
        /// <param name="newArrivalDate">変更後の納品予定日</param>
        public void ChangeArrivalDate(MemorieDeFleursDbContext context, string orderNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"context, {orderNo}, {newArrivalDate.ToString("yyyyMMdd")}");

            var order = context.OrdersToSuppliers.Find(orderNo);
            var oldArrivalDate = order.DeliveryDate;
            var details = context.OrderDetailsToSuppliers
                .Where(d => d.OrderToSupplierID == order.ID).OrderBy(d => d.OrderIndex);
            var detailStrings = details.Select(d => $"{d.PartsCode} x {d.LotCount}");

            LogUtil.Debug($"Order {orderNo} contains {details.Count()} parts: [{string.Join(", ", details)}]");

            foreach(var item in details.OrderBy(d => d.OrderIndex))
            {
                var part = context.BouquetParts.Find(item.PartsCode);
                ChangeArrivalDate(context, order.OrderDate, item.BouquetPart, item.InventoryLotNo, newArrivalDate);
            }
            context.SaveChanges();

            LogUtil.Info($"Arrival date changed: {orderNo}, {oldArrivalDate.ToString("yyyyMMdd")} -> {newArrivalDate.ToString("yyyyMMdd")}");
            LogUtil.DEBUGLOG_EndMethod($"context, {orderNo}, {newArrivalDate.ToString("yyyyMMdd")}");
        }

        /// <summary>
        /// 個々の単品について納品予定日を変更する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日：発注日は変更しない</param>
        /// <param name="part">発注対象単品</param>
        /// <param name="lotNo">在庫ロット番号</param>
        /// <param name="newArrivalDate">変更後の納品予定日</param>
        private void ChangeArrivalDate(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int lotNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{part.Code}, {lotNo}, {newArrivalDate.ToString("yyyyMMdd")}");
            try
            {
                var oldArrivalLot = context.InventoryActions
                    .Where(act => act.PartsCode == part.Code)
                    .Where(act => act.InventoryLotNo == lotNo)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE)
                    .Single();

                CancelOrder(context, lotNo);
                Order(context, orderDate, part, oldArrivalLot.Quantity / part.QuantitiesPerLot, newArrivalDate, oldArrivalLot.InventoryLotNo);
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod($"{part.Code}, {lotNo}, {newArrivalDate.ToString("yyyyMMdd")}");
            }
        }
        #endregion // 納品予定日変更
    }
}
