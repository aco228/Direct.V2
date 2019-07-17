using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core
{
  public static partial class DirectModelHelper
  {

    public static DirectQueryLoader<T> Query<T>(this DirectDatabaseBase db) where T : DirectModel
      => new DirectQueryLoader<T>() { Database = db };

    public static DirectQueryLoader<T> Where<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      loader.Where = input;
      return loader;
    }
    public static DirectQueryLoader<T> Where<T>(this DirectQueryLoader<T> loader, string input, params object[] parameters) where T : DirectModel
    {
      loader.SetWhere(input, parameters);
      return loader;
    }
    public static DirectQueryLoader<T> Select<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      using (var tempValue = (T)Activator.CreateInstance(typeof(T), (DirectDatabaseBase)null))
      {
        if (loader.Select.Equals("id"))
          loader.Select = tempValue.IdName;
        else if (!string.IsNullOrEmpty(input))
          loader.Select = tempValue.IdName + "," + input;
        else
          loader.Select = "*";
      }
      return loader;
    }
    public static DirectQueryLoader<T> Additional<T>(this DirectQueryLoader<T> loader, string input) where T : DirectModel
    {
      loader.Additional = input;
      return loader;
    }


  }
}
