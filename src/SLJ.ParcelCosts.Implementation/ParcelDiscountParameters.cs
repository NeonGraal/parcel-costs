using System;

namespace SLJ.ParcelCosts.Implementation
{
  internal class ParcelDiscountParameters
  {
    public ParcelDiscountParameters(ParcelCostingType costingType, Func<ParcelCosting, bool> selector, int count)
    {
      CostingType = costingType;
      Selector = selector;
      Count = count;
    }

    public ParcelCostingType CostingType { get; }
    public Func<ParcelCosting, bool> Selector { get; }
    public int Count { get; }
  }
}
