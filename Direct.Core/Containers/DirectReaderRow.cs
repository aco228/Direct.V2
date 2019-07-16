using System;
using System.Collections.Generic;
using System.Text;

namespace Direct.Core.Containers
{
  public class DirectReaderRow
  {
    public List<string> Columns { get; protected set; } = new List<string>();
    public List<object> Values { get; protected set; } = new List<object>();

    public int ColumnsCount { get => this.Columns.Count; }
    public object this[int index] { get => Values.Count - 1 >= index ? null : this.Values[index]; }
    public string[] ColumnsArray { get => this.Columns.ToArray(); }

    public void Add(string name, object value)
    {
      if(this.Columns.Contains(name))
        return;

      this.Columns.Add(name);
      this.Values.Add(value);
    }

  }
}
