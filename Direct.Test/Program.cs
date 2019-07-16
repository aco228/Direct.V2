using Direct.Core;
using Direct.Core.Models;
using Direct.Core.ModelsCreator;
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
      ModelsCreator.Create(db, "ctm_action", "Action", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_action_history", "ActionHistory", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_country", "Country", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_country_used", "CountryUsed", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_lander", "Lander", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_lead", "Lead", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_lead_history", "LeadHistory", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_prelander", "Prelander", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_service", "Service", @"C:\Users\aco228_\Desktop\output");
      ModelsCreator.Create(db, "ctm_click", "Click", @"C:\Users\aco228_\Desktop\output");


      //Test();
      Console.WriteLine("WTF?");
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
