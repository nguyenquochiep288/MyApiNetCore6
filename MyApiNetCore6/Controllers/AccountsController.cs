using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;
using MyApiNetCore6.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using DatabaseTHP;
using System.ComponentModel.DataAnnotations;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly dbApplicationUserContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository accountRepo;

        private readonly dbTrangHiepPhatContext _contextTHP;
        public AccountsController(dbApplicationUserContext context, dbTrangHiepPhatContext contextTHP, IAccountRepository repo, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            accountRepo = repo;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            //dbTrangHiepPhat = new DatabaseTHP.dbTrangHiepPhat(_configuration.GetConnectionString("TrangHiepPhat"));
            _context = context;
            _contextTHP = contextTHP;
            //Another Way

            //Assembly assem2 = Assembly.Load("DatabaseTHP");



            ////Get List of Class Name

            //Type[] types = assem2.GetTypes();

            //using var transaction = _contextTHP.Database.BeginTransaction();
            //string askey = "";
            //string strpublic = "";
            //foreach (Type tc in types)
            //{

            //    var web_NhomQuyen = _contextTHP.web_NoteClass!.FirstOrDefault(x => x.NAMECLASS.Equals(tc.Name));
            //    if (tc.FullName != null && tc.FullName.Split(".").Count() > 2)
            //        continue;


            //    var web_NoteTable = _contextTHP.web_NoteTable!.FirstOrDefault(x => x.NAMECLASS.Equals(tc.Name));
            //    if (web_NoteTable == null && tc.Name != "dbTrangHiepPhat")
            //    {
            //        web_NoteTable newweb_NoteTable = new web_NoteTable();
            //        newweb_NoteTable.NAMECLASS = tc.Name;
            //        _contextTHP.web_NoteTable!.Add(newweb_NoteTable);
            //    }
            //    //if (tc.IsAbstract)

            //    //{

            //    //    Response.Write("Abstract Class : " + tc.Name);

            //    //}

            //    //else if (tc.IsPublic)

            //    //{

            //    //    Response.Write("Public Class : " + tc.Name);

            //    //}

            //    //else if (tc.IsSealed)

            //    //{

            //    //    Response.Write("Sealed Class : " + tc.Name);

            //    //}


            //    string msg = "";
            //    var properties = tc.GetProperties();
            //    var property = properties.Where(p => p.GetCustomAttributes(false)
            //                .Any(a => a.GetType() == typeof(KeyAttribute))).ToList();

            //    if (property != null)
            //    {
            //        foreach (PropertyInfo itm in property)
            //        {
            //            msg += ",m." + itm.Name.ToString() + ",";
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(msg))
            //    {
            //        askey += "modelBuilder.Entity<" + tc.Name + ">()" + Environment.NewLine;
            //        askey += ".HasKey(m => new { " + msg.Substring(1) + " });" + Environment.NewLine;
            //    }

            //    strpublic += "public virtual DbSet<" + tc.Name + ">? " + tc.Name + "s { get; set; }" + Environment.NewLine;
            //    if (tc.Name != "dbTrangHiepPhat" && (tc.Name == "dm_BangLuong" || tc.Name == "dm_BangLuong_ChiTiet"))
            //    {
            //        int i = 1;
            //        foreach (PropertyInfo itmPropertyInfo in properties)
            //        {
            //            var web_NoteClass = _contextTHP.web_NoteClass!.FirstOrDefault(x => x.NAMECLASS.Equals(tc.Name) && x.NAMECOLUMN.Equals(itmPropertyInfo.Name));
            //            if (web_NoteClass == null)
            //            {
            //                web_NoteClass newweb_NoteClass = new web_NoteClass();
            //                newweb_NoteClass.NAMESPACE = "DatabaseTHP";
            //                newweb_NoteClass.NAMECLASS = tc.Name;
            //                newweb_NoteClass.NAMECOLUMN = itmPropertyInfo.Name;
            //                newweb_NoteClass.DISPLAYNAME = "";
            //                if (itmPropertyInfo.Attributes.GetType() == typeof(KeyAttribute))
            //                    newweb_NoteClass.ISPRIMARYKEY = msg.Contains(",m." + itmPropertyInfo.Name);
            //                else
            //                    newweb_NoteClass.ISPRIMARYKEY = false;
            //                newweb_NoteClass.ISREQUIRED = true;
            //                newweb_NoteClass.ISCREATE = true;
            //                newweb_NoteClass.ISEDIT = !newweb_NoteClass.ISPRIMARYKEY;
            //                newweb_NoteClass.ISVIEW = !newweb_NoteClass.ISPRIMARYKEY;
            //                newweb_NoteClass.ISSEARCH = !newweb_NoteClass.ISPRIMARYKEY;
            //                newweb_NoteClass.STT = i;
            //                _contextTHP.web_NoteClass!.Add(newweb_NoteClass);
            //                _contextTHP.SaveChanges();
            //            }
            //            else
            //            {
            //                web_NoteClass.STT = i;
            //                _contextTHP.Entry(web_NoteClass).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //                _contextTHP.SaveChanges();
            //            }
            //            i += 1;
            //        }
            //    }


            //    //Get List of Method Names of Class

            //    //MemberInfo[] methodName = tc.GetMethods();

            //    //foreach (MemberInfo method in methodName)
            //    //{


            //    //    if (method.ReflectedType.IsPublic)

            //    //    {

            //    //        Response.Write("Public Method : " + method.Name.ToString());

            //    //    }

            //    //    else

            //    //    {

            //    //        Response.Write("Non-Public Method : " + method.Name.ToString());

            //    //    }

            //    //}
            //}
            //transaction.Commit();
        }
        [HttpPut("ChangeUser/{id}")]
        public async Task<IActionResult> ChangeUser([FromBody] SignUpModel signUpModel, string id)
        {
            var userExist = await userManager.FindByIdAsync(id);
            if (userExist != null)
            {
                var userExist1 = await userManager.FindByNameAsync(signUpModel.UserName);
                if (userExist1 != null && userExist1.Id != id)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập " + userExist1.UserName + " đã tồn tại!"
                    });
                }
                var result = new IdentityResult();
                if (!string.IsNullOrEmpty(signUpModel.Password))
                {
                    result = await userManager.RemovePasswordAsync(userExist);
                    if (!result.Succeeded)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đổi mật khẩu thất bại!"
                        });
                    }
                    result = await userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.Password, clsMaHoa.PassMaHoa));
                    if (!result.Succeeded)
                    {
                        string mes = "";
                        foreach (var itm in result.Errors)
                            mes += itm.Description;
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đổi mật khẩu thất bại!" + mes
                        });
                    }
                    userExist.PasswordDecrypt = signUpModel.Password ?? "";
                }
                userExist.URL_IMAGE = signUpModel.URL_IMAGE ?? "";
                userExist.UserName = signUpModel.UserName;
                userExist.FullName = signUpModel.FullName ?? "";
                userExist.ID_NHOMQUYEN = signUpModel.ID_NHOMQUYEN ?? "";
                userExist.PhoneNumber = signUpModel.PhoneNumber ?? "";
                userExist.Email = signUpModel.Email;
                //userExist.IPLOCATION = signUpModel.IPLOCATION ?? "";
                result = await userManager.UpdateAsync(userExist);
                if (!result.Succeeded)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Thay đổi tài khoản thất bại!"
                    });
                }
                else
                {
                    var OKUser = await _contextTHP.view_AspNetUsers!.FirstOrDefaultAsync(e => e.ID == userExist.Id);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thay đổi tài khoản thành công!",
                        Data = OKUser
                    });

                }
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại!"
                });
            }

        }


        [HttpPut("ChangeUserPassword/{id}")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] SignUpModel signUpModel, string id)
        {
            var userExist = await userManager.FindByIdAsync(id);
            if (userExist != null)
            {
                var result = new IdentityResult();
                if (!string.IsNullOrEmpty(signUpModel.Password))
                {
                    if(userExist.PasswordDecrypt != signUpModel.Password)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Mật khẩu không chính xác!"
                        });
                    }    
                    result = await userManager.RemovePasswordAsync(userExist);
                    if (!result.Succeeded)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đổi mật khẩu thất bại!"
                        });
                    }
                    result = await userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, clsMaHoa.PassMaHoa));
                    if (!result.Succeeded)
                    {
                        string mes = "";
                        foreach (var itm in result.Errors)
                            mes += itm.Description;
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đổi mật khẩu thất bại!" + mes
                        });
                    }
                    userExist.PasswordDecrypt = signUpModel.ConfirmPassword ?? "";
                }
                result = await userManager.UpdateAsync(userExist);
                if (!result.Succeeded)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Thay đổi tài khoản thất bại!"
                    });
                }
                else
                {
                    var OKUser = await _contextTHP.view_AspNetUsers!.FirstOrDefaultAsync(e => e.ID == userExist.Id);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thay đổi tài khoản thành công!",
                        Data = OKUser
                    });

                }
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại!"
                });
            }

        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            if (string.IsNullOrEmpty(clsMaHoa.Decrypt(signUpModel.Password, clsMaHoa.PassMaHoa)))
                signUpModel.Password = clsMaHoa.Encrypt(signUpModel.Password, clsMaHoa.PassMaHoa);
            var userExist = await userManager.FindByNameAsync(signUpModel.UserName);
            if (userExist != null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tài khoản đã tồn tại!"
                });
            }

            ApplicationUser user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = signUpModel.UserName ?? "",
                FullName = signUpModel.FullName ?? "",
                PasswordDecrypt = signUpModel.Password ?? "",
                ID_NHOMQUYEN = signUpModel.ID_NHOMQUYEN ?? "",
                PhoneNumber = signUpModel.PhoneNumber ?? "",
                Email = signUpModel.Email,
                URL_IMAGE = signUpModel.URL_IMAGE ?? ""
        };
            var result = await userManager.CreateAsync(user, clsMaHoa.Decrypt(signUpModel.Password, clsMaHoa.PassMaHoa));
            if (!result.Succeeded)
            {
                string mes = "";
                foreach (var itm in result.Errors)
                    mes += itm.Description;
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Tạo tài khoản thất bại!" + mes
                });
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            var user1 = await userManager.FindByNameAsync(signUpModel.UserName);
            var OKUser = await _contextTHP.view_AspNetUsers!.FirstOrDefaultAsync(e => e.ID == user1.Id);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Tạo tài khoản thành công!",
                ID = user1 != null ? user1.Id : "",
                Data = OKUser
            }); ;

        }

        [HttpPost()]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            Class.Security clsSecurity = new Class.Security(_configuration);
            var UserName = await clsSecurity.GetUserName(accessToken ?? "");
            var userExist = await userManager.FindByNameAsync(UserName);
            if (userExist != null)
            {
                SignUpModel user = new SignUpModel()
                {
                    UserName = userExist.UserName,
                    FullName = userExist.FullName,
                    ID_NHOMQUYEN = userExist.ID_NHOMQUYEN,
                    PhoneNumber = userExist.PhoneNumber,
                    Email = userExist.Email
                };
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = user
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin tài khoản"
                });
            }


        }

        [HttpPost("ChangePassword")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ChangePassword([FromBody] SignUpModel signUpModel)
        {
            var user = await userManager.FindByNameAsync(signUpModel.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.Password, clsMaHoa.PassMaHoa)))
            {
                var result = await userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đổi mật khẩu thất bại!"
                    });
                }
                result = await userManager.AddPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, clsMaHoa.PassMaHoa));
                if (!result.Succeeded)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đổi mật khẩu thất bại!"
                    });
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Tạo tài khoản thành công!"
                });
            }
            else
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Mật khẩu không chính xác. Vui lòng kiểm tra lại!"
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            try
            {
                if (string.IsNullOrEmpty(clsMaHoa.Decrypt(signInModel.Password, clsMaHoa.PassMaHoa)))
                    signInModel.Password = clsMaHoa.Encrypt(signInModel.Password, clsMaHoa.PassMaHoa);
                var user = await userManager.FindByNameAsync(signInModel.UserName);
                if (user != null && await userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signInModel.Password, clsMaHoa.PassMaHoa)))
                {
                    if (!user.LockoutEnabled)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Tài khoản đã bị khóa!",
                        });
                    }
                    var userRoles = await userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    DateTime expires = DateTime.Now.AddMinutes(30);
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: expires,
                        claims: authClaims,

                        signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                    );
                    var GroupPermissions = await _contextTHP.web_NhomQuyen!.FirstOrDefaultAsync(e => e.ID == user.ID_NHOMQUYEN);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Authenticate success",
                        Data = new JwtSecurityTokenHandler().WriteToken(token),
                        Expires = expires.AddMinutes(-1),
                        Detail = new ApiResponseUser { FullName = user.FullName, idUser = user.Id, UserName = user.UserName, idNhomQuyen = (GroupPermissions != null && !GroupPermissions.ISPHANQUYEN ? "-1" : user.ID_NHOMQUYEN) }
                    });
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = user != null ? "Mật khẩu tài khoản không chính xác!" : "Tài khoản không tồn tại!",
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }



        // GET: api/GetNoteClass
        [HttpGet("GetNoteClass")]
        public async Task<IActionResult> GetNoteClass()
        {
            try
            {
                var lstValue = await _contextTHP.view_web_NoteClass!.ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        // GET: api/GetNoteClass
        [HttpGet("GetPhanQuyen/{LOC_ID}/{ID_NHOMQUYEN}")]
        public async Task<IActionResult> GetPhanQuyen(string ID_NHOMQUYEN)
        {
            try
            {
                var lstValue = await _contextTHP.view_web_PhanQuyen!.Where(e => e.ID_NHOMQUYEN == ID_NHOMQUYEN).ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }
    }
}
