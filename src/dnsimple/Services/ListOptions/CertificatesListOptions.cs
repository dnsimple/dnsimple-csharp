namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list certificates,
    /// such as pagination and sorting
    /// </summary>
    /// <see cref="ListOptions"/>
    public class CertificatesListOptions : ListOptions
    {
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>CertificatesListOptions</c></returns>
        public CertificatesListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by common name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>CertificatesListOptions</c></returns>
        public CertificatesListOptions SortByCommonName(Order order)
        {
            AddSortCriteria(new Sort { Field = "common_name", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by the certificate expiration date.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>CertificatesListOptions</c></returns>
        public CertificatesListOptions SortByExpiresOn(Order order)
        {
            return this.SortByExpiration(order);
        }

        /// <summary>
        /// Sets the order by which to sort by expiration.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>CertificateListOptions</c></returns>
        public CertificatesListOptions SortByExpiration(Order order)
        {
            AddSortCriteria(new Sort { Field = "expiration", Order = order });
            return this;
        }
    }
}
