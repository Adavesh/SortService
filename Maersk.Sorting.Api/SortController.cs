using Microsoft.AspNetCore.Http;
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
        private readonly ISortJobManager _SortJobManager;

        public SortController(ISortJobProcessor sortJobProcessor, ISortJobManager SortJobManager)
        {
            _sortJobProcessor = sortJobProcessor;
            _SortJobManager = SortJobManager;
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
            var sortJob = await _SortJobManager.CreateJob(values);
            return StatusCode(StatusCodes.Status202Accepted, sortJob);
        }

        [HttpGet]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            var jobs = await _SortJobManager.GetJobs();
            return Ok(jobs);
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId, [FromQuery]bool deleteIfCompleted = false)
        {
            var job = await _SortJobManager.GetJob(jobId, deleteIfCompleted);
            return Ok(job);
        }
    }
}
