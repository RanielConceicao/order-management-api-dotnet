using AutoMapper;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Produto mappings
        CreateMap<Produto, ProdutoDto>();
        CreateMap<CreateProdutoDto, Produto>()
            .ConstructUsing(src => new Produto(src.Nome, src.Preco, src.Descricao, src.Estoque));

        // Cliente mappings
        CreateMap<Cliente, ClienteDto>();
        CreateMap<CreateClienteDto, Cliente>()
            .ConstructUsing(src => new Cliente(src.Nome, src.Email, src.Telefone));

        // Pedido mappings
        CreateMap<Pedido, PedidoDto>();
        
        CreateMap<Pedido, PedidoDetalhesDto>()
            .ForMember(dest => dest.ClienteNome, opt => opt.MapFrom(src => src.Cliente.Nome))
            .ForMember(dest => dest.ClienteEmail, opt => opt.MapFrom(src => src.Cliente.Email))
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));

        // ItemPedido mappings
        CreateMap<ItemPedido, ItemPedidoDto>()
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome));
    }
}
