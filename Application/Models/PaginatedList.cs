using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PaginatedList<T> 
    {
        public ICollection<T> Data { get; set; }

        public PaginatedListMetaData Meta { get; set; }

        public PaginatedList(ICollection<T> items, int count, int pageNumber, int pageSize)
        {
            var totalPages = (int)Math.Ceiling(count / (double)Math.Max(pageSize, 1));

            this.Meta = new PaginatedListMetaData(pageNumber, totalPages, count);
            this.Data = items;
        }

        public PaginatedList()
        {
            this.Data = new List<T>();
            this.Meta = new PaginatedListMetaData();
        }

        public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int? pageNumber, int? pageSize)
        {
            var count = source.Count();
            List<T> items;

            if (pageNumber != null && pageSize != null)
            {
                var paginatedSource = ApplyPagination(source, pageNumber.Value, pageSize.Value);
                items = paginatedSource.ToList();
            }
            else
            {
                items = source.ToList();
            }

            return new PaginatedList<T>(items, count, pageNumber ?? 1, pageSize ?? count);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int? pageNumber, int? pageSize)
        {
            var count = source.Count();
            List<T> items;

            if (pageNumber != null && pageSize != null)
            {
                var paginatedSource = ApplyPagination(source, pageNumber.Value, pageSize.Value);
                items = paginatedSource.ToList();
            }
            else
            {
                items = source.ToList();
            }

            return new PaginatedList<T>(items, count, pageNumber ?? 1, pageSize ?? count);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int count, int? pageNumber, int? pageSize)
        {
            List<T> items;

            if (pageNumber != null && pageSize != null)
            {
                var paginatedSource = ApplyPagination(source, pageNumber.Value, pageSize.Value);
                items = paginatedSource.ToList();
            }
            else
            {
                items = source.ToList();
            }

            return new PaginatedList<T>(items, count, pageNumber ?? 1, pageSize ?? count);
        }

        private static IEnumerable<T> ApplyPagination(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var s = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return s;
        }

        private static IQueryable<T> ApplyPagination(IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }

    public class PaginatedListMetaData
    {
        public PaginatedListMetaData()
        {
        }

        public PaginatedListMetaData(int pageNumber, int totalPages, int totalCount)
        {
            this.PageNumber = pageNumber;
            this.TotalPages = totalPages;
            this.TotalCount = totalCount;
        }

        public int PageNumber { get; }

        public int TotalPages { get; }

        public int TotalCount { get; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public bool HasNextPage => this.PageNumber < this.TotalPages;
    }
}
