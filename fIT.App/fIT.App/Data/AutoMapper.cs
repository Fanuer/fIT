using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.Datamodels;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.App.Data
{
  class AutoMapper
  {
    public static UserModel ToViewModel(User datamodel)
    {
      UserModel result = null;
      if (datamodel != null)
      {
        result = new UserModel
        {
          Id = datamodel.UserId,
          DateOfBirth = datamodel.DateOfBirth,
          Email = datamodel.Email,
          Gender = datamodel.Gender,
          Job = datamodel.Job,
          Fitness = datamodel.Fitness,
        };
      }
      return result;
    }
  }
}
