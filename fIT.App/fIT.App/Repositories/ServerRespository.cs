using System;
using System.Threading.Tasks;
using fIT.App.Helpers;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Portable.Implementation;


namespace fIT.App.Repositories
{
    internal class ServerRespository
    {
        #region Const
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";

        #endregion

        #region FIELDS
        private static ServerRespository _current;

        #endregion

        #region CTOR

        private ServerRespository()
        {
            this.Server = new ManagementService(ServerRespository.URL);
        }
        #endregion

        #region METHODS

        #endregion

        #region PROPERTIES


        public static ServerRespository Current => ServerRespository._current ?? (ServerRespository._current = new ServerRespository());

        public IManagementSession ServerSession { get; internal set; }
        public IManagementService Server { get; private set; }
        #endregion
    }
}
