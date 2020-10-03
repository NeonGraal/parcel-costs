using System;
using System.Collections.Generic;
using System.Linq;

namespace SLJ.ParcelCosts.Implementation
{
  public class ParcelsCostCalculator : IParcelsCostCalculator
  {
    readonly IEnumerable<ParcelCostingParameters> ParcelCostings = new List<ParcelCostingParameters> {
      new ParcelCostingParameters(ParcelCostingType.Small, 3, 1, 2, 10),
      new ParcelCostingParameters(ParcelCostingType.Medium, 8, 3, 2, 50),
      new ParcelCostingParameters(ParcelCostingType.Large, 15, 6, 2, 100),
      new ParcelCostingParameters(ParcelCostingType.ExtraLarge, 25, 10, 2),
      new ParcelCostingParameters(ParcelCostingType.Heavy, 50, 50, 1),
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
      var parcelCostings = ParcelCostings
        .Where(c => ValidParcelCosting(parcel, c))
        .Select(c => MakeParcelCosting(parcel, c))
        .OrderBy(c => c.ParcelCost);

      return parcelCostings.FirstOrDefault();
    }

    bool ValidParcelCosting(IParcel parcel, ParcelCostingParameters costParams)
      => !costParams.DimensionsMax.HasValue || AllParcelDimensionsLessThan(parcel, costParams.DimensionsMax.Value);

    ParcelCosting MakeParcelCosting(IParcel parcel, ParcelCostingParameters costParams)
    {
      var result = new ParcelCosting {
        Parcel = parcel,
        CostingType = costParams.CostingType,
        ParcelCost = costParams.BaseCost
      };

      var overweight = parcel.Weight - costParams.MaxWeight;
      if ( overweight > 0 ) {
        result.ParcelCost += Math.Ceiling(overweight) * costParams.CostPerExtraKg;
      }

      return result;
    }

    bool AllParcelDimensionsLessThan(IParcel parcel, decimal maxDimension)
      => parcel.Height < maxDimension && parcel.Width < maxDimension && parcel.Depth < maxDimension;
  }
}
