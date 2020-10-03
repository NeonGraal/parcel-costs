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

    readonly IReadOnlyDictionary<ParcelCostingType, ParcelCostingParameters> ParcelCosts = new Dictionary<ParcelCostingType, ParcelCostingParameters> {
      [ParcelCostingType.Small] = new ParcelCostingParameters(3, 1, 2),
      [ParcelCostingType.Medium] = new ParcelCostingParameters(8, 3, 2),
      [ParcelCostingType.Large] = new ParcelCostingParameters(15, 6, 2),
      [ParcelCostingType.ExtraLarge] = new ParcelCostingParameters(25, 10, 2),
      [ParcelCostingType.Heavy] = new ParcelCostingParameters(50, 50, 1),
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

      parcelCosting.ParcelCost = CalculateParcelTypeCost(parcel, ParcelCosts[parcelCosting.CostingType]);

      var heavyCost = CalculateParcelTypeCost(parcel, ParcelCosts[ParcelCostingType.Heavy]);

      if ( heavyCost < parcelCosting.ParcelCost ) {
        parcelCosting.CostingType = ParcelCostingType.Heavy;
        parcelCosting.ParcelCost = heavyCost;
      }

      return parcelCosting;
    }

    decimal CalculateParcelTypeCost(IParcel parcel, ParcelCostingParameters costParams)
    {
      var result = costParams.BaseCost;
      var overweight = parcel.Weight - costParams.MaxWeight;
      if ( overweight > 0 ) {
        result += Math.Ceiling(overweight) * costParams.CostPerExtraKg;
      }
      return result;
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
