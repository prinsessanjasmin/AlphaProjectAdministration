using Business.Models;

namespace Business.Interfaces;

public interface ISearchService
{
    Task<SearchResult> SearchAsync (string query, CancellationToken cancellationToken = default);
}
