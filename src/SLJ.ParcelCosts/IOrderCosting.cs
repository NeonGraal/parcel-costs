using System.Collections.Generic;

namespace SLJ.ParcelCosts
{
  public interface IOrderCosting
  {
    decimal TotalCost { get; }
    IEnumerable<IParcelCosting> ParcelCosts { get; }
  }
}
