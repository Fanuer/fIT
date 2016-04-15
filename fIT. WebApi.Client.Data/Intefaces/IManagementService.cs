using System;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
  public interface IManagementService
  {
    /// <summary>
    /// Baut eine Verbindung zum Webserver auf
    /// </summary>
    /// <param name="username">Name des Nutzers</param>
    /// <param name="password">Passwort des Nutzers</param>
    /// <returns></returns>
    Task<IManagementSession> LoginAsync(string username, string password);

    /// <summary>
    /// Einloggen per RefreshToken
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<IManagementSession> LoginAsync(string refreshToken);
    /// <summary>
    /// Gibt einen Zeitstempel der Anfrage zurueck. Diese MEthode sollte BEnutzt werden, um zu pruefen, ob eine Verbindung zum Server besteht
    /// </summary>
    /// <returns></returns>
    Task<bool> PingAsync();
    /// <summary>
    /// Registriert einen neuen Benutzer an der Web-Application
    /// </summary>
    /// <param name="model">Daten des neuen Nutzers</param>
    /// <returns></returns>
    Task RegisterAsync(CreateUserModel model);
  }
}
