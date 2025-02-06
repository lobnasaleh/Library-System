using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        public IBookGenreRepository BookGenreRepository { get; }
        public IGenreRepository GenreRepository { get; }
        public IBookRepository BookRepository { get; }
        public IBorrowRepository BorrowRepository { get; }
        public Task<int> CompleteAsync();
        //For transactions ensuring all or None
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync(IDbContextTransaction transaction);
        Task RollbackAsync(IDbContextTransaction transaction);
    }
}
