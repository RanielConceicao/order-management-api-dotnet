using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.DTOs.Common;
using OrderManagement.Application.Interfaces;
using FluentValidation;
using System.Net;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IValidator<CreatePedidoDto> _createValidator;
    private readonly IValidator<UpdatePedidoDto> _updateValidator;

    public PedidosController(
        IPedidoService pedidoService,
        IValidator<CreatePedidoDto> createValidator,
        IValidator<UpdatePedidoDto> updateValidator)
    {
        _pedidoService = pedidoService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoDetalhesDto>>> GetAll()
    {
        var pedidos = await _pedidoService.GetAllWithDetailsAsync();
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PedidoDetalhesDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PedidoDetalhesDto>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("ID do pedido deve ser maior que zero.");

        var pedido = await _pedidoService.GetByIdWithDetailsAsync(id);
        
        if (pedido == null)
            return NotFound($"Pedido com ID {id} não foi encontrado.");

        return Ok(pedido);
    }

    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<PedidoDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetByClienteId(int clienteId)
    {
        if (clienteId <= 0)
            return BadRequest("ID do cliente deve ser maior que zero.");

        var pedidos = await _pedidoService.GetByClienteIdAsync(clienteId);
        return Ok(pedidos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PedidoDto>> Create([FromBody] CreatePedidoDto dto)
    {
        if (dto == null)
            return BadRequest("Dados do pedido são obrigatórios.");

        if (dto.Itens == null || !dto.Itens.Any())
            return BadRequest("Pedido deve conter pelo menos um item.");

        if (dto.Itens.Any(i => i.Quantidade <= 0))
            return BadRequest("Todos os itens devem ter quantidade maior que zero.");

        var validationResult = await _createValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var pedido = await _pedidoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PedidoDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PedidoDto>> Update(int id, [FromBody] UpdatePedidoDto dto)
    {
        if (id <= 0)
            return BadRequest("ID do pedido deve ser maior que zero.");

        if (dto == null)
            return BadRequest("Dados do pedido são obrigatórios.");

        if (dto.Itens == null || !dto.Itens.Any())
            return BadRequest("Pedido deve conter pelo menos um item.");

        if (dto.Itens.Any(i => i.Quantidade <= 0))
            return BadRequest("Todos os itens devem ter quantidade maior que zero.");

        var validationResult = await _updateValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var pedido = await _pedidoService.UpdateAsync(id, dto);
        return Ok(pedido);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("ID do pedido deve ser maior que zero.");

        await _pedidoService.DeleteAsync(id);
        return NoContent();
    }
} 
