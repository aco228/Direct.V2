using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core.Bulk
{
  public static class BulkHelper
  {

    public static BulkModel ToBulkModel(this DirectModel model)
      => new BulkModel(model);

    public static BulkModel ChainTo(this BulkModel bmodel, BulkInstance bi)
    {
      bi.Add(bmodel);
      return bmodel;
    }


  }
}
