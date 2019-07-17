using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core
{
  public static partial class DirectModelHelper
  {

    public static async Task<T> LoadAsync<T>(this DirectQueryLoader<T> loader, int id) where T : DirectModel
    {
      string command = string.Format("SELECT {0} FROM [].{1} WHERE {2}={3};",
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.Instance.GetIdNameValue(), id);

      var dc = await loader.Database.LoadContainerAsync(command);
      if (!dc.HasValue) return null;
      return loader.Cast(dc);
    }

    public static async Task<T> LoadAsync<T>(this DirectQueryLoader<T> loader, string query) where T : DirectModel
      => loader.Cast(await loader.Database.LoadContainerAsync(query));

    public static async Task<List<T>> LoadAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format("SELECT {0} FROM [].{1} {2} {3}",
        loader.SelectQuery,
        loader.Instance.TableName,
        loader.WhereQuery,
        loader.Additional);

      List<T> result = new List<T>();

      var dc = await loader.Database.LoadContainerAsync(command);
      if (!dc.HasValue)
        return result;

      foreach (var entry in loader.CastEnumerable(dc))
        result.Add(entry);

      return result;
    }

    public static async Task<T> LoadSingleAsync<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      string command = string.Format("SELECT {0} FROM [].{1} {2} LIMIT 1",
        loader.SelectQuery,
        loader.Instance.TableName,
        loader.WhereQuery);
      
      var dc = await loader.Database.LoadContainerAsync(command);
      if (!dc.HasValue) return null;
      return loader.Cast(dc);
    }

  }
}
