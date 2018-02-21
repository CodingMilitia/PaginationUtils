using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodingMilitia.Utils.Pagination
{
    public class Page<T> : IEnumerable<T>
    {
        public int Number { get; private set; }
        public int ItemCount { get; private set; }
        public int ItemsPerPage { get; private set; }
        public int TotalItemCount { get; private set; }

        private readonly IReadOnlyCollection<T> _items;

        public Page(int number, int itemsPerPage, int totalCount, IReadOnlyCollection<T> items)
        {
            Number = number;
            ItemCount = items.Count;
            ItemsPerPage = itemsPerPage;
            TotalItemCount = totalCount;
            _items = items;
        }

        public Page(int number, int itemsPerPage, int totalCount, IEnumerable<T> items) 
            : this(number, itemsPerPage, totalCount, items.ToList())
        { }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
