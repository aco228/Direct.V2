using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct.Core.Bulk
{
  ///
  /// Wrapper for linked DirectModels
  ///

  public class BulkInstance
  {

    public List<BulkModel> Models { get; protected set; } = new List<BulkModel>();


    public BulkInstance(params BulkModel[] models)
    {
      this.Models.AddRange(models.ToList());
    }

    public void Add(BulkModel m)
      => this.Models.Add(m);

    public string ConstructSql()
    {
      string query = string.Empty;
      List<string> variables = (from m in this.Models select m.VariableName).ToList();

      foreach(var model in this.Models)
        query += model.Model.GetBulkInsertQuery(variables);

      return query;
    }




  }
}
