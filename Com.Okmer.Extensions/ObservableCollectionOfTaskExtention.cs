//  
// Copyright okmer.com. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  
//  

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Okmer.Extensions.ObservableCollectionOfTask
{
    public static class ObservableCollectionOfTaskExtention
    {
        private const int INFINITE = -1;

        public static async Task MutableWhenAll(this ObservableCollection<Task> collection)
        {
            await MutableWhenSomething(collection, Task.WhenAll);
        }

        public static async Task MutableWhenAny(this ObservableCollection<Task> collection)
        {
            await MutableWhenSomething(collection, Task.WhenAny);
        }

        private static async Task MutableWhenSomething(this ObservableCollection<Task> collection, Func<IEnumerable<Task>, Task> whenSomething)
        {
            Task waitAllTask = null;
            Task helperTask = null;

            bool isCollectionChanged = false;

            do
            {
                //Cancellation on collection changed event
                CancellationTokenSource cts = new CancellationTokenSource();
                NotifyCollectionChangedEventHandler cancelActionHandler = (sender, arg) => cts.Cancel(false);
                collection.CollectionChanged += cancelActionHandler;

                //Current collection
                waitAllTask = whenSomething(collection);

                //Wait on current collection or collection changed event
                try
                {
                    helperTask = Task.Delay(INFINITE, cts.Token);
                    await Task.WhenAny(waitAllTask, helperTask);
                }
                finally
                {
                    isCollectionChanged = cts.IsCancellationRequested;
                    cts.Cancel(false);
                    cts.Dispose();
                    collection.CollectionChanged -= cancelActionHandler;
                }
            }
            while (isCollectionChanged);

            //Return the WaitAll on collection results
            await waitAllTask;
        }
    }
}
