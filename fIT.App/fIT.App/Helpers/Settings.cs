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

    private const string KEY_BASEURL = "settings_key_BaseUrl";
    private static readonly string DEFAULT_BASEURL = @"http://fit-bachelor.azurewebsites.net/";
    #endregion

    #region CTOR
    #endregion

    #region METHODS
    #endregion

    #region PROPERTIES
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }

    internal static string BaseUrl
    {
      get
      {
        return AppSettings.GetValueOrDefault<string>(KEY_BASEURL, DEFAULT_BASEURL);
      }
      set
      {
        AppSettings.AddOrUpdateValue<string>(KEY_BASEURL, value);
      }
    }
    #endregion
  }
}