using System.Text.Json;
using HTMLActionDemo.Models;

namespace HTMLActionDemo.Services;

public class JsonUserRepository
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly string _path;
    private readonly object _lock = new();

    public JsonUserRepository(IWebHostEnvironment env)
    {
        var dir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "users.json");
    }

    private List<User> Load()
    {
        if (!File.Exists(_path)) return new List<User>();
        return JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_path)) ?? new List<User>();
    }

    private void Save(List<User> users) => File.WriteAllText(_path, JsonSerializer.Serialize(users, Options));

    public List<User> GetAll() { lock (_lock) return Load(); }

    public User? Get(int id) { lock (_lock) return Load().FirstOrDefault(u => u.Id == id); }

    public User Add(User user)
    {
        lock (_lock)
        {
            var users = Load();
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            Save(users);
            return user;
        }
    }

    public bool Update(int id, User user)
    {
        lock (_lock)
        {
            var users = Load();
            var i = users.FindIndex(u => u.Id == id);
            if (i < 0) return false;
            user.Id = id;
            users[i] = user;
            Save(users);
            return true;
        }
    }

    public User? SetActive(int id, bool active)
    {
        lock (_lock)
        {
            var users = Load();
            var u = users.FirstOrDefault(x => x.Id == id);
            if (u == null) return null;
            u.IsActive = active;
            Save(users);
            return u;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var users = Load();
            if (users.RemoveAll(u => u.Id == id) == 0) return false;
            Save(users);
            return true;
        }
    }
}
