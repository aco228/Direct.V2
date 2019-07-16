using Direct.Core;
using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class CCClientDM : DirectModel
  {
    public CCClientDM(DirectDatabaseBase db) : base("cc_client", "clientid", db) { }

    public string clickid { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string firstname { get; set; } = string.Empty;

  }
}
