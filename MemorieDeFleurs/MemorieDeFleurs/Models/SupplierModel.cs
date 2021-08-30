using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // [TODO] 発注ロット番号=在庫ロット番号は発注時に採番する。
            var lotNo = Sequences.SEQ_STOCK_LOT_NUMBER.Next;

            var param = new StockActionParameterToOrder(arrivalDate, part, lotNo, quantityOfLot);

            AddScheduledToArriveStockAction(param);

            AddScheduledToDiscardStockAction(param);

            AddScheduledToUseStockAction(param);

            DbContext.SaveChanges();

            return lotNo;

        }

        private void AddScheduledToUseStockAction(StockActionParameterToOrder param)
        {
            foreach (var d in Enumerable.Range(0, param.DaysToExpire).Select(i => param.ArrivalDate.AddDays(i)))
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
        }
        #endregion // 発注

        #region 発注取消
        public void CancelOrder(int lotNo)
        {
            var actions = DbContext.StockActions.Where(a => a.StockLotNo == lotNo).ToList();

            DbContext.StockActions.RemoveRange(actions);
            DbContext.SaveChanges();

        }
        #endregion // 発注取消
    }
}
