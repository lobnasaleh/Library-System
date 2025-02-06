using LMS.Core.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext context;
        public IBookGenreRepository BookGenreRepository { get; private set; }

        public IGenreRepository GenreRepository { get; private set; }

        public IBookRepository BookRepository { get; private set; }

        public IBorrowRepository BorrowRepository { get; private set; }

   

        public UnitOfWork(ApplicationDBContext context)
        {
            this.context = context;
            BookGenreRepository=new BookGenreRepository(context);
            GenreRepository = new GenreRepository(context);
            BookRepository = new BookRepository(context);
            BorrowRepository = new BorrowRepository(context);
           
         

        }
        public async Task <int> CompleteAsync()
        {
           return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
           context.Dispose();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollbackAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }
    }
}
