using System.Collections.Generic;

namespace SLJ.ParcelCosts.Implementation
{
  internal class OrderCosting : IOrderCosting
  {
    public decimal TotalCost { get; set;  }
    public IEnumerable<IParcelCosting> ParcelCosts { get; set; }
  }
}
