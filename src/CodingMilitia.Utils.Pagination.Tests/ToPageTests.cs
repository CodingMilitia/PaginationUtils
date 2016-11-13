using Xunit;
using CodingMilitia.Utils.Pagination.Extensions;
using System;

namespace CodingMilitia.Utils.Pagination.Tests
{
    public class ToPageTests
    {
        private static readonly int PageNumber = 1;
        private static readonly int ItemsPerPage = 10;
        private static readonly int TotalItemCount = 50;
        private static readonly int[] Items = { 1, 2, 3, 4, 5 };
        private static readonly int[] NullItems;

        [Fact]
        public void ToPageInstantiatesNewPageWithProvidedValues()
        {
            var page = Items.ToPage(PageNumber, ItemsPerPage, TotalItemCount);
            Assert.Equal(PageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(Items.Length, page.ItemCount);
            Assert.Equal(TotalItemCount, page.TotalItemCount);
            Assert.Equal(Items, page);
        }

        [Fact]
        public void ToPageThrowsArgumentNullExceptionOnNullItems()
        {
            Assert.Throws<ArgumentNullException>("items", () => NullItems.ToPage(PageNumber, ItemsPerPage, TotalItemCount));

        }
        [Fact]
        public void PageNumberSmallerThanOneThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("pageNumber", () => Items.ToPage(0, ItemsPerPage, TotalItemCount));
        }
        [Fact]
        public void ItemsPerPageSmallerThanOneThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("itemsPerPage", () => Items.ToPage(1, 0, TotalItemCount));
        }
        [Fact]
        public void NegativeTotalItemCountThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("totalItemCount", () => Items.ToPage(1, ItemsPerPage, -1));
        }
    }
}
