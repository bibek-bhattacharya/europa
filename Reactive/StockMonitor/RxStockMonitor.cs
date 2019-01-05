using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Common;


namespace ReactiveStyle
{
    public class RxStockMonitor : IDisposable
    {
        private IDisposable subscription_;
        public event EventHandler<StockAlert> StockAlert;
        public RxStockMonitor(StockTicker ticker, decimal maxChangeRatio)
        {
            var ticks = 
            Observable.FromEventPattern<EventHandler<StockTick>, StockTick>(
                h =>  ticker.StockTick += h,
                h =>  ticker.StockTick -= h )
                .Select(tickEvent => tickEvent.EventArgs)
                .Synchronize();
            
            var alerts =
            from tick in ticks
            group tick by tick.StockSymbol
            into company
            from tickPair in company.Buffer(2,1)
            let changeRatio = (tickPair[1].Price - tickPair[0].Price)/tickPair[0].Price
            where Math.Abs(changeRatio) > maxChangeRatio
            select new
            {
                Symbol = company.Key,
                ChangeRatio = changeRatio,
                OldPrice = tickPair[0].Price,
                NewPrice = tickPair[1].Price
            };

            subscription_ = alerts.Subscribe( 
            change =>
                {
                    OnStockAlert(new StockAlert(change.Symbol, change.ChangeRatio, change.NewPrice));
                    System.Console.WriteLine($"Stock {change.Symbol} has changed to {change.NewPrice} by ratio {change.ChangeRatio:F2}"); // Format to 2 places of decimal.
                },
                ex => { /* Code to handle excetions */ },
                () => { /* Code to handle observable completeness */ }
            );

        }

        public void Dispose()
        {
            subscription_.Dispose();
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
