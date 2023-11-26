using Catalogo.Models;

namespace Catalogo.Services
{
    public interface IProdutoServices
    {
        List<Produto> GetProdutos(string termoBusca);
        Produto GetProduto(int produtoId);
        bool SaveProduto(Produto produto);
        Carrinho GetCarrinho(long id);
        void DeleteItemCarrinho(long id);
        void DeleteItensCarrinho(long carrinho_id);
        void UpdateItemCarrinho(ItemCarrinho item);
        void InsertItemCarrinho(ItemCarrinho itemCarrinho);
    }
}
