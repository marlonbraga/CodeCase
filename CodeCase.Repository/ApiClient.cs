using CodeCase.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CodeCase.Domain.Entities;

namespace CodeCase.Repository
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly IDistributedCache _cache;
        private readonly string _authToken;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient(
            IConfiguration configuration,
            IDistributedCache cache,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClient;
            _cache = cache;
            _authToken = configuration["AuthToken"];
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<DtoResponse>> GetDto(int dtoId)
        {
            var userData = JsonSerializer.Deserialize<UserData>(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.UserData)!, _jsonOptions)!;
            var requestUri = $"api/dto/getDto?dtoId={dtoId}";

            var cacheKey = requestUri;
            cacheKey = $"{cacheKey}_{userData.UserId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<DtoResponse>>(cached, _jsonOptions);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("token", userData.Token.ToString());

            var response = await _client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            await _cache.SetStringAsync(cacheKey, responseContent, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            var result = JsonSerializer.Deserialize<List<DtoResponse>>(responseContent, _jsonOptions);
            return result;
        }
    }
}
