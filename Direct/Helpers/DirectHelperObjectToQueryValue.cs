using Direct.Core.Code;
using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Direct.Core.Helpers
{
  public static class DirectHelperObjectToQueryValue
  {

    internal static string GetObjectQueryValue(this DirectDatabaseBase db, PropertyInfo obj, DirectModelPropertySignature signature, object parentObject)
    {
      if (obj == null)
        return "NULL";

      if (signature.UpdateDateTime)
        return db.CurrentDateQueryString;

      object value = obj.GetValue(parentObject);
      var type = obj.PropertyType;

      if (type == typeof(DirectTime))
        return db.CurrentDateQueryString;
      else if (type == typeof(DirectScopeID))
        return db.QueryScopeID;
      else if (type == typeof(bool))
        return (bool)value ? "1" : "0";
      else if (type == typeof(int) || type == typeof(double) || type == typeof(long)
        || type == typeof(uint) || type == typeof(ulong) || type == typeof(short)
        || type == typeof(int?) || type == typeof(double?) || type == typeof(long?)
        || type == typeof(uint?) || type == typeof(ulong?) || type == typeof(short?))
        return value.ToString();
      else if (type == typeof(string) || type == typeof(String) || type == typeof(char))
        return string.Format("'{0}'", value.ToString().Replace("'", string.Empty));
      else if (type == typeof(DateTime))
      {
        DateTime? dt = value as DateTime?;
        if (dt != null)
        {
          if (dt.Value == default(DateTime))
            return "NULL";
          else
            return db.ConstructDateTimeParam(dt.Value); ;
        }
        else
          return "NULL";
      }

      return "NULL";
    }


    internal static string GetObjectQueryValue(this DirectDatabaseBase db, object obj)
    {
      if (obj == null)
        return "NULL";

      var type = obj.GetType();
      if (type == typeof(DirectTime))
        return db.CurrentDateQueryString;
      else if (type == typeof(DirectScopeID))
        return db.QueryScopeID;
      else if (type == typeof(int) || type == typeof(double) || type == typeof(long)
        || type == typeof(uint) || type == typeof(ulong) || type == typeof(short))
        return obj.ToString();
      else if (type == typeof(string) || type == typeof(String) || type == typeof(char))
        return string.Format("'{0}'", obj.ToString());
      else if (type == typeof(DateTime))
      {
        DateTime? dt = obj as DateTime?;
        if (dt != null)
          return db.ConstructDateTimeParam(dt.Value);
        else
          return "NULL";
      }

      return "NULL";
    }


  }
}
