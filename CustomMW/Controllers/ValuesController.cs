using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomMW.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private int a = 1;
        // GET api/values
        [Route("add")]
        [HttpGet("{id}")]
        public IEnumerable<string> AddJob(string id)
        {
            var jobId = BackgroundJob.Schedule(() => Console.WriteLine("background job"), TimeSpan.FromSeconds(10));
            this.a++;

            RecurringJob.AddOrUpdate(id, () => Print(id), Cron.Minutely, TimeZoneInfo.Local, "default");

            return new string[] { "value1", "value2" };
        }

        [Route("delete")]
        [HttpGet("{id}")]
        public IEnumerable<string> DeleteJob(string id)
        {
            RecurringJob.RemoveIfExists(id);

            return new string[] { "value1", "value2" };
        }


        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public void Print(string id)
        {
            Console.WriteLine($"Came from a method. My Id = {id} and this.a = {this.a}");
        }
    }
}
