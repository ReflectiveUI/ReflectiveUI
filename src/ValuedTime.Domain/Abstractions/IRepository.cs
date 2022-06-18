using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.Domain.Abstractions;

public interface IRepository<T> : IReadRepositoryBase<T>, IRepositoryBase<T> where T : class, IAggregateRoot
{
}
