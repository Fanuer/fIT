﻿using System;
using System.Collections.Generic;

namespace fIT.WebApi.Client.Models.Account
{
  public class AuthenticationResultModel
  {
    #region Field

    private const string ACCESS_TOKEN = "access_token";
    private const string REFRESH_TOKEN = "refresh_token";
    private const string EXPIREDATE = ".expires";
    private const string ISSUEDATE = ".issued";
    private const string TOKEN_TYPE = "token_type";
    private const string DATEFORMAT = "ddd dd MMM yyyy h:mm tt zzz";
    #endregion

    #region Ctor

    public AuthenticationResultModel(string accessToken="", string refreshToken="", string tokenType="bearer", DateTime expireDate = default(DateTime))
    {
      AccessToken = accessToken;
      RefreshToken = refreshToken;
      TokenType = tokenType;
      ExpireDate = expireDate;
    }

    public AuthenticationResultModel(IDictionary<string, string> httpContentResults)
    {
      if (httpContentResults == null)
      {
        throw new ArgumentNullException("httpContentResults");
      }

      if (httpContentResults.ContainsKey(ACCESS_TOKEN))
      {
        AccessToken = httpContentResults[ACCESS_TOKEN];
      }
      if (httpContentResults.ContainsKey(REFRESH_TOKEN))
      {
        RefreshToken = httpContentResults[REFRESH_TOKEN];
      }
      if (httpContentResults.ContainsKey(TOKEN_TYPE))
      {
        TokenType = httpContentResults[TOKEN_TYPE];
      }
      if (httpContentResults.ContainsKey(ISSUEDATE))
      {
        IssueDate = DateTime.Parse(httpContentResults[ISSUEDATE]);
        //IssueDate = Convert.ToDateTime(httpContentResults[ISSUEDATE]);
      }
      if (httpContentResults.ContainsKey(EXPIREDATE))
      {
        ExpireDate = DateTime.Parse(httpContentResults[EXPIREDATE]);
      }

    }
    #endregion
    
    #region Methods

    #endregion

    #region Properties
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime IssueDate { get; set; }
    #endregion
  }
}