
using Microsoft.AspNetCore.Mvc;
using CustomerMvcDemo.Models;
using CustomerMvcDemo.ViewModels;

namespace CustomerMvcDemo.Controllers;

public class CustomerController : Controller
{
    private static List<Customer> customers = new();
    private static int nextId = 1;

    // GET: Display Create Form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Save Customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CustomerViewModel model)
    {
        if (ModelState.IsValid)
        {
            var customer = new Customer
            {
                Id = nextId++,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone
            };

            customers.Add(customer);

            return RedirectToAction("Edit",
                new { id = customer.Id });
        }

        return View(model);
    }

    // GET: Display Edit Form
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var customer = customers.FirstOrDefault(c => c.Id == id);

        if (customer == null)
            return NotFound();

        var model = new CustomerViewModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone
        };

        return View(model);
    }

    // POST: Update Customer
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CustomerViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var customer = customers.FirstOrDefault(c => c.Id == model.Id);

        if (customer == null)
            return NotFound();

        customer.Name = model.Name;
        customer.Email = model.Email;
        customer.Phone = model.Phone;

        return RedirectToAction("Edit",
            new { id = customer.Id });
    }
}
