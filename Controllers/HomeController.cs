using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController: ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> GetAsync(
            [FromServices] DataContext context)
        {
            var employee = new User(1, "robin", "robin", "employee");
            var manager = new User(2, "batman", "batman", "manager");
            var category = new Category(1, "Informática");
            var product = new Product(1, category,"Mouse", "Mouse gamer", 299, 1);
            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados."
            });
        }
        
    }
}