using System;
using System.Collections.Generic;

namespace SmsIntegration.Database.Models;

public partial class SmsProvider
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ImplementatorName { get; set; } = null!;

    public bool Active { get; set; }

    public byte ChanceOfSelection { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<SmsFlow> SmsFlows { get; set; } = new List<SmsFlow>();

    public virtual ICollection<SmsProviderConfig> SmsProviderConfigs { get; set; } = new List<SmsProviderConfig>();
}
