﻿using Entity_Framework_Slider.Data;
using Entity_Framework_Slider.Services.Interfaces;
using Entity_Framework_Slider.ViewModels;
using Newtonsoft.Json;

namespace Entity_Framework_Slider.Services
{
    public class LayoutService:ILayoutService 
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBasketService _basketService;

        public LayoutService(AppDbContext context,
                             IHttpContextAccessor httpContextAccessor,
                             IBasketService basketService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _basketService = basketService;
        }

        public LayoutVM GetSettingDatas()
        {
            Dictionary<string,string> settings = _context.Settings.AsEnumerable().ToDictionary(m => m.Key, m => m.Value);
            List<BasketVM> basketDatas = _basketService.GetBasketDatas();
            return new LayoutVM { Settings = settings, BasketCount = basketDatas.Sum(m => m.Count) };
        }
    }
}
