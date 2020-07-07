using DataContextLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextLayer.DataContext
{
    public class EFDataContext : DbContext
    { 
        public DbSet<AdminAccount> adminAccount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-N9I077O\SQLEXPRESS;Initial Catalog=DataManagement; Integrated Security=True");
        }
    }
}
