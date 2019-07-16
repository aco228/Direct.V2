using Direct.Core.Mysql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Test
{
  public class CCSubmitDirect : DirectDatabaseMysql
  {
    private static object LockObj = new object();
    private static CCSubmitDirect _instance = null;

    public static CCSubmitDirect Instance
    {
      get
      {
        return new CCSubmitDirect();
      }
    }

    public CCSubmitDirect(bool openConnection = true)
      : base("livesports", "Server=46.166.160.58; database=livesports; UID=livesports; password=a48i72V\"B?8>79Z", openConnection)
    { }

  }
}
