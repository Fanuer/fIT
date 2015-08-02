using System.Threading.Tasks;

namespace fIT.WebApi.Repository.Interfaces.CRUD.MultiId
{
    internal interface IRepositoryFindSingleMultiId<T, in TIdProperty> where T : IEntity<TIdProperty>
    {
        Task<T> FindAsync(TIdProperty[] id);
    }
}
