using Catalogo.Models;
using Catalogo.Models.Pagination;
using Catalogo.Models.PayPalTransaction;
using Catalogo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Catalogo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProdutoServices _produtoServices;
        private const int itensPorPagina = 3;
        private readonly string userName = "AYwlYSPxuARlKvmEkZbUt0iO8VGPhzvz9TC3dQ0MsSaZyRtX92dHpnMho1Vy2hr0EsFmV-mVw3Ilj6yk";
        private readonly string passwd = "EJ55aQp597XX20TTB2A8Zue6uyD-VgGUp29QvK1GJubgqCo2t7xesGRslbqpdPli7txm3H-16aM47HUk";
        public HomeController(IProdutoServices produtoServices)
        {
            _produtoServices = produtoServices;
        }

        public IActionResult Index(string termoBusca , int page = 1)
        {
            var produtos = _produtoServices.GetProdutos(termoBusca);

            // Pagina os resultados
            var resultadoPaginado = new PagedList<Produto>(produtos, page, itensPorPagina, produtos.Count);

            // Mantém o termo de busca na ViewBag para exibição contínua
            ViewBag.TermoBusca = termoBusca;

            return View(resultadoPaginado);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PaymentCanceled()
        {
            return View();
        }

        public async Task<IActionResult> PaymentConfirmed(string token)
        {
            try
            {
                bool status = false;
                string? transaction = string.Empty;

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                    var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                    var data = new StringContent("{}", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);

                    status = response.IsSuccessStatusCode;

                    if (status)
                    {
                        var jsonRespuesta = response.Content.ReadAsStringAsync().Result;

                        PaypalTransaction? objeto = JsonSerializer.Deserialize<PaypalTransaction>(jsonRespuesta);

                        transaction = objeto?.purchase_units[0].payments.captures[0].id;
                    }
                }
                _produtoServices.DeleteItensCarrinho(1);
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}