﻿using Microsoft.EntityFrameworkCore;
using Zoo.Models.DbModels;

namespace Zoo
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options) 
        { 
        }

        public DbSet<AnimalDbModel> Animal { get; set; }
        public DbSet<SpeciesDbModel> Species { get; set; }
        public DbSet<EnclosureDbModel> Enclosure { get; set; }
        public DbSet<ZookeeperDbModel> Zookeeper { get; set; }
        public DbSet<TransferDbModel> Transfer { get; set; }
        public DbSet<LocationDbModel> Location { get; set; }
    }
}