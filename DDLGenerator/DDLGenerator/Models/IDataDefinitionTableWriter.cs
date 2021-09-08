using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDLGenerator.Models
{
    interface IDataDefinitionTableWriter
    {
        public void WriteTables(IList<TableDefinition> tables);

        public void Validate();
    }
}
