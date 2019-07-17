using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core
{
  public enum DirectConnectionState { Open, Unknown };
  public abstract class DirectConnection : IDisposable
  {
    public static object LockObj = new object();
    protected object _connection = null;
    public Guid? Guid { get; set; } = null;
    public abstract DirectConnectionState State { get; }

    public object Connection
    {
      get
      {
        if (this._connection != null)
          return this._connection;
        this._connection = this.CreateConnection();
        return this._connection;
      }
    }
    protected string ConnectionString { get; } = null;

    public DirectConnection(string connectionString)
    {
      this.ConnectionString = connectionString;
    }
    ~DirectConnection() => this.OnDispose();
    public void Dispose() => this.OnDispose();

    public void OnDispose()
    {
      if (this._connection != null)
        this.DestructConnection();
    }
    private object CreateConnection()
    {
      this._connection = this.CreateNewConnection();
      return this._connection;
    }
    

    ///
    /// Abstractions
    ///
    protected abstract object CreateNewConnection();
    protected abstract void DestructConnection();

  }
}
