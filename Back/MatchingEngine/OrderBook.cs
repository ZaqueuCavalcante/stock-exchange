namespace Xtk.Back.MatchingEngine;

/// <summary>
/// An order book is a list of buy and sell orders for a specific
/// security or financial instrument, organized by price level.
/// Operations:
/// - Getting volume at a price level or between price levels.
/// - Placing a new order, canceling an order, and matching an order.
/// - Replacing an order.
/// - Query best bid/ask.
/// - Iterate through price levels.
/// </summary>
public class OrderBook
{
    public Book BuyBook { get; set; }
    public Book SellBook { get; set; }
    public PriceLevel BestBid { get; set; }
    public PriceLevel BestOffer { get; set; }
    public Dictionary<Guid, Order> OrderMap { get; set; }
}
