using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

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
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var part = Create(context);
                        transaction.Commit();
                        return part;
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            /// <summary>
            /// 現在の内容で単品オブジェクトを生成、データベースに登録する
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>生成/データベース登録された単品オブジェクト</returns>
            public BouquetPart Create(MemorieDeFleursDbContext context)
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

                context.BouquetParts.Add(p);
                context.SaveChanges();

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
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var bouquet = Create(context);
                        transaction.Commit();
                        return bouquet;
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            /// <summary>
            /// 現在の内容で商品オブジェクトを生成、データベースに登録する
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>データベースに登録された商品オブジェクト</returns>
            public Bouquet Create(MemorieDeFleursDbContext context)
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

                context.Bouquets.Add(ret);
                context.SaveChanges();
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
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.BouquetParts.Find(partCode);
            }
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
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                var bouquet = context.Bouquets.Find(bouquetCode);

                // [NOTE]
                // 以下の２点を追加しても、Bouquet.PartsList[i].Parts が正しくセットされない状況は変わらない。
                // （いずれにせよ商品を構成する単品を Find() でキャッシュに登録する必要がある）
                // 
                // 1) BouquetParts:
                //        public IList<BouquetPartsList> Bouquets {get;} = new List<BouquetPartsList>();
                //
                // 2) DbContext.OnModelCreating(modelBuilder):
                //        modelBuilder.Entity<BouquetPartsList>()
                //            .HasOne(i => i.Parts)
                //            .WithMany(b => b.Bouquets)
                //            .HasForeginKye(i => i.PartsCode)
                //
                // 3) 単品をキャッシュに読み込む
                foreach (var parts in context.PartsList.Where(i => i.BouquetCode == bouquetCode))
                {
                    parts.Part = context.BouquetParts.Find(parts.PartsCode);
                }
                return bouquet;
            }
        }
        #endregion // 商品の登録改廃

        #region UseBouquetPart
        /// <summary>
        /// 在庫から使用した単品を指定個数取り去る
        /// </summary>
        /// <param name="part">対象となる単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantity">取り去る数量</param>
        public void UseBouquetPart(BouquetPart part, DateTime date, int quantity)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    UseBouquetPart(context, part, date, quantity);
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
        /// 在庫から使用した単品を指定個数取り去る：同一トランザクション内で複数の登録削除を行う時用
        /// </summary>
        /// <param name="context">トランザクションコンテキスト</param>
        /// <param name="part">対象となる単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantity">取り去る数量</param>
        public void UseBouquetPart(MemorieDeFleursDbContext context, BouquetPart part, DateTime date, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{part.Code}, {date.ToString("yyyyMMdd")}, {quantity}");
            try
            {
                var inventory = context.InventoryActions
                    .Where(a => a.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .Where(a => 0 == a.ActionDate.CompareTo(date))
                    .Where(a => a.Remain > 0)
                    .OrderBy(a => a.ArrivalDate)
                    .FirstOrDefault();

                if (inventory == null)
                {
                    throw new NotImplementedException($"該当ストックなし：基準日={date.ToString("yyyyMMdd")}, 花コード{part.Code}, 数量={quantity}");
                }
                else
                {
                    var usedLot = new Stack<int>();
                    UseFromThisLot(context, inventory, quantity, usedLot);
                }

                context.SaveChanges();
                LogUtil.Info($"{date:yyyyMMdd}, {part.Code} x {quantity} used.");
            }
            catch (NotImplementedException ei)
            {
                LogUtil.Fatal($"★未実装★ {ei.Message}");
                throw;
            }
            catch (Exception e)
            {
                LogUtil.Error($"UseBouquetPart(part={part.Code}, date={date.ToString("yyyyMMdd")}, quantity={quantity}) failed. {e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }

        public void UseFromThisLot(MemorieDeFleursDbContext context, InventoryAction today, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"today={today.ToString("s")}, quantity={quantity}, usedLot={string.Join(",", usedLot)}");

            var theLot = context.InventoryActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.InventoryLotNo == today.InventoryLotNo)
                .Where(act => act.ActionDate >= today.ActionDate)
                .ToList();

            LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(today, quantity);
            if (today.Remain >= quantity)
            {
                // 全量引き出せる
                LogUtil.DEBUGLOG_InventoryActionQuantityChangingTo(today, quantity);

                today.Quantity += quantity;
                today.Remain -= quantity;
                context.InventoryActions.Update(today);

            }
            else
            {
                // 残数分はこのロットから、それ以外は他のロットから引き出す
                LogUtil.DEBUGLOG_InventoryActionQuantityChangingTo(today, today.Remain);

                var useFromThisLot = today.Remain;
                var useFromOtherLot = quantity - today.Remain;
                today.Quantity += useFromThisLot;
                today.Remain -= useFromThisLot;
                context.InventoryActions.Update(today);

                try
                {
                    usedLot.Push(today.InventoryLotNo);
                    UseFromOtherLot(context, today, useFromOtherLot, usedLot);
                }
                catch (InventoryShortageException eis)
                {
                    context.InventoryActions.Add(eis.InventoryShortageAction);
                    LogUtil.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                }
                usedLot.Pop();
            }

            var previousRemain = today.Remain;
            foreach (var action in theLot
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate > today.ActionDate)
                .OrderBy(act => act.ActionDate))
            {
                LogUtil.DEBUGLOG_ComparisonOfInventoryQuantityAndPreviousRemain(action, previousRemain);
                if(previousRemain >= action.Quantity)
                {
                    // 全量引き出せる
                    LogUtil.DEBUGLOG_InventoryActionQuantityChanged(action, action.Quantity, previousRemain - action.Quantity);

                    action.Remain = previousRemain - action.Quantity;
                    context.InventoryActions.Update(action);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    // 前日残の分はこのロットから、それ以外は他のロットから引き出す
                    LogUtil.DEBUGLOG_InventoryActionQuantityChanged(action, previousRemain, 0);

                    var usedFromThisLot = previousRemain;
                    var useFromOtherLot = action.Quantity - previousRemain;

                    action.Quantity = usedFromThisLot;
                    action.Remain = 0;
                    context.InventoryActions.Update(action);

                    try
                    {
                        usedLot.Push(action.InventoryLotNo);
                        UseFromOtherLot(context, action, useFromOtherLot, usedLot);
                    }
                    catch (InventoryShortageException eis)
                    {
                        context.InventoryActions.Add(eis.InventoryShortageAction);
                        LogUtil.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                    }
                    previousRemain = 0;
                }
            }

            var discard = theLot.Single(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
            discard.Quantity = previousRemain;
            context.InventoryActions.Update(discard);

            LogUtil.DEBUGLOG_EndMethod();
        }

        public void UseFromOtherLot(MemorieDeFleursDbContext context, InventoryAction inventory, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"inventory={inventory.ToString("s")}, quantity={quantity}, usedLot={string.Join(",", usedLot)}");

            var usableLots = context.InventoryActions
                .Where(act => act.PartsCode == inventory.PartsCode)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate == inventory.ActionDate)
                //.Where(act => !usedLot.Contains(act.InventoryLotNo)) // Linq式の中で Stack<>.Contains() などのメソッド呼び出しはできない
                .Where(act => act.Remain > 0)
                .ToList();

            var useToThisLot = quantity;
            var previousLot = inventory;

            foreach(var action in usableLots.OrderBy(act => act.ArrivalDate))
            {
                // すでに引当対象としたロットは除外：Linq式で usableLots を生成するタイミングでは除外できなかったため。
                if(usedLot.Contains(action.InventoryLotNo)) { continue; }

                LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(action, useToThisLot);
                if(action.Remain >= useToThisLot)
                {
                    // このロットから全量引き出す
                    LogUtil.DEBUGLOG_InventoryActionQuantityChangingTo(action, useToThisLot);

                    UseFromThisLot(context, action, quantity, usedLot);
                    LogUtil.DEBUGLOG_EndMethod(msg: $"resolved");
                    return;
                }
                else
                {
                    // 残数分はこのロットから、引き出せなかった分は次のロットから引き出す
                    LogUtil.DEBUGLOG_InventoryActionQuantityChangingTo(action, action.Remain);

                    useToThisLot -= action.Remain;
                    UseFromThisLot(context, action, action.Remain, usedLot);
                    previousLot = action;
                }
            }

            if(useToThisLot > 0)
            {
                LogUtil.Warn($"Inventory shortage detected. {previousLot.ToString("h")}, quantity={useToThisLot}");
                LogUtil.DEBUGLOG_EndMethod();
                throw new InventoryShortageException(previousLot, useToThisLot);
            }

            LogUtil.DEBUGLOG_EndMethod();
        }
        #endregion // UseBouquetPart

        #region 商品構成の追加削除
        #region 新規作成・追加
        /// <summary>
        /// 登録済み商品の商品構成に、登録済みの単品を追加する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        /// <param name="quantity">数量</param>
        public void AppendPartsTo(string bouquetCode, string partsCode, int quantity)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    AppendPartsTo(context, bouquetCode, partsCode, quantity);
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
        /// 登録済み商品の商品構成に，登録済みの単品を追加する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        /// <param name="quantity">数量</param>
        private void AppendPartsTo(MemorieDeFleursDbContext context, string bouquetCode, string partsCode, int quantity)
        {
            if (context.Bouquets.Find(bouquetCode) == null)
            {
                throw new ArgumentException($"花束未登録： {bouquetCode}");
            }
            if (context.BouquetParts.Find(partsCode) == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            CreateOrUpdatePartsList(context, bouquetCode, partsCode, quantity);
        }

        private void CreateOrUpdatePartsList(MemorieDeFleursDbContext context, string bouquetCode, string partsCode, int quantity)
        {
            if(quantity <= 0)
            {
                throw new ArgumentException($"本数不正： {bouquetCode}.{partsCode}, {quantity}");
            }

            var item = context.PartsList.Find(bouquetCode, partsCode);
            if(item == null)
            {
                context.PartsList.Add(new BouquetPartsList() { BouquetCode = bouquetCode, PartsCode = partsCode, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
                context.PartsList.Update(item);
            }
            context.SaveChanges();
        }


        /// <summary>
        /// 商品構成を新規作成する
        /// </summary>
        /// <param name="bouquetCode">花束コード：商品はあらかじめ登録されていなければならない。</param>
        /// <param name="partsList">単品の花コードと使用数量の一覧：単品はあらかじめ登録されていなければならない。</param>
        /// <remarks><see cref="BouquetBuilder"/> を使って商品を登録する時は <see cref="BouquetBuilder.Uses(string, int))"/>
        /// または <see cref="BouquetBuilder.Uses(BouquetPart, int)"/> を使用する。
        /// </remarks>
        public void CreatePartsListOf(string bouquetCode, IEnumerable<KeyValuePair<string, int>> partsList)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    CreatePartsListOf(context, bouquetCode, partsList);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 商品構成を新規作成する
        /// 
        /// トランザクション内での呼出用。
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="bouquetCode">花束コード：商品はあらかじめ登録されていなければならない。</param>
        /// <param name="partsList">単品の花コードと使用数量の一覧：単品はあらかじめ登録されていなければならない。</param>
        public void CreatePartsListOf(MemorieDeFleursDbContext context, string bouquetCode, IEnumerable<KeyValuePair<string, int>> partsList)
        {
            if (context.Bouquets.Find(bouquetCode) == null)
            {
                throw new ArgumentException($"花束未登録： {bouquetCode}");
            }
            foreach (var item in partsList)
            {
                if (context.BouquetParts.Find(item.Key) == null)
                {
                    throw new ArgumentException($"単品未登録： {item.Key}");
                }
                CreateOrUpdatePartsList(context, bouquetCode, item.Key, item.Value);
            }
            context.SaveChanges();
        }
        #endregion // 新規作成・追加

        #region 削除
        /// <summary>
        /// 指定商品の商品構成から指定した単品を除外する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        /// <exception cref="ArgumentException">
        /// コードに該当する単品または商品が存在しない、単品が商品構成にない
        /// </exception>
        public void RemovePartsFrom(string bouquetCode, string partsCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    RemovePartsFrom(context, bouquetCode, partsCode);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 指定商品の商品構成から指定した単品を除外する
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        private void RemovePartsFrom(MemorieDeFleursDbContext context, string bouquetCode, string partsCode)
        {
            if (context.Bouquets.Find(bouquetCode) == null)
            {
                throw new ArgumentException($"花束未登録： {bouquetCode}");
            }
            if (context.BouquetParts.Find(partsCode) == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            var item = context.PartsList.Find(bouquetCode, partsCode);
            if(item == null)
            {
                throw new ArgumentException($"{partsCode} は {bouquetCode} の構成要素ではない");
            }

            context.PartsList.Remove(item);
            context.SaveChanges();
        }

        /// <summary>
        /// 指定商品の商品構成をすべて破棄する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        public void RemoveAllPartsFrom(string bouquetCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    RemoveAllPartsFrom(context, bouquetCode);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 指定商品の商品構成をすべて破棄する
        /// 
        /// トランザクション内での呼出用。
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        private void RemoveAllPartsFrom(MemorieDeFleursDbContext context, string bouquetCode)
        {
            var pats = context.PartsList.Where(item => item.BouquetCode == bouquetCode).ToList();
            context.PartsList.RemoveRange(pats);
            context.SaveChanges();
        }
        #endregion // 削除

        #region 数量更新
        /// <summary>
        /// 商品構成数量を変更する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        /// <param name="newQuantity">数量</param>
        public void UpdateQuantityOf(string bouquetCode, string partsCode, int newQuantity)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    UpdateQuantityOf(context, bouquetCode, partsCode, newQuantity);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 商品構成数量を変更する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="bouquetCode">花束コード</param>
        /// <param name="partsCode">花コード</param>
        /// <param name="newQuantity">数量</param>
        private void UpdateQuantityOf(MemorieDeFleursDbContext context, string bouquetCode, string partsCode, int newQuantity)
        {
            if (context.Bouquets.Find(bouquetCode) == null)
            {
                throw new ArgumentException($"花束未登録： {bouquetCode}");
            }
            if (context.BouquetParts.Find(partsCode) == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            var item = context.PartsList.Find(bouquetCode, partsCode);
            if(item == null)
            {
                throw new ArgumentException($"該当構成なし： {bouquetCode} - {partsCode}");
            }

            item.Quantity = newQuantity;
            context.PartsList.Update(item);
            context.SaveChanges();
        }
        #endregion // 数量更新
        #endregion // 商品構成の追加削除
    }
}
