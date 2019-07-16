using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class TestClass
  {

    public TestClass()
    {
      CCSubmitDirect db = new CCSubmitDirect();
      PrelanderTypeDM dm = new PrelanderTypeDM(db)
      {
        Name = "test1",
        Description = "ok?"
      };

      dm.Insert();
    }

  }
}
