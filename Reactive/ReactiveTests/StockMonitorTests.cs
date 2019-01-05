using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Traditional;

namespace Tests
{
    public class StockMonitorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAlerts()
        {
            var ticker = new StockTicker();
            var monitor = new StockMonitor(ticker, 0.3m); // Max ratio: 0.3
            var alerts = new List<StockAlert>();

            System.EventHandler<StockAlert> handlerAlert = (object sender, StockAlert alert) => { alerts.Add(alert); };
            monitor.StockAlert += handlerAlert;

            ticker.UpdateStock("MSFT", 100);
            ticker.UpdateStock("MSFT", 120); // No Alert: Change < Max Change
            ticker.UpdateStock("INTC", 150);
            ticker.UpdateStock("MSFT", 165); // Alert: Change = 0.375 > Max Change (0.3)

            monitor.StockAlert -= handlerAlert;
            
            Assert.AreEqual(1, alerts.Count);
            Assert.IsFalse(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 120));
            Assert.IsTrue(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 165));
        }
    }
}