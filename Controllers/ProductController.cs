using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("products")]

    public class ProductController: ControllerBase
    {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetAsync
        (
            [FromServices]DataContext context
        )
        {   // Importante -> usar o AsNoTracking() caso não necessite manipular mais informações sobre os dados, só mostrá-los.
            // Importante -> usar o ToListAsync() no final da query, não no meio!
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")] // :int especifica que o servidor deverá somente aceitar o tipo indicado (int).
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetByIdAsync
        (
            int id,
            [FromServices]DataContext context
        )
        {
            var product = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(product);
        }


        [HttpGet] //products/categories/1
        [Route("categories/{id:int}")] // :int especifica que o servidor deverá somente aceitar o tipo indicado (int).
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategoryAsync
        (
            int id,
            [FromServices]DataContext context
        )
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();
                
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> PostAsync
        (
            [FromBody]Product model,
            [FromServices]DataContext context
        )
        {
            // verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();            
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Não foi possível criar o produto." + ex});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> PutAsync
        (
            int id, 
            [FromBody]Product model,
            [FromServices]DataContext context        
        )
        {
            // verifica se o id informado é o mesmo do model
            if (id != model.Id)
                return NotFound(new {message = "Produto não encontrado."});
            // verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new {message = "Este registro já foi atualizado." + ex});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Não foi possível atualizar o produto." + ex});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> DeleteAsync
        (
            int id,
            [FromServices]DataContext context
            )
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
                return NotFound(new { message = "Produto não encontrado."});
            
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new{ message = $"Produto {product.Title} foi removido com sucesso."});
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = "Não foi possível remover o Produto." + ex});
            }
        }
        
    }
}