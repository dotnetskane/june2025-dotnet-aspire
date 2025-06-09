namespace AspireStarter.Web;

public class TodoApiClient(HttpClient httpClient)
{
    public async Task<List<TodoItem>> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        var todos = await httpClient.GetFromJsonAsync<List<TodoItem>>("/todos", cancellationToken) 
            ?? new List<TodoItem>();
        return todos;
    }

    public async Task<TodoItem?> GetTodoAsync(string id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<TodoItem>($"/todos/{id}", cancellationToken);
    }

    public async Task<TodoItem?> CreateTodoAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/todos", todo, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItem>(cancellationToken: cancellationToken);
    }

    public async Task<bool> UpdateTodoAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/todos/{todo.Id}", todo, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTodoAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/todos/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}