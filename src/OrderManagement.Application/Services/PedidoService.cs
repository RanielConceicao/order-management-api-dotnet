using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PedidoService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PedidoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando pedido com ID: {PedidoId}", id);
        
        var pedidoEntity = await _unitOfWork.Pedidos.GetByIdAsync(id, cancellationToken);
        
        if (pedidoEntity == null)
        {
            _logger.LogWarning("Pedido com ID {PedidoId} não encontrado", id);
            return null;
        }

        return _mapper.Map<PedidoDto>(pedidoEntity);
    }

    public async Task<PedidoDetalhesDto?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando pedido com detalhes. ID: {PedidoId}", id);
        
        var pedidoEntity = await _unitOfWork.Pedidos.GetByIdWithDetailsAsync(id, cancellationToken);
        
        if (pedidoEntity == null)
        {
            _logger.LogWarning("Pedido com ID {PedidoId} não encontrado", id);
            return null;
        }

        return _mapper.Map<PedidoDetalhesDto>(pedidoEntity);
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando todos os pedidos");
        
        var pedidosEntity = await _unitOfWork.Pedidos.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PedidoDto>>(pedidosEntity);
    }

    public async Task<IEnumerable<PedidoDetalhesDto>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando todos os pedidos com detalhes");
        
        var pedidosEntity = await _unitOfWork.Pedidos.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PedidoDetalhesDto>>(pedidosEntity);
    }

    public async Task<PedidoDto> CreateAsync(CreatePedidoDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando novo pedido para cliente ID: {ClienteId}", dto.ClienteId);

        // primeiro: validações sem iniciar transação
        var clienteExists = await _unitOfWork.Clientes.ExistsAsync(dto.ClienteId, cancellationToken);
        if (!clienteExists)
        {
            _logger.LogWarning("Tentativa de criar pedido com cliente inexistente. ClienteId: {ClienteId}", dto.ClienteId);
            throw new KeyNotFoundException($"Cliente com ID {dto.ClienteId} não encontrado.");
        }

        // Agrupa quantidades por produto (somar duplicatas no mesmo pedido)
        var quantidadePorProduto = dto.Itens
            .GroupBy(i => i.ProdutoId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantidade));

        // Buscar todos os produtos necessários em batch
        var produtos = (await _unitOfWork.Produtos.GetByIdsAsync(quantidadePorProduto.Keys, cancellationToken)).ToDictionary(p => p.Id);

        // Validar existência/estoque
        foreach (var kv in quantidadePorProduto)
        {
            var produtoId = kv.Key;
            var quantidadeSolicitada = kv.Value;

            if (!produtos.TryGetValue(produtoId, out var produtoEntity))
            {
                _logger.LogWarning("Produto ID {ProdutoId} não encontrado ao criar pedido", produtoId);
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            }

            if (produtoEntity.Estoque < quantidadeSolicitada)
            {
                _logger.LogWarning("Estoque insuficiente para produto ID {ProdutoId}. Disponível: {Estoque}, Solicitado: {Quantidade}", 
                    produtoId, produtoEntity.Estoque, quantidadeSolicitada);
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produtoEntity.Nome}'. Disponível: {produtoEntity.Estoque}, Solicitado: {quantidadeSolicitada}");
            }
        }

        // Tudo validado — iniciar transação para aplicar alterações
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var itensPedido = new List<ItemPedido>();

            foreach (var itemDto in dto.Itens)
            {
                var produtoEntity = produtos[itemDto.ProdutoId];

                // aplicar alteração de estoque em memória e persistir
                produtoEntity.ReduceStock(itemDto.Quantidade);
                await _unitOfWork.Produtos.UpdateAsync(produtoEntity, cancellationToken);

                var itemPedido = new ItemPedido(itemDto.ProdutoId, itemDto.Quantidade, produtoEntity.Preco);
                itensPedido.Add(itemPedido);
            }

            var pedidoEntity = new Pedido(dto.ClienteId, itensPedido);
            var createdEntity = await _unitOfWork.Pedidos.AddAsync(pedidoEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Pedido criado com sucesso. ID: {PedidoId}, Total: {Total}", 
                createdEntity.Id, createdEntity.Total);

            return _mapper.Map<PedidoDto>(createdEntity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PedidoDto> UpdateAsync(int id, UpdatePedidoDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando pedido ID: {PedidoId}", id);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {

            var pedidoEntity = await _unitOfWork.Pedidos.GetByIdWithDetailsAsync(id, cancellationToken);
            
            if (pedidoEntity == null)
            {
                _logger.LogWarning("Pedido com ID {PedidoId} não encontrado para atualização", id);
                throw new KeyNotFoundException($"Pedido com ID {id} não encontrado.");
            }

            foreach (var itemAntigo in pedidoEntity.Itens)
            {
                var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(itemAntigo.ProdutoId, cancellationToken);
                if (produtoEntity != null)
                {
                    produtoEntity.IncreaseStock(itemAntigo.Quantidade);
                    await _unitOfWork.Produtos.UpdateAsync(produtoEntity, cancellationToken);
                }
            }

            var novosItens = new List<ItemPedido>();
            
            foreach (var itemDto in dto.Itens)
            {
                var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(itemDto.ProdutoId, cancellationToken);
                
                if (produtoEntity == null)
                {
                    throw new KeyNotFoundException($"Produto com ID {itemDto.ProdutoId} não encontrado.");
                }

                if (produtoEntity.Estoque < itemDto.Quantidade)
                {
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para o produto '{produtoEntity.Nome}'. Disponível: {produtoEntity.Estoque}, Solicitado: {itemDto.Quantidade}");
                }

                produtoEntity.ReduceStock(itemDto.Quantidade);
                await _unitOfWork.Produtos.UpdateAsync(produtoEntity, cancellationToken);

                var itemPedido = new ItemPedido(itemDto.ProdutoId, itemDto.Quantidade, produtoEntity.Preco);
                novosItens.Add(itemPedido);
            }

            pedidoEntity.UpdateItems(novosItens);
            await _unitOfWork.Pedidos.UpdateAsync(pedidoEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Pedido ID {PedidoId} atualizado com sucesso", id);
            return _mapper.Map<PedidoDto>(pedidoEntity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deletando pedido ID: {PedidoId}", id);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {

            var pedidoEntity = await _unitOfWork.Pedidos.GetByIdWithDetailsAsync(id, cancellationToken);
            
            if (pedidoEntity == null)
            {
                _logger.LogWarning("Pedido com ID {PedidoId} não encontrado para exclusão", id);
                throw new KeyNotFoundException($"Pedido com ID {id} não encontrado.");
            }

            foreach (var item in pedidoEntity.Itens)
            {
                var produtoEntity = await _unitOfWork.Produtos.GetByIdAsync(item.ProdutoId, cancellationToken);
                if (produtoEntity != null)
                {
                    produtoEntity.IncreaseStock(item.Quantidade);
                    await _unitOfWork.Produtos.UpdateAsync(produtoEntity, cancellationToken);
                }
            }

            await _unitOfWork.Pedidos.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Pedido ID {PedidoId} deletado com sucesso", id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<PedidoDto>> GetByClienteIdAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Buscando pedidos do cliente ID: {ClienteId}", clienteId);
        
        var pedidosEntity = await _unitOfWork.Pedidos.GetByClienteIdAsync(clienteId, cancellationToken);
        return _mapper.Map<IEnumerable<PedidoDto>>(pedidosEntity);
    }
}
