using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apimodels.models;

namespace apimodels.Context
{
    public class MaqueteContext : DbContext
    {
        public MaqueteContext(DbContextOptions<MaqueteContext> options) : base(options)
        {

        }
        
        public DbSet<Maquete> Maquetes { get; set; }

    }
}