using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core
{
  public class DirectExecuteResult
  {
    public long? LastID { get; set; } = null;
    public int NumberOfRowsAffected { get; set; } = 0;
    public Exception Exception { get; set; } = null;

    public bool IsSuccessfull { get => Exception == null; }

  }
}
