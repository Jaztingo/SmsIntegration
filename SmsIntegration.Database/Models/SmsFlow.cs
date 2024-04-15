using System;
using System.Collections.Generic;

namespace SmsIntegration.Database.Models;

public partial class SmsFlow
{
    public long Id { get; set; }

    public long SmsId { get; set; }

    public int Status { get; set; }

    public int? ProviderId { get; set; }

    public string Data { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual SmsProvider? Provider { get; set; }

    public virtual Smss Sms { get; set; } = null!;
}
