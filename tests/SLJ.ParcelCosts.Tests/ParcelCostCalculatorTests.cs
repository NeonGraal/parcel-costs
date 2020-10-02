using FluentAssertions;
using Moq;
using NUnit.Framework;
using SLJ.ParcelCosts.Implementation;
using System.Linq;

namespace SLJ.ParcelCosts.Tests
{
  public class ParcelCostCalculatorTests
  {
    private ParcelsCostCalculator _calculator;

    [SetUp]
    public void Setup()
      => _calculator = new ParcelsCostCalculator();

    [TestCase(5, 3, ParcelCostingType.Small)]
    [TestCase(25, 8, ParcelCostingType.Medium)]
    [TestCase(75, 15, ParcelCostingType.Large)]
    [TestCase(125, 25, ParcelCostingType.ExtraLarge)]
    public void CalculateCosts_ForOrderWithSizedParcel_ShouldBeAmount(decimal dimension, decimal cost, ParcelCostingType parcelType)
    {
      var parcel = new Mock<IParcel>();
      var order = new Mock<IOrder>();

      parcel.SetupGet(p => p.Height).Returns(dimension);
      parcel.SetupGet(p => p.Width).Returns(dimension);
      parcel.SetupGet(p => p.Depth).Returns(dimension);

      order.SetupGet(o => o.Parcels).Returns(new[] { parcel.Object });

      var result = _calculator.CalculateCosts(order.Object);

      result.TotalCost.Should().Be(cost);
      result.ParcelCosts.Should().ContainSingle();
      result.ParcelCosts.Single().ParcelCost.Should().Be(cost);
      result.ParcelCosts.Single().CostingType.Should().Be(parcelType);
    }
  }
}
