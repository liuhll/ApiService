using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Jueci.ApiService.Common;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.Dtos;
using Jueci.ApiService.UserAuth.Entities;
using Jueci.ApiService.UserAuth.Sales;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth
{
    public class UserAuthAppService : IUserAuthAppService
    {
        private readonly IUserAuthServiceManager _authServiceManager;
        private readonly IRepository<UserInfo> _userRepository;
        private readonly ISalesSoftwareProcessor _salesSoftwareProcessor;

        public UserAuthAppService(IUserAuthServiceManager authServiceManager,
            IRepository<UserInfo> userRepository,
            ISalesSoftwareProcessor salesSoftwareProcessor)
        {
            _authServiceManager = authServiceManager;
            _userRepository = userRepository;
            _salesSoftwareProcessor = salesSoftwareProcessor;
        }


        public async Task<ResultMessage<IList<UserServicePriceOutput>>> UserServiceList(int uid, int sid, string appId, string sign, long timestamp, int? agentId = null)
        {
            try
            {
                var userPriceList = await _authServiceManager.UserServiceList(uid, sid, agentId);
                return new ResultMessage<IList<UserServicePriceOutput>>(userPriceList.MapTo<List<UserServicePriceOutput>>(), "OK");
            }
            catch (Exception e)
            {
                return new ResultMessage<IList<UserServicePriceOutput>>(ResultCode.Fail, e.Message);
            }
        }

        public async Task<ResultMessage<SalesResultMessage>> UserAuthorize(UserAuthInput input)
        {
            var userInfo = await _userRepository.FirstOrDefaultAsync(input.UId);

            //不存在的用户
            if (userInfo == null)
            {
                return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.NullUser));
            }
            //被冻结的用户
            if (!userInfo.IsActive)
            {
                return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.InvalidUser));
            }

            var purchaseTypeCode = await _salesSoftwareProcessor.UserCanbyPruductService(userInfo, input.PId);

            //用户购买了比用户当前生效的更低版本的软件服务
            if (purchaseTypeCode == UserCanPurchaseCode.PurchaseHigherVersion)
            {
                return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.ExistHighVersion));
            }

            //用户购买了终身版本
            if (purchaseTypeCode == UserCanPurchaseCode.PurchaseCurrentServiceLifelong)
            {
                return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.PurchaseCurrentServiceLifelong));
            }

            SalesWay salesWay;
            if (!Enum.TryParse(input.SalesWay, true, out salesWay))
            {
                return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.Other, "销售途径取值非法"));
            }

            if (!string.IsNullOrEmpty(input.SubscriptionOrderId))
            {
                if (input.SuggestCost == null || input.SuggestCost <= 0)
                {
                    return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.Other, "服务授权升级,则产品剩余价值不能为空"));
                }

                var msg = string.Empty;
                if (!_salesSoftwareProcessor.TestProductValues(input.UId,input.PId,input.SubscriptionOrderId,input.SuggestCost.Value,input.Cost,out msg))
                {
                    return new ResultMessage<SalesResultMessage>(ResultCode.Fail,"Fail",new SalesResultMessage(SalesResultType.Other,msg));
                }
               
            }

            switch (salesWay)
            {
                case SalesWay.Admin:
                case SalesWay.CustomService:
                    if (input.AdminId == null || input.AdminId <= 0)
                    {
                        return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.Other, "销售人员的Id不能为空"));
                    }

                    try
                    {
                        await _salesSoftwareProcessor.PurchaseSoftService(userInfo, input.MapTo<SalesInfoModel>(), false);
                        return new ResultMessage<SalesResultMessage>(ResultCode.Success, "Success", new SalesResultMessage(SalesResultType.Success));
                    }
                    catch (Exception e)
                    {
                        return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.Other, e.Message));
                    }
                case SalesWay.Agent:
                    if (input.AgentId == null || input.AgentId <= 0)
                    {
                        return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.Other, "代理商的Id不能为空"));
                    }
                    try
                    {
                        await _salesSoftwareProcessor.PurchaseSoftService(userInfo, input.MapTo<SalesInfoModel>(), true);
                        return new ResultMessage<SalesResultMessage>(ResultCode.Success, "Success", new SalesResultMessage(SalesResultType.Success));
                    }
                    catch (Exception e)
                    {
                        return new ResultMessage<SalesResultMessage>(ResultCode.Fail,"Fail",new SalesResultMessage(SalesResultType.Other,e.Message));
                    }
                case SalesWay.Online:
                    if (string.IsNullOrEmpty(input.OrderId))
                    {
                        return new ResultMessage<SalesResultMessage>(ResultCode.Fail, "Fail", new SalesResultMessage(SalesResultType.OnlinePurchaseNullOrderId));
                    }
                    
                    break;
            }
            return null;
        }
    }
}