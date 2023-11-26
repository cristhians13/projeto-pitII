using Catalogo.Models;
using Catalogo.Repositorys;

namespace Catalogo.Services
{
    public class ProdutoServices : IProdutoServices
    {
        private readonly IProdutoRepositorys _produtoRepositorys;
        public ProdutoServices(IProdutoRepositorys produtoRepositorys)
        {
            _produtoRepositorys = produtoRepositorys;
        }

        public void DeleteItemCarrinho(long id)
        {
            _produtoRepositorys.DeleteItemCarrinho(id);
        }

        public void DeleteItensCarrinho(long carrinho_id)
        {
            _produtoRepositorys.DeleteItensCarrinho(carrinho_id);
        }

        public Carrinho GetCarrinho(long id)
        {
            return _produtoRepositorys.GetCarrinho(id);
        }

        public Produto GetProduto(int produtoId)
        {
            return _produtoRepositorys.GetProduto(produtoId);
        }

        public List<Produto> GetProdutos(string termoBusca)
        {
            return _produtoRepositorys.GetProdutos(termoBusca);
        }

        public void InsertItemCarrinho(ItemCarrinho itemCarrinho)
        {
            _produtoRepositorys.InsertItemCarrinho(itemCarrinho);
        }

        public bool SaveProduto(Produto produto)
        {
            return _produtoRepositorys.SaveProduto(produto);
        }

        public void UpdateItemCarrinho(ItemCarrinho item)
        {
            _produtoRepositorys.UpdateItemCarrinho(item);
        }
    }
}
