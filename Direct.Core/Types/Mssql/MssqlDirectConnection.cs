using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Direct.Core.Mssql
{
  public class MssqlDirectConnection : DirectConnection
  {
    public MssqlDirectConnection(string connectionString) : base(connectionString) { }

    public override DirectConnectionState State =>
      this.Connection != null && ((SqlConnection)this.Connection).State == System.Data.ConnectionState.Open ? DirectConnectionState.Open : DirectConnectionState.Unknown;

    protected override object CreateNewConnection()
    {
      var connection = new SqlConnection(this.ConnectionString);
      connection.Open();
      return connection;
    }

    protected override void DestructConnection()
    {
      var connection = this.Connection as SqlConnection;
      if (connection == null)
        return;
      connection.Close();
      connection.Dispose();
    }
  }
}
