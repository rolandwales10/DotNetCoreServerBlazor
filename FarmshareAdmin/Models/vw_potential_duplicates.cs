using System;
using System.Collections.Generic;

namespace FarmshareAdmin.Models;

public partial class vw_potential_duplicates
{
    public int citizen_agreement_id { get; set; }

    public string? farm_name { get; set; }

    public string last_name { get; set; } = null!;

    public string first_name { get; set; } = null!;

    public string? birth_date { get; set; }

    public DateTime? date_enrolled { get; set; }

    public string status { get; set; } = null!;

    public string? termination_reason { get; set; }

    public string? reason_full_value_not_used { get; set; }

    public string? address1 { get; set; }

    public string? address2 { get; set; }

    public string? city { get; set; }

    public string? state { get; set; }

    public string? zip { get; set; }
}
