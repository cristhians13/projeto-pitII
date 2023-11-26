namespace Catalogo.Models
{
    public class Produto
    {
        public long id { get; set; }
        public string? nome { get; set; }
        public string? imagem { get; set; }
        public decimal preco { get; set; }
        public string? sku { get; set; }
        public string? descricao { get; set; }

    }
}
