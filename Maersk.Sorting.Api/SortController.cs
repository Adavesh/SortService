using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private readonly IJobStore _jobStore;

        public SortController(ISortJobProcessor sortJobProcessor, IJobStore jobStore)
        {
            _sortJobProcessor = sortJobProcessor;
            _jobStore = jobStore;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            var job = new SortJob(Guid.NewGuid(), SortJobStatus.Pending, null, values, null);

            await _jobStore.AddJob(job);

            return Accepted(job);
        }

        [HttpGet]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            return await _jobStore.GetAllJobs();
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob?>> GetJob(Guid jobId)
        {
            return await _jobStore.GetJob(jobId);
        }
    }
}
