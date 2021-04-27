using HP.Data.Definitions.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataContext :DbContext
    {
        public DbSet<Suggestion> Suggestions { get; set; }
        public DataContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connString = @"Data Source=(localdb)\MSSQLLocalDB;
            //        Initial Catalog=master;Integrated Security=True;
            //        Encrypt=False;
            //        TrustServerCertificate=False;ApplicationIntent=ReadWrite;
            //        MultiSubnetFailover=False";
            //optionsBuilder.UseSqlServer(connString);
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; 
                                            Database=PersonalStockAlerter;");
        }
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
