using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Microsoft.ML.Tokenizers;

using static Qdrant.Client.Grpc.Conditions;
using System.Diagnostics;

namespace Diplom2.Data.Analyzer
{
    public class Recomender : IRecomender
    {
        private readonly QdrantClient _qdrantClient;
        private const string _collectionName = "Products";
        private readonly string _tagsVectorName = "tags";
        private readonly string _descriptionVectorName = "desc";
        private readonly float _coefficient = 0.1f;
        private readonly TiktokenTokenizer tokenizer;

        public Recomender()
        {
            tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");

            _qdrantClient = new QdrantClient("qdrant-headless.qdrant.svc.cluster.local", 6334);
            _qdrantClient.CreateCollectionAsync(
                collectionName: _collectionName,
                sparseVectorsConfig: new SparseVectorConfig {
                    Map = {
                        [_tagsVectorName] = new SparseVectorParams (),
			            [_descriptionVectorName] = new SparseVectorParams(),
		            }
                }
            );
        }

        public async Task<bool> BatchInsert(IEnumerable<Product> products)
        {
            return (await _qdrantClient.UpsertAsync(
                collectionName: _collectionName,
                points: products.Select(p => new PointStruct() {
                    Id = (uint)p.Id,
                    Vectors = new Dictionary<string, Vector> {
                        [_tagsVectorName] = ConvertProduct(p.Tags),
                        [_descriptionVectorName] = ConvertTokens(tokenizer.EncodeToIds(p.Name))
                    },
                    Payload = { ConvertToPayload(p) }
                }).ToList()
                )).Status == UpdateStatus.Completed;
        }

        public async Task<IEnumerable<uint>> RecomendMain(User user, uint page, uint perPage, List<Models.Filter> filters, string? search)
        {
            user.UserTags.Add(new UserTag() { Tag = new Tag(){ Id = 1_000_000}, Count = 1 });
            if (search == null || search == "") {
                return await RecomendWithoutSearch(user, page, perPage, filters);
            }
            IReadOnlyList<int> searchTokens = search != null ? tokenizer.EncodeToIds(search) : new List<int>();
            var filter = filters.Select(f => Match(f.Id.ToString(), f.ArrayOfValues())).Aggregate((c1,c2) => c1&c2);
            var response = await _qdrantClient.QueryAsync(
                collectionName: _collectionName,
                query: ConvertTokens(searchTokens),
                usingVector: _descriptionVectorName
                //filter: filter
            );
            ulong count = (ulong)(response.Count(s => s.Score > _coefficient));
            return (await _qdrantClient.QueryAsync(
                collectionName: _collectionName,
                prefetch: new List<PrefetchQuery> {
                    new() {
                        Query = ConvertTokens(searchTokens),
                        Limit = Math.Max(count,1),
                        Using = _descriptionVectorName,
                        //Filter = filter
                    }
                },
                query: ConvertPreferences(user),
                usingVector: _tagsVectorName,
                limit: perPage,
                offset: (page - 1) * perPage
            )).Select(r => (uint)r.Id.Num);
        }
        private async Task<IEnumerable<uint>> RecomendWithoutSearch(User user, uint page, uint perPage, List<Models.Filter> filters)
        {
            var task = _qdrantClient.QueryAsync(
                collectionName: _collectionName,
                query: ConvertPreferences(user),
                usingVector: _tagsVectorName,
                limit: perPage,
                offset: (page - 1) * perPage
            );
            task.Wait();
            var user1 = ConvertPreferences(user);
            Debug.WriteLine(user1);
            return task.Result.Select(r => (uint)r.Id.Num);
        }

        private (float, uint)[] ConvertProduct(List<Tag> tags) {
            var item = Enumerable.Repeat(1f, tags.Select(t => t.Id).Count()).Zip(tags.Select(t => (uint)t.Id));
            var item1 = item.ToList();
            item1.Add((1f,1_000_000));
            return item1.ToArray();
        }

        private (float, uint)[] ConvertPreferences(User user) {
            long sum = user.UserTags.Sum(t => t.Count);
            return user.UserTags.Select(t => t.Count / (float)sum).Zip(user.UserTags.Select(t => (uint)t.Tag.Id)).ToArray();
        }

        private (float, uint)[] ConvertTokens(IEnumerable<int> tokens)
        {
            return Enumerable.Repeat(1f, tokens.Count()).Zip(tokens.Select(t => (uint)t)).ToArray();
        }

        private Dictionary<string, Value> ConvertToPayload(Product product) {
            var payload = new Dictionary<string, Value>
            {
                { "category", product.Category.Id }
            };
            foreach (var tag in product.Tags)
            {
                payload.Add(tag.Id.ToString(), tag.Value);
            }
            return payload;
        }

        public Task<bool> Update(int id, Product products)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
