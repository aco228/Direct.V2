using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core
{
  public abstract partial class DirectDatabaseBase : IDisposable
  {

    ///
    /// Load int
    ///

    public virtual int? LoadInt(string query, params object[] parameters) { return this.LoadInt(this.Construct(query, parameters)); }
    public virtual int? LoadInt(string command)
    {
      int result = -1;
      if (Int32.TryParse(this.LoadString(command), out result))
        return result;
      return null;
    }
    public virtual async Task<int?> LoadIntAsync(string query, params object[] parameters) => await LoadIntAsync(this.Construct(query, parameters));
    public virtual async Task<int?> LoadIntAsync(string command)
    {

      int result = -1;
      if (Int32.TryParse(await this.LoadStringAsync(command), out result))
        return result;
      return null;
    }

    ///
    /// Load double
    ///

    public virtual double? LoadDouble(string query, params object[] parameters) { return this.LoadDouble(this.Construct(query, parameters)); }
    public virtual double? LoadDouble(string command)
    {
      double result = -1;
      if (double.TryParse(this.LoadString(command), out result))
        return result;
      return null;
    }
    public virtual async Task<double?> LoadDoubleAsync(string query, params object[] parameters) => await LoadDoubleAsync(this.Construct(query, parameters));
    public virtual async Task<double?> LoadDoubleAsync(string command)
    {
      double result = -1;
      if (double.TryParse(await this.LoadStringAsync(command), out result))
        return result;
      return null;
    }

    ///
    /// Load array int
    ///

    public virtual List<int> LoadArrayInt(string query, params object[] parameters) { return this.LoadArrayInt(this.Construct(query, parameters)); }
    public virtual List<int> LoadArrayInt(string command)
    {
      List<int> result = new List<int>();
      DirectLoadResult table = this.Load(command);
      if (!table.HasResult)
        return result;

      foreach (DataRow row in table.Rows)
      {
        int p;
        if (Int32.TryParse(row[0].ToString(), out p))
          result.Add(p);
      }

      return result;
    }
    public virtual async Task<List<int>> LoadArrayIntAsync(string query, params object[] parameters) => await LoadArrayIntAsync(this.Construct(query, parameters));
    public virtual async Task<List<int>> LoadArrayIntAsync(string command)
    {
      List<int> result = new List<int>();
      DirectLoadResult table = await this.LoadAsync(command);
      if (!table.HasResult)
        return result;

      foreach (DataRow row in table.Rows)
      {
        int p;
        if (Int32.TryParse(row[0].ToString(), out p))
          result.Add(p);
      }

      return result;

    }

    ///
    /// Load array string
    ///

    public virtual List<string> LoadArrayString(string query, params object[] parameters) { return this.LoadArrayString(this.Construct(query, parameters)); }
    public virtual List<string> LoadArrayString(string command)
    {
      List<string> result = new List<string>();
      DirectLoadResult table = this.Load(command);
      if (!table.HasResult)
        return result;

      foreach (DataRow row in table.Rows)
        result.Add(row[0].ToString());

      return result;
    }
    public virtual async Task<List<string>> LoadArrayStringAsync(string query, params object[] parameters) => await LoadArrayStringAsync(this.Construct(query, parameters));
    public virtual async Task<List<string>> LoadArrayStringAsync(string command)
    {
      List<string> result = new List<string>();
      DirectLoadResult table = await this.LoadAsync(command);
      if (!table.HasResult)
        return result;

      foreach (DataRow row in table.Rows)
        result.Add(row[0].ToString());

      return result;
    }

    ///
    /// Load bool
    ///

    public virtual bool? LoadBool(string query, params object[] parameters) { return this.LoadBool(this.Construct(query, parameters)); }
    public virtual bool? LoadBool(string command)
    {
      string result = this.LoadString(command);
      if (string.IsNullOrEmpty(result))
        return null;
      return result.ToLower().Equals("1") || result.ToLower().Equals("true");
    }
    public virtual async Task<bool?> LoadBoolAsync(string query, params object[] parameters) => await LoadBoolAsync(this.Construct(query, parameters));
    public virtual async Task<bool?> LoadBoolAsync(string command)
    {
      string result = await this.LoadStringAsync(command);
      if (string.IsNullOrEmpty(result))
        return null;
      return result.ToLower().Equals("1") || result.ToLower().Equals("true");
    }

    ///
    /// Load boolean
    ///

    public virtual bool LoadBoolean(string query, params object[] parameters) { return this.LoadBoolean(this.Construct(query, parameters)); }
    public virtual bool LoadBoolean(string command)
    {
      string result = this.LoadString(command);
      if (string.IsNullOrEmpty(result))
        return false;
      return result.ToLower().Equals("1") || result.ToLower().Equals("true");
    }
    public virtual Task<bool> LoadBooleanAsync(string query, params object[] parameters) => Task.Factory.StartNew(() => { return LoadBoolean(query, parameters); });
    public virtual async Task<bool> LoadBooleanAsync(string command)
    {
      string result = await LoadStringAsync(command);
      if (string.IsNullOrEmpty(result))
        return false;
      return result.ToLower().Equals("1") || result.ToLower().Equals("true");
    }

    ///
    /// Load guid
    ///

    public virtual Guid? LoadGuid(string query, params object[] parameters) { return this.LoadGuid(this.Construct(query, parameters)); }
    public virtual Guid? LoadGuid(string command)
    {
      Guid result;
      if (Guid.TryParse(this.LoadString(command), out result))
        return result;
      return null;
    }
    public virtual async Task<Guid?> LoadGuidAsync(string query, params object[] parameters) => await LoadGuidAsync(this.Construct(query, parameters));
    public virtual async Task<Guid?> LoadGuidAsync(string command)
    {

      Guid result;
      if (Guid.TryParse(await this.LoadStringAsync(command), out result))
        return result;
      return null;
    }

    ///
    /// Load Datetime
    ///

    public virtual DateTime? LoadDateTime(string query, params object[] parameters) { return this.LoadDateTime(this.Construct(query, parameters)); }
    public virtual DateTime? LoadDateTime(string command)
    {
      DateTime result;
      if (DateTime.TryParse(this.LoadString(command), out result))
        return result;
      return null;
    }
    public virtual async Task<DateTime?> LoadDateTimeAsync(string query, params object[] parameters) => await LoadDateTimeAsync(this.Construct(query, parameters));
    public virtual async Task<DateTime?> LoadDateTimeAsync(string command)
    {
      DateTime result;
      if (DateTime.TryParse(await this.LoadStringAsync(command), out result))
        return result;
      return null;
    }

    ///
    /// Load string
    ///

    public virtual string LoadString(string query, params object[] parameters) { return this.LoadString(this.Construct(query, parameters)); }
    public virtual string LoadString(string command)
    {
      DirectLoadResult table = this.Load(command);
      if (!table.HasResult)
        return string.Empty;
      return table.Rows[0][0].ToString();
    }
    public virtual async Task<string> LoadStringAsync(string query, params object[] parameters) => await LoadStringAsync(this.Construct(query, parameters));
    public virtual async Task<string> LoadStringAsync(string command)
    {
      DirectLoadResult table = await this.LoadAsync(command);
      if (!table.HasResult)
        return string.Empty;
      return table.Rows[0][0].ToString();
    }

    ///
    /// Load container
    ///

    public virtual DirectContainer LoadContainer(string query, params object[] parameters) { return this.LoadContainer(this.Construct(query, parameters)); }
    public virtual DirectContainer LoadContainer(string command) => this.Load(command).Container;
    public virtual async Task<DirectContainer> LoadContainerAsync(string query, params object[] parameters) => (await this.LoadAsync(this.Construct(query, parameters))).Container;
    public virtual async Task<DirectContainer> LoadContainerAsync(string command) => (await this.LoadAsync(command)).Container;

  }
}
