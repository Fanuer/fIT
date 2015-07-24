namespace fIT.WebApi.Client.Data.Models.Account
{
    /// <summary>
    /// Model to change a password
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Current Password
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// New Password
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// Confirmation of the new password
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
