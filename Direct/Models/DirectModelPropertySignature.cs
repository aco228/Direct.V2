using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Direct.Core.Code
{

  internal class DirectModelPropertySignature
  {
    public string AttributeName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;

    public string Name { get => string.IsNullOrEmpty(this.AttributeName) ? this.PropertyName : this.AttributeName; }
    public bool UpdateDateTime = true;
    public bool Nullable = false;
    public bool NotUpdatable = false;

    public DirectModelPropertySignature(PropertyInfo info)
    {
      this.PropertyName = info.Name;

      Object[] attributes = info.GetCustomAttributes(typeof(DColumn), true);
      if (attributes.Length > 0)
      {
        DColumn attribute = (DColumn)attributes[0];
        this.AttributeName = attribute.Name;
        this.UpdateDateTime = attribute.DateTimeUpdate;
        this.Nullable = attribute.Nullable;
        this.NotUpdatable = attribute.NotUpdatable;
      }

    }
  }
}
