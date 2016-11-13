using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingMilitia.Utils.Pagination.Extensions
{
    public static class PaginationExtensions
    {
        /// <summary>
        /// Instantiates a new <see cref="Page{T}"/> with the provided arguments.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the <see cref="Page{T}"/>.</typeparam>
        /// <param name="items">The items to store in the new <see cref="Page{T}"/>.</param>
        /// <param name="pageNumber">The number of the new <see cref="Page{T}"/>.</param>
        /// <param name="itemsPerPage">The number of items each <see cref="Page{T}"/> can contain.</param>
        /// <param name="totalItemCount">Total number of items in the original collection.</param>
        /// <returns>A new <see cref="Page{T}"/>.</returns>
        public static Page<T> ToPage<T>(this IEnumerable<T> items, int pageNumber, int itemsPerPage, int totalItemCount)
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
            if (totalItemCount < 0)
            {
                throw new ArgumentException("Total item count must be 0 or greater.", nameof(totalItemCount));
            }
            return new Page<T>(pageNumber, itemsPerPage, totalItemCount, items);
        }
        /// <summary>
        /// Extracts a <see cref="Page{T}"/> from the provided <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the <see cref="Page{T}"/>.</typeparam>
        /// <param name="items">The original <see cref="IEnumerable{T}"/> from which to extract the <see cref="Page{T}"/>.</param>
        /// <param name="pageNumber">The number of the <see cref="Page{T}"/> to fetch.</param>
        /// <param name="itemsPerPage">The number of items each <see cref="Page{T}"/> can contain.</param>
        /// <returns>A new <see cref="Page{T}"/> extracted from the provided collection.</returns>
        public static Page<T> Paginate<T>(this IEnumerable<T> items, int pageNumber, int itemsPerPage)
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
            var totalItemCount = items.Count();
            var page = items.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
            return new Page<T>(pageNumber, itemsPerPage, totalItemCount, page);
        }

        /// <summary>
        /// Extracts a <see cref="Page{T}"/> from the provided <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the <see cref="Page{T}"/>.</typeparam>
        /// <param name="items">The original <see cref="IEnumerable{T}"/> from which to extract the <see cref="Page{T}"/>.</param>
        /// <param name="paginationParameter">The information about the page that should be extracted.</param>
        /// <returns>A new <see cref="Page{T}"/> extracted from the provided collection.</returns>
        public static Page<T> Paginate<T>(this IEnumerable<T> items, PaginationParameter paginationParameter)
        {
            return items.Paginate(paginationParameter.PageNumber, paginationParameter.ItemsPerPage);
        }
    }
}
