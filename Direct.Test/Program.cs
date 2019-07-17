using Direct.Core;
using Direct.Core.Models;
using Direct.livesports.Models;
using System;
using System.Linq;

namespace Direct.Test
{
  class Program
  {
    static void Main(string[] args)
    {
      CCSubmitDirect db = new CCSubmitDirect();


      //Test();
      Console.WriteLine(db.LoadString("SELECT guid FROM [].tm_action WHERE actionid=5"));
      Console.ReadKey();

      

    }

    public static async void Test()
    {
      CCSubmitDirect db = new CCSubmitDirect();
      
      foreach(var entry in 
        db.Query<ClientDM>()
        .Additional("ORDER BY clientid DESC LIMIT 150")
        .LoadEnumerable())
      {
        int a = 0;
        Console.WriteLine(string.Format("{0} {1} {2}", entry.clickid, entry.email, entry.firstname));
      }
      
              
    }

  }
}
