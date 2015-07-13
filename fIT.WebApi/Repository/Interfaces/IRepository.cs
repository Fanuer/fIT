using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Repository.Interfaces
{
    public interface IRepository : IDisposable
    {
        IClientRepository Clients { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IExerciseRepository Excercies { get; }
    }
}
