﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Intefaces;
using fIT.WebApi.Client.Models.Account;

namespace fIT.WebApi.Client.Implementation
{
    public partial class ManagementSession : IUserManagement
    {
        #region Field
        #endregion

        #region Ctor
        #endregion

        #region Methods
        public async Task UpdatePasswordAsync(ChangePasswordModel model)
        {
            await PutAsJsonAsync(model, "/api/accounts/ChangePassword");
        }
        #endregion

        #region Properties
        public IUserManagement Users { get { return this; } }

        #endregion

        
    }
}
