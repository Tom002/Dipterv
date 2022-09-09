using Dipterv.Shared.Dto;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IProductDetailsService
    {
        [ComputeMethod]
        public Task<ProductDetailsDto?> TryGetWithDetails(Session session, int productId, CancellationToken cancellationToken);
    }
}
