using System.Collections.Generic;
using Zoo.Models.Enums;

namespace Zoo.Models.DbModels
{
    public class EnclosureDbModel
    {
        public Enclosure Id { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        public List<AnimalDbModel> Animals { get; set; }
        public List<ZookeeperDbModel> Zookeepers { get; set; }
    } 
}
