using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core
{
  public class DirectTransactionalManager
  {
    private static object LockObj = new object();
    private bool IsRunExecuting { get; set; } = false;

    private DirectDatabaseBase _database = null;
    public int Limit { get; set; } = 2000;
    private List<string> _queries = new List<string>();
    private List<DirectModel> _queryModels = new List<DirectModel>();
    private List<DirectModel> _queryInserter = new List<DirectModel>();
    private Dictionary<DirectModel, string> _queryLoader = new Dictionary<DirectModel, string>();

    public int Count { get { return this._queries.Count + this._queryInserter.Count + this._queryLoader.Count + this._queryModels.Count; } }

    public DirectTransactionalManager(DirectDatabaseBase db)
    {
      this._database = db;
    }

    ///
    /// Insert values 
    ///

    // SUMMARY: Used to update last change of the model (not multiple times)
    public void Add(DirectModel model)
    {
      lock(LockObj)
      {
        if (!this._queryModels.Contains(model))
          this._queryModels.Add(model);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Add(string query, params object[] parameters) => Add(this._database.Construct(query, parameters));
    public void Add(string command)
    {
      lock(LockObj)
      {
        this._queries.Add(command);
        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Insert(DirectModel model)
    {
      lock(LockObj)
      {
        if (!this._queryInserter.Contains(model))
          this._queryInserter.Add(model);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    public void Load(DirectModel model, string loadQuery)
    {
      lock(LockObj)
      {
        if (!this._queryLoader.ContainsKey(model))
          this._queryLoader.Add(model, loadQuery);

        if (this.Count >= this.Limit)
          this.RunAsync();
      }

    }

    ///
    /// Runners
    ///

    public Task RunAsync()
      => Task.Factory.StartNew(() => { Run(); });

    public void Run()
    {
      if (this.Count == 0) return;

      int originalTasks = this.Count;
      List<string> queries = new List<string>();
      List<DirectModel> queryModels = new List<DirectModel>();
      List<DirectModel> queryInserter = new List<DirectModel>();
      Dictionary<DirectModel, string> queryLoader = new Dictionary<DirectModel, string>();

      lock (LockObj)
      {
        queries = new List<string>(this._queries); this._queries.Clear(); 
        queryModels = new List<DirectModel>(this._queryModels); this._queryModels.Clear();
        queryInserter = new List<DirectModel>(this._queryInserter); this._queryInserter.Clear();
        queryLoader = new Dictionary<DirectModel, string>(this._queryLoader); this._queryLoader.Clear();
        GC.Collect();
      }

      try
      {
        this.IsRunExecuting = true;
        string mainQuery = "";

        foreach (string query in queries)
        {
          string qq = query.Trim();
          mainQuery += qq + (qq.EndsWith(";") ? "" : ";");
        }
        this._queries = new List<string>();

        DateTime create = DateTime.Now;
        Console.WriteLine(string.Format("TransactionalManager is starting with {0} tasks", originalTasks));
        //this.RunInserter();

        foreach (var model in queryModels)
          mainQuery += model.ConstructUpdateQuery();
        this._queryModels = new List<DirectModel>();

        if (!string.IsNullOrEmpty(mainQuery))
          this._database.Execute(mainQuery);

        double ms = (DateTime.Now - create).TotalMilliseconds;
        if (ms > 1500)
        {
          int a = 0;
        }
        Console.WriteLine(string.Format("TransactionalManager is finished {0} tasks after {1}", originalTasks, ms));
        Console.WriteLine(mainQuery);
        Console.WriteLine();
        Console.WriteLine();
      }
      catch(Exception e)
      {
        int a = 0;
      }
      finally
      {
        this.IsRunExecuting = false;
      }
    }

    private void RunInserter()
    {
      try
      {
        if (this._queryInserter.Count == 0 && this._queryLoader.Count == 0)
          return;

        string mainQuery = "START TRANSACTION;";
        string finalSelect = "SELECT ";
        List<string> variables = new List<string>();
        List<DirectModel> modelsToWorkWith = new List<DirectModel>();

        lock (LockObj)
        {
          // Loaders
          foreach (var umodel in this._queryLoader)
          {
            string variable_name = string.Format("{0}_{1}", umodel.Key.IdName, umodel.Key.InternalID);
            mainQuery += umodel.Key.GetDatabase().ConstructVariable(variable_name) + string.Format("({0});", umodel.Value) + Environment.NewLine;
            variables.Add(variable_name);
            finalSelect += (finalSelect.Equals("SELECT ") ? "" : ",") + string.Format("@{0} AS '{0}'", variable_name);
            modelsToWorkWith.Add(umodel.Key);
          }
          this._queryLoader = new Dictionary<DirectModel, string>();
          GC.Collect();
        }

        lock (LockObj)
        {
          // Inserters
          foreach (var model in this._queryInserter)
          {
            string variable_name = string.Format("{0}_{1}", model.IdName, model.InternalID);
            mainQuery += model.ConstructInsertQuery(variables) + Environment.NewLine;
            mainQuery += model.GetDatabase().ConstructVariable(variable_name) + string.Format("(SELECT {0});", model.GetDatabase().QueryScopeID) + Environment.NewLine;
            variables.Add(variable_name);
            finalSelect += (finalSelect.Equals("SELECT ") ? "" : ",") + string.Format("@{0} AS '{0}'", variable_name);
            modelsToWorkWith.Add(model);
          }
          this._queryInserter = new List<DirectModel>();
          GC.Collect();
        }

        DateTime dccounter = DateTime.Now;
        DirectContainer dc = this._database.LoadContainer(mainQuery + "COMMIT;" + finalSelect);
        double miliseonds = (DateTime.Now - dccounter).TotalMilliseconds;

        dccounter = DateTime.Now;
        foreach (string columnname in dc.ColumnNames)
        {
          // We split variable because format is (variableName_modelKey)
          string[] variableSplit = columnname.Split('_');
          string parameterName = variableSplit[0];
          string chidrenID = variableSplit[1];

          int? id = dc.GetInt(columnname);
          foreach (var model in modelsToWorkWith)
            model.CastProperty(parameterName, id.ToString(), chidrenID);
        }
        double miliseonds1 = (DateTime.Now - dccounter).TotalMilliseconds;

        int a = 0;
      }
      catch(Exception e)
      {
        int asdasda = 90;
      }
    }


  }
}
