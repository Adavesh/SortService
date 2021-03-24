using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public sealed class SortJobManager : ISortJobManager
    {
        private readonly Dictionary<Guid, SortJob> _sortJobsRepo;
        private readonly ISortJobProcessor _sortJobProcessor;

        public SortJobManager(ISortJobProcessor sortJobProcessor)
        {
            _sortJobsRepo = new Dictionary<Guid, SortJob>();
            _sortJobProcessor = sortJobProcessor;
        }

        public async Task<SortJob> CreateJob(IReadOnlyCollection<int> input)
        {
            var pendingJob = new SortJob(Guid.NewGuid(), SortJobStatus.Pending, null, input, null);
            _sortJobsRepo.Add(pendingJob.Id, pendingJob);
            
            ProcessJob(pendingJob);
            
            return await Task.FromResult(pendingJob);
        }

        public async Task<SortJob?> GetJob(Guid jobId, bool deleteIfCompleted)
        {
            _sortJobsRepo.TryGetValue(jobId, out SortJob? sortJob);
                        
            if (sortJob != null && sortJob.Status == SortJobStatus.Completed && deleteIfCompleted)
            {
                _sortJobsRepo.Remove(jobId);
            }

            return await Task.FromResult(sortJob);
        }

        public async Task<SortJob[]> GetJobs()
        {
            var jobs = _sortJobsRepo.Values.ToArray();
            return await Task.FromResult(jobs);
        }

        private async void ProcessJob(SortJob job)
        {
            var completedJob = await _sortJobProcessor.Process(job);
            _sortJobsRepo[job.Id] = completedJob;
        }
    }
}
