using CodingMilitia.Utils.Pagination.EFCore.Tests.Mocks;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CodingMilitia.Utils.Pagination.EFCore.Tests
{
    public class PaginateTests
    {
        private static readonly int ItemsPerPage = 3;
        private static readonly InMemoryContext Context = new InMemoryContext();
        private static readonly DbEntity[] Items = new DbEntity[10];
        private static readonly DbEntity[] InitalPage = new DbEntity[3];
        private static readonly DbEntity[] InnerPage = new DbEntity[3];
        private static readonly DbEntity[] LastPage = { new DbEntity { Key = 10 } };
        private static readonly DbEntity[] EmptyPage = { };
        private static readonly IQueryable<string> NullItems = null;

        static PaginateTests()
        {
            for (var i = 1; i <= 10; ++i)
            {
                Items[i - 1] = new DbEntity { Key = i };
            }
            for (var i = 1; i <= 3; ++i)
            {
                InitalPage[i - 1] = new DbEntity { Key = i };
            }
            for (var i = 4; i <= 6; ++i)
            {
                InnerPage[i - 4] = new DbEntity { Key = i };
            }
            Context.DbSet.AddRange(Items);
            Context.SaveChanges();

        }

        [Fact]
        public async Task PaginateGetsInitialPage()
        {
            var pageNumber = 1;
            var items = Context.DbSet;
            var page = await items.PaginateAsync(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(InitalPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(InitalPage, page);
        }

        [Fact]
        public async Task PaginateGetsInnerPage()
        {
            var pageNumber = 2;
            var items = Context.DbSet;
            var page = await items.PaginateAsync(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(InnerPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(InnerPage, page);
        }

        [Fact]
        public async Task PaginateGetsLastIncompletePage()
        {
            var pageNumber = 4;
            var items = Context.DbSet;
            var page = await items.PaginateAsync(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(LastPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(LastPage, page);
        }

        [Fact]
        public async Task PaginateGetsEmptyPage()
        {
            var pageNumber = 5;
            var items = Context.DbSet;
            var page = await items.PaginateAsync(pageNumber, ItemsPerPage);
            Assert.Equal(pageNumber, page.Number);
            Assert.Equal(ItemsPerPage, page.ItemsPerPage);
            Assert.Equal(EmptyPage.Length, page.ItemCount);
            Assert.Equal(Items.Length, page.TotalItemCount);
            Assert.Equal(EmptyPage, page);
        }

        [Fact]
        public async Task PaginateThrowsArgumentNullExceptionOnNullItems()
        {
            var items = Context.DbSet;
            await Assert.ThrowsAsync<ArgumentNullException>("items", async () => await NullItems.PaginateAsync(1, ItemsPerPage));
        }

        [Fact]
        public async Task PageNumberSmallerThanOneThrowsArgumentException()
        {
            var items = Context.DbSet;
            await Assert.ThrowsAsync<ArgumentException>("pageNumber", async () => await items.PaginateAsync(0, ItemsPerPage));
        }
        [Fact]
        public async Task ItemsPerPageSmallerThanOneThrowsArgumentException()
        {
            var items = Context.DbSet;
            await Assert.ThrowsAsync<ArgumentException>("itemsPerPage", async () => await items.PaginateAsync(1, 0));
        }
    }
}
