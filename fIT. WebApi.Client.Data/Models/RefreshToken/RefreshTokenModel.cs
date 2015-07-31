using System;

namespace fIT.WebApi.Client.Data.Models.RefreshToken
{
  public class RefreshTokenModel
  {
    /// <summary>
    /// Token Id
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// User name
    /// </summary>
    public string Subject { get; set; }
    /// <summary>
    /// Client name
    /// </summary>
    public string ClientId { get; set; }
    /// <summary>
    /// Issue date
    /// </summary>
    public DateTime IssuedUtc { get; set; }
    /// <summary>
    /// Expire Date
    /// </summary>
    public DateTime ExpiresUtc { get; set; }
  }
}