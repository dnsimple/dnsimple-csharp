namespace dnsimple.Services.ListOptions
{
    public class CertificatesListOptions : ListOptions
    {
        
        private const string IdSort = "id";
        private const string CommonNameSort = "common_name";
        private const string ExpiresOnSort = "expires_on";
        
        public CertificatesListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = IdSort, Order = order });
            return this;
        }

        public CertificatesListOptions SortByCommonName(Order order)
        {
            AddSortCriteria(new Sort { Field = CommonNameSort, Order = order });
            return this;
        }

        public CertificatesListOptions SortByExpiresOn(Order order)
        {
            AddSortCriteria(new Sort{ Field = ExpiresOnSort, Order = order });
        return this;
        }
    }
}