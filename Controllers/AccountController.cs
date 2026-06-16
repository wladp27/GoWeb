using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> signInManager;
        private readonly ICityService cityService;
        public AccountController(ICityService cityService, IMapper mapper, IUserRepository userRepository, ILogger<AccountController> logger, SignInManager<User> signInManager)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            _logger = logger;
            this.signInManager = signInManager;
            this.cityService = cityService;
        }
        public async Task<IActionResult> Register()
        {
            var city= await cityService.GetAllAsync();
            var selectList = city
                                .Select(city => new SelectListItem
                                                {
       
                                                Text = city.NameCity,
                                                Value = city.Id.ToString()
                                                })
                                .ToList();

            return View(new UserRegisterViewModel() {Cities=selectList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel userView)
        {
            if (ModelState.IsValid)
            {
               var user=mapper.Map<User>(userView);


                try
                {
                    var result = await userRepository.CreateAsync(user, userView.Password);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false); // установка куки 
                        TempData["SuccessMessage"] = "Успешная регистрация";
                        return RedirectToAction("Index", "Event", new { area = ""  });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Критическая ошибка EF Core/Identity при создании пользователя. Подробности исключения:");
                        ModelState.AddModelError(string.Empty, "Произошла критическая ошибка системы. Пожалуйста, проверьте логи или настройки БД.");
                    }
            }
            var city = await cityService.GetAllAsync();
            var selectList = city
                                .Select(city => new SelectListItem
                                {

                                    Text = city.NameCity,
                                    Value = city.Id.ToString()
                                })
                                .ToList();
            userView.Cities = selectList;;
            return View(userView);
        }



        public IActionResult Login(string returnUrl = null)
        {
            return View(new UserLoginViewModel {ReturnUrl=returnUrl });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel loginViewModel)
        {
            if(ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModel.UserName,loginViewModel.Password,loginViewModel.RememberMe,false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && Url.IsLocalUrl(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин или пароль");
                }
            }
            return View(loginViewModel);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // удаляем аутентификационные куки
            return RedirectToAction("Index", "Event", new {area=""});
        }

    }
}
