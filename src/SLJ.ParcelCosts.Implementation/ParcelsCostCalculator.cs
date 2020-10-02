using System;
using System.Linq;

namespace SLJ.ParcelCosts.Implementation
{
  public class ParcelsCostCalculator : IParcelsCostCalculator
  {
    public IOrderCosting CalculateCosts(IOrder order)
    {
      var parcelCosts = order.Parcels.Select(CalculateParcelCost).ToList();
      return new OrderCosting {
        TotalCost = parcelCosts.Sum(p => p.ParcelCost),
        ParcelCosts = parcelCosts
      };
    }

    private static ParcelCosting CalculateParcelCost(IParcel parcel)
    {
      var parcelCosting = new ParcelCosting {
        Parcel = parcel,
        CostingType = CalculateParcelCostingType(parcel)
      };

      switch ( parcelCosting.CostingType ) {
        case ParcelCostingType.Small:
          parcelCosting.ParcelCost = 3;
          break;
        case ParcelCostingType.Medium:
          parcelCosting.ParcelCost = 8;
          break;
        case ParcelCostingType.Large:
          parcelCosting.ParcelCost = 15;
          break;
        case ParcelCostingType.Unknown:
        case ParcelCostingType.ExtraLarge:
        default:
          parcelCosting.ParcelCost = 25;
          break;
      }

      return parcelCosting;
    }

    private static ParcelCostingType CalculateParcelCostingType(IParcel parcel)
      => parcel.Height < 10 && parcel.Width < 10 && parcel.Depth < 10 ? ParcelCostingType.Small
        : parcel.Height < 50 && parcel.Width < 50 && parcel.Depth < 50 ? ParcelCostingType.Medium
        : parcel.Height < 100 && parcel.Width < 100 && parcel.Depth < 100 ? ParcelCostingType.Large
        : ParcelCostingType.ExtraLarge;
  }
}
