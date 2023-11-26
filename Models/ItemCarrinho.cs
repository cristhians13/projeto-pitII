namespace Catalogo.Models
{
    public class ItemCarrinho
    {
        public long id { get; set; }
        public long quantidade { get; set; }
        public long produto_id { get; set; }
        public long carrinho_id { get; set; }
        public decimal preco { get; set; }  
        public string? nome { get; set; }

        public ItemCarrinho() { }
    }
}
