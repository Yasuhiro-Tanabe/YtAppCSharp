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

        /// <summary>
        /// 単品引当中に発生した在庫不足アクション一覧
        /// 
        /// 引当処理、とくにロット内日付方向への在庫払出展開処理では、
        /// 在庫不足発生時は在庫不足アクションを登録して先に進むため例外がスローされない。
        /// 都合の良いタイミングで在庫不足例外をスローするため、
        /// 登録された在庫不足アクションを逐次このリストに登録しておく。
        /// </summary>
        public IList<InventoryAction> ShortageInventories { get; } = new List<InventoryAction>();

        #region UseFromInventory
        /// <summary>
        /// 在庫から使用した単品を指定個数取り去る
        /// </summary>
        /// <param name="part">対象となる単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantity">取り去る数量</param>
        public void UseFromInventory(BouquetPart part, DateTime date, int quantity)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    UseFromInventory(context, part, date, quantity);
                    transaction.Commit();
                    LogUtil.Info($"{date:yyyyMMdd}, {part.Code} x {quantity} used.");
                }
                catch (Exception)
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
        public void UseFromInventory(MemorieDeFleursDbContext context, BouquetPart part, DateTime date, int quantity)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{part.Code}, {date.ToString("yyyyMMdd")}, {quantity}");
            try
            {
                ShortageInventories.Clear();

                var inventory = context.InventoryActions
                    .Where(a => a.PartsCode == part.Code)
                    .Where(a => a.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .Where(a => 0 == a.ActionDate.CompareTo(date))
                    .Where(a => a.Remain > 0)
                    .OrderBy(a => a.ArrivalDate)
                    .FirstOrDefault();

                if (inventory == null)
                {
                    var shortage = new InventoryAction()
                    {
                        Action = InventoryActionType.SHORTAGE,
                        ActionDate = date,
                        ArrivalDate = date,
                        InventoryLotNo = -1,
                        BouquetPart = part,
                        PartsCode = part.Code,
                        Quantity = quantity,
                        Remain = -quantity
                    };
                    ShortageInventories.Add(shortage);
                    throw new InventoryShortageException(shortage, quantity);
                }
                else
                {
                    var usedLot = new Stack<int>();
                    UseFromThisLot(context, inventory, quantity, usedLot);
                }

                context.SaveChanges();
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

            try
            {
                UseFromThisLotToday(context, today, quantity, usedLot);
                UseFromPreviousRemain(context, today, today.ActionDate.AddDays(1), usedLot);
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

        private void UseFromPreviousRemain(MemorieDeFleursDbContext context, InventoryAction today, DateTime startDate, Stack<int> usedLot)
        {
            var previousRemain = today.Remain;
            foreach (var action in context.InventoryActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.InventoryLotNo == today.InventoryLotNo)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate >= startDate)
                .OrderBy(act => act.ActionDate))
            {
                LogUtil.DEBUGLOG_ComparationOfInventoryQuantityAndPreviousRemain(action, previousRemain);
                if (previousRemain >= action.Quantity)
                {
                    var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain };

                    // 全量引き出せる
                    action.Remain = previousRemain - action.Quantity;
                    context.InventoryActions.Update(action);

                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);

                    previousRemain -= action.Quantity;
                }
                else
                {
                    var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain }; // デバッグログ用

                    // 前日残の分はこのロットから、それ以外は他のロットから引き出す
                    var usedFromThisLot = previousRemain;
                    var useFromOtherLot = action.Quantity - previousRemain;

                    action.Quantity = usedFromThisLot;
                    action.Remain = 0;
                    context.InventoryActions.Update(action);
                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);

                    try
                    {
                        usedLot.Push(action.InventoryLotNo);
                        UseFromOtherLot(context, action, useFromOtherLot, usedLot);
                    }
                    catch (InventoryShortageException eis)
                    {
                        ShortageInventories.Add(eis.InventoryShortageAction);
                        context.InventoryActions.Add(eis.InventoryShortageAction);
                        LogUtil.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                    }
                    previousRemain = 0;
                }
            }

            var discard = context.InventoryActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.InventoryLotNo == today.InventoryLotNo)
                .Single(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
            discard.Quantity = previousRemain;
            context.InventoryActions.Update(discard);
        }

        private void UseFromThisLotToday(MemorieDeFleursDbContext context, InventoryAction today, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(today, quantity);
            if (today.Remain >= quantity)
            {
                // 全量引き出せる
                today.Quantity += quantity;
                today.Remain -= quantity;
                context.InventoryActions.Update(today);

                LogUtil.DEBUGLOG_InventoryActionChanged(today, quantity);

            }
            else
            {
                var oldAction = new InventoryAction() { Quantity = today.Quantity, Remain = today.Remain }; // デバッグログ用

                // 残数分はこのロットから、それ以外は他のロットから引き出す
                var useFromThisLot = today.Remain;
                var useFromOtherLot = quantity - today.Remain;
                today.Quantity += useFromThisLot;
                today.Remain -= useFromThisLot;
                context.InventoryActions.Update(today);

                LogUtil.DEBUGLOG_InventoryActionChanged(today, oldAction);

                try
                {
                    usedLot.Push(today.InventoryLotNo);
                    UseFromOtherLot(context, today, useFromOtherLot, usedLot);
                }
                catch (InventoryShortageException eis)
                {
                    ShortageInventories.Add(eis.InventoryShortageAction);
                    context.InventoryActions.Add(eis.InventoryShortageAction);
                    LogUtil.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                }
                usedLot.Pop();
            }
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

                var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain }; // デバッグログ用

                LogUtil.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(action, useToThisLot);
                if(action.Remain >= useToThisLot)
                {

                    // このロットから全量引き出す
                    UseFromThisLot(context, action, quantity, usedLot);

                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);
                    LogUtil.DEBUGLOG_EndMethod(msg: $"resolved");
                    return;
                }
                else
                {
                    // 残数分はこのロットから、引き出せなかった分は次のロットから引き出す
                    useToThisLot -= action.Remain;
                    UseFromThisLot(context, action, action.Remain, usedLot);
                    previousLot = action;

                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);
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
        #endregion // UseFromInventory

        #region BackToInventory
        /// <summary>
        /// 指定数量を在庫に戻す
        /// </summary>
        /// <param name="part">対象単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantityToReturn">在庫に戻す数量</param>
        /// <remarks>
        /// 複数ロットから払い出した在庫を適切に戻せないので
        /// 使用数量を負数 (10本戻す→－10本使用する) にして <see cref="UseFromInventory(BouquetPart, DateTime, int)"/> を呼び出さないこと。
        /// 代わりにこのメソッドを呼び出すこと
        /// </remarks>
        public void ReturnToInventory(BouquetPart part, DateTime date, int quantityToReturn)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    ReturnToInventory(context, part, date, quantityToReturn);
                    transaction.Commit();
                    LogUtil.Info($"{date:yyyyMMdd}, {part.Code} x {quantityToReturn} backed to inventory.");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

            }
        }

        /// <summary>
        /// 指定数量を在庫に戻す、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="part">対象単品</param>
        /// <param name="date">日付</param>
        /// <param name="quantityToReturn">在庫に戻す数量</param>
        /// <remarks>
        /// 複数ロットから払い出した在庫を適切に戻せないので
        /// 使用数量を負数 (10本戻す→－10本使用する) にして
        /// <see cref="UseFromInventory(MemorieDeFleursDbContext, BouquetPart, DateTime, int)"/> を呼び出さないこと。
        /// 代わりにこのメソッドを呼び出すこと
        /// </remarks>
        public void ReturnToInventory(MemorieDeFleursDbContext context, BouquetPart part, DateTime date, int quantityToReturn)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{part.Code}, {date:yyyyMMdd}, {quantityToReturn}");
            try
            {
                ShortageInventories.Clear();

                var inventory = context.InventoryActions
                    .Where(a => a.PartsCode == part.Code)
                    .Where(a => a.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .Where(a => a.ActionDate == date)
                    .Where(a => a.Quantity > 0)
                    .OrderByDescending(a => a.ArrivalDate) // 入荷(予定)日の新しい在庫から戻していく
                    .FirstOrDefault();

                if (inventory == null)
                {
                    throw new NotImplementedException($"該当在庫なし：{part.Code}, 基準日 {date:yyyyMMdd}");
                }
                else
                {
                    var returnedLot = new Stack<int>();
                    ReturnToThisLot(context, inventory, quantityToReturn, returnedLot);
                }

                context.SaveChanges();
            }
            catch (NotImplementedException ei)
            {
                LogUtil.Fatal($"★未実装★ {ei.Message}");
                throw;
            }
            catch (Exception e)
            {
                LogUtil.Error($"{nameof(ReturnToInventory)}({part.Code}, {date:yyyyMMdd}, {quantityToReturn}) faled cause of unexpected exception, " +
                    $"{e.GetType().Name}: {e.Message}\n{e.StackTrace}");
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }

        }

        public void ReturnToThisLot(MemorieDeFleursDbContext context, InventoryAction today, int quantityToReturn, Stack<int> returnedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{today.ToString("s")}, {today:yyyyMMdd}, {quantityToReturn}, [{string.Join(", ", returnedLot)}]");
            try
            {
                ReturnToThisLotToday(context, today, quantityToReturn, returnedLot);

                // 翌日以降の数量引当は在庫払出時と同じ。基準日の前日残が変わったので「引当」をやりなおす。
                UseFromPreviousRemain(context, today, today.ActionDate.AddDays(1), returnedLot);
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

        private void ReturnToThisLotToday(MemorieDeFleursDbContext context, InventoryAction today, int quantityToReturn, Stack<int> returnedLot)
        {
            LogUtil.DEBUGLOG_ComparationOfInventoryUsedAndReturns(today, quantityToReturn);
            if (today.Quantity >= quantityToReturn)
            {
                // 全量戻せる
                today.Quantity -= quantityToReturn;
                today.Remain += quantityToReturn;
                context.InventoryActions.Update(today);
                LogUtil.DEBUGLOG_InventoryActionChanged(today, -quantityToReturn);
            }
            else
            {
                // 戻せる分だけこのロットに戻し、残りは他のロットに戻す
                var returnToThisLot = today.Quantity;
                var returnToOtherLot = quantityToReturn - returnToThisLot;

                today.Quantity -= returnToThisLot;
                today.Remain += returnToThisLot;
                context.InventoryActions.Update(today);
                LogUtil.DEBUGLOG_InventoryActionChanged(today, -returnToThisLot);

                try
                {
                    returnedLot.Push(today.InventoryLotNo);
                    ReturnToOtherLot(context, today, returnToOtherLot, returnedLot);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    returnedLot.Pop();
                }

            }
        }

        private void ReturnToOtherLot(MemorieDeFleursDbContext context, InventoryAction inventory, int quantityToReturn, Stack<int> returnedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{inventory.ToString("s")}, {quantityToReturn}, [{string.Join(", ", returnedLot)}]");

            var candidates = context.InventoryActions
                .Where(act => act.PartsCode == inventory.PartsCode)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate == inventory.ActionDate)
                .Where(act => act.Quantity > 0)
                .OrderByDescending(act => act.ArrivalDate) // 入荷(予定日)が遠い先の在庫から順次戻していく
                .ToList();

            var returnToThisLot = quantityToReturn;
            var previousLot = inventory;

            foreach (var action in candidates)
            {
                // すでに引当対象としたロットは除外：candidates 抽出時の式はSQLに変換されるため
                if (returnedLot.Contains(action.InventoryLotNo)) { continue; }

                var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain }; // デバッグログ用

                LogUtil.DEBUGLOG_ComparationOfInventoryUsedAndReturns(action, returnToThisLot);
                if (action.Quantity >= returnToThisLot)
                {
                    // このロットに全量戻す
                    ReturnToThisLot(context, action, quantityToReturn, returnedLot);

                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);
                    LogUtil.DEBUGLOG_EndMethod(msg: $"resolved");
                    return;
                }
                else
                {
                    // 加工数量分はこのロットに、戻し残った分は次のロットに戻す
                    returnToThisLot -= action.Quantity;
                    ReturnToThisLot(context, action, action.Remain, returnedLot);
                    previousLot = action;

                    LogUtil.DEBUGLOG_InventoryActionChanged(action, oldAction);
                }
            }

            if (returnToThisLot > 0)
            {
                throw new NotImplementedException($"ロットが少なく全量を在庫に戻せない：最終確認ロット={previousLot.PartsCode}.Lot{previousLot.InventoryLotNo}" +
                    $", 戻し残={returnToThisLot}");
            }

            LogUtil.DEBUGLOG_EndMethod();
        }
        #endregion // BackToInventory

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

        #region 入荷予定数量変更
        /// <summary>
        /// 入荷予定数量を変更し、必要に応じて在庫払出展開する、トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="partsCode">花コード</param>
        /// <param name="lotNo">発注ロット番号(=在庫ロット番号)</param>
        /// <param name="newQuantity">変更後の入荷数量</param>
        public void ChangeArrivalQuantity(MemorieDeFleursDbContext context, string partsCode, int lotNo, int newQuantity)
        {
            var arrived = context.InventoryActions
                .Where(act => act.PartsCode == partsCode)
                .Where(act => act.InventoryLotNo == lotNo)
                .SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE || act.Action == InventoryActionType.ARRIVED);

            if (arrived == null)
            {
                throw new NotImplementedException($"対象ロットが見つからない：{partsCode}.Lot{lotNo}");
            }
            else if (arrived.Action == InventoryActionType.ARRIVED)
            {
                throw new NotImplementedException($"入荷済み変更不可：{partsCode}.Lot{lotNo}");
            }

            if (arrived.Quantity == newQuantity)
            {
                // 変更不要
                return;
            }

            arrived.Quantity = newQuantity;
            arrived.Remain = newQuantity;
            context.InventoryActions.Update(arrived);

            var usedLot = new Stack<int>();
            Parent.BouquetModel.UseFromPreviousRemain(context, arrived, arrived.ArrivalDate, usedLot);
            context.SaveChanges();
        }
        #endregion // 入荷予定数量変更

        #region 出荷数量変更
        /// <summary>
        /// 指定日付の全受注に対し出荷確定処理を行う：
        /// 
        /// 受注ステータスを「出荷済(SHIPPED)」に変更する
        /// 当日分の加工予定在庫アクションを加工済在庫アクションに変更する
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="date">出荷確定処理を行う日</param>
        public void ChangeAllScheduledPartsOfTheDayUsed(MemorieDeFleursDbContext context, DateTime date)
        {
            LogUtil.DEBUGLOG_BeginMethod(date.ToString("yyyyMMdd"));

            try
            {
                foreach (var action in context.InventoryActions
                    .Where(act => act.ActionDate == date)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .ToList())
                {
                    // Action プロパティは InventoryAction テーブルのプライマリキーなので
                    // UPDATE ではなく DELETE → INSERT する。
                    var newAction = new InventoryAction()
                    {
                        Action = InventoryActionType.USED,
                        ActionDate = action.ActionDate,
                        ArrivalDate = action.ArrivalDate,
                        BouquetPart = action.BouquetPart,
                        InventoryLotNo = action.InventoryLotNo,
                        PartsCode = action.PartsCode,
                        Quantity = action.Quantity,
                        Remain = action.Remain
                    };
                    context.InventoryActions.Remove(action);
                    context.InventoryActions.Add(newAction);
                }

                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(date.ToString("yyyyMMdd"));
            }
        }

        /// <summary>
        /// 指定単品の指定数量分を加工予定から加工済に変更する
        /// 
        /// この処理を行うと、同一日の加工予定在庫アクションと加工済在庫アクションが両方並存することになる。
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="date">在庫アクションの状態変更対象日</param>
        /// <param name="partsQuantity">単品の商品コードと加工済への変更数量</param>
        public void UpdatePartsUsedQuantity(MemorieDeFleursDbContext context, DateTime date, KeyValuePair<string, int> partsQuantity)
        {
            // 指定単品指定日の使用予定・使用在庫アクションをロット毎にグループ化
            var inventories = context.InventoryActions
                .Where(act => act.ActionDate == date)
                .Where(act => act.PartsCode == partsQuantity.Key)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE || act.Action == InventoryActionType.USED)
                .AsEnumerable()
                .GroupBy(act => act.ArrivalDate)
                .ToDictionary(grp => grp.Key, grp => grp.AsEnumerable().GroupBy(act => act.InventoryLotNo).ToDictionary(grp => grp.Key, grp => grp.ToList()));

            var previousUsed = partsQuantity.Value;

            foreach(var act1 in inventories)
            {
                foreach(var act2 in act1.Value)
                {
                    // 使用予定在庫アクションから使用在庫アクションに partsQuantity で指定された数量を移し替える
                    var used = act2.Value.SingleOrDefault(act => act.Action == InventoryActionType.USED);
                    var scheduled = act2.Value.SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_USE);
                    if(scheduled == null)
                    {
                        // このロットから在庫を移し替えることができない：次のロットにすすむ
                        continue;
                    }

                    if(used == null)
                    {
                        used = new InventoryAction()
                        {
                            Action = InventoryActionType.USED,
                            ActionDate = scheduled.ActionDate,
                            ArrivalDate = scheduled.ArrivalDate,
                            BouquetPart = scheduled.BouquetPart,
                            InventoryLotNo = scheduled.InventoryLotNo,
                            PartsCode = scheduled.PartsCode,
                            Quantity = 0,
                            Remain = scheduled.Remain
                        };
                        UpdateAndAddInventoryActions(context, used, scheduled, ref previousUsed);
                    }
                    else
                    {
                        UpdateInventoryActions(context, used, scheduled, ref previousUsed);
                    }
                }
            }
        }

        private void UpdateAndAddInventoryActions(MemorieDeFleursDbContext context, InventoryAction used, InventoryAction scheduled, ref int quantity)
        {
            UpdateQuantity(used, scheduled, ref quantity);
            UpdateScheduledToUseInventoryAction(context, scheduled);

            context.InventoryActions.Add(used);
            LogUtil.DEBUGLOG_InventoryActionCreated(used);
        }

        private void UpdateInventoryActions(MemorieDeFleursDbContext context, InventoryAction used, InventoryAction scheduled, ref int quantity)
        {
            var oldUsed = new InventoryAction() { Quantity = used.Quantity, Remain = used.Remain }; // デバッグ用、変更前の数量残数

            UpdateQuantity(used, scheduled, ref quantity);
            UpdateScheduledToUseInventoryAction(context, scheduled);

            context.InventoryActions.Update(used);
            LogUtil.DEBUGLOG_InventoryActionChanged(used, oldUsed);
        }

        private void UpdateQuantity(InventoryAction used, InventoryAction scheduled, ref int quantity)
        {
            var oldScheduled = new InventoryAction() { Quantity = scheduled.Quantity, Remain = scheduled.Remain }; // デバッグ用、変更前の数量残数

            if(scheduled.Quantity > quantity)
            {
                scheduled.Quantity -= quantity;
                used.Quantity += quantity;
                quantity = 0;

                if(scheduled.Quantity == 0)
                {
                    scheduled.Remain = 0;
                }
            }
            else
            {
                quantity -= scheduled.Quantity;
                used.Quantity += scheduled.Quantity;
                scheduled.Quantity = 0;
                scheduled.Remain = 0;
            }

            LogUtil.DEBUGLOG_InventoryActionChanged(scheduled, oldScheduled);
        }
        
        private void UpdateScheduledToUseInventoryAction(MemorieDeFleursDbContext context, InventoryAction scheduled)
        {
            if(scheduled.Quantity == 0 && scheduled.Remain ==0)
            {
                context.InventoryActions.Remove(scheduled);
                LogUtil.DEBUGLOG_InventoryActionRemoved(scheduled);
            }
            else
            {
                context.InventoryActions.Update(scheduled);
            }
        }
        #endregion // 出荷数量変更

        #region 単品破棄
        public void DiscardBouquetParts(DateTime date, params Tuple<string, int>[] discardParts)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                var partsString = string.Join(", ", discardParts.Select(p => $"{p.Item1} x {p.Item2}"));
                LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, [ {partsString} ]");

                try
                {
                    DiscardBouquetParts(context, date, discardParts);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, [ {partsString} ]");
                }
            }

        }

        private void DiscardBouquetParts(MemorieDeFleursDbContext context, DateTime date, params Tuple<string, int>[] discardParts)
        {
            foreach(var parts in discardParts)
            {
                DiscardBouquetParts(context, date, parts.Item1, parts.Item2);
            }
        }

        private void DiscardBouquetParts(MemorieDeFleursDbContext context, DateTime date, string partsCode, int discardQuantity)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, {partsCode}, {discardQuantity}");
            try
            {
                var expectedActions = new SortedSet<InventoryActionType>()
            {
                InventoryActionType.DISCARDED,
                InventoryActionType.SCHEDULED_TO_DISCARD,
                InventoryActionType.SCHEDULED_TO_USE,
                InventoryActionType.USED
            };
                var inventories = context.InventoryActions
                    .Where(act => act.PartsCode == partsCode)
                    .Where(act => act.ActionDate == date)
                    .Where(act => expectedActions.Contains(act.Action))
                    .OrderBy(act => act.ArrivalDate)
                    .AsEnumerable()
                    .GroupBy(act => act.ArrivalDate)
                    .ToDictionary(grp => grp.Key, grp => grp.AsEnumerable().GroupBy(act => act.InventoryLotNo).ToDictionary(grp => grp.Key, grp => grp.ToList()));

                var remainToDiscard = discardQuantity;
                InventoryAction previousAction = null;
                foreach (var lots in inventories.OrderBy(kv => kv.Key))
                {
                    if (remainToDiscard == 0) { break; }

                    foreach (var actions in lots.Value.OrderBy(kv => kv.Key))
                    {
                        if (remainToDiscard == 0) { break; }

                        var scheduledToUse = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_USE);
                        var scheduledToDiscard = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
                        var discarded = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.DISCARDED);
                        var isNewCreated = false;

                        if (discarded == null)
                        {
                            discarded = new InventoryAction()
                            {
                                Action = InventoryActionType.DISCARDED,
                                ActionDate = scheduledToUse.ActionDate,
                                ArrivalDate = scheduledToUse.ArrivalDate,
                                BouquetPart = scheduledToUse.BouquetPart,
                                InventoryLotNo = scheduledToUse.InventoryLotNo,
                                PartsCode = scheduledToUse.PartsCode,
                                Quantity = 0,
                                Remain = 0
                            };
                            isNewCreated = true;
                        }
                        var oldDiscarded = new InventoryAction() { Quantity = discarded.Quantity, Remain = discarded.Remain };


                        // 破棄可能な予定在庫がこのロットにはない→次のロットから破棄する
                        if (scheduledToDiscard == null && scheduledToUse == null) { continue; }

                        if (scheduledToDiscard == null)
                        {
                            var oldScheduledToUse = new InventoryAction() { Quantity = scheduledToUse.Quantity, Remain = scheduledToUse.Remain };

                            // 加工予定在庫アクションの残数から破棄アクションの破棄数を切り出す
                            if (scheduledToUse.Remain > remainToDiscard)
                            {
                                // 今ある加工残数から破棄済在庫アクションに全量振り替える
                                // 振り替えた分を翌日以降の払い出し予定に反映する
                                scheduledToUse.Remain -= remainToDiscard;
                                discarded.Quantity += remainToDiscard;

                                var usedLot = new Stack<int>();
                                UseFromPreviousRemain(context, scheduledToUse, scheduledToUse.ActionDate.AddDays(1), usedLot);

                                remainToDiscard = 0;
                            }
                            else
                            {
                                // 今ある加工残数をすべて破棄済在庫アクションに振り替える
                                // 振り替えた分を翌日以降の払い出し予定に反映する
                                // 振り替えできなかった分 (remainToDiscard - used.Remain) は次以降のロットから引く
                                discarded.Quantity = scheduledToUse.Remain;
                                remainToDiscard -= scheduledToUse.Remain;
                                scheduledToUse.Remain = 0;

                                var usedLot = new Stack<int>();
                                UseFromPreviousRemain(context, scheduledToUse, scheduledToUse.ActionDate.AddDays(1), usedLot);
                            }

                            context.InventoryActions.Update(scheduledToUse);
                            LogUtil.DEBUGLOG_InventoryActionChanged(scheduledToUse, oldScheduledToUse);
                        }
                        else
                        {
                            var oldScheduledToDiscard = new InventoryAction() { Quantity = scheduledToDiscard.Quantity, Remain = scheduledToDiscard.Remain };

                            // 破棄予定在庫アクションの数量から破棄アクションの数量を切り出す
                            if (scheduledToDiscard.Quantity >= remainToDiscard)
                            {
                                // 破棄予定数量から今回破棄対象数量の全量を振り替え可能
                                discarded.Quantity += remainToDiscard;
                                scheduledToDiscard.Quantity -= remainToDiscard;
                                remainToDiscard = 0;
                            }
                            else
                            {
                                // 破棄数量をこの破棄予定アクションから全量振り替えできない：
                                // 足りない分(remainToDiscard - scheduledToDiscard.Quantity) は次のロットから引く
                                remainToDiscard -= scheduledToDiscard.Quantity;
                                discarded.Quantity += scheduledToDiscard.Quantity;
                                scheduledToDiscard.Quantity = 0;
                            }

                            if (scheduledToDiscard.Quantity == 0)
                            {
                                context.InventoryActions.Remove(scheduledToDiscard);
                                LogUtil.DEBUGLOG_InventoryActionRemoved(scheduledToDiscard);
                            }
                            else if (IsInventoryQuantityChanged(scheduledToDiscard, oldScheduledToDiscard))
                            {
                                context.InventoryActions.Update(scheduledToDiscard);
                                LogUtil.DEBUGLOG_InventoryActionChanged(scheduledToDiscard, oldScheduledToDiscard);
                            }
                        }

                        if (isNewCreated)
                        {
                            context.InventoryActions.Add(discarded);
                            LogUtil.DEBUGLOG_InventoryActionCreated(discarded);
                        }
                        else if(IsInventoryQuantityChanged(discarded, oldDiscarded))
                        {
                            context.InventoryActions.Update(discarded);
                            LogUtil.DEBUGLOG_InventoryActionChanged(discarded, oldDiscarded);
                        }
                        previousAction = discarded;
                    }
                }

                if (remainToDiscard > 0)
                {
                    // 破棄残がある：在庫払出の最後のロットへ SHORTAGE アクション追加
                    var shortage = new InventoryAction()
                    {
                        Action = InventoryActionType.SHORTAGE,
                        ActionDate = previousAction.ActionDate,
                        ArrivalDate = previousAction.ArrivalDate,
                        BouquetPart = previousAction.BouquetPart,
                        InventoryLotNo = previousAction.InventoryLotNo,
                        PartsCode = previousAction.PartsCode,
                        Quantity = remainToDiscard,
                        Remain = -remainToDiscard
                    };
                    context.InventoryActions.Add(shortage);
                    LogUtil.DEBUGLOG_InventoryActionCreated(shortage);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                LogUtil.DEBUGLOG_EndMethod(msg: $" failed: {ex.GetType().Name}, {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 数量または残数の変更があったかどうかを確認する
        /// </summary>
        /// <param name="lhs">左辺の在庫アクション</param>
        /// <param name="rhs">右辺の在庫アクション</param>
        /// <returns>数量と残数のいずれかで 左辺 ！＝ 右辺 だったとき真、そうでないとき偽</returns>
        private bool IsInventoryQuantityChanged(InventoryAction lhs, InventoryAction rhs)
        {
            return (lhs.Quantity != rhs.Quantity)
                || (lhs.Remain != rhs.Remain);
        }
        #endregion // 単品破棄
    }
}
