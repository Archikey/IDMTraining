using Idm.ConcurrencyLab.Counter;


ICounter counter = new Counter();


counter.Increment(10, 100_000);