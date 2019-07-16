using Direct.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class SnippetContainers
  {

    // basic implementation of framework
    public SnippetContainers()
    {
      CCSubmitDirect db = new CCSubmitDirect();
      int index = 1;
      string q = "SELECT * FROM [].tm_lead WHERE msisdn != '' ORDER BY leadid DESC LIMIT 5";

      foreach (var elem in db.LoadEnumerable(q))
      {
        Console.WriteLine(index + ": " + elem.GetString("leadid") + " - " + elem.GetString("email"));
        index++;
      }

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

      DirectContainer dc = db.LoadContainer(q);
      foreach (var elem in dc.Rows)
      {
        Console.WriteLine(index + ": " + elem.GetString("leadid") + " - " + elem.GetString("email"));
        index++;
      }

      db.Dispose();
    }


  }
}
