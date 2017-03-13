namespace Jueci.ApiService.Common.Enums
{
    public enum PurchaseType
    {
        /// <summary>
        /// 全新购买
        /// </summary>
        NewPuchase,

        /// <summary>
        /// 续费
        /// </summary>
        ReNew,

        /// <summary>
        /// 升级
        /// </summary>
        Upgrade
    }

    /// <summary>
    /// 用户是否可以购买服务授权
    /// </summary>
    public enum UserCanPurchaseCode
    {
        /// <summary>
        /// 可以购买服务
        /// </summary>
        CanPurchaseService = 0,

        /// <summary>
        /// 已经购买了当前版本的终身版
        /// </summary>
        PurchaseCurrentServiceLifelong = 1,

        /// <summary>
        /// 已经购买了更高版本
        /// </summary>
        PurchaseHigherVersion = 2
    }
}