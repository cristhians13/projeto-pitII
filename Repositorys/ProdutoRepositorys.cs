using Catalogo.DbContexts;
using Catalogo.Models;
using Dapper;

namespace Catalogo.Repositorys
{
    public class ProdutoRepositorys : IProdutoRepositorys
    {
        ContextCatalogo _contextCatalogo = new ContextCatalogo();

        public Carrinho GetCarrinho(long id)
        {
            try
            {
                string query = @"SELECT  ic.id,
                                         ic.quantidade,
                                         ic.produto_id,
                                         ic.carrinho_id,
                                         p.preco,
                                         p.nome
                                FROM item_carrinho ic inner join produto  p on ic.produto_id = p.id
                                WHERE ic.carrinho_id = @id";
                _contextCatalogo.GetConnection();
                List<ItemCarrinho> itens=  _contextCatalogo.Connection.Query<ItemCarrinho>(query, new {id}).ToList();
                Carrinho carrinho = new(){
                    CarrinhoId = id,
                    ItemsCarrinho = itens
                };


                return carrinho;
            }
            catch(Exception ex)
            {
                throw new Exception("Erro método GetCarrinho ex: " + ex.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public void DeleteItemCarrinho(long id)
        {
            try
            {
                string query = @"DELETE FROM item_carrinho WHERE id = @id";
                _contextCatalogo.GetConnection();
                _contextCatalogo.Connection.Execute(query, new { id });

            }
            catch (Exception ex)
            {
                throw new Exception("Erro método DeleteItemCarrinho ex: " + ex.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public void DeleteItensCarrinho(long carrinho_id)
        {
            try
            {
                string query = @"DELETE FROM item_carrinho WHERE carrinho_id = @carrinho_id";
                _contextCatalogo.GetConnection();
                _contextCatalogo.Connection.Execute(query, new { carrinho_id });

            }
            catch (Exception ex)
            {
                throw new Exception("Erro método DeleteItemCarrinho ex: " + ex.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public void UpdateItemCarrinho(ItemCarrinho item)
        {
            try
            {
                string query = @"UPDATE item_carrinho
                                        SET quantidade=@quantidade
                                        WHERE id=@id;";
                _contextCatalogo.GetConnection();
                _contextCatalogo.Connection.Execute(query, new { item.id , item.quantidade });

            }
            catch (Exception ex)
            {
                throw new Exception("Erro método UpdateItemCarrinho ex: " + ex.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public void InsertItemCarrinho(ItemCarrinho itemCarrinho)
        {
            try
            {
                string query = @"INSERT INTO item_carrinho
                                (id,quantidade, produto_id, carrinho_id)
                                VALUES(@id, @quantidade, @produto_id, @carrinho_id)
                                ON DUPLICATE KEY UPDATE quantidade = quantidade + @quantidade";
                _contextCatalogo.GetConnection();
                var id  = _contextCatalogo.Connection.QueryFirstOrDefault<long?>("SELECT id FROM item_carrinho WHERE produto_id = @produto_id AND carrinho_id = @carrinho_id "
                   , new { itemCarrinho.produto_id  , itemCarrinho.carrinho_id });

                _contextCatalogo.Connection.Execute(query, new { itemCarrinho.quantidade , itemCarrinho.produto_id , itemCarrinho.carrinho_id , id });

            }
            catch (Exception ex)
            {
                throw new Exception("Erro método InsertItemCarrinho ex: " + ex.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public Produto GetProduto(int produtoId)
        {
            try
            {
                _contextCatalogo.GetConnection();
                Produto produto = _contextCatalogo.Connection.QueryFirstOrDefault<Produto>("SELECT * FROM produto WHERE id=@produtoId", new { produtoId });
                return produto;
            }
            catch(Exception e) 
            {
                throw new Exception("Erro método GetProduto ex: "+ e.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public List<Produto> GetProdutos(string termoBusca)
        {
            try
            {
                string sql = @"
                                SELECT * 
                                FROM produto
                            ";

                if (!string.IsNullOrEmpty(termoBusca))
                    sql += " WHERE LOWER(nome) LIKE LOWER(@termoBusca) ";


                List<Produto> produtos = new();
                _contextCatalogo.GetConnection();
                if (!string.IsNullOrEmpty(termoBusca))
                    // Execução da consulta utilizando Dapper
                    produtos  = _contextCatalogo.Connection.Query<Produto>(sql, new { termoBusca = "%" + termoBusca + "%"}).ToList();
                else
                    produtos = _contextCatalogo.Connection.Query<Produto>(sql).ToList();

                return produtos;

            }
            catch (Exception e)
            {
                throw new Exception("Erro método GetProdutos ex: " + e.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }

        public bool SaveProduto(Produto produto)
        {
            try
            {
                string query = @"INSERT INTO produto
                                (nome, imagem, preco,sku, descricao)
                                VALUES( @nome, @imagem, @preco, @sku, @descricao);";
                _contextCatalogo.GetConnection();
                var rowsAf = _contextCatalogo.Connection.Execute(query, new {produto.nome,produto.preco,produto.imagem, produto.sku, produto.descricao});
                return rowsAf > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Erro método SaveProduto ex: " + e.Message);
            }
            finally
            {
                _contextCatalogo.Close();
            }
        }
    }
}
