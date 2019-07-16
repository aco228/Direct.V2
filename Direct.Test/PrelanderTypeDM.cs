using Direct.Core;
using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{

  public class PrelanderTypeDM : DirectModel
  {
    public PrelanderTypeDM(CCSubmitDirect db) : base("tm_prelandertype", "prelandertypeid", db) { }

    [DColumn(Name="name")]
    public string Name { get; set; } = string.Empty;

    [DColumn(Name="description")]
    public string Description { get; set; } = string.Empty;

  }
    
}
