using HTMLActionDemo.Models;
using HTMLActionDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace HTMLActionDemo.Controllers;

public class UsersController : Controller
{
    private readonly JsonUserRepository _repo;

    public UsersController(JsonUserRepository repo) => _repo = repo;

    // GET /Users
    [HttpGet]
    public IActionResult Index() => View(_repo.GetAll());

    // GET /Users/Details/5
    [HttpGet]
    public IActionResult Details(int id)
    {
        var user = _repo.Get(id);
        return user == null ? NotFound() : View(user);
    }

    // GET /Users/Create (show form)
    [HttpGet]
    public IActionResult Create() => View(new User());

    // POST /Users/Create (create new user)
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(User user)
    {
        if (!ModelState.IsValid) return View(user);
        _repo.Add(user);
        return RedirectToAction(nameof(Index));
    }

    // GET /Users/Edit/5 (show form)
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = _repo.Get(id);
        return user == null ? NotFound() : View(user);
    }

    // PUT /Users/Edit/5 (replace whole user; called with fetch)
    [HttpPut("Users/Edit/{id:int}"), ValidateAntiForgeryToken]
    public IActionResult Replace(int id, [FromBody] User user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return _repo.Update(id, user) ? Ok(user) : NotFound();
    }

    // PATCH /Users/Status/5?active=true (partial update of one attribute; called with fetch)
    [HttpPatch("Users/Status/{id:int}"), ValidateAntiForgeryToken]
    public IActionResult SetStatus(int id, [FromQuery] bool active)
    {
        var user = _repo.SetActive(id, active);
        return user == null ? NotFound() : Ok(user);
    }

    // DELETE /Users/Delete/5 (called with fetch)
    [HttpDelete("Users/Delete/{id:int}"), ValidateAntiForgeryToken]
    public IActionResult Remove(int id) => _repo.Delete(id) ? NoContent() : NotFound();

    // HEAD /Users/Exists/5 (status code only, no body)
    [HttpHead("Users/Exists/{id:int}")]
    public IActionResult Exists(int id) => _repo.Get(id) == null ? NotFound() : Ok();

    // OPTIONS /Users/Exists/5 (lists supported verbs)
    [HttpOptions("Users/Exists/{id:int}")]
    public IActionResult ExistsOptions(int id)
    {
        Response.Headers.Allow = "GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS";
        return NoContent();
    }
}
