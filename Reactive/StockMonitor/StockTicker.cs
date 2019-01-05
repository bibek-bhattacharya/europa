using System;

namespace Traditional
{
    public class StockTicker
    {
        public event EventHandler<StockTick> StockTick;

        public void UpdateStock(string stockSymbol, decimal price)
        {
            OnStockTick(new StockTick(stockSymbol, price));
        }

        protected virtual void OnStockTick(StockTick eventArgs)
        {
            StockTick?.Invoke(this, eventArgs);
        }
    }

    public class StockTick
    {
        public string StockSymbol { get; set; }
        public decimal Price { get; set; }

        public StockTick(string symbol, decimal price)    
        {        
            StockSymbol = symbol;        
            Price = price;    
        }    
    }
}
