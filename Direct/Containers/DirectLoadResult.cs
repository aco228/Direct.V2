using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Direct.Core
{
  public class DirectLoadResult
  {
    public DataTable DataTable { get; set; } = null;
    public DataRow DataRow { get; set; } = null;
    public Exception Exception { get; set; } = null;
    public DirectContainer Container { get => this.DataTable != null ? new DirectContainer(DataTable) : null; }

    public bool HasException { get => Exception != null; }
    public bool HasResult { get => Exception == null && DataTable != null && DataTable.Rows.Count > 0; }
    public DataRowCollection Rows { get => DataTable != null && DataTable.Rows != null ? DataTable.Rows : null; }
  }
}
