using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Traditional;

namespace ReactiveApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MonitorStock();
            
            //await GetPageAsync().ContinueWith(taskGetPage => System.Console.WriteLine(taskGetPage.Result));

            await MyAsyncMethod(Thread.CurrentThread.ManagedThreadId).ContinueWith(taskThreadCheck => System.Console.WriteLine($"Same thread: {taskThreadCheck.Result}")); ;
        }

        // static void Main(string[] args)
        // {
        //     Console.WriteLine("Hello World!");
        // }
        private static void MonitorStock()
        {
            var ticker = new StockTicker();
            var monitor = new StockMonitor(ticker, 0.3m);
            ticker.UpdateStock("MSFT", 100);
            ticker.UpdateStock("INTC", 150);
            ticker.UpdateStock("MSFT", 120);
            ticker.UpdateStock("MSFT", 165);
        }

        static async Task<string> GetPageAsync()
        {
            Console.WriteLine("Hello World!");
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://reactive.io");
            string page = await response.Content.ReadAsStringAsync();
            // We return a string but the compiler generates a returned task for us, when we add the async modifier to the method.
            // Because the method returns a task, the call can be awaited.
            return page;
        }

        static async Task<bool> MyAsyncMethod(int callingThreadId)
        {
            // To make your method asynchronous, you need to span the work to another task, and this can be done by using Task.Run. 
            return await Task.Run(() => Thread.CurrentThread.ManagedThreadId == callingThreadId);
        }
    }
}
