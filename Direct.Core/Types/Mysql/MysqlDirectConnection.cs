using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core.Mysql
{
  public class MysqlDirectConnection : DirectConnection
  {
    public MysqlDirectConnection(string connectionString) : base(connectionString) { }

    public override DirectConnectionState State => 
      this.Connection != null && ((MySqlConnection)this.Connection).State == System.Data.ConnectionState.Open ? DirectConnectionState.Open : DirectConnectionState.Unknown;

    protected override object CreateNewConnection()
    {
      lock(DirectConnection.LockObj)
      {
        var connection = new MySqlConnection(this.ConnectionString);
        connection.Open();
        return connection;
      }
    }

    protected override void DestructConnection()
    {
      var connection = (this.Connection as MySqlConnection);
      if (connection == null)
        return;

      lock(DirectConnection.LockObj)
      {
        if(connection.State == System.Data.ConnectionState.Executing || connection.State == System.Data.ConnectionState.Fetching)
          do {
            int a = 0;
          } while (connection.State != System.Data.ConnectionState.Open); // wait until connection is not running
        connection.Close();
        connection.Dispose();
        connection = null;

        if (this.Guid.HasValue)
          Console.WriteLine(" ------------------> " + DateTime.Now.ToString() + " --- " + Guid.ToString() + " we are destructing connection");
      }
    }
  }
}
