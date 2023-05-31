using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
    {
        var customersToReturn = Data.Instance.Customers.Select(customer => new CustomerDto()
        {
            Id = customer.Id,
            Name = customer.Name,
            Cpf = customer.Cpf
        });
        return Ok(customersToReturn);
    }

    [HttpGet("{id}", Name = "GetCustomerById")]
    public ActionResult<CustomerDto> GetCustomerById (int id) 
    {
        Console.WriteLine($"id: {id}");
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Id == id);
        
        if(customerFromDatabase == null) return NotFound();

        CustomerDto customerToReturn = new CustomerDto()
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };
        return Ok(customerToReturn);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDto> GetCustomerByCpf (string cpf) 
    {
        Console.WriteLine($"cpf: {cpf}");
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(c => c.Cpf == cpf);

        if(customerFromDatabase == null) return NotFound();

        CustomerDto customerToReturn = new CustomerDto()
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };
        return Ok(customerToReturn);
    }

    [HttpPost]
    public ActionResult<CustomerDto> CreateCustomer (
        CustomerForCreationDto customerForCreationDto) 
    {
        var customerEntity = new Customer 
        {
            Id = Data.Instance.Customers.Max(c => c.Id)+1,
            Name = customerForCreationDto.Name,
            Cpf = customerForCreationDto.Cpf
        };

        Data.Instance.Customers.Add(customerEntity);

        var customerToReturn = new CustomerDto()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Cpf = customerEntity.Cpf
        };

        return CreatedAtRoute
        (
            "GetCustomerById",
            new {id = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCustomer (int id, 
        CustomerForUpdateDto customerForUpdateDto) 
    {
        if(id != customerForUpdateDto.Id) return BadRequest();
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer => customer.Id == id);

        if(customerFromDatabase == null) return NotFound();

        customerFromDatabase.Name = customerForUpdateDto.Name;
        customerFromDatabase.Cpf = customerForUpdateDto.Cpf;

        return NoContent();
    }
   
   [HttpDelete("{id}")]

   public ActionResult DeleteCustomer (int id) 
   {
        var customerFromDatabase = Data.Instance.Customers.FirstOrDefault(customer => customer.Id == id);

        if(customerFromDatabase == null) return NotFound();

        Data.Instance.Customers.Remove(customerFromDatabase);

        return NoContent();
   }
   
   //https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0 = AspNetCore.JsonPatch e AspNetCore.Mvc.NewtonsoftJson - MyJPIF.cs
   //Utiliza o JsonPatch para utilizar o metodo Patch do http
   //E o newtonsoftJson é um requerimento do pacote JsonPatch, pois o padrao é o System.text e precisa se instalar e configurar os pacotes
   //Configuração adicional no Program.cs

   [HttpPatch("{id}")]
   public ActionResult PartiallyUpdateCustomer (
    [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
    [FromRoute] int id)
    {
        var customerFromDatabase = Data.Instance.Customers
            .FirstOrDefault(customer => customer.Id == id);
        
        if(customerFromDatabase == null) return NotFound();

        var customerToPatch = new CustomerForPatchDto 
        {
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };

        patchDocument.ApplyTo(customerToPatch);

        customerFromDatabase.Name = customerToPatch.Name;
        customerFromDatabase.Cpf = customerToPatch.Cpf;

        return NoContent();
    }
}