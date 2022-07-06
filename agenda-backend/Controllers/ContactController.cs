using agenda_backend.Models;
using agenda_backend.Models.Data;
using agenda_backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace agenda_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly myagendaContext _context;
        public ContactController(myagendaContext context)
        {
            this._context = context;
        }


        // GET: api/<ContactController>
        [HttpGet]
        public async Task<ActionResult<object>> GetAsync()
        {
            genericJsonResponse response = new();
            List<Directorio> contacts = new();
            try
            {
                contacts = await _context.Directorios.ToListAsync();
                response.success = true;
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
            }
            finally
            {
                response.data = contacts;
            }
            return response;
        }



        // POST api/<ContactController>
        [HttpPost]
        public async Task<ActionResult<object>> PostAsync([FromBody] genericJsonRequest request)
        {
            genericJsonResponse response = new();
            try
            {
                Directorio contact;
                Directorio contactParsed = JSON.Parse<Directorio>(request.stringify);
                response.success = true;
                switch (request.operation)
                {
                    case CONSTANT.CREATE:
                        await _context.Directorios.AddAsync(contactParsed);
                        response.message = "created: done";
                        break;

                    case CONSTANT.EDIT:
                        contact = await _context.Directorios.FindAsync(contactParsed.Id);
                        contact.Nombre = contactParsed.Nombre;
                        contact.Telefono = contactParsed.Telefono;
                        _context.Entry(contact).State = EntityState.Modified;
                        response.message = "edited: done";
                        break;

                    case CONSTANT.DELETE:
                        contact = await _context.Directorios.FindAsync(contactParsed.Id);
                        _context.Entry(contact).State = EntityState.Deleted;
                        response.message = "deleted: done";
                        break;

                    default:
                        response.success = false;
                        break;
                }
                if (response.success) await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
            }
            return response;
        }
    }
}
