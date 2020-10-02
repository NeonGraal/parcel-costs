using System.Collections.Generic;

namespace SLJ.ParcelCosts
{
  public interface IOrder
  {
    IEnumerable<IParcel> Parcels { get; }
    bool SpeedyShipping { get; }
  }
}
