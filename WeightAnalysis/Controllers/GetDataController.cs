using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlazorServerApp.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using WeightAnalysis.Models;

    [ApiController]
    [Route("api/[controller]")]
    public class GetDataController : Controller
    {
        private IDbContextFactory<WeightContext> ContextFactory { get; set; }

        public GetDataController(IDbContextFactory<WeightContext> contextFactory)
        {
            this.ContextFactory=contextFactory;
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var data = GetWeightData.getWeightData(ContextFactory);

            return Ok(data);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
