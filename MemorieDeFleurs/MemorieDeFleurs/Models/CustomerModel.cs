using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models
{
    public class CustomerModel
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
        internal CustomerModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
            DbContext = parent.DbContext;
        }
    }
}
