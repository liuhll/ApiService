using Abp.Logging;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.Sales
{
    public class SalesResultMessage
    {
        public SalesResultMessage(SalesResultType salesResultType, string message)
        {
            _salesResultCode = salesResultType;
            Message = message;
        }

        public SalesResultMessage(SalesResultType salesResultType)
        {
            _salesResultCode = salesResultType;
            Message = SetMessageByResultType(salesResultType);
            if (_salesResultCode == SalesResultType.Success)
            {
                LogHelper.Logger.Info(Message);
            }
            else
            {
                LogHelper.Logger.Error(Message);
            }
        }

        private string SetMessageByResultType(SalesResultType salesResultType)
        {
            string resultMsg = string.Empty;

            switch (salesResultType)
            {
                case SalesResultType.NullUser:
                    resultMsg = "系统中不存在该用户，请检查输入的用户Id是否正确!";
                    break;
                case SalesResultType.InvalidUser:
                    resultMsg = "该用户账号被冻结，该用户无法购买服务产品";
                    break;
                case SalesResultType.ExistHighVersion:
                    resultMsg = "该用户已经购买了更高版本的软件服务,在服务失效截止前，无法购买低版本软件服务";
                    break;
                case SalesResultType.PurchaseCurrentServiceLifelong:
                    resultMsg = "该用户已经购买了当前软件版本的终身版，无法购买当前服务，建议升级更高版本";
                    break;
                case SalesResultType.Success:
                    resultMsg = "购买成功！";
                    break;
                case SalesResultType.InsufficientFunds:
                    resultMsg = "您的余额不足，购买失败，请充值";
                    break;
                case SalesResultType.OnlinePurchaseNullOrderId:
                    resultMsg = "用户通过在线购买产品授权，但是不存在支付订单号";
                    break;
                default:
                    resultMsg = "系统异常，购买失败，请与我们联系";
                    break;
            }
            return resultMsg;
        }

        private readonly SalesResultType _salesResultCode;

        public string SalesResult
        {
            get { return _salesResultCode.ToString(); }
        }

        public string Message { private set; get; }
    }
}