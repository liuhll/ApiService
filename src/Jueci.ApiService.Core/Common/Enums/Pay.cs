namespace Jueci.ApiService.Common.Enums
{
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        Wechat = 1 ,

        /// <summary>
        /// 支付宝
        /// </summary>
        AliPay = 2,

    }

    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayMode
    {
        /// <summary>
        /// App支付
        /// </summary>
        App = 1,

        /// <summary>
        /// 手机网站支付
        /// </summary>
        Mobile = 2,

        /// <summary>
        /// PC网站
        /// </summary>
        PC = 3,
    }
}