

namespace Idm.ConcurrencyLab.Counter
{
    public interface ICounter
    {
        public void Increment(int countThread, int maxCount);


    }
}