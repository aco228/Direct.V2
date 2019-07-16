using Direct.Core;
using Direct.Core.Models;
using System;

namespace Direct.livesports.Models
{
  public class ClientDM : DirectModel
  {

    public ClientDM() : base("cc_client", "clientid", null) { }
    public ClientDM(DirectDatabaseBase db) : base("cc_client", "clientid", db) { }

    [DColumn(Name = "clickid")]
    public string clickid { get; set; } = default;

    [DColumn(Name = "affid")]
    public int affid { get; set; } = default;

    [DColumn(Name = "payment_provider")]
    public int payment_provider { get; set; } = default;

    [DColumn(Name = "pubid")]
    public string pubid { get; set; } = default;

    [DColumn(Name = "email")]
    public string email { get; set; } = default;

    [DColumn(Name = "firstname")]
    public string firstname { get; set; } = default;

    [DColumn(Name = "lastname")]
    public string lastname { get; set; } = default;

    [DColumn(Name = "country")]
    public string country { get; set; } = default;

    [DColumn(Name = "referrer")]
    public string referrer { get; set; } = default;

    [DColumn(Name = "host")]
    public string host { get; set; } = default;

    [DColumn(Name = "username")]
    public string username { get; set; } = default;

    [DColumn(Name = "password")]
    public string password { get; set; } = default;

    [DColumn(Name = "msisdn")]
    public string msisdn { get; set; } = default;

    [DColumn(Name = "status")]
    public string status { get; set; } = default;

    [DColumn(Name = "address")]
    public string address { get; set; } = default;

    [DColumn(Name = "zip")]
    public string zip { get; set; } = default;

    [DColumn(Name = "city")]
    public string city { get; set; } = default;

    [DColumn(Name = "has_subscription")]
    public int has_subscription { get; set; } = default;

    [DColumn(Name = "has_chargeback")]
    public int has_chargeback { get; set; } = default;

    [DColumn(Name = "has_refund")]
    public int has_refund { get; set; } = default;

    [DColumn(Name = "times_charged")]
    public int times_charged { get; set; } = default;

    [DColumn(Name = "times_upsell")]
    public int times_upsell { get; set; } = default;

    [DColumn(Name = "is_stolen")]
    public int is_stolen { get; set; } = default;

    [DColumn(Name = "updated")]
    public DateTime updated { get; set; } = default;

    [DColumn(Name = "created")]
    public DateTime created { get; set; } = default;

  }
}