using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;

namespace Dipterv.Bll.Services
{
    public class SeedService
    {
        private readonly FusionDbContext _dbContext;

        public SeedService(FusionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task SeedProducts()
        //{
        //    var product1 = new Product
        //    {

        //        Name = 
        //    }


        //}
    }
}
