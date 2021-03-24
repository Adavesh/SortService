using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobManager
    {
        Task<SortJob> CreateJob(IReadOnlyCollection<int> input);
        Task<SortJob?> GetJob(Guid jobId, bool deleteIfCompleted);
        Task<SortJob[]> GetJobs();
    }
}
