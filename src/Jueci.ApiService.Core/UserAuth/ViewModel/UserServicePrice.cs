using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.ViewModel
{
    public class UserServicePrice
    {
        public int Sid { get; set; }

        public int Pid { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal? AgentPrice { get; set; }

        public string AuthDesc { get; set; }

        public int AuthType { get; set; }

        public string Description { get; set; }

        public decimal SalesPrice { get; set; }

        public PurchaseType PurchaseType { get; set; }

        public string SubscriptionOrderId { get; set; }

    }
}