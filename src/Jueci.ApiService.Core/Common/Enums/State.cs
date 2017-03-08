namespace Jueci.ApiService.Common.Enums
{
    public enum State
    {
        /// <summary>
        /// 有效
        /// </summary>
        Active = 1,


        /// <summary>
        /// 无效
        /// </summary>
        Freeze = 0,
    }

    public enum OrderState
    {
        Invalid = 0,

        Legal = 1,

        Effective = 2,
    }

    /// <summary>
    /// 用户/管理员状态
    /// </summary>
    public enum UserState
    {
        /// <summary>
        /// 激活
        /// </summary>
        Active = 1,


        /// <summary>
        /// 冻结
        /// </summary>
        Freeze = 0,
    }
}