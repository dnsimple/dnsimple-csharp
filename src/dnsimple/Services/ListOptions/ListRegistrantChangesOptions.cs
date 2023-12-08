namespace dnsimple.Services.ListOptions
{
  /// <summary>
  /// Defines the options you may want to send to list registrant changes, such as
  /// pagination, sorting and filtering.
  /// </summary>
  /// <see cref="ListOptionsWithFiltering"/>
  public class RegistrantChangesListOptions : ListOptionsWithFiltering
  {
    /// <summary>
    /// Sets the account to be filtered by.
    /// </summary>
    /// <param name="account">The account id we want to filter by.</param>
    /// <returns>The instance of the <c>RegistrantChangesListOptions</c></returns>
    public RegistrantChangesListOptions FilterByAccount(object accountId)
    {
      AddFilter(new Filter { Field = "account", Value = accountId.ToString() });
      return this;
    }

    /// <summary>
    /// Sets the domain to be filtered by.
    /// </summary>
    /// <param name="domainId">The domain ID</param>
    /// <returns>The instance of the <c>RegistrantChangesListOptions</c></returns>
    public RegistrantChangesListOptions FilterByDomain(long domainId)
    {
      AddFilter(new Filter { Field = "domain_id", Value = domainId.ToString() });
      return this;
    }

    /// <summary>
    /// Sets the contact to be filtered by.
    /// </summary>
    /// <param name="contactId">The contact ID</param>
    /// <returns>The instance of the <c>RegistrantChangesListOptions</c></returns>
    public RegistrantChangesListOptions FilterByContact(long contactId)
    {
      AddFilter(new Filter { Field = "contact_id", Value = contactId.ToString() });
      return this;
    }

    /// <summary>
    /// Sets the state to be filtered by.
    /// </summary>
    /// <param name="state">The state we want to filter by.</param>
    /// <returns>The instance of the <c>RegistrantChangesListOptions</c></returns>
    public RegistrantChangesListOptions FilterByState(string state)
    {
      AddFilter(new Filter { Field = "state", Value = state });
      return this;
    }

    /// <summary>
    /// Sets the order by which to sort by id.
    /// </summary>
    /// <param name="order">The order in which we want to sort (asc or desc)</param>
    /// <returns>The instance of the <c>RegistrantChangesListOptions</c></returns>
    /// <see cref="Order"/>
    public RegistrantChangesListOptions SortById(Order order)
    {
      AddSortCriteria(new Sort { Field = "id", Order = order });
      return this;
    }
  }
}
