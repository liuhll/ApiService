using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.Entities;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth
{
    public class UserAuthServiceManager : IUserAuthServiceManager
    {
        private readonly IRepository<UserServiceSubscriptionInfo, string> _userServiceSubscriptionRepository;
        private readonly IRepository<UserInfo> _userRepository;
        private readonly IRepository<ServicePrice> _servicePriceRepository;
        private readonly IRepository<AgentInfo> _agentRepository;

        public UserAuthServiceManager(IRepository<UserServiceSubscriptionInfo,
            string> userServiceSubscriptionRepository,
            IRepository<UserInfo> userRepository,
            IRepository<ServicePrice> servicePriceRepository,
            IRepository<AgentInfo> agentRepository)
        {
            _userServiceSubscriptionRepository = userServiceSubscriptionRepository;
            _userRepository = userRepository;
            _servicePriceRepository = servicePriceRepository;
            _agentRepository = agentRepository;
        }

        public async Task<IList<UserServicePrice>> UserServiceList(int uid, int sid, int? agentId)
        {
            var userInfo = await
                _userRepository.FirstOrDefaultAsync(
                    p => p.Id == uid);

            if (userInfo == null)
            {
                throw new Exception(string.Format("不存在用户Id为{0}的用户,请核对您的用户Id是否正确", uid));
            }
            var currentUserServiceAuth = await
                _userServiceSubscriptionRepository.FirstOrDefaultAsync(
                    p => p.UId == userInfo.Id && p.SId == sid && p.State == OrderState.Effective);

            IList<UserServicePrice> userServicePrices = null;

            if (currentUserServiceAuth == null || !currentUserServiceAuth.IsValidityPeriod)
            {
                var priceList = await _servicePriceRepository.GetAllListAsync(p => p.ServiceId == sid && p.State == State.Active);
                userServicePrices = priceList.Select(p => new UserServicePrice()
                {
                    Sid = p.ServiceId,
                    Pid = p.Id,
                    OriginalCost = p.Price,
                    AgentCost = agentId == null || agentId <=0 ? (decimal?)null : GetGeneralSalesPrice(p, agentId),
                    AuthDesc = p.AuthDesc,
                    AuthType = p.AuthType,
                    Description = p.Description,
                    PurchaseType = PurchaseType.NewPuchase,
                    Cost = GetGeneralSalesPrice(p, agentId)

                }).ToList();
            }
            else
            {
                var priceList =
                    await _servicePriceRepository.GetAllListAsync(
                            p =>
                                p.ServiceId == sid && p.State == State.Active &&
                                p.AuthType >= currentUserServiceAuth.AuthType);

                userServicePrices = priceList.Select(p => new UserServicePrice()
                {
                    Sid = p.ServiceId,
                    Pid = p.Id,
                    OriginalCost = p.Price,
                    AgentCost = agentId == null || agentId <= 0 ? (decimal?) null : GetAgentSalesPrice(p, currentUserServiceAuth, agentId.Value),
                    AuthDesc = p.AuthDesc,
                    AuthType = p.AuthType,
                    Description = p.Description,
                    PurchaseType = p.AuthType > currentUserServiceAuth.AuthType ? PurchaseType.Upgrade : PurchaseType.ReNew,
                    Cost = GetUpgradePrice(p, currentUserServiceAuth, agentId),
                    SubscriptionOrderId = p.AuthType > currentUserServiceAuth.AuthType ? currentUserServiceAuth.Id : "",
                }).ToList();

                userServicePrices = userServicePrices.Where(p => p.Cost >= 0).ToList();
            }

            return userServicePrices;
        }

        private decimal GetAgentSalesPrice(ServicePrice servicePrice,UserServiceSubscriptionInfo currentUserServiceAuth, int agentId)
        {
            var agentInfo = _agentRepository.FirstOrDefault(p => p.Id == agentId);
            if (agentInfo == null)
            {
                throw new Exception(string.Format("不存在Id为{0}的代理商，请检查您代理商的Id是否正确", agentId));
            }
            if (!agentInfo.AgentIsActive)
            {
                throw new Exception(String.Format("代理商{0}被冻结,销售价格获取失败，冻结原因:{1}", agentId, agentInfo.FrozenRemarks));
            }

            var isUpgrade = servicePrice.AuthType > currentUserServiceAuth.AuthType;
            if (!isUpgrade)
            {
                return servicePrice.AgentPrice * agentInfo.AgentRole.Discount;
            }
            var agentDiscount = (servicePrice.AgentPrice/servicePrice.Price) * agentInfo.AgentRole.Discount;
            var serviceSalesPrice = servicePrice.Price;

            decimal surplusValue;

            if (currentUserServiceAuth.AuthExpiration != null)
            {
                var useRate = Math.Round((decimal)(DateTime.Now - currentUserServiceAuth.CreateTime).Days /
                                     (currentUserServiceAuth.AuthExpiration - currentUserServiceAuth.CreateTime).Value.Days, 2);
                surplusValue = currentUserServiceAuth.OriginalCost*(1 - useRate);
            }
            else
            {
                surplusValue = currentUserServiceAuth.OriginalCost;
            }

            var currentServiceCost = serviceSalesPrice - surplusValue;

            var agentSalesCost = Math.Round(currentServiceCost, 2);
            var upgradePrice = agentSalesCost*agentDiscount;
            return upgradePrice;
        }

        //private decimal GetSalesPrice(ServicePrice servicePrice, int? agentId, UserServiceSubscriptionInfo currentUserServiceAuth)
        //{
        //    var isUpgrade = servicePrice.AuthType > currentUserServiceAuth.AuthType ;

        //    if (agentId == null || agentId.Value <= 0)
        //    {
        //        if (!isUpgrade)
        //        {
        //            return servicePrice.Price;
        //        }
        //        return GetUpgradePrice(servicePrice,currentUserServiceAuth);
        //    }
        //    var agentInfo = _agentRepository.FirstOrDefault(p => p.Id == agentId.Value);
        //    if (agentInfo == null)
        //    {
        //        throw new Exception(string.Format("不存在Id为{0}的代理商，请检查您代理商的Id是否正确", agentId));
        //    }
        //    if (!agentInfo.AgentIsActive)
        //    {
        //        throw new Exception(String.Format("代理商{0}被冻结,销售价格获取失败，冻结原因:{1}", agentId, agentInfo.FrozenRemarks));
        //    }
          
        //    if (!isUpgrade)
        //    {
        //        return servicePrice.AgentPrice * agentInfo.AgentRole.Discount;
        //    }
        //    return GetUpgradePrice(servicePrice, currentUserServiceAuth);

        //}

        private decimal GetUpgradePrice(ServicePrice servicePrice, UserServiceSubscriptionInfo currentUserServiceAuth,int? agentId)
        {
            if (servicePrice.AuthType > currentUserServiceAuth.AuthType)
            {
                var serviceSalesPrice = servicePrice.Price;

                decimal productSurplusValue; 

                if (currentUserServiceAuth.AuthExpiration != null)
                {
                    var useRate = Math.Round((decimal)(DateTime.Now - currentUserServiceAuth.CreateTime).Days /
                               (currentUserServiceAuth.AuthExpiration - currentUserServiceAuth.CreateTime).Value.Days, 2);

                    productSurplusValue = currentUserServiceAuth.OriginalCost*(1 - useRate);
                }
                else
                {
                    productSurplusValue = currentUserServiceAuth.OriginalCost;
                }

                if (productSurplusValue > serviceSalesPrice)
                {
                    return 0;
                }

                var currentServiceCost = serviceSalesPrice - productSurplusValue;
                return Math.Round(currentServiceCost, 2);
            }
            return GetGeneralSalesPrice(servicePrice,agentId);
        }


        private decimal GetGeneralSalesPrice(ServicePrice servicePrice, int? agentId)
        {
            if (agentId == null || agentId.Value <= 0)
            {
                return servicePrice.Price;
            }
            var agentInfo = _agentRepository.FirstOrDefault(p => p.Id == agentId.Value);
            if (agentInfo == null)
            {
                throw new Exception(string.Format("不存在Id为{0}的代理商，请检查您代理商的Id是否正确", agentId));
            }
            if (!agentInfo.AgentIsActive)
            {
                throw new Exception(string.Format("代理商{0}被冻结,销售价格获取失败，冻结原因:{1}", agentId, agentInfo.FrozenRemarks));
            }

            return servicePrice.AgentPrice * agentInfo.AgentRole.Discount;
        }
    }
}
