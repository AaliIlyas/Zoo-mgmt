using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoo.Models.ApiModels;
using Zoo.Models.DbModels;
using Zoo.Models.Enums;

namespace Zoo.Services
{
    public interface IZookeeperService
    {
        ZookeeperResponseModel GetZookeeperById(int id);
        ZookeeperResponseModel AddZookeeperToDatabase(ZookeeperRequestModel zookeeper);
    }

    public class ZookeeperService : IZookeeperService
    {
        private readonly ZooDbContext _context;

        public ZookeeperService(ZooDbContext context)
        {
            _context = context;
        }

        public ZookeeperResponseModel GetZookeeperById(int id)
        {
            var zookeeperDbModel = _context.Zookeeper.FromSqlRaw($@"Exec GetZookeepers").Single(z => z.Id == id);

            //var zookeeperDbModel = zookeeperQuery.Single(z => z.Id == id);

            return new ZookeeperResponseModel(zookeeperDbModel, true);
        }

        public ZookeeperResponseModel AddZookeeperToDatabase(ZookeeperRequestModel zookeeper)
        {
            var enclosures = _context.Enclosure
                .Where(e => zookeeper.EnclosureIds.Contains(e.Id)).ToList();

            if (enclosures.Count != zookeeper.EnclosureIds.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var newZookeeper = new ZookeeperDbModel
            {
                Name = zookeeper.Name,
                Enclosures = enclosures
            };
            _context.Zookeeper.Add(newZookeeper);
            _context.SaveChanges();

            return new ZookeeperResponseModel(newZookeeper, true); //need true, but only for enclosures not animals
        }
    }
}
