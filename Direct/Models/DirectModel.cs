using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Direct.Core;
using Direct.Core.Code;
using Direct.Core.Helpers;

namespace Direct.Core.Models
{
  public abstract class DirectModel : IDisposable
  {
    internal string InternalID { get; } = Guid.NewGuid().ToString().Replace("-", string.Empty);
    internal DirectModelSnapshot Snapshot { get; set; } = null;
    internal string IdName { get; set; } = string.Empty;
    internal string TableName { get; set; } = string.Empty;
    internal string BulkVariableName => string.Format("{0}_{1}", this.IdName, this.InternalID);
    internal DirectDatabaseBase Database { get; } = null;

    internal List<DirectModel> Dependants { get; } = new List<DirectModel>(); // Links that we will emmit ID change
    internal List<DirectModel> Dependecies { get; } = new List<DirectModel>(); // Links from which we expect emmit of ID

    public long? LongID { get; internal set; } = null;
    public int? ID { get => (int?)LongID; }
    public DirectDatabaseBase GetDatabase() => this.Database;

    ///
    /// CONSTREUCTOR && DESONSTRUCTOR
    ///

    public DirectModel(string tableName, string id_name, DirectDatabaseBase db)
    {
      this.TableName = tableName;
      this.IdName = id_name;
      this.Database = db;
      this.Snapshot = new DirectModelSnapshot(this);

      this.PrepareProperties();
      this.Snapshot.SetSnapshot();
    }

    ~DirectModel() => OnDispose();
    public void Dispose() => OnDispose();

    protected void OnDispose()
    {
      if(this.Database != null)
        this.Database.Dispose();
    }

    ///
    /// Get data
    ///

    internal string GetTableName() => this.TableName;
    internal string GetIdNameValue() => this.IdName;


    ///
    /// Overrides
    ///
    
    public virtual void OnBeforeInsert() { }
    public virtual void OnBeforeUpdate() { }
    public virtual void OnBeforeDelete() { }

    //
    // SUMMARY: Properties manipulation
    internal void PrepareProperties()
      => this.Snapshot.PrepareProperties();

    internal DirectDatabaseBase GetDatabase(DirectDatabaseBase db = null)
    {
      if (db != null) return db;
      if (this.Database != null) return this.Database;
      throw new Exception("Database is not set!!");
    }

    ///
    /// LINKS and dependencies
    ///

    internal void EmitChange()
    {
      foreach (var parent in this.Dependants)
        parent.OnEmit(this);
    }

    // SUMMARY: On call from chidlen (emmit for change of the ID)
    internal void OnEmit(DirectModel emitter)
      => this.CastProperty(emitter.IdName, emitter.ID.ToString());

    internal void AddDependant(DirectModel parentModel)
    {
      if (!this.Dependants.Contains(parentModel))
        this.Dependants.Add(parentModel);
    }

    public DirectModel Link(params DirectModel[] elems)
    {
      foreach(var elem in elems)
      {
        if (elem == null)
          continue;

        elem.AddDependant(this);
        this.Dependecies.Add(elem);
      }
      return this;
    }

    internal string GetBulkInsertQuery(List<string> allVariables)
    {
      string query = this.ConstructInsertQuery(allVariables) + Environment.NewLine;
      if (this.Dependants.Count > 0)
        query += this.GetDatabase().ConstructVariable(this.BulkVariableName) + string.Format("(SELECT {0});", this.GetDatabase().QueryScopeID) + Environment.NewLine;
      return query;
    }

    ///
    /// LOAD
    /// 


    ///
    /// INSERT
    /// 

    public void Insert(DirectDatabaseBase db = null) => this.GetDatabase(db).Insert<DirectModel>(this);
    public T Insert<T>(DirectDatabaseBase db = null) where T: DirectModel => this.GetDatabase(db).Insert<T>(this);

    public Task InsertAsync(DirectDatabaseBase db = null) => this.GetDatabase(db).InsertAsync<DirectModel>(this);
    public Task<T> InsertAsync<T>(DirectDatabaseBase db = null) where T : DirectModel => this.GetDatabase(db).InsertAsync<T>(this);

    public void InsertLater(DirectDatabaseBase db = null) => this.GetDatabase(db).TransactionalManager.Insert(this);
    public void InsertOrUpdate(DirectDatabaseBase db = null) => this.GetDatabase(db).InsertOrUpdate(this);
    public async Task InsertOrUpdateAsync(DirectDatabaseBase db = null) => await this.GetDatabase(db).InsertOrUpdateAsync(this);


    /// 
    /// UPDATE
    /// 

    public void Update(DirectDatabaseBase db = null) => this.GetDatabase(db).Update(this);
    public void UpdateLater() => this.GetDatabase().TransactionalManager.Add(this);
    public async Task UpdateAsync(DirectDatabaseBase db = null) => await this.GetDatabase(db).UpdateAsync(this);
    
    /// 
    /// DELETE
    /// 

    public bool Delete(DirectDatabaseBase db = null) => this.GetDatabase(db).Delete(this);


    /// 
    /// CAST
    /// 

    internal Task CastProperty(string propertyName, string value, string modelInternalID = "")
    {
      return Task.Factory.StartNew(() =>
      {
        // We check if this property belongs to this models children (models that we expect to link to)
        if (!string.IsNullOrEmpty(modelInternalID)
          && (!modelInternalID.Equals(this.InternalID) && (from d in this.Dependecies where d.InternalID.Equals(modelInternalID) select d).FirstOrDefault() == null))
        {
          return;
        }


        if (propertyName.Equals(this.IdName))
        {
          DirectCastHelper.ConvertProperty<DirectModel>(this, this.Snapshot.IdPropertyInfo, value);
          this.EmitChange();
          return;
        }

        var prop = this.Snapshot.GetProperty(propertyName);
        if (prop == null)
          return;

        DirectCastHelper.ConvertProperty<DirectModel>(this, prop, value);
      });
    }

    public DirectModel Cast(DirectContainer dc, int depth = 0)
    {
      // convert ID
      int? columnIndex = dc.GetColumnIndexByName(this.IdName);
      if (columnIndex.HasValue)
      {
        if (Snapshot.IdPropertyInfo == null)
          throw new Exception("IdProperty info is not defined");

        this.EmitChange();
        dc.ConvertProperty<DirectModel>(this, Snapshot.IdPropertyInfo, dc.ColumnNames[columnIndex.Value], depth);
      }

      for(int i = 0; i < Snapshot.PropertySignatures.Count; i++)
      {
        var prop = Snapshot.PropertySignatures[i];

        columnIndex = dc.GetColumnIndexByName(prop.Name);
        if (!columnIndex.HasValue)
          continue;

        dc.ConvertProperty<DirectModel>(this, this.Snapshot.Properties[i], dc.ColumnNames[columnIndex.Value], depth);
      }

      this.Snapshot.SetSnapshot();
      return this;
    }


  }
}
