using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Common;
using ReactiveStyle;

namespace Tests
{
    public class StockMonitorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAlertOnIncrease()
        {
            var ticker = new StockTicker();
            var monitor = new RxStockMonitor(ticker, 0.3m); // Max ratio: 0.3
            var alerts = new List<StockAlert>();

            System.EventHandler<StockAlert> handlerAlert = (object sender, StockAlert alert) => { alerts.Add(alert); };
            monitor.StockAlert += handlerAlert;

            ticker.UpdateStock("MSFT", 100);
            ticker.UpdateStock("MSFT", 120); // No Alert: Change < Max Change
            ticker.UpdateStock("MSFT", 165); // Alert: Change  > Max Change

            monitor.StockAlert -= handlerAlert;
            
            Assert.AreEqual(1, alerts.Count);
            Assert.IsFalse(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 120));
            Assert.IsTrue(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 165));
            alerts.Clear();
        }

        [Test]
        public void TestAlertOnDecrease()
        {
            var ticker = new StockTicker();
            var monitor = new RxStockMonitor(ticker, 0.3m); // Max ratio: 0.3
            var alerts = new List<StockAlert>();

            System.EventHandler<StockAlert> handlerAlert = (object sender, StockAlert alert) => { alerts.Add(alert); };
            monitor.StockAlert += handlerAlert;

            ticker.UpdateStock("MSFT", 195);
            ticker.UpdateStock("MSFT", 120); // Alert: Change > Max Change
            ticker.UpdateStock("MSFT", 100); // No Alert: Change < Max Change

            monitor.StockAlert -= handlerAlert;
            
            Assert.AreEqual(1, alerts.Count);
            Assert.IsTrue(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 120));
            Assert.IsFalse(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 100));
            
            alerts.Clear();
        }

        [Test]
        public void TestMultipleAlerts()
        {
            var ticker = new StockTicker();
            var monitor = new RxStockMonitor(ticker, 0.3m); // Max Change : 0.3
            var alerts = new List<StockAlert>();

            System.EventHandler<StockAlert> handlerAlert = (object sender, StockAlert alert) => { alerts.Add(alert); };
            monitor.StockAlert += handlerAlert;

            ticker.UpdateStock("MSFT", 100);
            ticker.UpdateStock("MSFT", 220); // Alert: Change > Max Change
            ticker.UpdateStock("MSFT", 300); // No Alert: Change < Max Change

            monitor.StockAlert -= handlerAlert;
            
            Assert.AreEqual(2, alerts.Count);
            Assert.IsTrue(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 220));
            Assert.IsTrue(alerts.Any(x => x.Symbol == "MSFT" && x.NewPrice == 300));
            
            alerts.Clear();
        }

        [Test]
        public void TestNoAlert()
        {
            var ticker = new StockTicker();
            var monitor = new RxStockMonitor(ticker, 0.3m); // Max Change : 0.3
            var alerts = new List<StockAlert>();

            System.EventHandler<StockAlert> handlerAlert = (object sender, StockAlert alert) => { alerts.Add(alert); };
            monitor.StockAlert += handlerAlert;

            ticker.UpdateStock("MSFT", 100);
            ticker.UpdateStock("MSFT", 120); // No Alert: Change < Max Change
            ticker.UpdateStock("MSFT", 150); // No Alert: Change < Max Change
            ticker.UpdateStock("AAPL", 250); // No Alert: Different symbol

            monitor.StockAlert -= handlerAlert;
            
            Assert.AreEqual(0, alerts.Count);
        }
    }
}