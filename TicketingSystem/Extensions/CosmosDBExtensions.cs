using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;

public static class CosmosDbExtensions
{
    public static async Task<List<T>> QueryAsync<T>(this Container container, Expression<Func<T, bool>> predicate)
    {
        var feedIterator = container.GetItemLinqQueryable<T>()
                                    .Where(predicate)
                                    .ToFeedIterator();

        List<T> results = new List<T>();

        while (feedIterator.HasMoreResults)
        {
            FeedResponse<T> response = await feedIterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }
}