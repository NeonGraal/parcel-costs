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
    public void Setup() => _calculator = new ParcelsCostCalculator();

    [Test]
    public void CalculateCosts_ForOrderWithSmallParcel_ShouldBe3()
    {
      var parcel = new Mock<IParcel>();
      var order = new Mock<IOrder>();

      parcel.SetupGet(p => p.Height).Returns(5);
      parcel.SetupGet(p => p.Width).Returns(5);
      parcel.SetupGet(p => p.Depth).Returns(5);

      order.SetupGet(o => o.Parcels).Returns(new[] { parcel.Object });

      var result = _calculator.CalculateCosts(order.Object);

      result.TotalCost.Should().Be(3);
      result.ParcelCosts.Should().ContainSingle();
      result.ParcelCosts.Single().ParcelCost.Should().Be(3);
      result.ParcelCosts.Single().CostingType.Should().Be(ParcelCostingType.Small);
    }
  }
}
