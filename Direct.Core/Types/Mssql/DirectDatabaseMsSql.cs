using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Direct.Core.Code;

namespace Direct.Core.Mssql
{
  public class DirectDatabaseMsSql : DirectDatabaseBase
  {

    public DirectDatabaseMsSql(string databaseName, string connectionString, bool openConnection) : base(databaseName, connectionString)
    { }

    public override string CurrentDateQueryString => "getdate()";
    public override string QueryScopeID => "SCOPE_IDENTITY()";
    public override string SelectTopOne => "SELECT TOP 1 * FROM [].{0}";
    public override DirectModelGeneratorBase ModelsCreator => throw new NotImplementedException();
    public override string ConstructVariable(string name) => string.Format("SET @{0} =", name);
    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e) { }





    public override string ConstructDateTimeParam(DateTime dt) => string.Format("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss:fff"));
    protected override string OnBeforeCommandOverride(string command) => "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED " + command;

    public override IEnumerable<DirectContainerRow> LoadEnumerable(string command)
    {
      command = this.OnBeforeCommandOverride(this.ConstructDatabaseNameAndScheme(command));

      using (var sqlConnection = new SqlConnection(this.ConnectionString))
      using (var sqlCommand = new SqlCommand(command, sqlConnection))
      using (var sqlAdapter = new SqlDataAdapter(sqlCommand))
      {
        sqlConnection.Open();
        DataTable table = new DataTable();
        sqlAdapter.Fill(table);
        foreach (DataRow row in table.Rows)
          yield return new DirectContainerRow(row);

        sqlConnection.Close();
      }

      yield break;
    }
    public override DirectLoadResult Load(string command)
    {
      command = this.OnBeforeCommandOverride(this.ConstructDatabaseNameAndScheme(command));
      DirectLoadResult result = new DirectLoadResult();

      using (var sqlConnection = new SqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new SqlCommand(command, sqlConnection))
          using (var sqlAdapter = new SqlDataAdapter(sqlCommand))
          {
            sqlConnection.Open();
            result.DataTable = new DataTable();
            sqlAdapter.Fill(result.DataTable);
          }

        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnLoadWithOpenConnection, command, e);
        }
        finally
        {
          sqlConnection.Close();
        }
      }

      return result;
    }
    public override async Task<DirectLoadResult> LoadAsync(string command)
    {
      command = this.OnBeforeCommandOverride(this.ConstructDatabaseNameAndScheme(command));
      DirectLoadResult result = new DirectLoadResult();

      using (var sqlConnection = new SqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new SqlCommand(command, sqlConnection))
          using (var sqlAdapter = new SqlDataAdapter(sqlCommand))
          {
            await sqlConnection.OpenAsync();
            result.DataTable = new DataTable();
            sqlAdapter.Fill(result.DataTable);
          }
        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnLoadAsync, command, e);
        }
        finally
        {
          sqlConnection.Close();
        }
      }

      return result;
    }

    public override DirectExecuteResult Execute(string command)
    {
      command = this.OnBeforeCommandOverride(command);
      command = this.ConstructDatabaseNameAndScheme(command);
      DirectExecuteResult result = new DirectExecuteResult();

      using (var sqlConnection = new SqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new SqlCommand(command, sqlConnection))
          {
            sqlConnection.Open();
            result.NumberOfRowsAffected = sqlCommand.ExecuteNonQuery();

            #region # get id of inserted object #

            if (command.ToLower().Contains("insert into "))
            {
              sqlCommand.CommandText = "SELECT SCOPE_IDENTITY()";
              SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
              DataTable table = new DataTable();
              adapter.Fill(table);
              string resultString = "";
              if (table != null)
                resultString = table.Rows[0][0].ToString();
              int insertedID;
              if (Int32.TryParse(resultString, out insertedID))
                result.LastID = insertedID;
            }

            #endregion
          }
        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnExecute, command, e);
        }
        finally
        {
          sqlConnection.Close();
        }
      }

      return result;
    }
    public override async Task<DirectExecuteResult> ExecuteAsync(string command)
    {
      command = this.OnBeforeCommandOverride(command);
      command = this.ConstructDatabaseNameAndScheme(command);
      DirectExecuteResult result = new DirectExecuteResult();

      using (var sqlConnection = new SqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new SqlCommand(command, sqlConnection))
          {
            await sqlConnection.OpenAsync();
            result.NumberOfRowsAffected = await sqlCommand.ExecuteNonQueryAsync();

            #region # get id of inserted object #

            if (command.ToLower().Contains("insert into "))
            {
              sqlCommand.CommandText = "SELECT SCOPE_IDENTITY()";
              SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
              DataTable table = new DataTable();
              adapter.Fill(table);
              string resultString = "";
              if (table != null)
                resultString = table.Rows[0][0].ToString();
              int insertedID;
              if (Int32.TryParse(resultString, out insertedID))
                result.LastID = insertedID;
            }

            #endregion

          }
        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnExecuteAsync, command, e);
        }
        finally
        {
          sqlConnection.Close();
        }
      }

      return result;
    }

  }
}
