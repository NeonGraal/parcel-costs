using System.Linq;

namespace SLJ.ParcelCosts.Implementation
{
  public class ParcelsCostCalculator : IParcelsCostCalculator
  {
    public IOrderCosting CalculateCosts(IOrder order)
      => new OrderCosting {
        TotalCost = 3,
        ParcelCosts = order.Parcels.Select(p => new ParcelCosting {
          Parcel = p, ParcelCost = 3, CostingType = ParcelCostingType.Small
        })
      };
  }
}
