//  
// Copyright okmer.com. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  
//  

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Com.Okmer.Extensions.ObservableCollectionOfTask;

namespace MutableWhenAllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            Task t1 = Task.Run(async () => { await Task.Delay(1000); Console.WriteLine("t1"); });
            Task t2 = Task.Run(async () => { await Task.Delay(2000); Console.WriteLine("t2"); });
            Task t3 = Task.Run(async () => { await Task.Delay(3000); Console.WriteLine("t3"); });

            tasks.Add(t1);
            tasks.Add(t2);

            Task a1 = tasks.MutableWhenAll();

            tasks.Add(t3);

            a1.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Console.WriteLine("Canceled");
                }

                if (t.IsFaulted)
                {
                    Console.WriteLine("Faulted");
                }

                if (t.IsCompleted)
                {
                    Console.WriteLine("Completed");
                }
            });

            a1.Wait();

            Console.ReadLine();
        }

    }
}
