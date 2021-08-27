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
    public class SupplierModel
    {
        private MemorieDeFleursDbContext DbContext { get; set; }
        private MemorieDeFleursModel Parent { get; set; }

        private SequenceUtil Sequence { get; set; }

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
            Sequence = new SequenceUtil(DbContext.Database.GetDbConnection() as SqliteConnection);
        }

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

            public SupplierProcesser NameIs(string name)
            {
                _name = name;
                return this;
            }

            public SupplierProcesser AddressIs(string address1, string address2 = null)
            {
                _address1 = address1;
                _address2 = address2;
                return this;
            }

            public SupplierProcesser PhoneNumberIs(string tel)
            {
                _tel = tel;
                return this;
            }

            public SupplierProcesser FaxNumberIs(string fax)
            {
                _fax = fax;
                return this;
            }

            public SupplierProcesser EmailIs(string email)
            {
                _email = email;
                return this;
            }

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

        internal int NextSequenceCode { get { return Sequence.SEQ_SUPPLIERS.Next; } }

        public SupplierProcesser Entity<Sypplier>()
        {
            return SupplierProcesser.GetInstance(this);
        }

        public IEnumerable<Supplier> Find(Func<Supplier,bool> condition)
        {
            return DbContext.Suppliers.Where(condition);
        }

        public Supplier Find(int code)
        {
            return DbContext.Suppliers.SingleOrDefault(s => s.Code == code);
        }

    }
}
