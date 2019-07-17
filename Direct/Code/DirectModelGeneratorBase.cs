using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Direct.Core.Code
{
  public abstract class DirectModelGeneratorBase
  {
    protected DirectDatabaseBase Database = null;
    protected List<string> DefaultHeaderLines = new List<string>();
    protected List<string> MainClassLines = new List<string>();
    protected List<string> DerivatedClassLines = new List<string>();
    protected List<string> DefaultFooterLines = new List<string>();

    public DirectModelGeneratorBase(DirectDatabaseBase db)
    {
      this.Database = db;

      DefaultHeaderLines.Add("using Direct.Core;");
      DefaultHeaderLines.Add("using Direct.Core.Models;");
      DefaultHeaderLines.Add("using System;");
      DefaultHeaderLines.Add("");
      DefaultHeaderLines.Add("namespace Direct." + db.DatabaseName + ".Models");
      DefaultHeaderLines.Add("{");

      DefaultFooterLines.Add("}");
      DefaultFooterLines.Add("}");

    }

    protected virtual string ConstructClassName(string name)
    {
      return name + "DM";
    }

    protected virtual void GenerateClassHeader(string className)
    {
      this.MainClassLines.Add(string.Format("public partial class {0} : DirectModel", this.ConstructClassName(className)));
      this.MainClassLines.Add("{");
      this.MainClassLines.Add("");

      this.DerivatedClassLines.Add(string.Format("public partial class {0} : DirectModel", this.ConstructClassName(className)));
      this.DerivatedClassLines.Add("{");
      this.DerivatedClassLines.Add("");
    }

    protected virtual void GenerateConstructor(string className, string tableName, string idName)
    {
      this.MainClassLines.Add(string.Format("public {0}(DirectDatabaseBase db) : base(\"{1}\", \"{2}\", db)", this.ConstructClassName(className), tableName, idName) + "{}");
      this.MainClassLines.Add("");
    }
    
    protected virtual void GenerateProperty(string type, string name, string defaultProperty, bool isDateTimeUpdate, bool notUpdatable, bool isNullable)
    {
      string DateTimeUpdate = isDateTimeUpdate && !notUpdatable ? ", DateTimeUpdate = true" : "";
      string Nullable = isNullable && !notUpdatable ? ", Nullable = true" : "";
      string NotUpdatable = notUpdatable ? ", NotUpdatable = true" : "";

      this.MainClassLines.Add(string.Format("[DColumn(Name = \"{0}\"{1}{2}{3})]", name, DateTimeUpdate, Nullable, NotUpdatable));

      this.MainClassLines.Add(string.Format("public {0} {1} {2} = {3};", 
        type, 
        name, 
        "{ get; set; }", 
        (string.IsNullOrEmpty(defaultProperty) ? "default" : defaultProperty) ));

      this.MainClassLines.Add("");
    }

    protected virtual void CreateFile(string className, string output)
    {
      className = this.ConstructClassName(className);
      List<string> mainFileLines = new List<string>();
      mainFileLines.AddRange(DefaultHeaderLines);
      mainFileLines.AddRange(MainClassLines);
      mainFileLines.AddRange(DefaultFooterLines);

      List<string> derivatedFileLines = new List<string>();
      derivatedFileLines.AddRange(DefaultHeaderLines);
      derivatedFileLines.AddRange(DerivatedClassLines);
      derivatedFileLines.AddRange(DefaultFooterLines);

      DirectoryInfo definitionDirectory = new DirectoryInfo(output + "/Definition");
      if (!definitionDirectory.Exists) definitionDirectory.Create();
      DirectoryInfo modelsDirectory = new DirectoryInfo(output + "/Models");
      if (!modelsDirectory.Exists) modelsDirectory.Create();


      File.WriteAllLines(modelsDirectory.FullName + "/" + className + ".cs", mainFileLines.ToArray());
      File.WriteAllLines(definitionDirectory.FullName + "/" + className + ".cs", derivatedFileLines.ToArray());
      Console.WriteLine(className + " finished!");
    }

    public abstract void GenerateFile(string tableName, string className, string outputFolder);



  }
}
