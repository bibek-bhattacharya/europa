using System;
using System.Collections.Generic;
using Common;

namespace EventsStyle
{
    public class StockMonitor
    {
        class StockInfo
        {    
            public StockInfo(string symbol, decimal price)    
            {        
                Symbol = symbol;        
                PrevPrice = price;    
            }
            public string Symbol { get; set; }    
            public decimal PrevPrice { get; set; }
        }

        public event EventHandler<StockAlert> StockAlert;
        private readonly decimal MaxChangeRatio_;
        private Dictionary<string, StockInfo> stockInfos = new  Dictionary<string, StockInfo>();
        private readonly StockTicker ticker_;

        public StockMonitor(StockTicker ticker, decimal maxChangeRatio)
        {
            MaxChangeRatio_ = maxChangeRatio;
            ticker_ = ticker;
            ticker_.StockTick += OnStockTick;
        }

        public void Dispose()
        {
            ticker_.StockTick -= OnStockTick;
            stockInfos.Clear();
        }
        private void OnStockTick(object sender, StockTick tick)
        {
            
            StockInfo stockInfo;
            string symbol = tick.StockSymbol;
            if (stockInfos.TryGetValue(symbol, out stockInfo))
            {
                var priceDiff = tick.Price - stockInfo.PrevPrice;
                var changeRatio = priceDiff / stockInfo.PrevPrice;
                if (Math.Abs(changeRatio) > MaxChangeRatio_)
                {
                    OnStockAlert(new StockAlert(symbol, changeRatio, tick.Price));
                    System.Console.WriteLine($"Stock {symbol} has changed by ratio {changeRatio:F2}"); // Format to 2 places of decimal.
                }
                stockInfos[symbol].PrevPrice = tick.Price;
            }
            else
            {
                stockInfos[symbol] = new StockInfo(symbol, tick.Price);
            }
        }

        protected virtual void OnStockAlert(StockAlert eventArgs)
        {
            StockAlert?.Invoke(this, eventArgs);
        }
    }

    public class StockAlert
    {
        public string Symbol { get; set; }
        public decimal ChangeRatio { get; set; }
        public decimal NewPrice { get; set; }

        public StockAlert(string symbol, decimal changeRatio, decimal newPrice)    
        {        
            Symbol = symbol;
            ChangeRatio = changeRatio;       
            NewPrice = newPrice;   
        }    
    }  
}
