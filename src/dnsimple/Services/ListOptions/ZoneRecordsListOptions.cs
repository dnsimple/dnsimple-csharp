namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list zone records, such as
    /// pagination, sorting and filtering.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class ZoneRecordsListOptions : ListOptionsWithFiltering
    {
        private const string NameLikeFilter = "name_like";
        private const string NameExactFilter = "name";
        private const string TypeFilter = "type";

        private const string IdSort = "id";
        private const string NameSort = "name";
        private const string ContentSort = "content";
        private const string TypeSort = "type";

        /// <summary>
        /// Creates a new instance of <c>ZoneRecordsListOptions</c>.
        /// </summary>
        public ZoneRecordsListOptions() =>
            Pagination = new Pagination();

        /// <summary>
        /// Only include records containing given string.
        /// </summary>
        /// <param name="name">The name we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByName(string name)
        {
            AddFilter(new Filter {Field = NameLikeFilter, Value = name});
            return this;
        }

        /// <summary>
        /// Only include records with name equal to given string.
        /// </summary>
        /// <param name="name">The name we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByExactName(string name)
        {
            AddFilter(new Filter {Field = NameExactFilter, Value = name});
            return this;
        }
        
        /// <summary>
        /// Only include records with record type equal to given string
        /// </summary>
        /// <param name="type">The record type we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByType(ZoneRecordType type)
        {
            AddFilter(new Filter {Field = TypeFilter, Value = type.ToString()});
            return this;
        }

        /// <summary>
        /// Sort records by ID.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort {Field = IdSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by name (alphabetical order).
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort {Field = NameSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by content.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByContent(Order order)
        {
            AddSortCriteria(new Sort {Field = ContentSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by type.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByType(Order order)
        {
            AddSortCriteria(new Sort {Field = TypeSort, Order = order});
            return this;
        }
    }
}