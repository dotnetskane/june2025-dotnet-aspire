using Azure;
using Azure.Data.Tables;

namespace AspireStarter.ApiService;

public class TodoService
{
    private readonly TableClient _tableClient;

    public TodoService(TableServiceClient tableServiceClient)
    {
        tableServiceClient.CreateTableIfNotExists("todos");
        _tableClient = tableServiceClient.GetTableClient("todos");
    }

    public async Task<List<TodoEntity>> GetAllTodosAsync()
    {
        var todos = new List<TodoEntity>();
        var queryResults = _tableClient.QueryAsync<TodoEntity>();

        await foreach (var todo in queryResults)
        {
            todos.Add(todo);
        }

        return todos;
    }

    public async Task<TodoEntity?> GetTodoAsync(string id)
    {
        try
        {
            return await _tableClient.GetEntityAsync<TodoEntity>("todo", id);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<TodoEntity> CreateTodoAsync(TodoEntity todo)
    {
        todo.PartitionKey = "todo";
        todo.RowKey = string.IsNullOrEmpty(todo.RowKey) ? Guid.NewGuid().ToString() : todo.RowKey;
        todo.Timestamp = DateTimeOffset.UtcNow;

        await _tableClient.UpsertEntityAsync(todo);
        return todo;
    }

    public async Task UpdateTodoAsync(TodoEntity todo)
    {
        todo.PartitionKey = "todo";
        await _tableClient.UpdateEntityAsync(todo, ETag.All, TableUpdateMode.Replace);
    }

    public async Task DeleteTodoAsync(string id)
    {
        await _tableClient.DeleteEntityAsync("todo", id);
    }
}

public class TodoEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "todo";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}