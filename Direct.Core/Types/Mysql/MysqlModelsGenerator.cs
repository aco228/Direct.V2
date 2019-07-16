using Direct.Core.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Direct.Core.Types.Mysql
{
  public class MysqlModelsGenerator : DirectModelGeneratorBase
  {
    public MysqlModelsGenerator(DirectDatabaseBase db) : base(db) { }

    public override void GenerateFile(string tableName, string className, string outputFolder)
    {
      DirectContainer dc = this.Database.LoadContainer(
        @"SELECT 
            TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, 
            CHARACTER_OCTET_LENGTH , NUMERIC_PRECISION , NUMERIC_SCALE AS SCALE, 
            COLUMN_DEFAULT, IS_NULLABLE 
        FROM INFORMATION_SCHEMA.COLUMNS;");

      this.GenerateClassHeader(className);

      int index = 0;
      foreach (var row in dc.Rows)
      {
        string TABLE_NAME = row.GetString("TABLE_NAME");
        string COLUMN_NAME = row.GetString("COLUMN_NAME");
        string DATA_TYPE = row.GetString("DATA_TYPE");
        string CHARACTER_MAXIMUM_LENGTH = row.GetString("CHARACTER_MAXIMUM_LENGTH");
        string CHARACTER_OCTET_LENGTH = row.GetString("CHARACTER_OCTET_LENGTH");
        string NUMERIC_PRECISION = row.GetString("NUMERIC_PRECISION");
        string NUMERIC_SCALE = row.GetString("NUMERIC_SCALE");
        string COLUMN_DEFAULT = row.GetString("COLUMN_DEFAULT");
        string IS_NULLABLE = row.GetString("IS_NULLABLE");

        if (!TABLE_NAME.Equals(tableName))
          continue;

        if (index == 0)
        {
          this.GenerateConstructor(className, tableName, COLUMN_NAME);
          index++;
          continue;
        }

        index++;
        ConvertProperty(DATA_TYPE, COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE);
      }

      this.CreateFile(className, outputFolder);
    }

    private void ConvertProperty(string columnType, string columnName, string columnDefault, string columnAcceptNullable)
    {
      bool isDateTimeUpdate = columnDefault.Equals("CURRENT_TIMESTAMP");
      bool acceptNullable = columnAcceptNullable.Equals("YES");
      string type = ConvertMysqTypeToCsharp(columnType, acceptNullable);
      bool notUpdatable = type.StartsWith("DateTime") && !string.IsNullOrEmpty(columnDefault);

      if(columnName.Equals("actions_count"))
      {
        int a = 0;
      }

      string defaultValue = "";
      if (!string.IsNullOrEmpty(columnDefault) && type.StartsWith("int")) defaultValue = columnDefault;
      else if (!string.IsNullOrEmpty(columnDefault) && type.StartsWith("string")) defaultValue = string.Format("\"{0}\"", columnDefault);
      else if (!string.IsNullOrEmpty(columnDefault) && type.StartsWith("bool")) defaultValue = columnDefault.Equals("1") ? "true" : "false";

      this.GenerateProperty(type, columnName, defaultValue, isDateTimeUpdate, notUpdatable, acceptNullable);
    }

    private string ConvertMysqTypeToCsharp(string type, bool isNullable)
    {
      string result = "";
      switch (type)
      {
        case "int":
          result = "int";
          break;
        case "bigint":
          result = "uint";
          break;
        case "longtext":
        case "varchar":
        case "text":
          result = "string";
          isNullable = false;
          break;
        case "timestamp":
          result = "DateTime";
          break;
        case "double":
        case "decimal":
          result = "double";
          break;
        case "tinyint":
          result = "bool";
          break;
      }

      result += (isNullable ? "?" : "");
      return result;
    }

  }
}
