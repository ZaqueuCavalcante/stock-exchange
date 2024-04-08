namespace Xtk.Back.MatchingEngine;

public class PriceLevel
{
    public long LimitPrice { get; set; }
    public long TotalVolume { get; set; }
    public LinkedList<Order> Orders { get; set; } = [];

    /// <summary>
    /// Placing a new order means adding a new Order to the tail of the PriceLevel.
    /// This is O(1) time complexity for a doubly-linked list.
    /// </summary>
    public void PlacingNewOrder(Order order)
    {
        Orders.AddLast(order);
    }

    /// <summary>
    /// Matching an order means deleting an Order from the head of the PriceLevel.
    /// This is O(1) time complexity for a doubly-linked list.
    /// </summary>
    public void MatchingOrder()
    {
        Orders.RemoveFirst();
    }
}
