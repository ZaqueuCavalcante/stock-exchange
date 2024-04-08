using Xtk.Back.MatchingEngine;

namespace Xtk.Tests.Unit;

public class OrderBookUnitTests
{
    [Test]
    public void Should_shorten_url()
    {
        // Arrange
        var order = new Order {};
        var priceLevel = new PriceLevel {};

        // Act
        priceLevel.PlacingNewOrder(order);

        // Assert
        priceLevel.Orders.Should().HaveCount(1);
    }
}
