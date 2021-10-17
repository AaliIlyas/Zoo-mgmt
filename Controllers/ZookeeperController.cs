using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Zoo.Models.ApiModels;
using Zoo.Services;

namespace Zoo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZookeeperController : ControllerBase
    {
        private readonly IZookeeperService _zookeeper;


        public ZookeeperController(IZookeeperService zookeeper)
        {
            _zookeeper = zookeeper;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ZookeeperResponseModel> Get(int id)
        {
            try
            {
                return _zookeeper.GetZookeeperById(id);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Route("create")]
        public IActionResult Add([FromBody] ZookeeperRequestModel zookeeper)
        {
            try{
                var newZookeeper = _zookeeper.AddZookeeperToDatabase(zookeeper);
                return Created(Url.Action("Get", new { id = newZookeeper.Id }), newZookeeper);
            }
            catch (ArgumentOutOfRangeException) {
                return BadRequest($"Enclosure IDs are invalid: {string.Join(", ", zookeeper.EnclosureIds)}");
            }
        }
    }
}
