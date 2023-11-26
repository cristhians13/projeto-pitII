namespace Catalogo.Models
{
    public class ContentOrdersPayPalResult
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<Link> links { get; set; }
    }
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}
