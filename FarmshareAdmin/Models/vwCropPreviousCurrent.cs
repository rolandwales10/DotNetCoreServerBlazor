using System;
using System.Collections.Generic;

namespace FarmshareAdmin.Models;

public partial class vwCropPreviousCurrent
{
    public int FARM_ID { get; set; }

    public string? type { get; set; }

    public string produce { get; set; } = null!;

    public int? prevId { get; set; }

    public decimal? prevAcres { get; set; }

    public bool prevEarly { get; set; }

    public bool prevMid { get; set; }

    public bool prevFall { get; set; }

    public int? curId { get; set; }

    public decimal? curAcres { get; set; }

    public bool curEarly { get; set; }

    public bool curMid { get; set; }

    public bool curFall { get; set; }
}
