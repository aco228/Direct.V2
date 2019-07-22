using Direct.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Direct.Core.Bulk
{
  public class BulkManager
  {
    private static object LockObj = new object();
    private DirectDatabaseBase Database = null;
    private int CurrentIndex = 0;
    public int CurrentOutputIteration = 0;
    public int CurrentCount => ModelNodes[CurrentIndex].Count;
    public long CurrentSize = 0;

    public string OutputFolder { get; private set; }
    public string OutputFile => string.Format(@"{0}\{1}.sql", this.OutputFolder, this.CurrentOutputIteration);

    public List<List<BulkInstance>> ModelNodes = new List<List<BulkInstance>>();


    public BulkManager(DirectDatabaseBase database, string outputFolder)
    {
      this.OutputFolder = outputFolder;
      this.Database = database;
      this.ModelNodes.Add(new List<BulkInstance>());
    }

    public void Add(params BulkModel[] models)
      => this.ModelNodes[this.CurrentIndex].Add(new BulkInstance(models));

    public void Add(BulkInstance bi)
    {
      lock (LockObj)
      {
        this.ModelNodes[this.CurrentIndex].Add(bi);
      }
    }

    public async Task Iteration()
    {
      Console.WriteLine("BulkManager:: Writing on iteration " + this.CurrentIndex);

      int tempIndex = this.CurrentIndex;

      lock (LockObj)
      {
        this.ModelNodes.Add(new List<BulkInstance>());
        this.CurrentIndex++;
      }

      string content = "";
      foreach (var instance in this.ModelNodes[tempIndex])
        content += instance.ConstructSql();

      File.AppendAllText(this.OutputFile, content);

      this.CurrentSize = new System.IO.FileInfo(this.OutputFile).Length;

      this.ModelNodes[tempIndex] = null;
      GC.Collect();

      return;
    }

    public void ResetOutputIteration()
    {

      Console.WriteLine("BulkManager:: Executing file iteration : ");
      this.CurrentOutputIteration++;
    }




  }
}
