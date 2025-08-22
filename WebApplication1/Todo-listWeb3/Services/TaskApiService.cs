using System.Text;
using System.Text.Json;
using Todo_listWeb3.Models;

namespace Todo_listWeb3.Services
{
    public class TaskApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TaskApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8000/");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<TaskDto>> GetTasksAsync(string? statusFilter = null, int? priorityFilter = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(statusFilter))
                queryParams.Add($"status_filter={Uri.EscapeDataString(statusFilter)}");
            if (priorityFilter.HasValue)
                queryParams.Add($"priority_filter={priorityFilter}");

            var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"tasks/{query}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TaskDto>>(json, _jsonOptions) ?? new List<TaskDto>();
        }

        public async Task<TaskDto?> GetTaskAsync(int id)
        {
            var response = await _httpClient.GetAsync($"tasks/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskDto>(json, _jsonOptions);
        }

        public async Task<TaskDto?> CreateTaskAsync(TaskCreateDto task)
        {
            var json = JsonSerializer.Serialize(task, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("tasks/", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskDto>(responseJson, _jsonOptions);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int id, TaskUpdateDto task)
        {
            var json = JsonSerializer.Serialize(task, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"tasks/{id}", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskDto>(responseJson, _jsonOptions);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"tasks/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Dictionary<string, object>> GetTasksByStatusAsync(string status)
        {
            var response = await _httpClient.GetAsync($"tasks/status/{Uri.EscapeDataString(status)}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json, _jsonOptions) ?? new Dictionary<string, object>();
        }
    }
}