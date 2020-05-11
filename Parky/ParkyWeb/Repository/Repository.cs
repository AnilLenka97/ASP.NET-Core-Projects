using ParkyWeb.Repository.IRepository;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Repository(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        public object JsonConvert { get; private set; }

        public Task<bool> CreateAsync(string url, T objToCreate)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if(objToCreate != null)
            {
                request.Content = new StringContent(JsonConvert.
            }
        }

        public Task<bool> DeleteAsync(string url, int Id)
        {
            throw new System.NotImplementedException();
        }

        public Task<System.Collections.Generic.IEnumerable<T>> GetAllAsync(string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> GetAsync(string url, int Id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateAsync(string url, T objToUpdate)
        {
            throw new System.NotImplementedException();
        }
    }
}
