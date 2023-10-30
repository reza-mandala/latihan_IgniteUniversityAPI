using MyIgniteApi.Models;

public static class TaskService
{
    private static Dictionary<int, Task<Dictionary<string, List<University>>>> _tasks = new();

    public static int CreateNewTask(Func<Task<Dictionary<string, List<University>>>> taskFunc)
    {
        var task = Task.Run(taskFunc);
        _tasks[task.Id] = task;
        return task.Id;
    }

    public static Task<Dictionary<string, List<University>>>? GetTask(int id)
    {
        if (_tasks != null && _tasks.Count() > 0)
        {
            _tasks.TryGetValue(id, out var task);
            return task;
        }
        return null;
    }
}
