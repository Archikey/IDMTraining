

namespace Idm.ConcurrencyLab.Counter
{
    public class Counter : ICounter
    {

        private int _count =0;

        public void Increment(int countThread, int maxCount)
        {
            Thread[] threads = new Thread[countThread];

            for (int i =0; i< countThread; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < maxCount; j++)
                    {
                        _count++;
                    }
                });
               
            }

            for (int i =0; i< countThread; i++)
            {

                threads[i].Start();
               
            }

            for (int i =0; i< countThread; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine($"Count: {_count}");
        }
    }


}