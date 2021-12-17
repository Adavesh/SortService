using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class JobStore : IJobStore
    {
        public event Func<SortJob, Task>? OnJobCreated;
        private readonly ConcurrentDictionary<Guid, SortJob> _jobs;

        public JobStore()
        {
            _jobs = new ConcurrentDictionary<Guid, SortJob>();
        }

        public async Task<bool> AddJob(SortJob sortJob)
        {
            if(_jobs.TryAdd(sortJob.Id, sortJob))
            {
                await Task.CompletedTask;
                OnJobCreated?.Invoke(sortJob);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateJob(SortJob sortJob)
        {
            if (_jobs.TryGetValue(sortJob.Id, out SortJob? existingJob))
            {
                await Task.CompletedTask;
                var result = _jobs.TryUpdate(sortJob.Id, sortJob, existingJob);
                return result;
            }

            return false;
        }

        public async Task<SortJob?> GetJob(Guid jobId)
        {
            _jobs.TryGetValue(jobId, out SortJob? result);
            return await Task.FromResult(result);
        }

        public async Task<SortJob[]> GetAllJobs()
        {
            var allJobs = _jobs.Values.ToArray();
            return await Task.FromResult(allJobs);
        }

        public async Task<bool> DeleteJob(Guid sortJobId)
        {
            var result = _jobs.TryRemove(sortJobId, out SortJob _);
            return await Task.FromResult(result);
        }
    }
}
