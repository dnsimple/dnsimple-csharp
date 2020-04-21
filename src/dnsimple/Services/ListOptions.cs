using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dnsimple.Services
{
    /// <summary>
    /// Contains the common options you can pass to a "List" method in order
    /// to control parameters such as pagination and sorting.
    /// </summary>
    public abstract class ListOptions
    {
        /// <summary>
        /// A List of <c>Sort</c> objects.
        /// </summary>
        /// <see cref="List{T}"/>
        /// <see cref="Sort"/>
        private List<Sort> SortCriteria { get; } = new List<Sort>();

        /// <summary>
        /// The <c>Pagination object</c>.
        /// </summary>
        /// <see cref="Pagination"/>
        public Pagination Pagination { get; set; } = new Pagination();

        /// <summary>
        /// Unpacks the sorting into a <c>KeyValuePair</c>.
        /// </summary>
        /// <returns>A <c>KeyVaulePair</c> with the sort options</returns>
        /// <see cref="KeyValuePair{TKey,TValue}"/>
        public KeyValuePair<string, string> UnpackSorting()
        {
            var value = new StringBuilder();
            foreach (var sort in SortCriteria)
            {
                value.Append($"{sort.Field}:{sort.Order},");
            }

            if (value.Length > 0)
            {
                return new KeyValuePair<string, string>("sort",
                    value.ToString().TrimEnd(','));
            }

            return new KeyValuePair<string, string>();

        }

        public List<KeyValuePair<string, string>> UnpackPagination()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", Pagination.PerPage.ToString()),
                new KeyValuePair<string, string>("page", Pagination.Page.ToString())
            };
        }

        /// <summary>
        /// Adds a <c>Sort</c> criteria to the options.
        /// </summary>
        /// <param name="sort">The sort criteria to add</param>
        /// <see cref="Sort"/>
        protected void AddSortCriteria(Sort sort) => SortCriteria.Add(sort);
    }

    /// <summary>
    /// Contains the common options you can pass to a "List" method in order
    /// to control parameters such as pagination, sorting and filtering.
    /// </summary>
    public abstract class ListOptionsWithFiltering : ListOptions
    {
        /// <summary>
        /// A list of <c>Filter</c> objects.
        /// </summary>
        /// <see cref="List{T}"/>
        /// <see cref="Filter"/>
        private List<Filter> Filters { get; } = new List<Filter>();
        
        /// <summary>
        /// Unpacks the filters into <c>KeyValuePair</c> objects.
        /// </summary>
        /// <returns>A list of KeyValuePair objects</returns>
        /// <see cref="List{T}"/>
        /// <see cref="KeyValuePair{TKey,TValue}"/>
        public List<KeyValuePair<string, string>> UnpackFilters()
        {
            return Filters.Select(filter =>
                    new KeyValuePair<string, string>(filter.Field,
                        filter.Value))
                .ToList();
        }
        
        /// <summary>
        /// Adds a <c>Filter</c> to the options.
        /// </summary>
        /// <param name="filter">The filter to add</param>
        /// <see cref="Filter"/>
        protected void AddFilter(Filter filter) => Filters.Add(filter);
    }

    /// <summary>
    /// Defines a sort criteria
    /// </summary>
    public struct Sort
    {
        /// <summary>
        /// The field to sort.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The order we want to sort the field by.
        /// </summary>
        /// <see cref="Order"/>
        public Order Order { get; set; }
    }

    /// <summary>
    /// Defines a filter.
    /// </summary>
    public struct Filter
    {
        /// <summary>
        /// The field we are filtering by.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The value of our filter
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Defines the Ascending and Descending order options.
    /// </summary>
    public enum Order
    {
        asc,
        desc
    }

    /// <summary>
    /// Defines the pagination options.
    /// </summary>
    public class Pagination
    {
        private const int _page = 1;
        private const int _perPage = 30;
        
        /// <summary>
        /// The items to display per page (defaults to 30).
        /// </summary>
        public int PerPage { get; set; } = _perPage;

        /// <summary>
        /// The page number requested (defaults to 1).
        /// </summary>
        public int Page { get; set; } = _page;

        public bool IsDefault()
        {
            return Page == _page && PerPage == _perPage;
        }
    }
}