using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.ShoppingCart
{
    public class ShoppingCartDto
    {
        public bool CanSendOrder { get; set; }

        public List<ShoppingCartItemDetailsDto> ShoppingCartItems { get; set; } = new List<ShoppingCartItemDetailsDto>();
    }
}
