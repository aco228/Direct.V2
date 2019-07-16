using Direct.Core;
using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class CCUndercoverTransactionDM : DirectModel
  {
    public CCUndercoverTransactionDM(DirectDatabaseBase db) : base("cc_undercover_transaction", "id", db) { }
    

    [DColumn(Name ="offerID")]
    public int OfferID { get; set; } = default;

    [DColumn(Name = "aff_id")]
    public int AffID { get; set; } = -1;

    [DColumn(Name = "pub_id")]
    public string PubID { get; set; } = string.Empty;

    [DColumn(Name = "url")]
    public string Url { get; set; } = string.Empty;

    [DColumn(Name = "created", DateTimeUpdate =true)]
    public DateTime Created { get; set; } = DateTime.Now;
  }
}
