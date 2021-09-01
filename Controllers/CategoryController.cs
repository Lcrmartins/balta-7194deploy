using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{

    // Endpoint => URL
    // https://localhost/5001/categories
    // http://localhost/5000/categories
    // https://meuapp.azurewebsites.net/categories
    // v1/ versionamento da API

    [Route("v1/categories")]
    public class CategoryController: ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        // opção para a inclusão de cache por método:
        [ResponseCache(VaryByHeader ="User-Agent", Location = ResponseCacheLocation.Any, Duration =30)]
        // caso inclua o cache geral no startup.cs, vc pode optar por excluir métodos específicos, assim:
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore =true)]
        public async Task<ActionResult<List<Category>>> GetAsync
        (
            [FromServices]DataContext context
        )
        {   // Importante -> usar o AsNoTracking() caso não necessite manipular mais informações sobre os dados, só mostrá-los.
            // Importante -> usar o ToListAsync() no final da query, não no meio!
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")] // :int especifica que o servidor deverá somente aceitar o tipo indicado (int).
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetByIdAsync
        (
            int id,
            [FromServices]DataContext context
        )
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> PostAsync
        (
            [FromBody]Category model,
            [FromServices]DataContext context
        )
        {
            // verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();            
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Não foi possível criar a categoria." + ex});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> PutAsync
        (
            int id, 
            [FromBody]Category model,
            [FromServices]DataContext context        
        )
        {
            // verifica se o id informado é o mesmo do model
            if (id != model.Id)
                return NotFound(new {message = "Categoria não encontrada."});
            // verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new {message = "Este registro já foi atualizado." + ex});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Não foi possível atualizar a categoria." + ex});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> DeleteAsync
        (
            int id,
            [FromServices]DataContext context
            )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null)
                return NotFound(new { message = "Categoria não encontrada."});
            
            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new{ message = $"Categoria {category.Title} foi removida com sucesso."});
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = "Não foi possível remover a Categoria." + ex});
            }
        }
    }
}