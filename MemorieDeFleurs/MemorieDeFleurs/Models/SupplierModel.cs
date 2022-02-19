using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using YasT.Framework.Logging;

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

            private ISet<string> _supplyParts = new SortedSet<string>();

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
            /// この仕入先が提供する単品を登録する
            /// </summary>
            /// <param name="partsCodes">仕入先が提供する単品：複数指定可能</param>
            /// <returns></returns>
            public SupplierBuilder SupplyParts(params string[] partsCodes)
            {
                foreach(var code in partsCodes)
                {
                    if(!_supplyParts.Contains(code))
                    {
                        _supplyParts.Add(code);
                    }
                }

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

            /// <summary>
            /// 登録変更した内容で仕入先を登録する
            /// 
            /// 仕入先コードの採番はこのメソッド内で行われる。
            /// トランザクション内での呼出用。
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>登録された仕入先オブジェクト</returns>
            public Supplier Create(MemorieDeFleursDbContext context)
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

                foreach(var code in _supplyParts)
                {
                    _model.StartProvidingParts(context, s.Code, code);
                }

                context.SaveChanges();

                return s;
            }
        }

        /// <summary>
        /// DB登録オブジェクト生成器を取得する
        /// </summary>
        /// <returns>仕入先オブジェクト生成器</returns>
        public SupplierBuilder GetSupplierBuilder()
        {
            return SupplierBuilder.GetInstance(this);
        }
        #endregion // SupplierBuilder

        #region OrderToSupplierBuilder
        /// <summary>
        /// 仕入先発注オブジェクト生成器
        /// </summary>
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

            /// <summary>
            /// 発注する仕入先を指定する
            /// </summary>
            /// <param name="supplier">仕入先ID</param>
            /// <returns>仕入先発注オブジェクト生成器(自分自身)</returns>
            public OrderToSupplierBuilder SupplierTo(Supplier supplier)
            {
                _supplier = supplier;
                return this;
            }

            /// <summary>
            /// 発注日を指定する
            /// </summary>
            /// <param name="orderDate">発注日</param>
            /// <returns>仕入先発注オブジェクト生成器(自分自身)</returns>
            public OrderToSupplierBuilder OrderAt(DateTime orderDate)
            {
                _orderDate = orderDate;
                return this;
            }

            /// <summary>
            /// 入荷予定日を指定する
            /// </summary>
            /// <param name="derivalyDate">入荷予定日</param>
            /// <returns>仕入先発注オブジェクト生成器(自分自身)</returns>
            public OrderToSupplierBuilder DerivalyAt(DateTime derivalyDate)
            {
                _delivaryDate = derivalyDate;
                return this;
            }

            /// <summary>
            /// 発注する単品と数量を指定する
            /// </summary>
            /// <param name="part">発注する単品</param>
            /// <param name="count">発注ロット数</param>
            /// <param name="lotNo">(省略可)発注ロット番号：省略された場合は内部で自動採番する</param>
            /// <returns></returns>
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

            /// <summary>
            /// 仕入先発注情報をデータベースに登録する
            /// </summary>
            /// <returns>登録した仕入先発注情報</returns>
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

            /// <summary>
            /// 仕入先発注情報をデータベースに登録する
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>生成した仕入先発注情報</returns>
            public OrdersToSupplier Create(MemorieDeFleursDbContext context)
            {
                var currentOrders = context.OrdersToSuppliers
                    .Count(o => o.OrderDate == _orderDate);

                var order = new OrdersToSupplier()
                {
                    ID = $"{_orderDate.ToString("yyyyMMdd")}-{currentOrders + 1:000000}",
                    DeliveryDate = _delivaryDate,
                    OrderDate = _orderDate,
                    Supplier = _supplier.Code,
                    Status = OrderToSupplierStatus.SCHEDULED
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

        /// <summary>
        /// 仕入先発注オブジェクト生成器を取得する
        /// </summary>
        /// <returns>仕入先発注オブジェクト生成器</returns>
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
                    InventoryActionLogger.DEBUGLOG_InventoryActionCreated(toUse);
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
                InventoryActionLogger.DEBUGLOG_InventoryActionCreated(discard);
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
                InventoryActionLogger.DEBUGLOG_InventoryActionCreated(arrive);
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
        public Supplier FindSupplier(int supplierCode)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return FindSupplier(context, supplierCode);
            }
        }
        private Supplier FindSupplier(MemorieDeFleursDbContext context, int supplierCode)
        {
            return context.Suppliers
                .Include(s => s.SupplyParts)
                .ThenInclude(p => p.Part)
                .SingleOrDefault(s => s.Code == supplierCode);
        }

        /// <summary>
        /// 登録されている全仕入先オブジェクトを取得する
        /// </summary>
        /// <returns>登録されている仕入先オブジェクトの一覧。仕入先が登録されていないときは空の一覧</returns>
        public IEnumerable<Supplier> FindAllSuppliers()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.Suppliers
                    .Include(s => s.SupplyParts)
                    .ThenInclude(p => p.Part)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 仕入先オブジェクトを削除する。
        /// 
        /// 仕入先への発注実績があると削除できない。
        /// </summary>
        /// <param name="supplierCode">仕入先コード</param>
        public void RemoveSupplier(int supplierCode)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(supplierCode.ToString());
                    RemoveSupplier(context, supplierCode);
                    transaction.Commit();
                    LogUtil.Info($"Supplier {supplierCode} is removed.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(supplierCode.ToString());
                }
            }
        }

        /// <summary>
        /// 仕入先情報をデータベースから削除する
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="supplierCode">仕入先ID</param>
        /// <exception cref="ApplicationException"></exception>
        public void RemoveSupplier(MemorieDeFleursDbContext context, int supplierCode)
        {
            if(context.OrdersToSuppliers.Count(o => o.Supplier == supplierCode) > 0)
            {
                throw new ApplicationException($"発注実績のある仕入先は削除できない:仕入先ID={supplierCode}");
            }
            else
            {
                var supplier = FindSupplier(context, supplierCode);
                context.PartsSuppliers.RemoveRange(supplier.SupplyParts);
                context.Suppliers.Remove(supplier);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 仕入先情報をデータベースに登録する
        /// </summary>
        /// <param name="supplier">仕入先情報</param>
        /// <returns>データベースから再取得した、関連情報が適切に設定された仕入先情報</returns>
        public Supplier Save(Supplier supplier)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(supplier.Code.ToString());
                    var saved = Save(context, supplier);
                    transaction.Commit();
                    LogUtil.Info($"Supplier {saved.Code} saved.");
                    return saved;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(supplier.Code.ToString());
                }
            }

        }
        private Supplier Save(MemorieDeFleursDbContext context, Supplier supplier)
        {
            var found = FindSupplier(supplier.Code);
            if(found == null)
            {
                if(supplier.Code == 0)
                {
                    supplier.Code = Parent.Sequences.SEQ_SUPPLIERS.Next(context);
                }
                found = context.Suppliers.Add(supplier).Entity;
            }
            else
            {
                if(found.CheckAndModify(supplier))
                {
                    context.Suppliers.Update(found);
                }

                foreach(var parts in found.SupplyParts)
                {
                    if(supplier.SupplyParts.SingleOrDefault(p => p.PartCode == parts.PartCode) == null)
                    {
                        // found にあって supplier にない SupplyParts を削除
                        context.PartsSuppliers.Remove(parts);
                    }
                    else
                    {
                        // 何もしない：2つのキー(仕入先コードと花束コード)以外にDB上のカラムがないので、更新は発生しない。
                    }
                }

                foreach(var parts in supplier.SupplyParts)
                {
                    if(found.SupplyParts.SingleOrDefault(p => p.PartCode == parts.PartCode) == null)
                    {
                        // supplier にあって found にない SupplyParts を追加
                        context.PartsSuppliers.Add(parts);
                    }
                }
            }
            context.SaveChanges();

            return FindSupplier(context, found.Code);
        }
        #endregion // Supplier の生成・更新・削除

        #region 発注
        /// <summary>
        /// 発注する
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="supplierID">発注先ID</param>
        /// <param name="derivalyDate">入荷予定日</param>
        /// <param name="orderParts">発注明細:花コードと注文ロット数のタプルを必要個数列挙する</param>
        /// <returns>発注番号</returns>
        public string Order(DateTime orderDate, int supplierID, DateTime derivalyDate, params Tuple<string, int>[] orderParts)
        {
            Supplier supplier;
            Tuple<BouquetPart, int>[] parts;

            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                supplier = context.Suppliers.Find(supplierID);
                if (supplier == null)
                {
                    throw new ApplicationException($"仕入先未登録またはID入力ミス： {supplierID}");
                }

                var expectedParts = orderParts.Select(item => Tuple.Create(context.BouquetParts.Find(item.Item1), item.Item2));

                var notFound = orderParts.Zip(expectedParts, (p1, p2) => Tuple.Create(p1.Item1, p2.Item1, p2.Item2)).Where(item => item.Item2 == null);
                if (notFound.Count() > 0)
                {
                    throw new ApplicationException($"単品未登録または花コード入力ミス： {string.Join(", ", notFound.Select(item => item.Item1))}");
                }

                parts = expectedParts.Where(p => p.Item1 != null).ToArray();
            }

            return Order(orderDate, supplier, derivalyDate, parts);
        }

        /// <summary>
        /// 発注する
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="supplier">発注先</param>
        /// <param name="derivalyDate">入荷予定日</param>
        /// <param name="orderParts">発注明細</param>
        /// <returns>発注番号</returns>
        public string Order(DateTime orderDate, Supplier supplier, DateTime derivalyDate, IList<Tuple<BouquetPart,int>> orderParts)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod($"{orderDate:yyyyMMdd}, {supplier.Code}, {derivalyDate:yyyyMMdd}, " +
                    $"[{string.Join(", ", orderParts.Select(t => $"({t.Item1.Code} x{t.Item2})"))}]");
                try
                {
                    var orderNo = Order(context, orderDate, supplier, derivalyDate, orderParts);
                    transaction.Commit();

                    var details = context.OrderDetailsToSuppliers
                        .Where(item => item.OrderToSupplierID == orderNo)
                        .OrderBy(item => item.OrderIndex)
                        .Select(item => $"{item.PartsCode}.Lot{item.InventoryLotNo}({item.LotCount} lot(s))");

                    LogUtil.Info($"{orderDate:yyyyMMdd}, {orderNo} ordered: Supplier{supplier.Code}, {derivalyDate:yyyyMMdd}, [{string.Join(", ", details)}]");

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
        /// <param name="derivalyDate">入荷予定日</param>
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
                var nearestArrivalDate = orderDate.AddDays(part.LeadTime);
                if (derivalyDate < nearestArrivalDate)
                {
                    throw new ApplicationException($"納品日は発注日+{part.LeadTime}日以降でなければならない：" +
                        $"納品予定日{derivalyDate:yyyyMMdd} < {nearestArrivalDate:yyyyMMdd} = {orderDate:yyyyMMdd}+{part.LeadTime}日");
                }
                var lotNo = Order(context, orderDate, part, lotcount, derivalyDate);
                builder.Order(part, lotcount, lotNo);
            }

            var order = builder.Create(context);

            var orderDetails = orderParts.Select(t => $"({t.Item1.Code} x{t.Item2})");
            return order.ID;
        }

        /// <summary>
        /// 注文処理に伴う在庫アクション登録
        /// </summary>
        /// <param name="orderDate">発注日</param>
        /// <param name="part">単品</param>
        /// <param name="quantityOfLot">注文ロット数</param>
        /// <param name="arrivalDate">入荷予定日</param>
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
        /// 発注処理に伴う在庫アクション登録、トランザクション内での呼出用、ロット番号を新規発行する
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日</param>
        /// <param name="part">単品</param>
        /// <param name="quantityOfLot">注文ロット数</param>
        /// <param name="arrivalDate">入荷予定日</param>
        /// <returns>発注ロット番号(＝在庫ロット番号)</returns>
        public int Order(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int quantityOfLot, DateTime arrivalDate)
        {
            var lotNo = SEQ_INVENTORY_LOT_NUMBER.Next(context);
            var orderParam = new InventoryAction()
            {
                ArrivalDate = arrivalDate,
                ActionDate = arrivalDate,
                BouquetPart = part,
                PartsCode = part.Code,
                Quantity = quantityOfLot * part.QuantitiesPerLot,
                InventoryLotNo = lotNo,
            };
            Order(context, orderDate, orderParam);
            return lotNo;
        }

        /// <summary>
        /// 単品を発注する
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日</param>
        /// <param name="orderParam">発注情報：以下の値をセットしておくこと。
        ///     <list type="">
        ///         <item>
        ///             <term>【必須】<see cref="InventoryAction.ArrivalDate"/></term>
        ///             <description>入荷予定日</description>
        ///         </item>
        ///         <item>
        ///             <term>【必須】<see cref="InventoryAction.InventoryLotNo"/></term>
        ///             <description>在庫ロット番号。ゼロを指定した場合は内部で新規番号を採番する。</description>
        ///         </item>
        ///         <item>
        ///             <term>【必須】<see cref="InventoryAction.PartsCode"/></term>
        ///             <description>単品の花コード。<see cref="InventoryAction.BouquetPart"/>を指定しない場合でも花コードは必須。</description>
        ///         </item>
        ///         <item>
        ///             <term>【必須】<see cref="InventoryAction.Quantity"/></term>
        ///             <description>数量。注文ロット数ではなく、注文ロット数×購入単位数で入力すること。</description>
        ///         </item>
        ///     </list>"
        /// </param>
        private void Order(MemorieDeFleursDbContext context, DateTime orderDate, InventoryAction orderParam)
        {
            LogUtil.DEBUGLOG_BeginMethod($"context, {orderDate.ToString("yyyyMMdd")}, {orderParam.ToString("o")}");

            if (orderParam.BouquetPart == null)
            {
                orderParam.BouquetPart = context.BouquetParts.Find(orderParam.PartsCode);
            }

            var lotCount = orderParam.Quantity / orderParam.BouquetPart.QuantitiesPerLot;

            // 発注分の在庫アクションを登録する
            InventoryActionLotBuilder.GetInstance()
                .OrderPartIs(orderParam.BouquetPart, lotCount)
                .ArriveAt(orderParam.ArrivalDate)
                .LotNumberIs(orderParam.InventoryLotNo)
                .Create(context);

            var usedLot = new Stack<int>();
            // 発注した在庫アクションを起点に、後続の入荷予定ロットすべてに対して加工予定数の前詰めを行う。
            var target = orderParam;
            do
            {
                EliminateInventoryShortages(context, target, usedLot);
                context.SaveChanges(); // 削除した在庫不足アクション確定のため、ここで一度キャッシュを確定する

                FrontUpInventories(context, target, usedLot);
                //context.SaveChanges();

                usedLot.Push(target.InventoryLotNo);

                // 次の要素：
                var next = context.InventoryActions
                    .Where(act => act.PartsCode == target.PartsCode)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .Where(act => act.ArrivalDate > target.ArrivalDate)
                    .Where(act => act.Remain > 0)
                    .OrderBy(act => act.ArrivalDate)
                    .ThenBy(act => act.ActionDate)
                    .ThenBy(act => act.InventoryLotNo)
                    .FirstOrDefault();
                if(next == null) { break; }
                if(next.BouquetPart == null)
                {
                    next.BouquetPart = context.BouquetParts.Find(next.PartsCode);
                }
                target = next;
            } while (target != null);

            LogUtil.DEBUGLOG_EndMethod($"context, {orderDate.ToString("yyyyMMdd")}, {orderParam.ToString("o")}");
        }

        /// <summary>
        /// 在庫の「前詰め」：
        /// 指定された在庫ロットの各使用予定アクションに、
        /// 入荷予定日が指定在庫ロットの入荷予定日以降であるロットの仕様数量を移し替える
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="targetAction">移し替え先の在庫予定アクションを指定するためのパラメータ</param>
        /// <param name="usedLot">すでに移し替えを行った在庫ロットの一覧：
        /// usedLot に含まれる在庫ロット番号を持つ在庫アクションは前詰めの対象外とする。</param>
        private void FrontUpInventories(MemorieDeFleursDbContext context, InventoryAction targetAction, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{targetAction.ToString("h")}, [{string.Join(", ", usedLot)}]");
            usedLot.Push(targetAction.InventoryLotNo);
            foreach (var action in context.InventoryActions
                .Where(act => act.InventoryLotNo == targetAction.InventoryLotNo)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .OrderBy(act => act.ActionDate))
            {

                // 入荷予定が翌日以降の在庫ロットからの振り替え
                TransferLotQuantites(context, action, usedLot);

            }
            usedLot.Pop();
            LogUtil.DEBUGLOG_EndMethod();
        }


        /// <summary>
        /// 他ロットの在庫を指定された在庫に移し替える
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="action">移し替え先アクション</param>
        /// <param name="usedLot">移し替え対象外のロット</param>
        private void TransferLotQuantites(MemorieDeFleursDbContext context, InventoryAction action, Stack<int> usedLot)
        {
            foreach (var others in context.InventoryActions
                    .Where(act => act.PartsCode == action.PartsCode)
                    .Where(act => act.ActionDate == action.ActionDate)
                    .Where(act => act.ArrivalDate > action.ArrivalDate)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .Where(act => act.Quantity > 0)
                    .OrderBy(act => act.ArrivalDate))
            {
                InventoryActionLogger.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(action, others.Quantity);
                if (action.Remain >= others.Quantity)
                {
                    // 全量をこの在庫ロットに振り替える
                    var quantity = others.Quantity;

                    Parent.BouquetModel.UseFromThisLot(context, action, quantity, usedLot);

                    usedLot.Push(action.InventoryLotNo);
                    Parent.BouquetModel.ReturnToThisLot(context, others, quantity, usedLot);
                    usedLot.Pop();

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, quantity);
                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(others, -quantity);
                }
                else
                {
                    // 振替可能な分は currentOrder に振り替え、残余は usedFromOthers に残す
                    var quantity = action.Remain;

                    Parent.BouquetModel.UseFromThisLot(context, action, quantity, usedLot);

                    usedLot.Push(action.InventoryLotNo);
                    Parent.BouquetModel.ReturnToThisLot(context, others, quantity, usedLot);
                    usedLot.Pop();

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, quantity);
                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(others, -quantity);
                }

                DEBUGLOG_ShowInventoryActions(context, action.PartsCode, new int[] { 4, 5, 6 });
            }
        }

        /// <summary>
        /// 新規登録したロットの入荷日～入荷日＋品質維持可能日数の範囲内にある在庫不足アクションを解消する
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderParam">新規登録したロットの情報</param>
        /// <param name="usedLot">在庫不足解消済のロット一覧：指定されたロットへの在庫振替は行わない</param>
        private void EliminateInventoryShortages(MemorieDeFleursDbContext context, InventoryAction orderParam, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"param=[{orderParam.BouquetPart.Code}, Lot{orderParam.InventoryLotNo}, ArriveAt({orderParam.ArrivalDate:yyyyMMdd}]");
            var begin = orderParam.ActionDate;
            var end = orderParam.ArrivalDate.AddDays(orderParam.BouquetPart.ExpiryDate);

            var shortages = context.InventoryActions
                .Where(act => act.PartsCode == orderParam.BouquetPart.Code)
                .Where(act => act.Action == InventoryActionType.SHORTAGE)
                .Where(act => begin <= act.ActionDate && act.ActionDate <= end);

            foreach (var shortage in shortages)
            {
                var action = context.InventoryActions
                    .Where(act => act.PartsCode == orderParam.BouquetPart.Code)
                    .Where(act => act.InventoryLotNo == orderParam.InventoryLotNo)
                    .Where(act => act.ActionDate == shortage.ActionDate)
                    .Where(act => act.Remain > 0)
                    .SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_USE);

                if(action != null)
                {
                    Parent.BouquetModel.UseFromThisLot(context, action, shortage.Quantity, usedLot);
                    context.InventoryActions.Remove(shortage);
                }
            }
            // 削除した shortage をキャッシュから削除するため context.SaveChange() 必要。呼び出し元で実施すること。
            LogUtil.DEBUGLOG_EndMethod();
        }

        /// <summary>
        /// 現在の在庫アクション一覧をログ出力する。出力対象は引数指定可能。
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="partsCode">出力対象単品</param>
        /// <param name="lots">(任意)出力対象ロットを絞りたいとき、対象ロットを指定する</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        private void DEBUGLOG_ShowInventoryActions(MemorieDeFleursDbContext context, string partsCode, int[] lots, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.DEBUGLOG_BeginMethod(partsCode, $"in {caller},{Path.GetFileName(path)}:{line}");
           
            foreach(var action in context.InventoryActions
                .Where(act => act.PartsCode == partsCode)
                .OrderBy(act => act.ArrivalDate)
                .ThenBy(act => act.InventoryLotNo)
                .ThenBy(act => act.Action))
            {
                if (lots.Contains(action.InventoryLotNo))
                {
                    LogUtil.DebugWithoutLineNumber(action.ToString("DB"));
                }
            }

            LogUtil.DEBUGLOG_EndMethod(partsCode);
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
                    LogUtil.Info($"Order {orderNo} canceled.");
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

        /// <summary>
        /// 在庫ロット番号指定で注文をキャンセルする
        /// </summary>
        /// <param name="lotNo">キャンセルする注文の在庫ロット番号</param>
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
                    LogUtil.Info($"Lot {lotNo} removed.");
                }
                catch (Exception)
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

        /// <summary>
        /// 在庫ロット番号指定で注文をキャンセルする
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="lotNo">キャンセルする注文の在庫ロット番号</param>
        public void CancelOrder(MemorieDeFleursDbContext context, int lotNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"lotNo={lotNo}");
            try
            {
                var lot = context.InventoryActions.Where(a => a.InventoryLotNo == lotNo);
                if (lot.Count() == 0)
                {
                    LogUtil.Warn($"CancelOrder: Lot {lotNo} was not found in inventory actions.");
                    return;
                }

                var arrived = lot.SingleOrDefault(act => act.Action == InventoryActionType.ARRIVED);
                if (arrived != null)
                {
                    throw new ApplicationException($"単品入荷済み変更不可： {arrived.PartsCode}.Lot{arrived.InventoryLotNo}, 入荷日 {arrived.ArrivalDate:yyyyMMdd}");
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
                    catch (InventoryShortageException eis)
                    {
                        context.InventoryActions.Add(eis.InventoryShortageAction);
                        InventoryActionLogger.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                    }
                }

                context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod($"lotNo={lotNo}");
            }

        }
        #endregion // 発注取消

        #region 入荷予定日変更
        /// <summary>
        /// 入荷予定日を変更する
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <param name="newArrivalDate">変更後の入荷予定日</param>
        public void ChangeArrivalDate(string orderNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{orderNo}, {newArrivalDate:yyyyMMDD}");

            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var oldArrivalDate = context.OrdersToSuppliers.Find(orderNo).DeliveryDate;

                    ChangeArrivalDate(context, orderNo, newArrivalDate);
                    transaction.Commit();

                    LogUtil.Info($"Arrival date changed: {orderNo}, from {oldArrivalDate:yyyyMMdd} to {newArrivalDate:yyyyMMdd}");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod($"{orderNo}, {newArrivalDate:yyyyMMDD}");
                }

            }
        }

        /// <summary>
        /// 入荷予定日を変更する：トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderNo">発注番号</param>
        /// <param name="newArrivalDate">変更後の入荷予定日</param>
        public void ChangeArrivalDate(MemorieDeFleursDbContext context, string orderNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"context, {orderNo}, {newArrivalDate.ToString("yyyyMMdd")}");

            try
            {
                var order = context.OrdersToSuppliers.Find(orderNo);
                var oldArrivalDate = order.DeliveryDate;
                var details = context.OrderDetailsToSuppliers
                    .Where(d => d.OrderToSupplierID == order.ID).OrderBy(d => d.OrderIndex);
                var detailStrings = details.Select(d => $"{d.PartsCode} x {d.LotCount}");

                LogUtil.Debug($"Order {orderNo} contains {details.Count()} parts: [{string.Join(", ", details)}]");

                foreach (var item in details.OrderBy(d => d.OrderIndex))
                {
                    var part = context.BouquetParts.Find(item.PartsCode);
                    ChangeArrivalDate(context, order.OrderDate, item.BouquetPart, item.InventoryLotNo, newArrivalDate);
                }
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod($"context, {orderNo}, {newArrivalDate.ToString("yyyyMMdd")}");
            }

        }

        /// <summary>
        /// 個々の単品について入荷予定日を変更する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">発注日：発注日は変更しない</param>
        /// <param name="part">発注対象単品</param>
        /// <param name="lotNo">在庫ロット番号</param>
        /// <param name="newArrivalDate">変更後の入荷予定日</param>
        private void ChangeArrivalDate(MemorieDeFleursDbContext context, DateTime orderDate, BouquetPart part, int lotNo, DateTime newArrivalDate)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{part.Code}, {lotNo}, {newArrivalDate.ToString("yyyyMMdd")}");
            try
            {
                var oldArrivalLot = context.InventoryActions
                    .Where(act => act.PartsCode == part.Code)
                    .Where(act => act.InventoryLotNo == lotNo)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE || act.Action == InventoryActionType.ARRIVED)
                    .Single();

                if(oldArrivalLot.Action == InventoryActionType.ARRIVED)
                {
                    throw new ApplicationException($"単品入荷済み変更不可： {part.Code}.Lot{lotNo}, 入荷日={oldArrivalLot.ArrivalDate:yyyyMMdd}");
                }

                var orderParameter = new InventoryAction()
                {
                    ArrivalDate = newArrivalDate,
                    ActionDate = newArrivalDate,
                    BouquetPart = oldArrivalLot.BouquetPart == null ? context.BouquetParts.Find(oldArrivalLot.PartsCode) : oldArrivalLot.BouquetPart,
                    InventoryLotNo = oldArrivalLot.InventoryLotNo,
                    PartsCode = oldArrivalLot.PartsCode,
                    Quantity = oldArrivalLot.Quantity
                };
                CancelOrder(context, lotNo);
                Order(context, orderDate, orderParameter);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod($"{part.Code}, {lotNo}, {newArrivalDate.ToString("yyyyMMdd")}");
            }
        }
        #endregion // 入荷予定日変更

        #region 単品仕入先の登録改廃
        /// <summary>
        /// 仕入先が部品の提供を開始する
        /// </summary>
        /// <param name="supplierCode">仕入先コード</param>
        /// <param name="partsCode">花コード</param>
        public void StartProvidingParts(int supplierCode, string partsCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    StartProvidingParts(context, supplierCode, partsCode);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 仕入先が部品の提供を開始する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="supplierCode">仕入先コード</param>
        /// <param name="partsCode">花コード</param>
        private void StartProvidingParts(MemorieDeFleursDbContext context, int supplierCode, string partsCode)
        {
            if(context.Suppliers.Find(supplierCode) == null)
            {
                throw new ArgumentException($"仕入先未登録： {supplierCode}");
            }
            if(context.BouquetParts.Find(partsCode) == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            var item = context.PartsSuppliers.Find(supplierCode, partsCode);
            if(item == null)
            {
                context.PartsSuppliers.Add(new PartSupplier() { SupplierCode = supplierCode, PartCode = partsCode });
                context.SaveChanges();
            }
            else
            {
                // 登録済み：何もしない
            }
        }

        /// <summary>
        /// 仕入先が部品の提供を停止する
        /// </summary>
        /// <param name="supplieerCode">仕入先コード</param>
        /// <param name="partsCode">花コード</param>
        public void StopProvidingParts(int supplieerCode, string partsCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    StopProvidingParts(context, supplieerCode, partsCode);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 仕入先が部品の提供を停止する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="supplierCode">仕入先コード</param>
        /// <param name="partsCode">花コード</param>
        public void StopProvidingParts(MemorieDeFleursDbContext context, int supplierCode, string partsCode)
        {
            var supplier = context.Suppliers.Find(supplierCode);
            if (supplier == null)
            {
                throw new ArgumentException($"仕入先未登録： {supplierCode}");
            }
            if (context.BouquetParts.Find(partsCode) == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            var item = context.PartsSuppliers.Find(supplierCode, partsCode);
            if(item == null)
            {
                throw new ArgumentException($"仕入先 {supplierCode} ({supplier.Name}) は単品を提供していない： {partsCode}");
            }
            else
            {
                context.PartsSuppliers.Remove(item);
                context.SaveChanges();
            }
        }
        #endregion // 単品仕入先の登録改廃

        #region 入荷
        /// <summary>
        /// 指定日に指定された発注が指定数量通り入荷されたことを登録する。
        /// </summary>
        /// <param name="date">入荷日</param>
        /// <param name="otherOrders">入荷された発注番号：複数指定した場合の2つめ以降の発注番号</param>
        public void OrdersAreArrived(DateTime date, params string[] otherOrders)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, {string.Join(", ", otherOrders)}");
                try
                {
                    foreach (var order in otherOrders) { OrderIsArrived(context, date, order); }
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    LogUtil.Warn($"{e.GetType().Name}: {e.Message}");
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
        /// 発注情報の入荷処理を行う
        /// </summary>
        /// <param name="date">入荷日</param>
        /// <param name="orderNo">仕入先への発注番号</param>
        public void OrderIsArrived(DateTime date, string orderNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, {orderNo}");
                    OrderIsArrived(context, date, orderNo);
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod($"{date:yyyyMMdd}, {orderNo}");
                }
            }
        }

        private void OrderIsArrived(MemorieDeFleursDbContext context, DateTime date, string orderNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, {orderNo}");

            try
            {
                var order = context.OrdersToSuppliers.Find(orderNo);

                if (order.Status == OrderToSupplierStatus.ARRIVED)
                {
                    throw new ApplicationException($"入荷済み変更不可： {orderNo}");
                }

                var details = context.OrderDetailsToSuppliers
                    .Where(d => d.OrderToSupplierID == orderNo)
                    .OrderBy(d => d.OrderIndex).ToList();

                foreach (var item in details)
                {
                    OrderPartsIsArrived(context, date, orderNo, item);
                }

                order.Status = OrderToSupplierStatus.ARRIVED;
                context.OrdersToSuppliers.Update(order);
                context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }

        private void OrderPartsIsArrived(MemorieDeFleursDbContext context, DateTime date, string orderNo, OrderDetailsToSupplier item)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, {orderNo}, {item.PartsCode}.Lot{item.InventoryLotNo}");

            try
            {
                var arrived = context.InventoryActions
                    .Where(act => act.PartsCode == item.PartsCode)
                    .Where(act => act.InventoryLotNo == item.InventoryLotNo)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE || act.Action == InventoryActionType.ARRIVED)
                    .SingleOrDefault(); // ロットの在庫アクションは発注時点で一式登録されるので、必ず1つ存在するはず。
                if (arrived == null)
                {
                    throw new ApplicationException($"発注分の在庫予定アクションが見つからない： {orderNo}");
                }
                else if (arrived.Action == InventoryActionType.ARRIVED)
                {
                    throw new ApplicationException($"入荷済み変更不可： {orderNo}-{item.OrderIndex}, {item.PartsCode}.Lot{item.InventoryLotNo}");
                }

                if (arrived.ArrivalDate != date)
                {
                    var oldArrivalDate = arrived.ArrivalDate;

                    // 入荷予定日を先に変更する
                    ChangeArrivalDate(context, orderNo, date);

                    arrived = context.InventoryActions
                        .Where(act => act.BouquetPart == item.BouquetPart)
                        .Where(act => act.InventoryLotNo == item.InventoryLotNo)
                        .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .SingleOrDefault();
                    if (arrived == null)
                    {
                        throw new ApplicationException($"入荷日変更後の在庫予定アクションが見つからない： {orderNo}-{item.OrderIndex}" +
                            $", {item.PartsCode}(Lot{item.InventoryLotNo}), {oldArrivalDate:yyyyMMdd}->{date:yyyyMMdd}");
                    }
                }

                // InventoryAction.Action は在庫アクションテーブルのプライマリキーなので UPDATE ではなく DELETE → INSERT する。
                var newArrived = new InventoryAction()
                {
                    Action = InventoryActionType.ARRIVED,
                    ActionDate = arrived.ActionDate,
                    ArrivalDate = arrived.ArrivalDate,
                    BouquetPart = arrived.BouquetPart,
                    InventoryLotNo = arrived.InventoryLotNo,
                    PartsCode = arrived.PartsCode,
                    Quantity = arrived.Quantity,
                    Remain = arrived.Remain
                };
                context.InventoryActions.Remove(arrived);
                context.InventoryActions.Add(newArrived);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }

        }
        #endregion // 入荷

        #region 入荷予定数量変更
        /// <summary>
        /// 入荷予定数量を変更する
        /// </summary>
        /// <param name="orderNo">対象発注番号</param>
        /// <param name="newQuantites">単品毎の更新後の数量：変更のある単品だけでも良い。</param>
        public void ChangeArrivedQuantities(string orderNo, IEnumerable<Tuple<BouquetPart, int>> newQuantites)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    ChangeArrivedQuantities(context, orderNo, newQuantites);
                    transaction.Commit();
                    LogUtil.Info($"QuantityChanged: {orderNo}, changed=[{string.Join(", ", newQuantites.Select(item => $"{item.Item1.Code} -> {item.Item2}"))}]");
                }
                catch(Exception ex)
                {
                    LogUtil.Warn($"{orderNo}: {ex.Message}");
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 入荷予定数量を変更する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderNo">対象発注番号</param>
        /// <param name="newQuantites">単品毎の更新後の数量：ロット数ではなく入荷した数量を指定する。変更のある単品だけでも良い。</param>
        public void ChangeArrivedQuantities(MemorieDeFleursDbContext context, string orderNo, IEnumerable<Tuple<BouquetPart, int>> newQuantites)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{orderNo}, [{string.Join(", ", newQuantites.Select(t => $"{t.Item1.Code} x {t.Item2}"))}]");
            try
            {
                foreach (var item in newQuantites)
                {
                    ChangeArrivedPartQuantity(context, orderNo, item.Item1, item.Item2);
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(orderNo);
            }
        }

        private void ChangeArrivedPartQuantity(MemorieDeFleursDbContext context, string orderNo, BouquetPart parts, int quantity)
        {
            var orderDetail = context.OrderDetailsToSuppliers
                .Where(d => d.OrderToSupplierID == orderNo)
                .SingleOrDefault(d => d.PartsCode == parts.Code);
            if (orderDetail == null)
            {
                throw new NotImplementedException($"注文にない単品： {parts.Code}");
            }

            Parent.BouquetModel.ChangeArrivalQuantity(context, parts.Code, orderDetail.InventoryLotNo, quantity);

        }
        #endregion // 入荷予定数量変更

        #region 発注履歴の取得
        /// <summary>
        /// 発注履歴を取得する
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <returns>発注番号に該当する発注履歴</returns>
        public OrdersToSupplier FindOrder(string orderNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrdersToSuppliers
                    .Where(order => order.ID == orderNo)
                    .Include(o => o.Details)
                    .ThenInclude(d => d.BouquetPart)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// 指定日に入荷する(予定の)発注番号の一覧を取得する
        /// </summary>
        /// <param name="arrivalDate">入荷(予定)日</param>
        /// <returns>指定日に入荷する仕入先発注番号一覧</returns>
        public IEnumerable<string> FindAllOrdersAt(DateTime arrivalDate)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrdersToSuppliers
                    .Where(order => order.DeliveryDate == arrivalDate)
                    .OrderBy(order => order.ID)
                    .Select(order => order.ID)
                    .ToList();
            }
        }

        /// <summary>
        /// 発注日・入荷予定日に関係なくすべての仕入先発注履歴を取得する
        /// </summary>
        /// <returns>仕入先発注履歴</returns>
        public IEnumerable<OrdersToSupplier> FindAllOrders()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrdersToSuppliers
                    .Include(o => o.Details)
                    .ThenInclude(d => d.BouquetPart)
                    .OrderBy(o => o.OrderDate)
                    .ThenBy(o => o.Supplier)
                    .ThenBy(o => o.DeliveryDate)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 発注日が指定範囲内にある仕入先発注履歴を取得する
        /// </summary>
        /// <param name="from">発注日の範囲：この日以降に発注した仕入先発注履歴を抽出する</param>
        /// <param name="to">入発注日の範囲：この日までに発注した仕入先発注履歴を抽出する</param>
        /// <returns>仕入先発注履歴</returns>
        public IEnumerable<OrdersToSupplier> FindAllOrders(DateTime from, DateTime to)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrdersToSuppliers
                    .Where(order => from <= order.OrderDate && order.OrderDate <= to)
                    .Include(o => o.Details)
                    .ThenInclude(d => d.BouquetPart)
                    .OrderBy(order => order.OrderDate)
                    .ThenBy(order => order.Supplier)
                    .ThenBy(order => order.DeliveryDate)
                    .ToList()
                    .AsEnumerable();
            }
        }
        /// <summary>
        /// 発注日が指定範囲内にある仕入先発注履歴を、仕入先を絞って取得する
        /// </summary>
        /// <param name="from">発注日の範囲：この日以降に発注した仕入先発注履歴を抽出する</param>
        /// <param name="to">入発注日の範囲：この日までに発注した仕入先発注履歴を抽出する</param>
        /// <param name="supplier">絞り込み対象仕入先ID</param>
        /// <returns>仕入先発注履歴</returns>
        public IEnumerable<OrdersToSupplier> FindAllOrders(DateTime from, DateTime to, int supplier)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrdersToSuppliers
                    .Where(order => from <= order.OrderDate && order.OrderDate <= to)
                    .Where(order => order.Supplier == supplier)
                    .Include(o => o.Details)
                    .ThenInclude(d => d.BouquetPart)
                    .OrderBy(order => order.OrderDate)
                    .ThenBy(order => order.DeliveryDate)
                    .ToList()
                    .AsEnumerable();
            }
        }
        #endregion // 発注履歴取得

        #region 入荷処理
        /// <summary>
        /// 特定仕入先の指定日に入荷する単品とそのロット数を取得する
        /// </summary>
        /// <param name="supplier">仕入先</param>
        /// <param name="date">入荷(予定)日</param>
        /// <returns>単品毎のロット数一覧：入荷予定のない単品も、入荷ロット数=0で登録されている</returns>
        public IDictionary<string, int> GetArrivalBouquetPartsCountAt(int supplier, DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                var found = context.OrdersToSuppliers
                    .Where(o => o.Supplier == supplier)
                    .Where(o => o.DeliveryDate == date)
                    .Include(o => o.Details)
                    .SelectMany(o => o.Details)
                    .AsEnumerable()
                    .GroupBy(d => d.PartsCode)
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.LotCount));

                var all = context.PartsSuppliers
                    .Where(p => p.SupplierCode == supplier)
                    .Select(p => p.PartCode);

                foreach(var part in all)
                {
                    if(!found.ContainsKey(part))
                    {
                        found.Add(part, 0);
                    }
                }
                return found;
            }
        }
        #endregion // 入荷処理
    }
}
