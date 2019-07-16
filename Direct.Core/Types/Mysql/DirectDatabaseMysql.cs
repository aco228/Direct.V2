using Direct.Core.Code;
using Direct.Core.Containers;
using Direct.Core.Types.Mysql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Direct.Core.Mysql
{
  public class DirectDatabaseMysql : DirectDatabaseBase
  {

    public DirectDatabaseMysql(string databaseName, string connectionString) : base(databaseName, connectionString)
    {
    }

    public override string CurrentDateQueryString => "CURRENT_TIMESTAMP";
    public override string QueryScopeID => "LAST_INSERT_ID()";
    public override string SelectTopOne => "SELECT * FROM [].{0} LIMIT 1";
    public override DirectModelGeneratorBase ModelsCreator => new MysqlModelsGenerator(this);
    public override string ConstructVariable(string name) => string.Format("SET @{0} :=", name);
    public override void OnException(DirectDatabaseExceptionType type, string query, Exception e) { }

    protected override string OnBeforeCommandOverride(string command) => command;
    public override string ConstructDateTimeParam(DateTime dt) => string.Format("'{0}'", dt.ToString("yyyy-MM-dd HH:mm:ss"));

    public override IEnumerable<DirectContainerRow> LoadEnumerable(string command)
    {
      command = this.OnBeforeCommandOverride(this.ConstructDatabaseNameAndScheme(command));

      using (var sqlConnection = new MySqlConnection(this.ConnectionString))
      using (var sqlCommand = new MySqlCommand(command, sqlConnection))
      using (var sqlAdapter = new MySqlDataAdapter(sqlCommand))
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

      using (var sqlConnection = new MySqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new MySqlCommand(command, sqlConnection))
          using (var sqlAdapter = new MySqlDataAdapter(sqlCommand))
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

      using (var sqlConnection = new MySqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new MySqlCommand(command, sqlConnection))
          using (var sqlAdapter = new MySqlDataAdapter(sqlCommand))
          {
            await sqlConnection.OpenAsync();
            result.DataTable = new DataTable();
            await sqlAdapter.FillAsync(result.DataTable);
          }
        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnLoadAsync, command, e);
        }
        finally
        {
          await sqlConnection.CloseAsync();
        }
      }
      
      return result;
    }

    public override DirectExecuteResult Execute(string command)
    {
      command = this.OnBeforeCommandOverride(command);
      command = this.ConstructDatabaseNameAndScheme(command);
      DirectExecuteResult result = new DirectExecuteResult();

      using (var sqlConnection = new MySqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new MySqlCommand(command, sqlConnection))
          {
            sqlConnection.Open();
            result.NumberOfRowsAffected = sqlCommand.ExecuteNonQuery();
            result.LastID = sqlCommand.LastInsertedId;
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

      using (var sqlConnection = new MySqlConnection(this.ConnectionString))
      {
        try
        {
          using (var sqlCommand = new MySqlCommand(command, sqlConnection))
          {
            await sqlConnection.OpenAsync();
            result.NumberOfRowsAffected = await sqlCommand.ExecuteNonQueryAsync();
            result.LastID = sqlCommand.LastInsertedId;
          }
        }
        catch (Exception e)
        {
          result.Exception = e;
          this.OnException(DirectDatabaseExceptionType.OnExecuteAsync, command, e);
        }
        finally
        {
          await sqlConnection.CloseAsync();
        }
      }

      return result;
    }

  }
}
