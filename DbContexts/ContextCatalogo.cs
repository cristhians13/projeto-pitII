namespace Catalogo.DbContexts
{
    public class ContextCatalogo : ContextBase
    {
        public ContextCatalogo()
        {
            ConnectionString = Environment.GetEnvironmentVariable("CONECTION_STRING_CATALOGO") ?? "";
        }
    }
}
