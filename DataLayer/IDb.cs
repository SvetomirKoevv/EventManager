using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    interface IDb<T, K> where K : IConvertible
    {
        Task CreateAsync(T item);

        Task<T> ReadAsync(K key, bool useNavigationalProperties = false);

        Task<ICollection<T>> ReadAllAsync(bool useNavigationalProperties = false);

        Task UpdateAsync(T item, bool useNavigationalProperties = false);

        Task DeleteAsync(K key);
    }
}
