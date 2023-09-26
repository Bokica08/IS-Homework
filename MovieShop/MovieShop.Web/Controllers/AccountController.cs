using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Domain.Identity;
using MovieShop.Domain.Domain;
using MovieShop.Domain.DTO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using System;
using MovieShop.Service.Interface;
using MovieShop.Service.Implementation;
using Microsoft.EntityFrameworkCore;

namespace MovieShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<MovieApplicationUser> userManager;
        private readonly SignInManager<MovieApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IShoppingCartService _shopingCartService;
        public AccountController(UserManager<MovieApplicationUser> userManager,
            SignInManager<MovieApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IShoppingCartService shopingCartService

            )
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this._shopingCartService = shopingCartService;
        }

        public IActionResult Register()
        {
            UserRegistrationDTO model = new UserRegistrationDTO();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDTO request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new MovieApplicationUser
                    {
                        FirstName = request.Name,
                        LastName = request.LastName,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = request.PhoneNumber,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        if (!await roleManager.RoleExistsAsync("User"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("User"));
                        }
                        await userManager.AddToRoleAsync(user, "User");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDTO model = new UserLoginDTO();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    if (!await userManager.IsInRoleAsync(user, "User"))
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public IActionResult ImportUsers()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file");
                return View();
            }

            string filename = Path.GetFileName(file.FileName);
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{filename}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                await file.CopyToAsync(fileStream);
            }

            List<ImportedUserDTO> users = getUsersFromExcelFile(filename);

            foreach (var importedUser in users)
            {
                var user = new MovieApplicationUser
                {
                    UserName = importedUser.Email,
                    Email = importedUser.Email,
                    EmailConfirmed = true,
                    UserCart = new ShoppingCart(),
                };

                var result = await userManager.CreateAsync(user, importedUser.Password);

                if (result.Succeeded)
                {
                    if (importedUser.Role == "Admin")
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
        private List<ImportedUserDTO> getUsersFromExcelFile(string fileName)
        {
            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<ImportedUserDTO> userList = new List<ImportedUserDTO>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        userList.Add(new ImportedUserDTO
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            Role = reader.GetValue(2).ToString()
                        });
                    }
                }
            }

            return userList;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var model = new List<UserWithRoleDTO>();

            foreach (var user in users)
            {
                var viewModel = new UserWithRoleDTO
                {
                    User = user,
                    IsAdmin = await userManager.IsInRoleAsync(user, "Admin")
                };
                model.Add(viewModel);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != currentUser)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            return RedirectToAction("ManageUsers", "Account");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DemoteToUser(string id)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != currentUser)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var isInRole = await userManager.IsInRoleAsync(user, "Admin");
                    if (isInRole)
                    {
                        await userManager.RemoveFromRoleAsync(user, "Admin");
                    }
                }
            }

            return RedirectToAction("ManageUsers", "Account");
        }
    }


    }
