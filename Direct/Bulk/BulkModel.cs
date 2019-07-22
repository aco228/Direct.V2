using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct.Core.Bulk
{
  public class BulkModel
  {
    protected DirectModel _model = null;

    public int? ID { get => this._model.ID; }
    public string UniqueID { get => this._model.InternalID; }
    public DirectModel Model => this._model;
    public T Cast<T>() where T : DirectModel => (T)this._model;
    public string VariableName => this.Model.BulkVariableName;

    public BulkModel(DirectModel model)
    {
      this._model = model;
    }

    public BulkModel Link(params BulkModel[] links)
    {
      this.Model.Link((from l in links select l.Model).ToArray());
      return this;
    }




  }
}
