using System;
using System.Collections.Generic;

namespace OnlineShop.Api.Models;

public partial class Order
{
    public int Id { get; set; }

    public string UserEmail { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public decimal Total { get; set; }

    public string ShipName { get; set; } = null!;

    public string ShipPhone { get; set; } = null!;

    public string ShipAddress { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
