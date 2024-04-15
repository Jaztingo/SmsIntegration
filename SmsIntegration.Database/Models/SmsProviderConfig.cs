using System;
using System.Collections.Generic;

namespace SmsIntegration.Database.Models;

public partial class SmsProviderConfig
{
    public int Id { get; set; }

    public int SmsProviderId { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string ConfigValue { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual SmsProvider SmsProvider { get; set; } = null!;
}
