using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class Address
{
    public Guid Id { get; set; }

    public string Address1 { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;
}
