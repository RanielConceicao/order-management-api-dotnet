using AutoMapper;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClienteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var clienteEntity = await _unitOfWork.Clientes.GetByIdAsync(id);
        return clienteEntity == null ? null : _mapper.Map<ClienteDto>(clienteEntity);
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientesEntity = await _unitOfWork.Clientes.GetAllAsync();
        return _mapper.Map<IEnumerable<ClienteDto>>(clientesEntity);
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        var existingCliente = await _unitOfWork.Clientes.GetByEmailAsync(dto.Email);
        if (existingCliente != null)
        {
            throw new InvalidOperationException($"Já existe um cliente cadastrado com o email {dto.Email}");
        }

        var clienteEntity = new Cliente(dto.Nome, dto.Email, dto.Telefone);
        var createdEntity = await _unitOfWork.Clientes.AddAsync(clienteEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ClienteDto>(createdEntity);
    }

    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var clienteEntity = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (clienteEntity == null)
        {
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
        }

        var existingCliente = await _unitOfWork.Clientes.GetByEmailAsync(dto.Email);
        if (existingCliente != null && existingCliente.Id != id)
        {
            throw new InvalidOperationException($"Já existe outro cliente cadastrado com o email {dto.Email}");
        }

        clienteEntity.Update(dto.Nome, dto.Email, dto.Telefone);
        await _unitOfWork.Clientes.UpdateAsync(clienteEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ClienteDto>(clienteEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var clienteEntity = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (clienteEntity == null)
        {
            throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
        }

        await _unitOfWork.Clientes.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
} 
