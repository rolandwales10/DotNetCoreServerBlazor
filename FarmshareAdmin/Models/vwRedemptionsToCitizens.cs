using System;
using System.Collections.Generic;

namespace FarmshareAdmin.Models;

public partial class vwRedemptionsToCitizens
{
    public int CITIZEN_AGREEMENT_ID { get; set; }

    public int FARM_ID { get; set; }

    public string FARM_NAME { get; set; } = null!;

    public string FIRST_NAME { get; set; } = null!;

    public string LAST_NAME { get; set; } = null!;

    public string? STATUS { get; set; }

    public string? REASON_FULL_VALUE_NOT_USED { get; set; }

    public string? EXPLANATION { get; set; }

    public decimal? amount_distributed { get; set; }
}
