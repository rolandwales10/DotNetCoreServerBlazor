using System;
using System.Collections.Generic;

namespace FarmshareAdmin.Models;

public partial class vwFarmShareStatus
{
    public string farm_name { get; set; } = null!;

    public int Shares_Allocated { get; set; }

    public int? Shares_Contracted { get; set; }

    public int? Remaining { get; set; }

    public int? Spares { get; set; }
}
