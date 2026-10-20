using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Shared
{
    public record TableData(string[] Columns, List<string[]> Rows);
}
