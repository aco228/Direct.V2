using Direct.Core;
using Direct.Core.Models;
using System;

namespace Direct.livesports.Models
{
  public class LeadDM : DirectModel
  {

    public LeadDM(DirectDatabaseBase db) : base("tm_lead", "leadid", db) { }

    [DColumn(Name = "msisdn")]
    public string msisdn { get; set; } = default;

    [DColumn(Name = "email")]
    public string email { get; set; } = default;

    [DColumn(Name = "first_name")]
    public string first_name { get; set; } = default;

    [DColumn(Name = "last_name")]
    public string last_name { get; set; } = default;

    [DColumn(Name = "country")]
    public string country { get; set; } = default;

    [DColumn(Name = "countryid")]
    public int countryid { get; set; } = default;

    [DColumn(Name = "address")]
    public string address { get; set; } = default;

    [DColumn(Name = "city")]
    public string city { get; set; } = default;

    [DColumn(Name = "zip")]
    public string zip { get; set; } = default;

    [DColumn(Name = "device")]
    public string device { get; set; } = default;

    [DColumn(Name = "operator")]
    public string Operator { get; set; } = default;

    [DColumn(Name = "device_mf")]
    public string device_mf { get; set; } = default;

    [DColumn(Name = "device_os")]
    public string device_os { get; set; } = default;

    [DColumn(Name = "actions_count")]
    public int actions_count { get; set; } = default;

    [DColumn(Name = "updated")]
    public DateTime updated { get; set; } = default;

    [DColumn(Name = "created")]
    public DateTime created { get; set; } = default;

  }
}