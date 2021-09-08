using MemorieDeFleurs.Entities;

using System.Linq;

namespace MemorieDeFleurs.Models
{
    public class CustomerModel
    {
        private MemorieDeFleursDbContext DbContext { get; set; }
        private MemorieDeFleursModel Parent { get; set; }

        private int NextCustomerID
        {
            get { return Parent.Sequences.SEQ_CUSTOMERS.Next; }
        }

        /// <summary>
        /// (パッケージ内限定)コンストラクタ
        /// 
        /// モデルのプロパティとして参照できるので、外部でこのオブジェクトを作成することは想定しない。
        /// </summary>
        /// <param name="parent"></param>
        internal CustomerModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
            DbContext = parent.DbContext;
        }

        #region CustomerBuilder
        /// <summary>
        /// 得意先オブジェクト生成器
        /// 
        /// 各プロパティはフルーエントインタフェースで入力し、最後に Create() で生成とデータベース登録を行う。
        /// </summary>
        public class CustomerBuilder
        {
            private CustomerModel _model;

            string _emailAddress;
            string _name;
            string _password;
            string _cardNo;

            internal static CustomerBuilder GetInstance(CustomerModel parent)
            {
                return new CustomerBuilder(parent);   
            }

            private CustomerBuilder(CustomerModel model)
            {
                _model = model;
            }

            private MemorieDeFleursDbContext DbContext { get { return _model.DbContext; } }

            /// <summary>
            /// 得意先名称を登録/変更する
            /// </summary>
            /// <param name="name">得意先名称</param>
            /// <returns>得意先名称変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder NameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// e-メールアドレスを登録/変更する
            /// </summary>
            /// <param name="address">e-メールアドレス</param>
            /// <returns>e-メールアドレス変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder EmailAddressIs(string address)
            {
                _emailAddress = address;
                return this;
            }

            /// <summary>
            /// パスワードを登録/変更する
            /// </summary>
            /// <param name="passwd">パスワード</param>
            /// <returns>パスワード変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder PasswordIs(string passwd)
            {
                _password = passwd;
                return this;
            }

            /// <summary>
            /// キャッシュカード番号を登録/変更する
            /// </summary>
            /// <param name="no">キャッシュカード番号</param>
            /// <returns>キャッシュカード番号変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder CardNoIs(string no)
            {
                _cardNo = no;
                return this;
            }

            public Customer Create()
            {
                var c = new Customer()
                {
                    ID = _model.NextCustomerID,
                    Name = _name,
                    EmailAddress = _emailAddress,
                    Password = _password,
                    CardNo = _cardNo,
                    Status = 0
                };

                DbContext.Customers.Add(c);
                DbContext.SaveChanges();

                return c;
            }
        }

        public CustomerBuilder Entity<Customer>()
        {
            return CustomerBuilder.GetInstance(this);
        }
        #endregion // CustomerBuilder

        #region 仕入先の登録改廃
        public Customer Find(int id)
        {
            return DbContext.Customers.SingleOrDefault(c => c.ID == id);
        }
        #endregion // 仕入先の登録改廃

        #region 注文
        #endregion // 注文

        #region 注文取消
        #endregion // 注文取消
    }
}
