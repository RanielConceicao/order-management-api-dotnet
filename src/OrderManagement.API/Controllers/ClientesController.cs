using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using FluentValidation;
using System.Net;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IValidator<CreateClienteDto> _createValidator;
    private readonly IValidator<UpdateClienteDto> _updateValidator;

    public ClientesController(
        IClienteService clienteService,
        IValidator<CreateClienteDto> createValidator,
        IValidator<UpdateClienteDto> updateValidator)
    {
        _clienteService = clienteService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        var cliente = await _clienteService.GetByIdAsync(id);
        
        if (cliente == null)
            return NotFound($"Cliente com ID {id} não foi encontrado.");

        return Ok(cliente);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CreateClienteDto dto)
    {
        if (dto == null)
            return BadRequest("Dados do cliente são obrigatórios.");

        var validationResult = await _createValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var cliente = await _clienteService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClienteDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        if (dto == null)
            return BadRequest("Dados do cliente são obrigatórios.");

        var validationResult = await _updateValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var cliente = await _clienteService.UpdateAsync(id, dto);
        return Ok(cliente);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        await _clienteService.DeleteAsync(id);
        return NoContent();
    }
} 
