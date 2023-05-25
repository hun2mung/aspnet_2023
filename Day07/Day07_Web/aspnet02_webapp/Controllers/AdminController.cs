using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet02_boardapp.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;    // 로그인 이후 사용자 잡는 역할

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            // 생성자 생성, Null 추가
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role);  // 지정한 권한명이 DB에 저장

                if(result.Succeeded)
                {
                    // 토스트 메시지


                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);    // aspnetroles 테이블에서 id 조회
            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }

            var model = new EditRoleModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var userList = await _userManager.Users.ToListAsync();      // 사용자 리스트

            foreach (var user in userList)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name)){
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);    // aspnetroles 테이블에서 id 조회
            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }

            var model = new List<UserRoleModel>();

            var userList = await _userManager.Users.ToListAsync();

            foreach (var user in userList)
            {
                var userRoleViewModel = new UserRoleModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleModel> model, string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                TempData["error"] = $"권한이 없습니다.";
                return NotFound();
            }

            IdentityResult result = null;

            foreach (var user in model)
            {
                var userObj = await _userManager.FindByIdAsync(user.UserId);

                if (user.IsSelected && !(await _userManager.IsInRoleAsync(userObj, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(userObj, role.Name); // 사용자에 권한 할당
                }
                else if (!user.IsSelected && await _userManager.IsInRoleAsync(userObj, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(userObj, role.Name); // 사용자를 권한 삭제, 
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }
}
