using Entity_Framework_Slider.Data;
using Entity_Framework_Slider.Models;
using Entity_Framework_Slider.Services.Interfaces;
using Entity_Framework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;

namespace Entity_Framework_Slider.Controllers
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IBasketService _basketService;
		private readonly IProductService _productService;

        public HomeController(AppDbContext context,
			                  IBasketService basketService,
                              IProductService productService)
        {
			_context = context;
			_basketService = basketService;
			_productService = productService;
        }


		[HttpGet]
        public async Task<IActionResult> Index()
		{

			int basketCount;

			if (Request.Cookies["basket"] != null)
			{
				basketCount = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]).Count;
			}
			else
			{
				basketCount = 0;
			}

			ViewBag.Count = basketCount;



			List<Slider> sliders = await _context.Sliders.ToListAsync();

            SliderInfo? sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

			IEnumerable<Blog> blogs = await _context.Blogs.Where(m=> !m.SoftDelete).ToListAsync();

			IEnumerable<Category> categories = await _context.Categories.Where(m => !m.SoftDelete).ToListAsync();

			IEnumerable<Product> products = await _productService.GetAll();

			About? about = await _context.Abouts.Where(m => !m.SoftDelete).FirstOrDefaultAsync();


            IEnumerable<Advantage> advantages = await _context.Advantages.Where(m => !m.SoftDelete).ToListAsync();


            IEnumerable<Instagram> instagram = await _context.Instagrams.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Say> says = await _context.Says.Where(m => !m.SoftDelete).ToListAsync();




            List<int> nums = new List<int>() { 1, 2, 3, 4, 5, 6 };


			var res = nums.FirstOrDefault();
			ViewBag.num = res;

			HomeVM model = new()
			{
				Sliders = sliders,
				SliderInfo = sliderInfo,
				Blogs = blogs,
				Categories = categories,
				Products = products,
				Advantages = advantages,
				About = about,
				Instagrams = instagram,
				says = says,

			};
			
			return View(model);
		}


		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddBasket(int? id)
		{
			if (id is null) return BadRequest();

			Product? dbProduct = await _productService.GetById((int)id);

			if (dbProduct == null) return NotFound();

			List<BasketVM> basket = _basketService.GetBasketDatas();

			BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbProduct.Id);

            _basketService.AddProductToBasket(existProduct, dbProduct, basket);

			int basketCount = basket.Sum(m => m.Count);

			return Ok(basketCount);

		}

		
	}
}