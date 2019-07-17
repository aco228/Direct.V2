using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core.Models
{
  public class DirectQueryLoader<T> where T : DirectModel
  {
    private T _instance = null;

    public DirectDatabaseBase Database { get; set; }
    public string Select { get; set; } = string.Empty;
    public string Where { get; set; } = string.Empty;
    public string Additional { get; set; } = string.Empty;

    public void SetWhere(string where, params object[] parameters)
    {
      this.Where = this.Database.Construct(where, parameters);
    }

    public string SelectQuery { get => string.IsNullOrEmpty(this.Select) ? "*" : this.Select; }
    public string WhereQuery { get => string.IsNullOrEmpty(this.Where) ? "" : " WHERE " + this.Where; }
    public T Instance
    {
      get
      {
        if (this._instance != null)
          return this._instance;
        this._instance = (T)Activator.CreateInstance(typeof(T), this.Database); 
        return this._instance;
      }
    }

    public T Cast(DirectContainer dc)
    {
      this.Instance.Cast(dc);
      return this.Instance;
    }

    public IEnumerable<T> CastEnumerable(DirectContainer dc)
    {
      for (int i = 0; i < dc.RowsCount; i++)
      {
        T newValue = (T)Activator.CreateInstance(typeof(T), this.Database);
        newValue.Cast(dc, i);
        yield return newValue;
      }
      yield break;
    }

  }
}
