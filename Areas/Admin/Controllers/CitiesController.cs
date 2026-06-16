using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Администратор")]
    public class CitiesController : Controller
    {
        private readonly ICityService cityService;
        private readonly IMapper mapper;

        public CitiesController(ICityService cityRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.cityService = cityRepository;
        }

        public async Task<IActionResult> Index()
        {
            var listCities = await cityService.GetAllAsync();
            var listCitiesViews = mapper.Map<List<CityViewModel>>(listCities);
            return View(listCitiesViews);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityViewModel cityView)
        {
            if (ModelState.IsValid)
            {
                await cityService.AddAsync(cityView);
                return RedirectToAction("Index");
            }
            return View(cityView);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var city = await cityService.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            else
            {
                var cityView = mapper.Map<CityViewModel>(city);
                return View(cityView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CityViewModel cityView)
        {
            if (ModelState.IsValid)
            {
                var existingCity = await cityService.GetByIdAsync(cityView.Id.Value);
                if (existingCity == null)
                {
                    ModelState.AddModelError(string.Empty, "Город не найден для обновления. Возможно, он был удален.");
                    return View(cityView);
                }
                await cityService.Update(cityView);
                return RedirectToAction(nameof(Index));
            }
            return View(cityView); 
        }


        public async Task<IActionResult> Delete(int id)
        {
            var existingCity = await cityService.GetByIdAsync(id);
            if (existingCity == null)
            {
                return NotFound();
            }
            var cityView = mapper.Map<CityViewModel>(existingCity);
            return View(cityView); 
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingCity = await cityService.GetByIdAsync(id);
            if (existingCity != null)
            {
                await cityService.DeleteAsync(existingCity);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
