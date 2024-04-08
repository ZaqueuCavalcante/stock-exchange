namespace Xtk.Back.MatchingEngine;

public class OrderBook
{
    public Book BuyBook { get; set; }
    public Book SellBook { get; set; }
    public PriceLevel BestBid { get; set; }
    public PriceLevel BestOffer { get; set; }
    public Dictionary<Guid, Order> OrderMap { get; set; }
}
