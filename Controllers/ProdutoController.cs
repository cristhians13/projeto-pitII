using Catalogo.Models;
using Catalogo.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using PayPal;
using Catalogo.Models.PayPalTransaction;
using MySqlX.XDevAPI.Common;

namespace Catalogo.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IProdutoServices _produtoServices;
        private readonly string userName = "AYwlYSPxuARlKvmEkZbUt0iO8VGPhzvz9TC3dQ0MsSaZyRtX92dHpnMho1Vy2hr0EsFmV-mVw3Ilj6yk";
        private readonly string passwd = "EJ55aQp597XX20TTB2A8Zue6uyD-VgGUp29QvK1GJubgqCo2t7xesGRslbqpdPli7txm3H-16aM47HUk";
        public ProdutoController(IProdutoServices produtoServices)
        {
            _produtoServices = produtoServices;
        }
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Produto produto, IFormFile file)
        {
            try
            {

                // Fazer upload da imagem
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    produto.imagem = Path.Combine("images", fileName); 
                }

                _produtoServices.SaveProduto(produto);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) 
            {
                ViewData["MensagemErro"] = "Ocorreu um erro durante o processo." + ex.Message;
                return View(produto);
            }
        }

    
        public async Task<IActionResult> GetCarrinho(long id)
        {
            try
            {
                Carrinho carrinho= _produtoServices.GetCarrinho(id);
                return Json(new { success = true, data = carrinho });

            }
            catch (Exception ex)
            {
                ViewData["MensagemErro"] = "Ocorreu um erro durante get do carrinho." + ex.Message;
                return Json(new { success = false, message = "Carrinho não carregado!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItemCarrinho(ItemCarrinho item)
        {
            try
            {
                _produtoServices.UpdateItemCarrinho(item);
                return Json(new { success = true, message = "Item Atualizado!" });
            }
            catch (Exception ex)
            {
                ViewData["MensagemErro"] = "Ocorreu um erro durante update dos itens do carrinho." + ex.Message;
                return Json(new { success = false, message = "Item Não Atualizado!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItemCarrinho(long id)
        {
            try
            {
                _produtoServices.DeleteItemCarrinho(id);
                return Json(new { success = true, message = "Item Deletado!" });
            }
            catch (Exception ex)
            {
                ViewData["MensagemErro"] = "Ocorreu um erro durante a deleção do item." + ex.Message;
                return Json(new { success = false, message = "Item Não Deletado!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertItemCarrinho([FromBody] ItemCarrinho item)
        {
            try
            {
                _produtoServices.InsertItemCarrinho(item);
                return Json(new { success = true, message = "Item Inserido!" });
            }
            catch (Exception ex)
            {
                ViewData["MensagemErro"] = "Ocorreu um erro durante salvar o item." + ex.Message;
                return Json(new { success = false, message = "Item Não Inserido!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PaypalCreateOrder(long carrinho_id)
        {
            try
            {
                Carrinho carrinho = _produtoServices.GetCarrinho(carrinho_id);
                bool status = false;
                ContentOrdersPayPalResult? result = new();
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                    var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                    string amount = carrinho.ItemsCarrinho.Select(s => s.preco).ToList().Sum().ToString();
                    string products = string.Join(", ", carrinho.ItemsCarrinho.Select(s => s.nome + " - ( " + s.preco + " ) "));
                    var orden = new PaypalOrder()
                    {
                        intent = "CAPTURE",
                        purchase_units = new List<PurchaseUnit>() {

                        new PurchaseUnit() {
                            reference_id = Util.Util.GenerateRandomAlphanumericString(15),
                            amount = new Amount() {
                                currency_code = "BRL",
                                value = amount
                            },
                            description = products
                        }
                    },
                        application_context = new ApplicationContext()
                        {
                            brand_name = "Catalogo",
                            landing_page = "NO_PREFERENCE",
                            user_action = "PAY_NOW", //Action for paypal to show the payment amount
                            return_url = "http://localhost:8090/Home/PaymentConfirmed",// when the payment request was approved
                            cancel_url = "http://localhost:8090/Home/PaymentCanceled"// when you cancel the operation
                        }
                    };

                    var json = JsonSerializer.Serialize(orden);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);

                    status = response.IsSuccessStatusCode;

                    if (response.IsSuccessStatusCode)
                    {
                        result = JsonSerializer.Deserialize<ContentOrdersPayPalResult>(response.Content.ReadAsStringAsync().Result);
                    }
                }

                return Json(new { success = true, status, result });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }

        public async Task<IActionResult> CapturePaymentForOrder(string token)
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
                return Json(new { success = true ,status, idTransaction = transaction });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}

