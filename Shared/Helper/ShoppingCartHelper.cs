using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Helper
{
    public static class ShoppingCartHelper
    {
        public static string GetCustomerShoppingCartKey(int customerId) => $"customer-{customerId}";

        public static string GetGuestShoppingCartKey(string sessionId) => $"guest-{sessionId}";
    }
}
