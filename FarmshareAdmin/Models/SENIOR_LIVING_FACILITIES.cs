using System;
using System.Collections.Generic;

namespace FarmshareAdmin.Models;

public partial class SENIOR_LIVING_FACILITIES
{
    public int FACILITY_ID { get; set; }

    public string FACILITY_NAME { get; set; } = null!;

    public string PARENT_COMPANY { get; set; } = null!;

    public string? ADDRESS { get; set; }

    public string? CITY { get; set; }

    public string? STATE { get; set; }

    public string? ZIP { get; set; }

    public string? CONTACT_NAME { get; set; }

    public string? CONTACT_ROLE { get; set; }

    public string? FACILITY_PHONE { get; set; }

    public string? CONTACT_PHONE { get; set; }

    public string? CONTACT_EMAIL { get; set; }

    public string? PHONE { get; set; }

    public string? EMAIL { get; set; }

    public virtual ICollection<CITIZEN_AGREEMENTS> CITIZEN_AGREEMENTS { get; } = new List<CITIZEN_AGREEMENTS>();
}
