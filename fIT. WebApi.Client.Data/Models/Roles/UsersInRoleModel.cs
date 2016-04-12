using System.Collections.Generic;

namespace fIT.WebApi.Client.Data.Models.Roles
{
  /// <summary>
  /// Definies a Role and the added and removed users
  /// </summary>
  public class UsersInRoleModel
  {
    /// <summary>
    /// Role Id
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Ids of Users, tha shall be added/shall or keep this role
    /// </summary>
    public List<string> EnrolledUsers { get; set; }
    /// <summary>
    /// Ids of users, that shall be removed from this role
    /// </summary>
    public List<string> RemovedUsers { get; set; }
  }
}