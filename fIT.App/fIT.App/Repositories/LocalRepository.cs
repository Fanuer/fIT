using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.App.Data.Datamodels;
using fIT.App.Data.Exceptions;
using fIT.App.Interfaces;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.RefreshToken;
using fIT.WebApi.Client.Data.Models.Roles;
using fIT.WebApi.Client.Data.Models.Schedule;
using Xamarin.Forms;

namespace fIT.App.Repositories
{
  public class LocalRepository
  {
    #region FIELDS

    private static LocalRepository _instance;
    #endregion

    #region CTOR

    public LocalRepository()
    {
      this.CreateTables();
      this.AdminManagement = new LocalAdminManagement();
      this.UserManagement = new LocalUserManagement();
    }
    #endregion

    #region METHODS

    internal async Task SyncData()
    {
      
    }

    private void CreateTables()
    {
      using (var con = DependencyService.Get<ISqlLite>().GetConnection())
      {
        con.BeginTransaction();

        try
        {
          con.CreateTable<User>();
          con.CreateTable<ScheduleHasExercises>();
          con.CreateTable<Schedule>();
          con.CreateTable<Exercise>();
          con.CreateTable<Practice>();
          con.Commit();

        }
        catch (Exception)
        {
          con.Rollback();
        }
      }
    }
        #endregion

    public async Task<bool> LoginAsync(string username, string password)
    {
      var result = false;

      if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
      {
        var conn = DependencyService.Get<ISqlLite>().GetAsyncConnection();
        var user = await conn.FindAsync<User>(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && x.Password.Equals(password, StringComparison.CurrentCultureIgnoreCase));
        if (user != null)
        {
          this.CurrentUserId = user.UserId;
          result = true;
        }
      }
      return result;
    }

    #region PROPERTIES

    public IUserManagement UserManagement { get; set; }
    public IAdminManagement AdminManagement { get; set; }
    public Guid CurrentUserId { get; set; }

    public static LocalRepository Current => LocalRepository._instance ?? (LocalRepository._instance = new LocalRepository());

      #endregion

    #region NESTED
    private class LocalAdminManagement:IAdminManagement
    {
      public Task DeleteUserAsync(string userId)
      {
        throw new NotImplementedException();
      }

      public Task<UserModel> GetUserByUsernameAsync(string username)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<UserModel>> GetAllUsersAsync()
      {
        throw new NotImplementedException();
      }

      public Task<UserModel> GetUserByIdAsync(Guid id)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<RoleModel>> GetAllRolesAsync()
      {
        throw new NotImplementedException();
      }

      public Task<RoleModel> GetRoleByIdAsync(string id)
      {
        throw new NotImplementedException();
      }

      public Task<RoleModel> GetRoleByNameAsync(string name)
      {
        throw new NotImplementedException();
      }

      public Task DeleteRoleAsync(string id)
      {
        throw new NotImplementedException();
      }

      public Task<RoleModel> CreateRoleAsync(string roleName)
      {
        throw new NotImplementedException();
      }

      public Task ManageUsersInRolesAsync(UsersInRoleModel model)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<RefreshTokenModel>> GetAllRefreshtokensAsync()
      {
        throw new NotImplementedException();
      }

      public Task<ExerciseModel> CreateExerciseAsync(ExerciseModel model)
      {
        throw new NotImplementedException();
      }

      public Task DeleteExerciseAsync(int id)
      {
        throw new NotImplementedException();
      }

      public Task UpdateExerciseAsync(int id, ExerciseModel model)
      {
        throw new NotImplementedException();
      }

      
    }

    private class LocalUserManagement : IUserManagement
    {
      #region FIELDS
      #endregion

      #region CTOR
      #endregion

      #region METHODS
      public Task UpdatePasswordAsync(ChangePasswordModel model)
      {
        throw new InvalidForOfflineException();
      }

      public async Task<UserModel> GetUserDataAsync()
      {
        User result = null;

        if (!String.IsNullOrEmpty(this.CurrentUserID.ToString()))
        {
          var conn = DependencyService.Get<ISqlLite>().GetAsyncConnection();
          result = await conn.FindAsync<User>(x => x.UserId == CurrentUserID);
        }
        return null;
      }


      public Task UpdateUserDataAsync(UserModel newData)
      {
        throw new NotImplementedException();
      }

      public Task<ScheduleModel> GetScheduleByIdAsync(int id)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync()
      {
        throw new NotImplementedException();
      }

      public Task<ScheduleModel> CreateScheduleAsync(ScheduleModel model)
      {
        throw new NotImplementedException();
      }

      public Task DeleteScheduleAsync(int id)
      {
        throw new NotImplementedException();
      }

      public Task UpdateScheduleAsync(int scheduleId, ScheduleModel model)
      {
        throw new NotImplementedException();
      }

      public Task AddExerciseToScheduleAsync(int scheduleId, int exerciseId)
      {
        throw new NotImplementedException();
      }

      public Task RemoveExerciseFromScheduleAsync(int scheduleId, int exerciseId)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<ExerciseModel>> GetAllExercisesAsync()
      {
        throw new NotImplementedException();
      }

      public Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId)
      {
        throw new NotImplementedException();
      }

      public Task<IEnumerable<PracticeModel>> GetAllPracticesAsync()
      {
        throw new NotImplementedException();
      }

      public Task<PracticeModel> GetPracticeByIdAsync(int id)
      {
        throw new NotImplementedException();
      }

      public Task<PracticeModel> CreatePracticeAsync(PracticeModel model)
      {
        throw new NotImplementedException();
      }

      public Task DeletePracticeAsync(int id)
      {
        throw new NotImplementedException();
      }

      public Task UpdatePracticeAsync(int id, PracticeModel model)
      {
        throw new NotImplementedException();
      }

      #endregion

      #region PROPERTIES
      public Guid CurrentUserID { get; set; }

      private bool UserIdSet => !String.IsNullOrWhiteSpace(this.CurrentUserID.ToString()) && this.CurrentUserID != Guid.Empty;

      #endregion
    }
    #endregion
  }
}
