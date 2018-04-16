//  
// Copyright okmer.com. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  
//  

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Okmer.Extensions.ObservableCollectionOfTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

namespace Com.Okmer.Extensions.ObservableCollectionOfTask.Tests
{
    /// <summary>
    /// The tests will compare the normal WhenAll/WhenAny with the mutable WhenAll/WhenAny.
    /// </summary>
    [TestClass()]
    public class ObservableCollectionOfTaskExtensionTests
    {
        /// <summary>
        /// Wait until 3 tasks are complete, the slowest thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAllVsWhenAllCompletedAddTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(100));
            tasks.Add(Task.Delay(200));

            Task a1 = tasks.MutableWhenAll();

            tasks.Add(Task.Delay(300));

            Task a2 = Task.WhenAll(tasks);

            a1.Wait();

            Assert.AreEqual(a1.IsCompleted, true);
            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until 2 tasks are complete, the slowest thirth task is removed after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAllVsWhenAllCompletedRemoveTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(100));
            tasks.Add(Task.Delay(200));
            tasks.Add(Task.Delay(300));

            Task a1 = tasks.MutableWhenAll();

            tasks.Remove(tasks.Last());

            Task a2 = Task.WhenAll(tasks);

            a1.Wait();

            Assert.AreEqual(a1.IsCompleted, true);
            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 3 tasks is canceled, the soon to be canceled thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAllVsWhenAllCanceledTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(100));
            tasks.Add(Task.Delay(200));

            Task a1 = tasks.MutableWhenAll();

            CancellationTokenSource cts = new CancellationTokenSource();

            tasks.Add(Task.Delay(300, cts.Token));

            Task a2 = Task.WhenAll(tasks);

            cts.Cancel();
            cts.Dispose();

            Task.Delay(400).Wait();

            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 3 tasks throws exception, the slowest thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAllVsWhenAllFaultedTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(100));
            tasks.Add(Task.Run(async () => { await Task.Delay(200); throw new Exception(); }));

            Task a1 = tasks.MutableWhenAll();

            tasks.Add(Task.Delay(300));

            Task a2 = Task.WhenAll(tasks);

            Task.Delay(400).Wait();

            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 3 tasks is complete, the fastest thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAnyVsWhenAnyCompletedAddTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(300));
            tasks.Add(Task.Delay(200));

            Task a1 = tasks.MutableWhenAny();

            tasks.Add(Task.Delay(100));

            Task a2 = Task.WhenAny(tasks);

            a2.Wait();

            Assert.AreEqual(a1.IsCompleted, true);
            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 2 tasks is complete, the fastest thirth task is removed after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAnyVsWhenAnyCompletedRemoveTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(300));
            tasks.Add(Task.Delay(200));
            tasks.Add(Task.Delay(100));

            Task a1 = tasks.MutableWhenAny();

            tasks.Remove(tasks.Last());

            Task a2 = Task.WhenAny(tasks);

            a2.Wait();

            Assert.AreEqual(a1.IsCompleted, true);
            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 3 tasks is canceled, the soon to be canceled thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAnyVsWhenAnyCanceledTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(300));
            tasks.Add(Task.Delay(200));

            Task a1 = tasks.MutableWhenAny();

            CancellationTokenSource cts = new CancellationTokenSource();

            tasks.Add(Task.Delay(100, cts.Token));

            Task a2 = Task.WhenAny(tasks);

            cts.Cancel();
            cts.Dispose();

            Task.Delay(50).Wait();

            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }

        /// <summary>
        /// Wait until one of the 3 tasks throws exception, the fastest thirth task is added after calling MutableWhenAll.
        /// </summary>
        [TestMethod()]
        public void MutableWhenAnyVsWhenAnyFaultedTest()
        {
            ObservableCollection<Task> tasks = new ObservableCollection<Task>();

            tasks.Add(Task.Delay(300));
            tasks.Add(Task.Delay(200));

            Task a1 = tasks.MutableWhenAny();

            tasks.Add(Task.Run(async () => { await Task.Delay(100); throw new Exception(); }));

            Task a2 = Task.WhenAny(tasks);

            Task.Delay(150).Wait();

            Assert.AreEqual(a1.IsCompleted, a2.IsCompleted);
            Assert.AreEqual(a1.IsCanceled, a2.IsCanceled);
            Assert.AreEqual(a1.IsFaulted, a2.IsFaulted);
            Assert.AreEqual(a1.Status, a2.Status);
        }
    }
}