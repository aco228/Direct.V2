using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core
{
  public static partial class DirectModelHelper
  {

    ///
    /// CREATE MODEL
    ///

    public static T CreateModel<T>(this DirectDatabaseBase db, int loadID) where T : DirectModel
    {
      T temp = (T)Activator.CreateInstance(typeof(T), db);
      temp.LongID = loadID;
      temp.Snapshot.SetSnapshot();
      return temp;
    }

    ///
    /// LOAD BY ID
    ///

    internal static string ContructLoadByID<T>(this DirectQueryLoader<T> loader, long id) where T : DirectModel
    {
      return string.Format("SELECT {0} FROM [].{1} WHERE {2}={3};",
        loader.SelectQuery,
        loader.Instance.GetTableName(),
        loader.Instance.GetIdNameValue(), id);
    }

    public static T Load<T>(this DirectQueryLoader<T> loader, int id) where T : DirectModel
      => Load(loader, (long)id);

    public static T Load<T>(this DirectQueryLoader<T> loader, long id) where T : DirectModel
    {
      var dc = loader.Database.LoadContainer(loader.ContructLoadByID(id));
      if (!dc.HasValue) return null;
      T result =  loader.Cast(dc);
      return result;
    }

    ///
    /// LOAD BY WHERE
    ///

    internal static string ContructLoad<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      return string.Format("SELECT {0} FROM [].{1} {2} {3}",
        loader.SelectQuery,
        loader.Instance.TableName,
        loader.WhereQuery,
        loader.Additional);
    }

    public static T Load<T>(this DirectQueryLoader<T> loader, string query) where T : DirectModel
      => loader.Cast(loader.Database.LoadContainer(query));

    public static IEnumerable<T> LoadEnumerable<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      var dc = loader.Database.LoadContainer(loader.ContructLoad());
      if (!dc.HasValue) return null;

      return loader.CastEnumerable(dc);
    }

    public static List<T> Load<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      List<T> result = new List<T>();
      foreach (var entry in LoadEnumerable<T>(loader))
        result.Add(entry);
      return result;
    }

    public static T LoadLater<T>(this DirectQueryLoader<T> loader) where T : DirectModel
    {
      using (var tempValue = (T)Activator.CreateInstance(typeof(T), (DirectDatabaseBase)null))
        loader.Select = tempValue.IdName;

      T temp = (T)Activator.CreateInstance(typeof(T), loader.Database);
      loader.Database.TransactionalManager.Load(temp, loader.ContructLoad());
      return temp;
    }


  }
}
