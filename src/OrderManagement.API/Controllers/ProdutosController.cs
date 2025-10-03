using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using FluentValidation;
using System.Net;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly IValidator<CreateProdutoDto> _createValidator;
    private readonly IValidator<UpdateProdutoDto> _updateValidator;

    public ProdutosController(
        IProdutoService produtoService,
        IValidator<CreateProdutoDto> createValidator,
        IValidator<UpdateProdutoDto> updateValidator)
    {
        _produtoService = produtoService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ProdutoDto>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        var produto = await _produtoService.GetByIdAsync(id);
        
        if (produto == null)
            return NotFound($"Produto com ID {id} não foi encontrado.");

        return Ok(produto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Create([FromBody] CreateProdutoDto dto)
    {
        if (dto == null)
            return BadRequest("Dados do produto são obrigatórios.");

        var validationResult = await _createValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var produto = await _produtoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Update(int id, [FromBody] UpdateProdutoDto dto)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        if (dto == null)
            return BadRequest("Dados do produto são obrigatórios.");

        var validationResult = await _updateValidator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => new { 
                Field = e.PropertyName, 
                Error = e.ErrorMessage 
            }));

        var produto = await _produtoService.UpdateAsync(id, dto);
        return Ok(produto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("ID deve ser maior que zero.");

        await _produtoService.DeleteAsync(id);
        return NoContent();
    }
} 
