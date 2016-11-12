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
        private T[] _items;

        public Page(int number, int itemsPerPage, int totalCount, IEnumerable<T> items)
        {
            Number = number;
            ItemCount = items.Count();
            ItemsPerPage = itemsPerPage;
            TotalItemCount = totalCount;
            _items = items.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
