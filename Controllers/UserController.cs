using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController: ControllerBase
    {
        
        [HttpGet]
        [Route("")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<List<User>>> GetAsync 
        (
            [FromServices] DataContext context
        )
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }


        
        
        
        
        [HttpPost]
        [Route("")]
        [AllowAnonymous]        
        public async Task<ActionResult<User>> PostAsync
        (
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            // verificar se os dados são válidos
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                // força o usuário a ser sempre "funcionário"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();
                // Esconde a senha no retorno
                model.Password = "";
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = "Não foi possível criar o usuário." + ex});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> PutAsync
        (
            [FromServices] DataContext context,
            int id,
            [FromBody] User model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado." });
            
            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Não foi possível alterar o usuário" + ex });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync
        (
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();
            if (user is null)
                return NotFound(new { message = "Usuário ou senha inválidos." });

            var token = TokenService.GenerateToken(user);
            // Esconde a senha no retorno
            user.Password = "";

            return Ok(new
            {
                user = user,
                token = token
            });
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> DeleteAsync
        (
            int id,
            [FromServices]DataContext context
            )
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
                return NotFound(new { message = "Usuário não encontrado."});
            
            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new{ message = $"Usuário {user.Username} foi removido com sucesso."});
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = "Não foi possível remover o Usuário." + ex});
            }
        }

        // [HttpGet]
        // [Route("anonimo")]
        // [AllowAnonymous]
        // public string Anonimo() => "Anonimo";

        // [HttpGet]
        // [Route("autenticado")]
        // [Authorize]
        // public string Autenticado() => "Autenticado";

        // [HttpGet]
        // [Route("funcionario")]
        // [Authorize(Roles="employee")]
        // public string Funcionario() => "Funcionario";

        // [HttpGet]
        // [Route("gerente")]
        // [Authorize(Roles="manager")]
        // public string Gerente() => "Gerente";
    }
}