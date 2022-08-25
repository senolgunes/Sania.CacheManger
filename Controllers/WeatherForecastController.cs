
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sania.RedisAndMemory;

namespace TestApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CacheManager _cacheManager;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, CacheManager cacheManager)
        {
            _logger = logger;
            _cacheManager = cacheManager;


        }

        [HttpGet]
        public async Task Get()
        {
            //redis set
            await _cacheManager.SetAsync("testkey", "testvalue", TimeSpan.FromHours(3), CachePlatform.Distributed);
            //get value from redis 
            var val = _cacheManager.GetString("testkey", CachePlatform.Distributed);

            //set value ınmemory cache 
            await _cacheManager.SetAsync("testkey", "testvalue", TimeSpan.FromHours(3), CachePlatform.InMemory);
            //get value from ınmemory cache 
            var val2 = _cacheManager.GetString("testkey", CachePlatform.InMemory);

            //remove key from redis 
            await _cacheManager.RemoveAsync("testkey", CachePlatform.Distributed);
            //remove key from inmemory  
            await _cacheManager.RemoveAsync("testkey", CachePlatform.InMemory);

            var student = new Student()
            {
                Id = 1,
                Name = "senol",
                LastName = "gunes"
            };

            //set object 
            await _cacheManager.SetAsync("key", student, TimeSpan.FromDays(1), CachePlatform.Distributed);

            //get  or set value  async
            var resut=await _cacheManager.GetOrSetAsync<string>("key",
                TimeSpan.FromDays(1),
                async  ()=> await GetStudentNameAsync(),
                CachePlatform.Distributed
                );

            //get  or set value 
            var resut2 =  _cacheManager.GetOrSet<string>("key",
              TimeSpan.FromDays(1),
               () =>   GetStudentNameAsync().Result,
              CachePlatform.Distributed
              );


            var resut3 = _cacheManager.GetOrSet<Student>("key",
            TimeSpan.FromDays(1),
             () => GetStudent(),
            CachePlatform.Distributed
            );
        }

        private async Task<string> GetStudentNameAsync()
        {
            return await Task.FromResult("senol"); 
        }
        private Student GetStudent()
        {
            return new Student();
        }
        class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
        }
    }
}
