using System;
using System.Collections.Generic;

namespace SmsIntegration.Database.Models;

public partial class Smss
{
    public long Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string MessageText { get; set; } = null!;

    public int ProviderId { get; set; }

    public int AttamptCount { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<SmsFlow> SmsFlows { get; set; } = new List<SmsFlow>();
}
