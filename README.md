# Stock Exchange

System Design - Stock Exchange

The basic function of an exchange is to facilitate the matching of buyers and sellers efficiently.

Examples:
- The New York Stock Exchange (NYSE)
- Hong Kong Exchanges and Clearing Limited (HKEX)
- National Association of Securities Dealers Automated Quotations (Nasdaq)

Scale:
- NYSE trading billions of matches per day
- HKEX trading 200 billion shares per day

## Step 1 - Understand the Problem and Establish Design scope

### Functional Requirements

- Only stocks trade (not options or futures)
- Only limit order (not market or conditional) placing and canceling (not replacing)
- Just need to support the normal trading hours

- A client can place new limit orders or cancel them, and receive matched trades in real-time
- A client can view the real-time order book (the list of buy and sell orders)
- The exchange needs to support at least tens of thousands of users trading at the same time
- The exchange needs to support at least 100 symbols
- For the trading volume, we should support billions of orders per day
- Also, the exchange is a regulated facility, so we need to make sure it runs risk checks
- For example, a user can only trade a maximum of 1 million shares of Apple stock in one day.

- User wallet management:
    - We need to make sure users have sufficient funds when they place orders
    - If an order is waiting in the order book to be filled, the funds required for the order need to be withheld to prevent overspending

### Non-Functional Requirements

- **Availability**:
    - At least 99.99%.
    - Availability is crucial for exchanges. Downtime, even seconds, can harm reputation.

- **Fault tolerance**:
    - Fault tolerance and a fast recovery mechanism are needed to limit the impact of a production incident.

- **Latency**:
    - The round-trip latency should be at the millisecond level, with a particular focus on the 99th percentile latency.
    - The round-trip latency is measured from the moment a market order enters the exchange to the point where the market order returns as a filled execution.
    - A persistently high 99th percentile latency causes a terrible user experience for a small number of users.

- **Security**:
    - The exchange should have an account management system.
    - For legal compliance, the exchange performs a KYC (Know Your Client) check to verify a user’s identity before a new account is opened.
    - For public resources, such as web pages containing market data, we should prevent distributed denial-of-service (DDoS) attacks.

### Back-of-the-envelope estimation

- 100 symbols
- 1 billion orders per day
- NYSE Stock Exchange is open Monday through Friday from 9:30 am to 4:00 pm Eastern Time. That’s 6.5 hours in total.
- QPS: 1 billion / 6.5 / 3600 = ~43,000 (queries/s)
- Peak QPS: 5 * QPS = 215,000.
- The trading volume is significantly higher when the market first opens in the morning and before it closes in the afternoon.

## Step 2 - Propose High-Level Design and Get Buy-In

### Business Knowledge 101

- **Broker**:
    - Provide a friendly user interface for retail users to place trades and view market data.
    - Charles Schwab, Robinhood, Etrade, Fidelity, etc.

- **Institutional client**:
    - Institutional clients trade in large volumes using specialized trading software.
    - Pension funds aim for a stable income.
        - They trade infrequently, but when they do trade, the volume is large.
        - They need features like order splitting to minimize the market impact of their sizable orders.
    - Some hedge funds specialize in market making and earn income via commission rebates.
        - They need low latency trading abilities, so obviously they cannot simply view market data on a web page or a mobile app, as retail clients do.

- **Limit order**:
    - Is a buy or sell order with a fixed price.
    - It might not find a match immediately, or it might just be partially matched.

- **Market order**:
    - A market order doesn’t specify a price.
    - It is executed at the prevailing market price immediately.
    - A market order sacrifices cost in order to guarantee execution.
    - It is useful in certain fast-moving market conditions.

- **Market data levels**:
    - The US stock market has three tiers of price quotes: L1 (level 1), L2, and L3.
    - L1 market data contains the best bid price, ask price, and quantities.
    - Bid price refers to the highest price a buyer is willing to pay for a stock.
    - Ask price refers to the lowest price a seller is willing to sell the stock.

- **Candlestick chart**:
    - Represents the stock price for a certain period of time.
    - A candlestick shows the market’s open, close, high, and low price for a time interval.
    - The common time intervals are one-minute, five-minute, one-hour, one-day, one-week, and one-month.

- **FIX**:
    - FIX protocol, which stands for Financial Information eXchange protocol, was created in 1991.
    - It is a vendor-neutral communications protocol for exchanging securities transaction information.
    - See below for an example of a securities transaction encoded in FIX.

### High-level design

- **Trading flow**:
    - The trading flow is on the critical path of the exchange
    - The heart of the trading flow is the matching engine:
        - Maintain the order book for each symbol
        - Match buy and sell orders.
        - A match results in two executions (fills), with one each for the buy and sell sides.
        - The matching function must be fast and accurate.
        - Distribute the execution stream as market data.
        - A highly available matching engine implementation must be able to produce matches in a deterministic order.
        - That is, given a known sequence of orders as an input, the matching engine must produce the same sequence of executions (fills) as an output when the sequence is replayed.
        - This determinism is a foundation of high availability which we will discuss at length in the deep dive section.
    - The sequencer is the key component that makes the matching engine deterministic:
        - It stamps every incoming order with a sequence ID before it is processed by the matching engine.








