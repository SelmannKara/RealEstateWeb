﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate_Dapper_UI.Dtos.ProductDetailDtos;
using RealEstate_Dapper_UI.Dtos.ProductDtos;

namespace RealEstate_Dapper_UI.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PropertyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7067/api/Products/ProductListWithCategory");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);
                return View(values);
            }
            return View();
        }
        public async Task<IActionResult> PropertyListWithSearch(string searchKeyValue,
            int propertyCategoryId, string city)
        {
            ViewBag.SearchKeyValue = TempData["searchKeyValue"];
            ViewBag.propertyCategoryId = TempData["propertyCategoryId"];
            ViewBag.city = TempData["city"];

			searchKeyValue = TempData["searchKeyValue"].ToString();
			propertyCategoryId = int.Parse(TempData["propertyCategoryId"].ToString());
			city = TempData["city"].ToString();
			var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7067/api/Products/ResultProductWithSearchList?searchKeyValue=" +
                $"{searchKeyValue}&propertyCategoryId={propertyCategoryId}&city={city}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductWithSearchListDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        [HttpGet("property/{slug}/{id}")]
        public async Task<IActionResult>PropertySingle(string slug,int id)
        {
                 ViewBag.i = id;
                var client = _httpClientFactory.CreateClient();    
                var responseMessage = await client.GetAsync("https://localhost:7067/api/Products/GetProductByProductId?id="+id);
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<ResultProductDto>(jsonData);

                var client2 = _httpClientFactory.CreateClient();
                var responseMessage2 = await client2.GetAsync("https://localhost:7067/api/ProductDetails/GetProductDetailByProductId?id=" + id);
                var jsonData2 = await responseMessage2.Content.ReadAsStringAsync();
                var values2 = JsonConvert.DeserializeObject<GetProductDetailByIdDto>(jsonData2);

                ViewBag.productId = values.productID.ToString();
                ViewBag.title1 = values.title.ToString();
                ViewBag.price=values.price;
                ViewBag.city=values.city;
                ViewBag.district=values.district;
                ViewBag.address=values.address;
                ViewBag.type=values.type;
                ViewBag.description = values.description;
                ViewBag.slugUrl = values.SlugUrl;

                ViewBag.bathCount = values2.bathCount;
                ViewBag.bedRoomCount = values2.bedRoomCount;
                ViewBag.size = values2.productSize;
                ViewBag.roomcount = values2.roomCount;
                ViewBag.garageCount = values2.garageSize;
                ViewBag.buildYear = values2.buildYear;
                ViewBag.date = values.AdvertisementDate;
                ViewBag.location=values2.location;
                ViewBag.videourl = values2.videoUrl;

            DateTime date1 = DateTime.Now;
            DateTime date2 = values.AdvertisementDate;

            TimeSpan timeSpan = date1 - date2;
            int totalMonths = ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;

            if (date1.Day < date2.Day)
            {
                totalMonths--;
            }

            ViewBag.datediff = totalMonths;

            string slugFromTitle = CreateSlug(values.title);
            ViewBag.slugUrl=slugFromTitle;

            return View(values);
        }
        private string CreateSlug(string title)
        {
            title = title.ToLowerInvariant(); // Küçük harfe çevir
            title = title.Replace(" ", "-"); // Boşlukları tire ile değiştir
            title = System.Text.RegularExpressions.Regex.Replace(title, @"[^a-z0-9\s-]", ""); // Geçersiz karakterleri kaldır
            title = System.Text.RegularExpressions.Regex.Replace(title, @"\s+", " ").Trim(); // Birden fazla boşluğu tek boşluğa indir ve kenar boşluklarını kaldır
            title = System.Text.RegularExpressions.Regex.Replace(title, @"\s", "-"); // Boşlukları tire ile değiştir

            return title;
        }
    }
}