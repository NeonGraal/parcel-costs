using System;
using System.Collections.Generic;
using System.Linq;

namespace SLJ.ParcelCosts.Implementation
{
  public class ParcelsCostCalculator : IParcelsCostCalculator
  {
    const decimal SmallMaxDimension = 10;
    const decimal MediumMaxDimension = 50;
    const decimal LargeMaxDimension = 100;

    readonly IReadOnlyDictionary<ParcelCostingType, decimal> ParcelCosts = new Dictionary<ParcelCostingType, decimal> {
      [ParcelCostingType.Small] = 3,
      [ParcelCostingType.Medium] = 8,
      [ParcelCostingType.Large] = 15,
      [ParcelCostingType.ExtraLarge] = 25,
    };

    readonly IReadOnlyDictionary<ParcelCostingType, decimal> ParcelMaxWeight = new Dictionary<ParcelCostingType, decimal> {
      [ParcelCostingType.Small] = 1,
      [ParcelCostingType.Medium] = 3,
      [ParcelCostingType.Large] = 6,
      [ParcelCostingType.ExtraLarge] = 10,
    };

    public IOrderCosting CalculateCosts(IOrder order)
    {
      var parcelCosts = order?.Parcels?.Select(CalculateParcelCost).ToList() ?? new List<ParcelCosting>();

      if ( order?.SpeedyShipping == true ) {
        var speedyCost = new ParcelCosting {
          ParcelCost = parcelCosts.Sum(p => p.ParcelCost),
          CostingType = ParcelCostingType.SpeedyShipping
        };
        parcelCosts.Add(speedyCost);
      }

      return new OrderCosting {
        TotalCost = parcelCosts.Sum(p => p.ParcelCost),
        ParcelCosts = parcelCosts
      };
    }

    ParcelCosting CalculateParcelCost(IParcel parcel)
    {
      var parcelCosting = new ParcelCosting {
        Parcel = parcel,
        CostingType = CalculateParcelCostingType(parcel)
      };

      parcelCosting.ParcelCost = ParcelCosts[parcelCosting.CostingType];

      var overweight = parcel.Weight - ParcelMaxWeight[parcelCosting.CostingType];

      if ( overweight > 0 ) {
        parcelCosting.ParcelCost += 2 * Math.Ceiling(overweight);
      }

      return parcelCosting;
    }

    ParcelCostingType CalculateParcelCostingType(IParcel parcel)
      => AllParcelDimensionsLessThan(parcel, SmallMaxDimension) ? ParcelCostingType.Small
        : AllParcelDimensionsLessThan(parcel, MediumMaxDimension) ? ParcelCostingType.Medium
        : AllParcelDimensionsLessThan(parcel, LargeMaxDimension) ? ParcelCostingType.Large
        : ParcelCostingType.ExtraLarge;

    bool AllParcelDimensionsLessThan(IParcel parcel, decimal maxDimension)
      => parcel.Height < maxDimension && parcel.Width < maxDimension && parcel.Depth < maxDimension;
  }
}
