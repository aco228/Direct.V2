﻿using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core
{
  public static partial class DirectModelHelper
  {

    public static async Task InsertOrUpdateAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (model.LongID.HasValue)
        await UpdateAsync(db, model);
      else
        await InsertAsync<DirectModel>(db, model);
    }

    public static async Task<T> InsertAsync<T>(this DirectDatabaseBase db, DirectModel model) where T : DirectModel
    {
      string command = string.Format("INSERT INTO {0}.{1}{2} ({3}) VALUES ({4});",
        db.DatabaseName, db.DatabaseSchemeString, model.GetTableName(),
        model.Snapshot.GetPropertyNamesForInsert(), model.Snapshot.GetPropertyValuesForInsert());
      DirectExecuteResult result = await db.ExecuteAsync(command);
      if (result.IsSuccessfull && result.LastID.HasValue)
      {
        model.LongID = result.LastID;
        model.Snapshot.SetSnapshot();
        return (T)model;
      }
      return (T)model;
    }

    public static async Task<int?> UpdateAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (!model.LongID.HasValue)
        throw new Exception("ID is not set, maybe this table was not loaded");

      // UPDATE MobilePaywall.core.A SET A=1 WHERE AID=1
      string command = string.Format("UPDATE {0}.{1}{2} SET {3} WHERE {4}={5};",
        db.DatabaseName, db.DatabaseSchemeString, model.GetTableName(),
        model.Snapshot.GetUpdateData(),
        model.GetIdNameValue(), model.LongID.Value);

      DirectExecuteResult result = await db.ExecuteAsync(command);
      if (!result.IsSuccessfull)
        return null;
      else
      {
        model.Snapshot.SetSnapshot();
        return result.NumberOfRowsAffected;
      }
    }

    public static async Task<bool> DeleteAsync(this DirectDatabaseBase db, DirectModel model)
    {
      if (!model.LongID.HasValue)
        throw new Exception("THIS model has not ID");

      string command = string.Format("DELETE FROM {0}.{1}{2} WHERE {3}={4};",
        db.DatabaseName, db.DatabaseSchemeString, model.GetTableName(),
        model.GetIdNameValue(), model.LongID.Value);
      DirectExecuteResult result = await db.ExecuteAsync(command);
      if (result.IsSuccessfull)
      {
        model.LongID = null;
        model.Snapshot.SetSnapshot();
        return true;
      }
      return false;
    }

  }
}
