using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core.Bulk
{
  public class DirectBulker
  {
    private static object LockObj = new object();

    public int MaximumNumberOfConnections = 8;
    private int CurrentNumberOfConnections = 0;
    public long RowsInserted { get; protected set; } = 0;

    private DirectDatabaseBase database = null;
    private List<DirectModel> Models = new List<DirectModel>();

    public DirectBulker(DirectDatabaseBase db)
    {
      this.database = db;
    }

    public void Add(DirectModel model)
    {
      lock(LockObj)
      {
        this.Models.Add(model);
      }
    }


    public void Run()
    {
      if (this.Models.Count == 0)
        return;

      if (this.CurrentNumberOfConnections >= this.MaximumNumberOfConnections)
      {
        Console.WriteLine($"DirectBulker:: MaxConnection of {MaximumNumberOfConnections} is reached. Wait until other connections catch up! ");
        do { } while (this.CurrentNumberOfConnections != 0);
        Console.WriteLine($"DirectBulker:: Catched up. Current number of connections is {MaximumNumberOfConnections}");
      }

      List<DirectModel> models = null;
      lock (LockObj)
      {
        models = new List<DirectModel>(this.Models);
        this.Models = new List<DirectModel>();
        GC.Collect();
      }

      Task.Factory.StartNew(() => { Execute(models); });
    }

    private async Task Execute(List<DirectModel> models)
    {
      CurrentNumberOfConnections++;

      Dictionary<Type, string> types = new Dictionary<Type, string>();

      foreach(var model in models)
      {
        if (!types.ContainsKey(model.GetType()))
        {
          string header = string.Format("INSERT INTO {0}.{1}{2} ({3}) VALUES ",
            model.GetDatabase().DatabaseName, model.GetDatabase().DatabaseSchemeString, model.GetTableName(),
            model.Snapshot.GetPropertyNamesForInsert(true));
          types.Add(model.GetType(), header);
        }

        types[model.GetType()] += string.Format("({0}),", model.Snapshot.GetPropertyValuesForInsert(true));
      }

      string finalQuery = "";
      foreach(var t in types)
      {
        finalQuery += t.Value.Substring(0, t.Value.Length - 1) + ";" + Environment.NewLine;
      }

      DateTime dt = DateTime.Now;
      var result = this.database.Execute("SET FOREIGN_KEY_CHECKS=0;START TRANSACTION;" + finalQuery+ "COMMIT;SET FOREIGN_KEY_CHECKS=1;");
      double ms = (DateTime.Now - dt).TotalMilliseconds;

      Console.WriteLine($"DirectBulker: Inserted {models.Count} objects in {ms}ms (in use ${CurrentNumberOfConnections}/{MaximumNumberOfConnections})! ");
      RowsInserted += models.Count;

      CurrentNumberOfConnections--;
    }

  }
}
