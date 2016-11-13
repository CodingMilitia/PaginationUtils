using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodingMilitia.Utils.Pagination.EFCore.Extensions
{
    public static class PaginationExtensions
    {
        /// <summary>
        /// Extracts a <see cref="Page{T}"/> from the provided <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the <see cref="Page{T}"/>.</typeparam>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a new <see cref="Page{T}"/> extracted from the provided collection.</returns>
        public static async Task<Page<T>> PaginateAsync<T>(this IQueryable<T> items, int pageNumber, int itemsPerPage, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (pageNumber < 1)
            {
                throw new ArgumentException("Page number must be 1 or greater.", nameof(pageNumber));
            }
            if (itemsPerPage < 1)
            {
                throw new ArgumentException("Items per page must be 1 or greater.", nameof(itemsPerPage));
            }
            var totalItemCount = await items.CountAsync(cancellationToken).ConfigureAwait(false);
            var page = await items.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            return new Page<T>(pageNumber, itemsPerPage, totalItemCount, page);
        }

        /// <summary>
        /// Extracts a <see cref="Page{T}"/> from the provided <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the <see cref="Page{T}"/>.</typeparam>
        /// <param name="items">The original <see cref="IQueryable{T}"/> from which to extract the <see cref="Page{T}"/>.</param>
        /// <param name="paginationParameter">The information about the page that should be extracted.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a new <see cref="Page{T}"/> extracted from the provided collection.</returns>
        public static Task<Page<T>> PaginateAsync<T>(this IQueryable<T> items, PaginationParameter paginationParameter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return items.PaginateAsync(paginationParameter.PageNumber, paginationParameter.ItemsPerPage, cancellationToken);
        }
    }
}
