using CodingMilitia.Utils.Pagination.Extensions;
using System;
using Xunit;

namespace CodingMilitia.Utils.Pagination.Tests
{
    public class PaginateTests
    {
        private static readonly int ItemsPerPage = 3;
        private static readonly int[] Items = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static readonly int[] InitalPage = { 1, 2, 3 };
        private static readonly int[] InnerPage = { 4, 5, 6 };
        private static readonly int[] LastPage = { 10 };
        private static readonly int[] EmptyPage = { };
        private static readonly int[] NullItems = null;

        [Fact]
        public void PaginateGetsInitialPage()
        {
            var pageNumber = 1;
            var page = Items.Paginate(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(InitalPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(InitalPage, page);
        }

        [Fact]
        public void PaginateGetsInnerPage()
        {
            var pageNumber = 2;
            var page = Items.Paginate(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(InnerPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(InnerPage, page);
        }

        [Fact]
        public void PaginateGetsLastIncompletePage()
        {
            var pageNumber = 4;
            var page = Items.Paginate(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(LastPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(LastPage, page);
        }

        [Fact]
        public void PaginateGetsEmptyPage()
        {
            var pageNumber = 5;
            var page = Items.Paginate(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(EmptyPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(EmptyPage, page);
        }

        [Fact]
        public void PaginateThrowsArgumentNullExceptionOnNullItems()
        {
            Assert.Throws<ArgumentNullException>("items", () => NullItems.Paginate(1, ItemsPerPage));
        }

        [Fact]
        public void PageNumberSmallerThanOneThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("pageNumber", () => Items.Paginate(0, ItemsPerPage));
        }
        [Fact]
        public void ItemsPerPageSmallerThanOneThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("itemsPerPage", () => Items.Paginate(1, 0));
        }
    }
}
