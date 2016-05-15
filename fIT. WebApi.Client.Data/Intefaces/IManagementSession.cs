using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IManagementSession : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Encrypted Access Token
        /// </summary>
        string Token { get; }
        /// <summary>
        /// Token to refresh an accessToken
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Access Token Expire Date
        /// </summary>
        DateTimeOffset ExpiresOn { get; }
        /// <summary>
        /// Performs a Refresh of the Access token
        /// </summary>
        /// <returns></returns>
        Task PerformRefreshAsync();
        /// <summary>
        /// Current User name
        /// </summary>
        string CurrentUserName { get; }
        /// <summary>
        /// Current User Id
        /// </summary>
        Guid CurrentUserId { get; }
        /// <summary>
        /// All Methods a users with role 'User' can perform
        /// </summary>
        IUserManagement Users { get; }
        /// <summary>
        /// All Methods a users with role 'Admin' can perform
        /// </summary>
        IAdminManagement Admins { get; }
    }
}
