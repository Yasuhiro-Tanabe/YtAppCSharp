using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

using YasT.Framework.Logging;

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

            /// <summary>
            /// 商品オブジェクト生成器を作成する
            /// </summary>
            /// <param name="parent">モデルオブジェクト</param>
            /// <returns>商品オブジェクト生成器</returns>
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
                    var parts = context.BouquetParts.Find(p.Key);
                    if(parts == null)
                    {
                        throw new ApplicationException($"該当単品なし：{p.Key}");
                    }
                    if(ret.LeadTime < parts.LeadTime)
                    {
                        ret.LeadTime = parts.LeadTime;
                    }
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
                return FindBouquetPart(context, partCode);
            }
        }
        private BouquetPart FindBouquetPart(MemorieDeFleursDbContext context, string partCode)
        {
            return context.BouquetParts
                .Include(p => p.Suppliers)
                .ThenInclude(i => i.Supplier)
                .SingleOrDefault(p => p.Code == partCode);
        }

        /// <summary>
        /// 登録されている単品オブジェクトをすべて取得する
        /// </summary>
        /// <returns>登録されている単品オブジェクトの一覧。何も登録されていないときは空の一覧を返す。</returns>
        public IEnumerable<BouquetPart> FindAllBoueuqtParts()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.BouquetParts
                    .Include(p => p.Suppliers)
                    .ThenInclude(i => i.Supplier)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 指定された花コードを持つ単品を削除する
        /// </summary>
        /// <param name="partsCode"></param>
        public void RemoveBouquetParts(string partsCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(partsCode);
                    RemoveBouquetParts(context, partsCode);
                    transaction.Commit();
                    LogUtil.Info($"Bouquet parts {partsCode} removed.");
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(partsCode);
                }
            }
        }
        private void RemoveBouquetParts(MemorieDeFleursDbContext context, string partsCode)
        {
            var parts = context.BouquetParts.Find(partsCode);
            if(parts != null)
            {
                // チェックなしで削除しても商品構成の外部キー制約で SQLException がスローされるが、
                // メッセージがわかりづらいので「使用している商品」がわかるようなエラーメッセージを出す
                var used = context.PartsList.Where(i => i.PartsCode == partsCode).ToList();
                if (used.Count > 0)
                {
                    throw new ApplicationException($"単品 {partsCode} は以下の商品で使用中です： {string.Join(", ", used.Select(i => i.BouquetCode))}");
                }
                context.BouquetParts.Remove(parts);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 単品をデータベースに登録する
        /// </summary>
        /// <param name="parts">登録対象の単品</param>
        /// <returns>登録後にデータベースから再取得した、他エンティティとの関連が適切に設定されたオブジェクトを返す。</returns>
        public BouquetPart Save(BouquetPart parts)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(parts.Code);
                    var saved = Save(context, parts);
                    transaction.Commit();
                    LogUtil.Info($"Bouquet parts {saved.Code} saved.");
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
                    LogUtil.DEBUGLOG_EndMethod(parts.Code);
                }
            }
        }
        private BouquetPart Save(MemorieDeFleursDbContext context, BouquetPart parts)
        {
            var found = FindBouquetPart(context, parts.Code);
            if(found == null)
            {
                found = context.BouquetParts.Add(parts).Entity;
            }
            else
            {
                if(found.CheckAndModify(parts))
                {
                    context.BouquetParts.Update(found);
                }

                foreach(var supplier in found.Suppliers)
                {
                    if(parts.Suppliers.SingleOrDefault(s => s.SupplierCode == supplier.SupplierCode) == null)
                    {
                        // found にあって supplier にない SupplyParts を削除
                        context.PartsSuppliers.Remove(supplier);
                    }
                    else
                    {
                        // 何もしない：2つのキー(仕入先コードと花束コード)以外にDB上のカラムがないので、更新は発生しない。
                    }
                }

                foreach(var supplier in parts.Suppliers)
                {
                    if (found.Suppliers.SingleOrDefault(s => s.SupplierCode == supplier.SupplierCode) == null)
                    {
                        // supplier にあって found にない SupplyParts を追加
                        context.PartsSuppliers.Add(supplier);
                    }
                }
            }
            context.SaveChanges();

            return FindBouquetPart(context, found.Code);
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
                return FindBouquet(context, bouquetCode);
            }
        }
        private Bouquet FindBouquet(MemorieDeFleursDbContext context, string bouquetCode)
        {
            return context.Bouquets
                .Include(b => b.PartsList)
                .ThenInclude(p => p.Part)
                .SingleOrDefault(b => b.Code == bouquetCode);
        }

        /// <summary>
        /// 登録されている全商品を取得する
        /// </summary>
        /// <returns>登録されている商品オブジェクトの一覧、商品が登録されていないときは空のリスト</returns>
        public IEnumerable<Bouquet> FindAllBouquets()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.Bouquets
                    .Include(b => b.PartsList)
                    .ThenInclude(p => p.Part)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 指定された商品オブジェクトを削除する
        /// </summary>
        /// <param name="bouquetCode">削除対象商品の花束コード</param>
        public void RemoveBouquet(string bouquetCode)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(bouquetCode);
                    RemoveBouquet(context, bouquetCode);
                    transaction.Commit();
                    LogUtil.Info($"Bouquet {bouquetCode} removed.");
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(bouquetCode);
                }
            }
        }

        private void RemoveBouquet(MemorieDeFleursDbContext context, string bouquetCode)
        {
            var partsList = context.PartsList.Where(i => i.BouquetCode == bouquetCode);
            if(partsList.Count() > 0)
            {
                context.PartsList.RemoveRange(partsList);
            }
            var bouquet = context.Bouquets.Find(bouquetCode);
            if(bouquet != null)
            {
                context.Bouquets.Remove(bouquet);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// 商品をデータベースに登録する
        /// </summary>
        /// <param name="bouquet">登録する商品</param>
        /// <returns>登録完了した bouquet：引数で渡したオブジェクトそのものではなく、DB登録後にDBから取得した、関連データへの参照が適切に設定されたオブジェクトを返す。</returns>
        public Bouquet Save(Bouquet bouquet)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(bouquet.Code);
                    var saved = Save(context, bouquet);
                    transaction.Commit();
                    LogUtil.Info($"Bouquet {saved.Code} saved.");
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
                    LogUtil.DEBUGLOG_EndMethod(bouquet.Code);
                }
            }
        }
        private Bouquet Save(MemorieDeFleursDbContext context, Bouquet bouquet)
        {
            var found = FindBouquet(context, bouquet.Code);
            if(found == null)
            {
                found = context.Bouquets.Add(bouquet).Entity;
            }
            else
            {
                if(found.CheckAndModify(bouquet))
                {
                    context.Bouquets.Update(found);
                }

                foreach(var parts in found.PartsList)
                {
                    var foundParts = bouquet.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode);
                    if(foundParts == null)
                    {
                        // found にあって bouquet にない PartsList を削除
                        context.PartsList.Remove(parts);
                    }
                    else if(parts.CheckAndModify(foundParts))
                    {
                        // found と bouquet に共通する PartsList に値の変更があったら更新
                        context.PartsList.Update(parts);
                    }
                }

                foreach(var parts in bouquet.PartsList)
                {
                    if(found.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode) == null)
                    {
                        // bouquet にあって found にない PartsList を削除
                        context.PartsList.Add(parts);
                    }
                }
            }
            context.SaveChanges();

            return FindBouquet(context, found.Code);
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

        /// <summary>
        /// 指定数量を指定された在庫ロットから引き当てる
        /// 
        /// トランザクション内での呼出用
        /// 
        /// 指定日以降にこの在庫ロットからの引当残が発生した場合は、 usedLot にないロットから引当を行う。
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="today">在庫引当日の在庫アクション：この在庫アクションから quantity 本を引当、この日以降の残数を更新する</param>
        /// <param name="quantity">引当数量</param>
        /// <param name="usedLot">これまでに引当実施済のロット一覧：ここで指定された在庫ロットへの在庫振替は行わない</param>
        public void UseFromThisLot(MemorieDeFleursDbContext context, InventoryAction today, int quantity, Stack<int> usedLot)
        {
            LogUtil.DEBUGLOG_BeginMethod($"{today.ToString("s")}, {quantity}, [ {string.Join(",", usedLot)} ]");

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
            LogUtil.DEBUGLOG_BeginMethod($"{today.ToString("s")}, {startDate:yyyyMMdd}, [ {string.Join(", ", usedLot)} ]");
            var previousRemain = today.Remain;
            foreach (var action in context.InventoryActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.InventoryLotNo == today.InventoryLotNo)
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                .Where(act => act.ActionDate >= startDate)
                .OrderBy(act => act.ActionDate))
            {
                InventoryActionLogger.DEBUGLOG_ComparationOfInventoryQuantityAndPreviousRemain(action, previousRemain);
                if (previousRemain >= action.Quantity)
                {
                    var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain };

                    // 全量引き出せる
                    action.Remain = previousRemain - action.Quantity;
                    context.InventoryActions.Update(action);

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);

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
                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);

                    try
                    {
                        usedLot.Push(action.InventoryLotNo);
                        UseFromOtherLot(context, action, useFromOtherLot, usedLot);
                    }
                    catch (InventoryShortageException eis)
                    {
                        ShortageInventories.Add(eis.InventoryShortageAction);
                        context.InventoryActions.Add(eis.InventoryShortageAction);
                        InventoryActionLogger.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                    }
                    usedLot.Pop();
                    previousRemain = 0;
                }
            }

            var discard = context.InventoryActions
                .Where(act => act.PartsCode == today.PartsCode)
                .Where(act => act.InventoryLotNo == today.InventoryLotNo)
                .Single(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
            var oldDiscard = new InventoryAction() { Quantity = discard.Quantity, Remain = discard.Remain };
            discard.Quantity = previousRemain;
            context.InventoryActions.Update(discard);
            InventoryActionLogger.DEBUGLOG_InventoryActionChanged(discard, oldDiscard);

            LogUtil.DEBUGLOG_EndMethod($"{today.ToString("s")}, {startDate:yyyyMMdd}, [ {string.Join(", ", usedLot)} ]");
        }

        private void UseFromThisLotToday(MemorieDeFleursDbContext context, InventoryAction today, int quantity, Stack<int> usedLot)
        {
            InventoryActionLogger.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(today, quantity);
            if (today.Remain >= quantity)
            {
                // 全量引き出せる
                today.Quantity += quantity;
                today.Remain -= quantity;
                context.InventoryActions.Update(today);

                InventoryActionLogger.DEBUGLOG_InventoryActionChanged(today, quantity);

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

                InventoryActionLogger.DEBUGLOG_InventoryActionChanged(today, oldAction);

                try
                {
                    usedLot.Push(today.InventoryLotNo);
                    UseFromOtherLot(context, today, useFromOtherLot, usedLot);
                }
                catch (InventoryShortageException eis)
                {
                    ShortageInventories.Add(eis.InventoryShortageAction);
                    context.InventoryActions.Add(eis.InventoryShortageAction);
                    InventoryActionLogger.DEBUGLOG_InventoryActionCreated(eis.InventoryShortageAction);
                }
                usedLot.Pop();
            }
        }

        /// <summary>
        /// 指定数量を別の在庫から引き当てる
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="inventory">直前に引当てした在庫ロットの在庫アクション</param>
        /// <param name="quantity">引当数量</param>
        /// <param name="usedLot">これまでに引当を行ったロットのロット番号一覧：咲き引当はこれら以外のロットから行う。</param>
        /// <exception cref="InventoryShortageException">当日分の在庫が足りずに引当ができなかった</exception>
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

            LogUtil.Debug($"usableLots={string.Join(", ", usableLots.Select(act => act.ToString("s")))}");

            foreach(var action in usableLots.OrderBy(act => act.ArrivalDate))
            {
                // すでに引当対象としたロットは除外：Linq式で usableLots を生成するタイミングでは除外できなかったため。
                if(usedLot.Contains(action.InventoryLotNo)) { continue; }

                var oldAction = new InventoryAction() { Quantity = action.Quantity, Remain = action.Remain }; // デバッグログ用

                InventoryActionLogger.DEBUGLOG_ComparationOfInventoryRemainAndQuantity(action, useToThisLot);
                if(action.Remain >= useToThisLot)
                {

                    // このロットから全量引き出す
                    UseFromThisLot(context, action, quantity, usedLot);

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);
                    LogUtil.DEBUGLOG_EndMethod(msg: $"resolved");
                    return;
                }
                else
                {
                    // 残数分はこのロットから、引き出せなかった分は次のロットから引き出す
                    useToThisLot -= action.Remain;
                    UseFromThisLot(context, action, action.Remain, usedLot);
                    previousLot = action;

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);
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

        /// <summary>
        /// 指定日の使用数を指定された数量在庫に戻し、指定日以降の在庫引当をやり直す
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="today">数量変更日</param>
        /// <param name="quantityToReturn">在庫に戻す数量</param>
        /// <param name="returnedLot">すでに在庫再引当済のロット：これらのロットに対する在庫再引当は行わない</param>
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
            InventoryActionLogger.DEBUGLOG_ComparationOfInventoryUsedAndReturns(today, quantityToReturn);
            if (today.Quantity >= quantityToReturn)
            {
                // 全量戻せる
                today.Quantity -= quantityToReturn;
                today.Remain += quantityToReturn;
                context.InventoryActions.Update(today);
                InventoryActionLogger.DEBUGLOG_InventoryActionChanged(today, -quantityToReturn);
            }
            else
            {
                // 戻せる分だけこのロットに戻し、残りは他のロットに戻す
                var returnToThisLot = today.Quantity;
                var returnToOtherLot = quantityToReturn - returnToThisLot;

                today.Quantity -= returnToThisLot;
                today.Remain += returnToThisLot;
                context.InventoryActions.Update(today);
                InventoryActionLogger.DEBUGLOG_InventoryActionChanged(today, -returnToThisLot);

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

                InventoryActionLogger.DEBUGLOG_ComparationOfInventoryUsedAndReturns(action, returnToThisLot);
                if (action.Quantity >= returnToThisLot)
                {
                    // このロットに全量戻す
                    ReturnToThisLot(context, action, quantityToReturn, returnedLot);

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);
                    LogUtil.DEBUGLOG_EndMethod(msg: $"resolved");
                    return;
                }
                else
                {
                    // 加工数量分はこのロットに、戻し残った分は次のロットに戻す
                    returnToThisLot -= action.Quantity;
                    ReturnToThisLot(context, action, action.Remain, returnedLot);
                    previousLot = action;

                    InventoryActionLogger.DEBUGLOG_InventoryActionChanged(action, oldAction);
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
            var bouquet = context.Bouquets.Find(bouquetCode);
            var parts = context.BouquetParts.Find(partsCode);
            if ( bouquet == null)
            {
                throw new ArgumentException($"花束未登録： {bouquetCode}");
            }
            if ( parts == null)
            {
                throw new ArgumentException($"単品未登録： {partsCode}");
            }

            CreateOrUpdatePartsList(context, bouquetCode, partsCode, quantity);

            if (bouquet.LeadTime < parts.LeadTime)
            {
                bouquet.LeadTime = parts.LeadTime;
                context.Bouquets.Update(bouquet);
            }
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
        /// <remarks><see cref="BouquetBuilder"/> を使って商品を登録する時は <see cref="BouquetBuilder.Uses(string, int)"/>
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
        /// 指定日の商品加工数量を取得する
        /// </summary>
        /// <param name="bouquet">加工対象の花コード</param>
        /// <param name="date">加工数量取得日</param>
        /// <returns></returns>
        public int GetNumberOfProcessingBouquetsOf(string bouquet, DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrderFromCustomers
                    .Where(o => o.ShippingDate == date)
                    .Where(o => o.BouquetCode == bouquet)
                    .Count();
            }
        }

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
            InventoryActionLogger.DEBUGLOG_InventoryActionCreated(used);
        }

        private void UpdateInventoryActions(MemorieDeFleursDbContext context, InventoryAction used, InventoryAction scheduled, ref int quantity)
        {
            var oldUsed = new InventoryAction() { Quantity = used.Quantity, Remain = used.Remain }; // デバッグ用、変更前の数量残数

            UpdateQuantity(used, scheduled, ref quantity);
            UpdateScheduledToUseInventoryAction(context, scheduled);

            context.InventoryActions.Update(used);
            InventoryActionLogger.DEBUGLOG_InventoryActionChanged(used, oldUsed);
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

            InventoryActionLogger.DEBUGLOG_InventoryActionChanged(scheduled, oldScheduled);
        }
        
        private void UpdateScheduledToUseInventoryAction(MemorieDeFleursDbContext context, InventoryAction scheduled)
        {
            if(scheduled.Quantity == 0 && scheduled.Remain ==0)
            {
                context.InventoryActions.Remove(scheduled);
                InventoryActionLogger.DEBUGLOG_InventoryActionRemoved(scheduled);
            }
            else
            {
                context.InventoryActions.Update(scheduled);
            }
        }
        #endregion // 出荷数量変更

        #region 単品破棄
        /// <summary>
        /// 単品を破棄する
        /// </summary>
        /// <param name="date">破棄実施日</param>
        /// <param name="discardParts">破棄対象単品の花コードおよび破棄数量
        /// 
        /// 花コードと破棄数量のペアは複数指定可能。</param>
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
                    LogUtil.DEBUGLOG_EndMethod($"{date:yyyyMMdd}, [ {partsString} ]");
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
                    foreach (var actions in lots.Value.OrderBy(kv => kv.Key))
                    {

                        var scheduledToUse = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_USE);
                        var scheduledToDiscard = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD);
                        var discarded = actions.Value.SingleOrDefault(act => act.Action == InventoryActionType.DISCARDED);
                        var isNewCreated = false;

                        // 破棄可能な予定在庫がこのロットにはない→次のロットから破棄する
                        if (scheduledToDiscard == null && scheduledToUse == null) { continue; }

                        if (discarded == null)
                        {
                            var action = scheduledToUse == null ? scheduledToDiscard : scheduledToUse;

                            discarded = new InventoryAction()
                            {
                                Action = InventoryActionType.DISCARDED,
                                ActionDate = action.ActionDate,
                                ArrivalDate = action.ArrivalDate,
                                BouquetPart = action.BouquetPart,
                                InventoryLotNo = action.InventoryLotNo,
                                PartsCode = action.PartsCode,
                                Quantity = 0,
                                Remain = 0
                            };
                            isNewCreated = true;
                        }
                        var oldDiscarded = new InventoryAction() { Quantity = discarded.Quantity, Remain = discarded.Remain };


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
                            InventoryActionLogger.DEBUGLOG_InventoryActionChanged(scheduledToUse, oldScheduledToUse);
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
                                InventoryActionLogger.DEBUGLOG_InventoryActionRemoved(scheduledToDiscard);
                            }
                            else if (IsInventoryQuantityChanged(scheduledToDiscard, oldScheduledToDiscard))
                            {
                                context.InventoryActions.Update(scheduledToDiscard);
                                InventoryActionLogger.DEBUGLOG_InventoryActionChanged(scheduledToDiscard, oldScheduledToDiscard);
                            }
                        }

                        if (isNewCreated)
                        {
                            context.InventoryActions.Add(discarded);
                            InventoryActionLogger.DEBUGLOG_InventoryActionCreated(discarded);
                        }
                        else if(IsInventoryQuantityChanged(discarded, oldDiscarded))
                        {
                            context.InventoryActions.Update(discarded);
                            InventoryActionLogger.DEBUGLOG_InventoryActionChanged(discarded, oldDiscarded);
                        }
                        previousAction = discarded;

                        if (remainToDiscard == 0) { break; }
                    }

                    if (remainToDiscard == 0) { break; }
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
                    InventoryActionLogger.DEBUGLOG_InventoryActionCreated(shortage);
                }
                context.SaveChanges();
                LogUtil.DEBUGLOG_EndMethod($"{date:yyyyMMdd}, {partsCode}, {discardQuantity}", "Succeeded.");
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

        /// <summary>
        /// 指定日の各単品在庫数を取得する
        /// </summary>
        /// <param name="date">単品在庫数取得日</param>
        /// <returns>date で指定された日の各単品在庫数量一覧</returns>
        public IDictionary<string, int> FindInventoriesAt(DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.InventoryActions
                    .Include(a => a.BouquetPart)
                    .Where(a => a.ActionDate == date)
                    .Where(a => a.Action == InventoryActionType.SCHEDULED_TO_USE)
                    .AsEnumerable()
                    .GroupBy(o => o.PartsCode)
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.Quantity));
            }
        }
        #endregion // 単品破棄
    }
}
