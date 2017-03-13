using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Logging;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Exceptions;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.UserAuth.Entities;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth.Sales
{
    public class SalesSoftwareProcessor : ISalesSoftwareProcessor
    {
      //  private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserServiceAuthInfo> _userServiceAuthRepository;
        private readonly IRepository<UserServiceSubscriptionInfo, string> _userServiceSubscriptionRepository;
        private readonly IRepository<ServicePrice> _servicePriceRepository;
        private readonly IRepository<AgentInfo> _agentRepository;
        private readonly IRepository<UserInfo> _userRepository;
        private readonly IRepository<UserRecharge,string> _userRechargeRepository;

        public SalesSoftwareProcessor(IRepository<UserServiceAuthInfo> userServiceAuthRepository,
            IRepository<ServicePrice> servicePriceRepository,
            IRepository<UserInfo> userRepository, 
            IRepository<AgentInfo> agentRepository, 
            IRepository<UserServiceSubscriptionInfo, string> userServiceSubscriptionRepository,
            IRepository<UserRecharge, string> userRechargeRepository)
        {
            _userServiceAuthRepository = userServiceAuthRepository;
            _servicePriceRepository = servicePriceRepository;
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _userServiceSubscriptionRepository = userServiceSubscriptionRepository;
            _userRechargeRepository = userRechargeRepository;
        }

        public async Task<UserCanPurchaseCode> UserCanbyPruductService(UserInfo user, int servicePriceId)
        {
            var userServiceAuthInfos = await _userServiceAuthRepository.GetAllListAsync(p => p.UId == user.Id
                                    && (p.AuthExpiration == null || p.AuthExpiration.Value >= DateTime.Now));

            var servicePriceInfo = await _servicePriceRepository.GetAsync(servicePriceId);

            if (userServiceAuthInfos == null || userServiceAuthInfos.Count <= 0)
            {
                return UserCanPurchaseCode.CanPurchaseService;
            }

            //已经购买了更高级的版本
            if (userServiceAuthInfos.Any(p => p.AuthType > servicePriceInfo.AuthType && p.SId == servicePriceInfo.ServiceId))
            {
                return UserCanPurchaseCode.PurchaseHigherVersion;
            }

            //已经购买了当前版本的终生版
            if (userServiceAuthInfos.Any(p => p.AuthType == servicePriceInfo.AuthType && p.AuthExpiration == null))
            {
                return UserCanPurchaseCode.PurchaseCurrentServiceLifelong;
            }

            return UserCanPurchaseCode.CanPurchaseService;
        }

        [UnitOfWork]
        public async Task PurchaseSoftService(UserInfo userInfo, SalesInfoModel model, bool isAgent)
        {
            UserInfo salesMan = null;
            var servicePrice = await _servicePriceRepository.FirstOrDefaultAsync(p => p.Id == model.PId);
           // decimal agentCost = 0;
            if (isAgent)
            {
                Debug.Assert(model.AgentId != null, "model.AgentId != null");
                salesMan = await _userRepository.GetAsync(model.AgentId.Value);
                var preSalesFund = salesMan.Fund;
                salesMan.Fund -= model.Cost;
                var agentInfo = await _agentRepository.GetAsync(model.AgentId.Value);
                if (!agentInfo.AgentIsActive)
                {
                    throw new AgentNoActiveException("您的代理商权限被冻结，无法进行销售活动，请与客服联系！");
                }
                if (salesMan.Fund < 0)
                {
                    throw new InsufficientFundsException(string.Format("您当前的余额为{0},余额已不足，无法完成销售，请充值后再销售!", preSalesFund.ToString("C")));
                }
              
            }

            //获取之前购买的记录，如果为空，则直接购买
            var userServiceAuth = await _userServiceAuthRepository.FirstOrDefaultAsync(
                        p => p.UId == model.UId && p.SId == servicePrice.ServiceId);

            //历史购买的已经生效的订单
            var historyEffectiveOrder =
                await
                    _userServiceSubscriptionRepository.GetAllListAsync(p => p.UId == userInfo.Id && p.SId == servicePrice.ServiceId && p.State == OrderState.Effective);

            bool isNewPurchase = false;

            if (userServiceAuth == null)
            {
                isNewPurchase = true;
                userServiceAuth = new UserServiceAuthInfo()
                {
                    UId = userInfo.Id,
                    SId = servicePrice.ServiceId,
                    AuthType = servicePrice.AuthType,
                };
                userServiceAuth.AuthExpiration = GetAuthExpiration(servicePrice, userServiceAuth, true);
            }
            else
            {
                userServiceAuth.AuthExpiration = GetAuthExpiration(servicePrice, userServiceAuth);
                userServiceAuth.AuthType = servicePrice.AuthType;
                userServiceAuth.UpdateTime = DateTime.Now;
            }

            var userServiceSubscriptionInfo = new UserServiceSubscriptionInfo()
            {
               Id = ToolHelper.GetPrimaryId(), //GetServiceSubscriptionId(),
                UId = userInfo.Id,
                SId = servicePrice.ServiceId,
                //SpId = model.ServicePriceId,
                AuthDesc = servicePrice.AuthDesc,
                Cost = model.Cost,
                Profit = model.Cost,
                AuthExpiration = userServiceAuth.AuthExpiration,
                AuthType = userServiceAuth.AuthType,
                Remarks = model.Remarks,
                // AdminId = salesManId,
                OriginalCost = GetOriginalCost(historyEffectiveOrder,servicePrice,model), //:todo 计算OriginCost
                State = OrderState.Effective,

            };

            if (isAgent)
            {
                userServiceSubscriptionInfo.AgentId = model.AgentId;
                userServiceSubscriptionInfo.Profit = await GetProfitBySalesManId(model.AgentId.Value, servicePrice);
            }
            else
            {        
                userServiceSubscriptionInfo.AdminId = model.AdminId;
            }

            try
            {
                if (isNewPurchase)
                {
                    await _userServiceAuthRepository.InsertAsync(userServiceAuth);
                }
                else
                {
                    await _userServiceAuthRepository.UpdateAsync(userServiceAuth);
                }
                if (historyEffectiveOrder != null && historyEffectiveOrder.Count > 0)
                {
                    foreach (var historyOrder in historyEffectiveOrder)
                    {
                        historyOrder.State = OrderState.Legal;
                        historyOrder.UpdateTime = DateTime.Now;
                        await _userServiceSubscriptionRepository.UpdateAsync(historyOrder);
                    }
                }
                if (isAgent)
                {
                    await _userRepository.UpdateAsync(salesMan);
                }
                await _userServiceSubscriptionRepository.InsertAsync(userServiceSubscriptionInfo);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error(string.Format("销售失败，服务器内部错误,单号为{0}", userServiceSubscriptionInfo.Id), ex);
                throw ex;
            }

        }

        private decimal GetOriginalCost(List<UserServiceSubscriptionInfo> historyEffectiveOrder, ServicePrice servicePrice,SalesInfoModel model)
        {

            var effectiveOrder = historyEffectiveOrder.OrderByDescending(p => p.CreateTime).FirstOrDefault();

            if (effectiveOrder == null)
            {
                return servicePrice.Price;
            }

            bool isUpgrade = servicePrice.AuthType > effectiveOrder.AuthType;
            if (!model.SuggestCost.HasValue)
            {
                throw new Exception("用户的指导价不能为空");
            }
            if (isUpgrade)
            {
                //Debug.Assert(model.SuggestCost != null, "model.SuggestCost != null");
                
                return effectiveOrder.OriginalCost - model.SuggestCost.Value + servicePrice.Price;
            }
            return effectiveOrder.OriginalCost + model.SuggestCost.Value;
        }

        public bool TestProductValues(int uid,int pid,string subscriptionOrderId, decimal productValue, decimal salesCost, out string msg)
        {
            var subscriptionOrder = _userServiceSubscriptionRepository.FirstOrDefault(p => p.Id == subscriptionOrderId);
            if (subscriptionOrder == null)
            {
                msg = string.Format("用户{0}不存在历史订单号为{1}的订单,用户升级失败",uid, subscriptionOrderId);
                return false;
            }
            if (subscriptionOrder.State != OrderState.Effective)
            {
                RechargeUserCost(uid,salesCost);
                msg = string.Format("用户订单{0}已经被撤销，无法根据历史订单升级用户权限，交易金额已被充值到用户账号",subscriptionOrderId);
                return false;
            }
            var servicePrice = _servicePriceRepository.Get(pid);
            if (productValue != ComputeProductValue(subscriptionOrder,servicePrice))
            {
                RechargeUserCost(uid, salesCost);
                msg = "软件剩余价值不相等,请重新购买,交易金额已被充值到用户账号";
                return false;
            }

            msg = "OK";
            return true;
        }

        [UnitOfWork]
        private void RechargeUserCost(int uid, decimal salesCost)
        {
            var userInfo = _userRepository.Get(uid);
            var userRechage = new UserRecharge()
            {
                Id = ToolHelper.GetPrimaryId(),
                Cost = salesCost,
                UId = uid,
                Remarks = "用户升级失败，充值到用户本人账号金额"
            };
            userInfo.Fund += salesCost;
            _userRechargeRepository.Insert(userRechage);   
            _userRepository.Update(userInfo);
        }

        private decimal ComputeProductValue(UserServiceSubscriptionInfo subscriptionOrder,ServicePrice servicePrice)
        {
            var serviceSalesPrice = servicePrice.Price;

            decimal productSurplusValue;

            if (subscriptionOrder.AuthExpiration != null)
            {
                var useRate = Math.Round((decimal)(DateTime.Now - subscriptionOrder.CreateTime).Days /
                           (subscriptionOrder.AuthExpiration - subscriptionOrder.CreateTime).Value.Days, 2);

                productSurplusValue = subscriptionOrder.OriginalCost * (1 - useRate);
            }
            else
            {
                productSurplusValue = subscriptionOrder.OriginalCost;
            }

            var currentServiceCost = serviceSalesPrice - productSurplusValue;
            return Math.Round(currentServiceCost, 2);
        }

        private async Task<decimal> GetProfitBySalesManId(int value, ServicePrice servicePrice)
        {
            return 0;
        }

        private DateTime? GetAuthExpiration(ServicePrice servicePrice, UserServiceAuthInfo currentUserServiceAuth, bool isNewPurchase = false)
        {
            if (servicePrice.IsLifeLongVersion)
            {
                return null;
            }

            Debug.Assert(servicePrice.DateYear != null, "servicePrice.DateYear != null");
            if (isNewPurchase)
            {
                return DateTime.Now.AddYears(servicePrice.DateYear.Value)
                      .AddMonths(servicePrice.DateMonth)
                      .AddDays(servicePrice.DateDay);
            }

            if (servicePrice.AuthType > currentUserServiceAuth.AuthType)
            {
                return DateTime.Now.AddYears(servicePrice.DateYear.Value)
                     .AddMonths(servicePrice.DateMonth)
                     .AddDays(servicePrice.DateDay);
            }

            Debug.Assert(currentUserServiceAuth.AuthExpiration != null, "currentUserServiceAuth.AuthExpiration != null");
            if (currentUserServiceAuth.IsActive)
            {
                return currentUserServiceAuth.AuthExpiration.Value.AddYears(servicePrice.DateYear.Value)
                       .AddMonths(servicePrice.DateMonth)
                       .AddDays(servicePrice.DateDay);
            }
            return DateTime.Now.AddYears(servicePrice.DateYear.Value)
                .AddMonths(servicePrice.DateMonth)
                .AddDays(servicePrice.DateDay);
        }
    }
}
