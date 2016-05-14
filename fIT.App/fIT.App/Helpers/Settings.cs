// Helpers/Settings.cs

using System;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace fIT.App.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {

        #region FIELDS
        #endregion

        #region CONST

        private const string KEY_REFRESHTOKEN = "settings_key_RefreshToken";
        private static readonly string DEFAULT_REFRESHTOKEN = String.Empty;

        private const string KEY_USERID = "settings_key_UserId";
        private static readonly Guid DEFAULT_USERID = Guid.Empty;

        private const string KEY_USERNAME = "settings_key_UserName";
        private static readonly string DEFAULT_USERNAME = String.Empty;

        #endregion

        #region CTOR
        #endregion

        #region METHODS

        public static void RemoveUserSettings()
        {
            Settings.RefreshToken = Settings.Username = DEFAULT_REFRESHTOKEN;
            Settings.UserId = DEFAULT_USERID;
        }
        #endregion

        #region PROPERTIES
        private static ISettings AppSettings => CrossSettings.Current;

        internal static string RefreshToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(KEY_REFRESHTOKEN, DEFAULT_REFRESHTOKEN);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KEY_REFRESHTOKEN, value);
            }
        }

        internal static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(KEY_USERNAME, DEFAULT_USERNAME);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KEY_USERNAME, value);
            }
        }

        internal static Guid UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(KEY_USERID, DEFAULT_USERID);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KEY_USERID, value);
            }
        }


        #endregion
    }
}