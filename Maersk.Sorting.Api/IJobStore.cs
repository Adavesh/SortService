using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface IJobStore
    {
        event Func<SortJob, Task>? OnJobCreated;

        Task<bool> AddJob(SortJob sortJob);
        Task<SortJob[]> GetAllJobs();
        Task<SortJob?> GetJob(Guid jobId);
        Task<bool> UpdateJob(SortJob sortJob);
        Task<bool> DeleteJob(Guid sortJobId);
    }
}
