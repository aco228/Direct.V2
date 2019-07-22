using Direct.Core.Bulk;
using Direct.Core.Code;
using Direct.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Direct.Core
{
  public abstract partial class DirectDatabaseBase : IDisposable
  {
    private bool _openConnection = false;
    public string DatabaseName { get; protected set; } = string.Empty;
    public string DatabaseScheme { get; protected set; } = string.Empty;
    public string DatabaseSchemeString { get => string.IsNullOrEmpty(this.DatabaseScheme) ? "" : this.DatabaseScheme + "."; }
    public DirectTransactionalManager TransactionalManager { get; protected set; } = null;
    public bool PreventDispose { get; set; } = false;

    protected string ConnectionString { get; private set; } = string.Empty;
    public abstract DirectModelGeneratorBase ModelsCreator { get; }
    public abstract string CurrentDateQueryString { get; }
    public abstract string QueryScopeID { get; }
    public abstract string SelectTopOne { get; }

    public enum DirectDatabaseExceptionType { OnLoadWithOpenConnection, OnLoadWithSharedConnection, OnLoadAsync, OnExecute, OnExecuteAsync }
    public abstract void OnException(DirectDatabaseExceptionType type, string query, Exception e);
    public abstract string ConstructVariable(string name);
    
    public DirectDatabaseBase(string databaseName, string connectionString)
      : this(databaseName, string.Empty, connectionString) { }
    public DirectDatabaseBase(string databaseName, string databaseScheme, string connectionString)
    {
      this.DatabaseName = databaseName;
      this.DatabaseScheme = databaseScheme;
      this.ConnectionString = connectionString;
      this.TransactionalManager = new DirectTransactionalManager(this);
    }

    ~DirectDatabaseBase() => OnDispose();
    public void Dispose() => OnDispose();
    protected void OnDispose()
    {
      // here we will close DirectConnection
      if(!this.PreventDispose)
        this.TransactionalManager.RunAsync(); 
    }


    ///
    /// LOAD METHODS
    ///

    public virtual IEnumerable<DirectContainerRow> LoadEnumerable(string command, params object[] parameters) => this.LoadEnumerable(this.Construct(command, parameters));
    public abstract IEnumerable<DirectContainerRow> LoadEnumerable(string command);

    public virtual DirectLoadResult Load(string query, params object[] parameters) => this.Load(this.Construct(query, parameters));
    public abstract DirectLoadResult Load(string command);
    public virtual Task<DirectLoadResult> LoadAsync(string query, params object[] parameters) => this.LoadAsync(query, this.Construct(query));
    public abstract Task<DirectLoadResult> LoadAsync(string command);

    ///
    /// EXECUTE METHODS
    ///

    public virtual DirectExecuteResult Execute(string query, params object[] parameters) => this.Execute(this.Construct(query, parameters));
    public abstract DirectExecuteResult Execute(string command);
    public virtual Task<DirectExecuteResult> ExecuteAsync(string query, params object[] parameters) => this.ExecuteAsync(this.Construct(query, parameters));
    public abstract Task<DirectExecuteResult> ExecuteAsync(string command);


    ///
    /// HELPERS
    ///

    protected abstract string OnBeforeCommandOverride(string command);
    public abstract string ConstructDateTimeParam(DateTime dt);
    public virtual string ConstructDatabaseNameAndScheme(string query) => query.Replace("[].", string.Format("{0}.{1}", this.DatabaseName, this.DatabaseSchemeString));
    
    // Construct query with multiple parameters
    public virtual string Construct(string query, params object[] parameters)
    {
      if (query.ToLower().Trim().StartsWith("insert into") && !query.ToLower().Contains("values"))
      {
        string[] split = query.Split('(');
        if (split.Length == 2)
        {
          split = split[1].Split(',');
          query += " VALUES ( ";
          for (int i = 0; i < split.Length; i++) query += "{" + i + "}" + (i != split.Length - 1 ? "," : "");
          query += " ); ";
        }
      }

      for (int i = 0; i < parameters.Length; i++)
      {
        string pattern = "{" + i + "}";
        string value = this.GetObjectQueryValue(parameters[i]);
        query = query.Replace(pattern, value);
      }
      return this.ConstructDatabaseNameAndScheme(query);
    }

  }
}
