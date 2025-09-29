using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using System.Text.RegularExpressions;
using Ovia.Models;
using System.Net.Mail;
using Ovia.Services.SendEmails;
using Ovia.Services;
using Telegram.Bot;
using Ovia.Services.StorageFiles;
using Ovia.Services.SendWhatsApp360Live;
using Microsoft.EntityFrameworkCore.Query;
using MailChimp.Net.Models;
using Org.BouncyCastle.Bcpg;


namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {



        private readonly IMailingServices mailing;
        private readonly TelegramService _telegramService;
        private readonly TelegramBotClient _botClient;
        private readonly IStorageService storage;
        private readonly MomEntity momDb;
        private readonly Whats360Client _whats360Client;

        //  private readonly ZoomClient _zoomClient;
        private JWTService _JWTService;



        public UserController(MomEntity _momDb,
            IMailingServices _mailing,
            TelegramService telegramService,
            IStorageService _storage,
            // ZoomClient zoomClient
            JWTService JWTService,
            Whats360Client whats360Client


            )
        {
            momDb = _momDb;
            mailing = _mailing;
            _telegramService = telegramService;
            _botClient = new TelegramBotClient("6935466790:AAHBegNUuZw8DK2bvNYVfruK4MUGl626l9E");
            storage = _storage;
            _JWTService = JWTService;
            _whats360Client = whats360Client;


        }






       
    





        [HttpPost("authenticate")]
        public async  Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            if (model == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest(new { message = "Should entered " });
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "Should entered Password" });
            var dbUser = momDb.CustomerAttributes
               .Include(i => i.CustomerInfo)
                .Where(c => (c.CustomerInfo.Email == model.Username || c.ReferId == model.Username) && (c.CustomerInfo.Password == model.Password))
                .Include(i => i.CustomerInfo.Role)
                .OrderBy(o => o.Id)
                .FirstOrDefault();

            if (dbUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            List<string> Roles = new List<string>() { dbUser.CustomerInfo.Role.Name };
            var user = _JWTService.GenerateToken(dbUser.Id.ToString(), Roles);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            dbUser.LastLoginDateUtc = DateTime.Now;

            momDb.log.Add(new Log()
            {
                CustomerId = dbUser.Id,
                ShortMessage = "Login",
            });
            await momDb.SaveChangesAsync();


            var nsBalance = await momDb.NsStartingBalances
                .FirstOrDefaultAsync(c => c.NsId == dbUser.Id);

            var BinanceAccount = await momDb.RequestsCards.FirstOrDefaultAsync(c=>c.CustomerId == dbUser.Id);

            return Ok(new
            {
                token = user,
                CustomerAttributeId = dbUser.Id,
                ReferId = dbUser.ReferId,
                Email = dbUser.CustomerInfo.Email,
                Role = dbUser.CustomerInfo.Role.Name,
                Name = dbUser.CustomerInfo.NameEn,
                ns_balance = nsBalance !=null? true : false,
                binance_account = BinanceAccount != null? true : false,
                AllowToCreateMeeting = dbUser.CustomerInfo.AllowToCreateMeetings
            });
        }


        //check password validator
        private bool IsPasswordValid(string password)
        {
            // Password must contain at least one uppercase letter, one lowercase letter,
            // one special character, one number, and be at least eight characters long.
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$");
            return regex.IsMatch(password);
        }

        //check email validator
        private bool IsEmailValid(string email)
        {
            try
            {
                // Attempt to create a MailAddress instance which validates the email format
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //random password
        private string GenerateRandomString(int length)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string specialChars = "@#$&";

            var random = new Random();



            // Ensure at least one character from each category
            var password = new string(new[]
            {
        uppercaseChars[random.Next(uppercaseChars.Length)],
        lowercaseChars[random.Next(lowercaseChars.Length)],
        numberChars[random.Next(numberChars.Length)],
        specialChars[random.Next(specialChars.Length)]
    });

            // Fill the rest of the password with random characters
            password += new string(Enumerable.Repeat(
                uppercaseChars + lowercaseChars + numberChars + specialChars,
                length - 4 // Subtracting 4 to account for the characters added above
            )
            .Select(s => s[random.Next(s.Length)]).ToArray());

            // Shuffle the characters in the password to randomize their order
            password = new string(password.ToCharArray().OrderBy(c => random.Next()).ToArray());

            return password;
        }







        [HttpGet]
        [Route("{SponsorReferId}")]
        public async Task<IActionResult> GetSponsorData(string SponsorReferId)
        {
            var sponsor = await momDb.CustomerAttributes
                .Where(a => a.ReferId == SponsorReferId)
                .FirstOrDefaultAsync();

            if (sponsor != null)
            {
                var sponsorData = await momDb.CustomerInfo
                    .Where(c => c.Id == sponsor.CustomerInfoId)
                    .FirstOrDefaultAsync();

                if (sponsorData != null)
                {
                    var sponsorDto = new SponsorDto
                    {
                        CustomerAttributeId = sponsor.Id,
                        Email = sponsorData.Email,
                        NationalId = sponsorData.NationalId,
                        phone = sponsorData.Mobile,
                        BackOfficeId = SponsorReferId,
                        Name = sponsorData.NameEn
                    };

                    return Ok(sponsorDto);
                }
                else
                {
                    return NotFound("Sponsor data not found");
                }
            }
            else
            {
                return NotFound("Sponsor not found");
            }
        }


       





        [HttpPost("SignUp")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDTO request)
        {
            //validate email
            if (!IsEmailValid(request.Email))
            {
                return BadRequest("Invalid email format. Please provide a valid email address.");
            }


            var existingUser = await momDb.CustomerInfo
                .Where(c => c.Email == request.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return BadRequest("This E-mail is already registered!");
            }
            else
            {
                // Validate password
                if (!IsPasswordValid(request.Pass))
                {
                    return BadRequest("Password must contain at least one uppercase letter, one lowercase letter, one special character, one number, and be at least eight characters long.");
                }

                var customerInfo = new Models.CustomerInfo
                {
                    Id = 0,
                    NameEn = request.Name,
                    Username = "",
                    Email = request.Email,
                    Password = request.Pass,
                    RoleId = 3,
                    CountryId = 63,
                    LiveStyleId = request.LiveStyleId
                };

                momDb.CustomerInfo.Add(customerInfo);
                await momDb.SaveChangesAsync();


                // Generate a random number between 100 and 999 (inclusive)
                Random rand = new Random();
                int randomNumber = rand.Next(100, 1000);
                string referId = customerInfo.Id.ToString() + randomNumber.ToString();


                // int random = GenerateRandomNumber();

                var customerAttribute = new CustomerAttribute
                {
                    Id = 0,
                    CustomerInfoId = customerInfo.Id,
                    ReferId = referId   //
                };

                momDb.CustomerAttributes.Add(customerAttribute);
                await momDb.SaveChangesAsync();

                var sponsorAttribute = await momDb.CustomerAttributes
                    .Where(a => a.ReferId == request.SponsorId)
                    .FirstOrDefaultAsync();

                if (sponsorAttribute != null)
                {
                    var network = new CustomerNetwork
                    {
                        Id = 0,
                        ChildId = customerAttribute.Id,
                        SponsorId = sponsorAttribute.Id,
                        //  ParentId = 0
                    };

                    momDb.CustomerNetwork.Add(network);
                    await momDb.SaveChangesAsync();



                    //////mail to user
                    await mailing
                        .SendWelcomeEmail(request.Email, request.Name, customerAttribute.ReferId,customerInfo.Password );

                    /////mail to sponsor
                    if (request.SponsorId != null)
                    {
                        var customInfoId = await momDb.CustomerAttributes
                            .Where(c => c.ReferId == request.SponsorId)
                            .FirstOrDefaultAsync();


                        var customer = await momDb.CustomerInfo
                            .Where(c => c.Id == customInfoId.CustomerInfoId)
                            .FirstOrDefaultAsync();

                        //SendEmailToSponsor(string userEmail, string username,
                        //    string sponsorId, string childName, string childId)

                        await mailing.SendEmailToSponsor(customer.Email, customer.NameEn
                            , request.SponsorId, request.Name, customerAttribute.ReferId);

                    }




                    return Ok("Registered Successfully");
                }














            }

            return Ok("User signed up successfully!");
        }


        [HttpPost("SignupInstructor")]
        public async Task<IActionResult> SignupInstructor([FromBody] InstructorSignup request)
        {
            //validate email
            if (!IsEmailValid(request.Email))
            {
                return BadRequest("Invalid email format. Please provide a valid email address.");
            }


            var existingUser = await momDb.CustomerInfo
                .Where(c => c.Email == request.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return BadRequest("This E-mail is already registered!");
            }
            else
            {
                // Validate password
                var password = GenerateRandomString(8);

                var RoleId = await momDb.Roles
                    .Where(r => r.Name == "Instructor")
                    .Select(r => r.Id).FirstOrDefaultAsync();


                string whatsappnumber = request.PhoneNumber;
                if (request.PhoneNumber.StartsWith("+2"))
                    whatsappnumber = request.PhoneNumber;
                if (request.PhoneNumber.StartsWith("2"))
                    whatsappnumber = "+" + request.PhoneNumber;
                if (request.PhoneNumber.StartsWith("0"))
                    whatsappnumber = "+2" + request.PhoneNumber;


                var customerInfo = new Models.CustomerInfo
                {
                    Id = 0,
                    NameEn = request.Name,
                    Mobile = whatsappnumber,
                    whatsappmobile = whatsappnumber,
                    CountryId = request.CountryId,
                    NationalId = request.NationalId,
                    Email = request.Email,
                    Password = password,
                    RoleId = RoleId,
                };

                momDb.CustomerInfo.Add(customerInfo);
                await momDb.SaveChangesAsync();


                // Generate a random number between 100 and 999 (inclusive)
                Random rand = new Random();
                int randomNumber = rand.Next(100, 1000);
                string referId = customerInfo.Id.ToString() + randomNumber.ToString();


                // int random = GenerateRandomNumber();

                var customerAttribute = new CustomerAttribute
                {
                    Id = 0,
                    CustomerInfoId = customerInfo.Id,
                    Renewal = false,
                    ReferId = referId   //
                };

                momDb.CustomerAttributes.Add(customerAttribute);
                await momDb.SaveChangesAsync();

                

                var network = new CustomerNetwork
                {
                    Id = 0,
                    ChildId = customerAttribute.Id,
                    SponsorId = null,
                };

                momDb.CustomerNetwork.Add(network);
                await momDb.SaveChangesAsync();

                ////////////
                //////mail to user
                
                await mailing
                    .SendEmailToProfessor(request.Email, request.Name,
                    customerAttribute.ReferId, customerInfo.Password);
                //                var dto = new SendWhatsDTO
                //                {
                //                    recipientPhoneNumber = customerInfo.whatsappmobile,
                //                    message = $@"
                //                            Dear {customerInfo.NameEn}

                //Thank you for regestiration in  our   community. 

                //ID: {customerAttribute.ReferId}
                //Password: {customerInfo.Password}

                
                //Best regards,
                //The Momentum Team
                //"
                //                };



                //                await _whats360Client.SendMessage(dto);




                return Ok(new { customerInfo.Email, customerInfo.Password });

            }

            return Ok("User signed up successfully!");
        }



        [HttpPost("NewSignup")]
        public async Task<IActionResult> NewSignup([FromBody] NewSignUpDTO request)
        {
            //validate email
            if (!IsEmailValid(request.Email))
            {
                return BadRequest("Invalid email format. Please provide a valid email address.");
            }


            var existingUser = await momDb.CustomerInfo
                .Where(c => c.Email == request.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return BadRequest("This E-mail is already registered!");
            }
            else
            {
                // Validate password
                var password = GenerateRandomString(8);

                var RoleId = await momDb.Roles
                    .Where(r => r.Name == "InActive")
                    .Select(r => r.Id).FirstOrDefaultAsync();


                string whatsappnumber = request.PhoneNumber;
                if (request.PhoneNumber.StartsWith("+2"))
                     whatsappnumber = request.PhoneNumber;
                  if (request.PhoneNumber.StartsWith("2"))
                     whatsappnumber = "+"+request.PhoneNumber;
                  if (request.PhoneNumber.StartsWith("0"))
                    whatsappnumber = "+2"+request.PhoneNumber;


                var customerInfo = new Models.CustomerInfo
                {
                    Id = 0,
                    NameEn = request.Name,
                    Mobile = whatsappnumber,
                    whatsappmobile = whatsappnumber,
                    CountryId = request.CountryId,
                    NationalId = request.NationalId,
                    Email = request.Email,
                    Password = password,
                    RoleId = RoleId,
                    AllowToCreateMeetings = false
                };

                momDb.CustomerInfo.Add(customerInfo);
                await momDb.SaveChangesAsync();


                // Generate a random number between 100 and 999 (inclusive)
                Random rand = new Random();
                int randomNumber = rand.Next(100, 1000);
                string referId = customerInfo.Id.ToString() + randomNumber.ToString();


                // int random = GenerateRandomNumber();

                var customerAttribute = new CustomerAttribute
                {
                    Id = 0,
                    CustomerInfoId = customerInfo.Id,
                    Renewal = false,
                    ReferId = referId   //
                };
                
                momDb.CustomerAttributes.Add(customerAttribute);
                await momDb.SaveChangesAsync();

                var sponsorAttribute = await momDb.CustomerAttributes
                    .Where(a => a.ReferId == request.SponsorId).Include(c=> c.CustomerInfo)
                    .FirstOrDefaultAsync();

               
                    var network = new CustomerNetwork
                    {
                        Id = 0,
                        ChildId = customerAttribute.Id,
                        SponsorId = sponsorAttribute != null?  sponsorAttribute.Id: null,
                    };

                    momDb.CustomerNetwork.Add(network);
                    await momDb.SaveChangesAsync();

                    ////////////
                    //////mail to user
                    await mailing
                        .SendWelcomeEmail(request.Email, request.Name,
                        customerAttribute.ReferId, customerInfo.Password);
                //                var dto = new SendWhatsDTO
                //                {
                //                    recipientPhoneNumber = customerInfo.whatsappmobile,
                //                    message = $@"
                //                            Welcome to Momentum, {customerInfo.NameEn}

                //Thank you for joining our network marketing community. 
                //We are excited to have you on board.

                //Back Office Id: {customerAttribute.ReferId}
                //Password: {customerInfo.Password}

                //Here, you will discover endless opportunities for growth, success, and collaboration. 
                //Feel free to explore our platform and connect with fellow members to maximize your experience.

                //Best regards,
                //The Momentum Team
                //"
                //                };



                //                await _whats360Client.SendMessage(dto);



                /////mail to sponsor
                /////mail to sponsor
                if (sponsorAttribute != null)
                {
                    await mailing.SendEmailToSponsor(sponsorAttribute?.CustomerInfo.Email,
    sponsorAttribute?.CustomerInfo.NameEn,
    request.SponsorId,
    request.Name,
    customerAttribute.ReferId);

                    return Ok(new { customerInfo.Email, customerInfo.Password });

                }


                return Ok(new { customerInfo.Email, customerInfo.Password });

            }

            return Ok("User signed up successfully!");
        }






        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            // Validate email format
            if (!IsEmailValid(request.Email))
            {
                return BadRequest("Invalid email format. Please provide a valid email address.");
            }

            var ExistingUser = await momDb.CustomerInfo
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (ExistingUser == null)
            {
                return NotFound("User not found. Please check your email or sign up.");
            }

            // Verify the password
            if (ExistingUser.Password != request.Password)
            {
                return BadRequest("Invalid password. Please try again.");
            }

            return Ok(ExistingUser);
        }



        //forget password

        [HttpGet("ForgetPassword")]
        //  [Route("{Email}")]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            var user = await momDb.CustomerInfo
                  .Where(u => u.Email == Email)
                  .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("this email not found");
            }

            else
            {
                var password = GenerateRandomString(8);

                user.Password = password;
                user.RequestPasswordResetDate = DateTime.UtcNow;

                await momDb.SaveChangesAsync();


                await mailing.SendPasswordResetEmail(Email, user.NameEn,password);
//                var dto = new SendWhatsDTO
//                {
//                    recipientPhoneNumber = user.whatsappmobile,
//                    message = $@"
//                           Dear, {user.NameEn}
//You have requested to reset password

//Your new auto-generated password is: {password}
//For security reasons, please do not share this password with anyone

//Best regards,
//The Momentum Team
//"
//                };



//                await _whats360Client.SendMessage(dto);
                return Ok("Password Changed Successfully!");

            }



        }


        //Add Or Update User Image
        [HttpPost("AddUserImg")]
        //[Route("{userId}")]
        public async Task<IActionResult> AddUserImage(int userId, IFormFile file)
        {
            var existingUserImage = await momDb.UserImgs
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();
            if (existingUserImage == null)
            {

                if (file != null || file.ContentDisposition != null)
                {
                    var fileUploaded = await storage.Upload(file);

                    var f = new UserImg
                    {
                        Id = 0,
                        UserId = userId,
                        Key = fileUploaded.Key,
                        FileName = fileUploaded.FileName,
                        Extension = fileUploaded.Extension,
                        FileSize = fileUploaded.FileSize,
                    };

                    momDb.UserImgs.Add(f);
                    await momDb.SaveChangesAsync();

                    return Ok(f);
                }

                else
                    return Ok("This File Is Not Valid");

            }

            else
            {
                if (file != null || file.ContentDisposition != null)
                {
                    var fileUploaded = await storage.Upload(file);


                    existingUserImage.Key = fileUploaded.Key;
                    existingUserImage.FileName = fileUploaded.FileName;
                    existingUserImage.Extension = fileUploaded.Extension;
                    existingUserImage.FileSize = fileUploaded.FileSize;


                    await momDb.SaveChangesAsync();

                    return Ok("Update Successfully");
                }

                else
                    return Ok("This File Is Not Valid");

            }



        }



        private async Task<GetCustomerDataDTO> GetUserData(int userId)
        {
            var user = await momDb.CustomerAttributes
                .Where(u => u.Id == userId)
                .Include(c => c.CustomerInfo)
                .Include(c => c.CustomerInfo.Country)
                .Include(c => c.CustomerInfo.Role)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var userImage = await momDb.UserImgs
                    .Where(u => u.UserId == user.Id)
                    .FirstOrDefaultAsync();

                string imgurl = "";
                if (userImage != null)
                {
                    imgurl = userImage?.Key?.SetDownloadFileUrlByKey(storage);
                }

                var sponsor = await momDb.CustomerNetwork
                    .FirstOrDefaultAsync(u => u.ChildId == user.Id);

                var sponsorid = "";
                if (sponsor != null)
                {
                    sponsorid = await momDb.CustomerAttributes
                        .Where(u => u.Id == sponsor.SponsorId)
                        .Select(u => u.ReferId)
                        .FirstOrDefaultAsync();
                }

                var BinanceAccount = await momDb.RequestsCards.FirstOrDefaultAsync(c => c.CustomerId == userId);

                bool? isApproved = null;
                if(BinanceAccount != null)
                {
                    isApproved = BinanceAccount.IsApproved;
                }

                return new GetCustomerDataDTO
                {
                    Id = user.Id,
                    Username = user.CustomerInfo.Username,
                    NameEn = user.CustomerInfo.NameEn,
                    Email = user.CustomerInfo.Email,
                    BackOfficeId = user.ReferId,
                    SponsorId = sponsorid,
                    Mobile = user.CustomerInfo.Mobile,
                    whatsappmobile = user.CustomerInfo.whatsappmobile,
                    NationalId = user.CustomerInfo.NationalId,
                    Gender = user.CustomerInfo.Gendar,
                    BirthDate = user.CustomerInfo.BirthDate,
                    Role = user.CustomerInfo.Role.Name,
                    PictureUrl = imgurl,
                    CountryName = user.CustomerInfo.Country.name,
                    StartDate = user.CustomerInfo.CreationDate,
                    Binance_Account = BinanceAccount !=null? true : false,
                    IsApproved_BinanceAccount = isApproved,
                    AllowToCreate = user.CustomerInfo.AllowToCreateMeetings
                };
            }
            else
            {
                return null; // Return null if user is not found
            }
        }



        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var userData = await GetUserData(userId);
            if (userData != null)
                return Ok(userData);
            return NotFound("This user not found !!");

        }




        //edit user information
        [HttpPut("EditUserData")]
        public async Task<IActionResult> EditUserData([FromBody] EditUserDTO dto)
        {

            var user = await momDb.CustomerAttributes.Where(c => c.Id == dto.UserId).Include(c => c.CustomerInfo).FirstOrDefaultAsync();
               
            if (user != null)
            {

                var existingUserEdit = await momDb.EditProfileHistory.FirstOrDefaultAsync(c => c.UserId == dto.UserId);
                if(existingUserEdit != null)
                {
                    user.CustomerInfo.Mobile = dto.MobileNumber;
                    user.CustomerInfo.whatsappmobile = dto.WhatsAppNumber;
                    user.CustomerInfo.Email = dto.Email;

                    await momDb.SaveChangesAsync();
                    var editProfile = new EditProfileHistory
                    {
                        Id=0,
                        UserId = dto.UserId,
                        Email = dto.Email ,
                        MobileNumber = dto.MobileNumber,
                        WhatsAppNumber = dto.WhatsAppNumber,
                        Notes = "Edit Information"
                    };
                     momDb.EditProfileHistory.Add(editProfile);
                    await momDb.SaveChangesAsync();
                    return Ok("Updated Successfully");

                }

                //add data before edit
                var edit = new EditProfileHistory
                {
                    Id = 0,
                    UserId = dto.UserId,
                    Email = user.CustomerInfo.Email,
                    MobileNumber = user.CustomerInfo.Mobile,
                    WhatsAppNumber = user.CustomerInfo.whatsappmobile,
                    Notes = "The First Information Added"

                };

                momDb.EditProfileHistory.Add(edit);
                await momDb.SaveChangesAsync();

                user.CustomerInfo.Mobile = dto.MobileNumber;
                user.CustomerInfo.whatsappmobile = dto.WhatsAppNumber;
                user.CustomerInfo.Email = dto.Email;

                await momDb.SaveChangesAsync();

                //add data after edit 
                var afteredit = new EditProfileHistory
                {
                    Id = 0,
                    UserId = dto.UserId,
                    Email = dto.Email,
                    MobileNumber = dto.MobileNumber,
                    WhatsAppNumber = dto.WhatsAppNumber,
                    Notes = "Edit Information"
                };

                 momDb.EditProfileHistory.Add(afteredit);
                await momDb.SaveChangesAsync();


                return Ok("Updated Successfully");
            }

            else
            {
                return NotFound("User Not Found");

            }

        }




        [HttpGet("UserEditProfileHistory")]
        public async Task<IActionResult> UserEditProfileHistory(int userId)
        {
            var userHistory = await momDb.EditProfileHistory.Where(c => c.UserId == userId).ToListAsync();

            if (userHistory.Count == 0)
                return BadRequest("You dont have history edit information");

            return Ok(userHistory);


        }




        //    [HttpGet("ShowAnnualPackageIfUserInActive")]
        //public async Task<IActionResult> ShowAnnualPackageIfUserInActive(int userId)
        //{


        //    // Retrieve the Role ID for the "Inactive" role
        //    var roleId = await momDb.Roles
        //        .Where(r => r.Name == "InActive")
        //        .Select(r => r.Id)
        //        .FirstOrDefaultAsync();

        //    if (roleId != 0)
        //    {
        //        // Check if the user exists and is in the "Inactive" role
        //        var inActiveUser = await momDb.CustomerInfo
        //            .FirstOrDefaultAsync(u => u.Id == userId && u.RoleId == roleId);

        //        if (inActiveUser != null)
        //        {
        //            // Retrieve the package type associated with "Inactive" users
        //            var packageType = await momDb.PackagesTypes
        //                .FirstOrDefaultAsync(pt => pt.Notes == "InActive" && pt.Name == "Annual");

        //            if (packageType != null)
        //            {
        //                // Retrieve packages associated with the package type
        //                var packages = await momDb.Packages
        //                    .Where(p => p.PackageTypeId == packageType.Id)
        //                    .ToListAsync();

        //                if (packages.Count != 0)
        //                {
        //                    List<GetPackagesDTO> AllPackages = new List<GetPackagesDTO>();

        //                    foreach (var p in packages)
        //                    {
        //                        var image = await momDb.PackageImages
        //                                            .Where(i => i.PackageId == p.Id)
        //                                            .FirstOrDefaultAsync();

        //                        var url = "";

        //                        if (image != null)
        //                        {
        //                            url = image?.Key?.SetDownloadFileUrlByKey(storage);
        //                        }

        //                        var pack = new GetPackagesDTO
        //                        {
        //                            Id = p.Id,
        //                            Name = p.Name,
        //                            ShortDescription = p.ShortDescription,
        //                            FullDescription = p.FullDescription,
        //                            ShowOnHomePage = p.ShowOnHomePage,
        //                            Key = image?.Key,
        //                            Url = url,
        //                            Price = p.Price,
        //                            OldPrice = p.OldPrice,
        //                            BusinessValue = p.BusinessValue,
        //                            DisplayOrder = p.DisplayOrder,
        //                            MembershipID = p.MembershipID,
        //                            Published = p.Published,
        //                            PackageTypeId = p.PackageTypeId,
        //                            SponsorCustomerToCustomer = p.SponsorCustomerToCustomer,
        //                            SponsorDistributorToCustomer = p.SponsorDistributorToCustomer,
        //                            SponsorDistributorToDistributor = p.SponsorDistributorToDistributor,
        //                            Summit_Coins = p.Summit_Coins,
        //                            Summit_Cost = p.Summit_Cost,
        //                            TeamId = p.TeamId,
        //                            SponsorTeam = p.SponsorTeam

        //                        };

        //                        AllPackages.Add(pack);
        //                    }
        //                    return Ok(AllPackages);



        //                }
        //                else
        //                {
        //                    return NotFound("No annual packages found for inactive users.");
        //                }
        //            }
        //            else
        //            {
        //                return NotFound("Package type for inactive users not found.");
        //            }
        //        }
        //        else
        //        {
        //            return NotFound("User not found or not in inactive state.");
        //        }
        //    }
        //    else
        //    {
        //        return NotFound("Role ID for 'Inactive' role not found.");
        //    }


        //}


        //[HttpGet("GetPersonalInformation")]
        //public async Task<IActionResult> GetPersonalInformation(int userId)
        //{
        //    var user = await momDb.customerInfo
        //        .Where(c => c.Id == userId)
        //        .FirstOrDefaultAsync();


        //    if (user != null) {

        //        var attribute = await momDb.customerAttribute
        //            .Where(x => x.Id == user.Id)
        //            .FirstOrDefaultAsync();
        //        if(attribute != null)
        //        {
        //            network

        //        }



        //    }
        //}

















     





        [HttpPost("AddCountries")]
        public async Task<IActionResult> AddCountries(List<Countries> countries)
        {


            foreach (Countries country in countries)
            {

                momDb.Country.Add(country);
                await momDb.SaveChangesAsync();


            }



            return Ok("Countries data inserted successfully!");
        }



        [HttpGet("GeTAllCountries")]
        public async Task<IActionResult> GeTAllCountries()
        {
            var countries = await momDb.Country.ToListAsync();

            if (countries.Count != 0)
            {
                return Ok(countries);
            }

            else
            {
                return NotFound("not found any countries");
            }

        }
















        [HttpGet("CheckAboutHisChildren")]
        public async Task<IActionResult> CheckAboutHisChildren(string sponsorId)
        {
            if (string.IsNullOrEmpty(sponsorId))
            {
                return BadRequest("Sponsor ID is required.");
            }

            var custom = await momDb.CustomerAttributes
                .FirstOrDefaultAsync(c => c.ReferId == sponsorId);

            if (custom == null)
            {
                return NotFound("Sponsor not found.");
            }


            var hisChildren = await momDb.CustomerNetwork
                .Where(c => c.ParentId == custom.Id)
                .ToListAsync();
            if(hisChildren.Count == 0)
            {
                return NotFound("Sponsor not have children.");
            }
            else
            {
                List<HisChildrenDataDTO> hisChildrenData = new List<HisChildrenDataDTO>();
                foreach(var i in hisChildren)
                {
                    var customAttribute = await momDb.CustomerAttributes
                        .FirstOrDefaultAsync(c => c.Id == i.ChildId);

                    var customInfo = await momDb.CustomerInfo
                        .FirstOrDefaultAsync(c => c.Id == customAttribute.CustomerInfoId);
                    var child = new HisChildrenDataDTO
                    {
                        CustomerAttributeId = customAttribute.Id,
                        Name = customInfo.NameEn,
                        Email = customInfo.Email,
                        HandSide = i.HandSide,
                        ReferId = customAttribute.ReferId
                    };
                    hisChildrenData.Add(child);
                }
                return Ok(hisChildrenData);
            }



        }





        [HttpGet("GetAllHoldingTank")]
        public async Task<IActionResult> GetAllHoldingTank(string sponsorId)
        {
            int sponsor = await momDb.CustomerAttributes
                    .Where(a => a.ReferId == sponsorId).Select(s => s.Id)
                    .FirstOrDefaultAsync();

            int roleId = await momDb.Roles
.Where(c => c.Name == "InActive")
.Select(c => c.Id)
.FirstOrDefaultAsync();



            if (sponsor != 0)
            {
                var NetworkList = await momDb.CustomerNetwork
                    .Where(a => a.SponsorId == sponsor).ToListAsync();

                if (NetworkList.Count != 0)
                {
                    List<CustomerAttributeInfo> customerAtteributeIds = new List<CustomerAttributeInfo>();
                    List<CustomerDataInHoldTankDTO> CustomersData = new List<CustomerDataInHoldTankDTO>();

                    foreach (var i in NetworkList)
                    {
                        //int? customerId = i.ChildId;

                        int? customerIdNullable = i.ChildId;
                        int customerId = customerIdNullable ?? 0;

                        if (customerId != 0)
                        {
                            var attributeInfo = new CustomerAttributeInfo
                            {
                                CustomerId = customerId,
                                HasParent = i.ParentId != null ? true : false
                            };
                            customerAtteributeIds.Add(attributeInfo);

                        }

                    }

                    if (customerAtteributeIds.Count != 0)
                    {
                        List<dynamic> customerInfoIds = new List<dynamic>();

                        foreach (var id in customerAtteributeIds)
                        {
                            var customerAttribute = await momDb.CustomerAttributes
                                .Where(c => c.Id == id.CustomerId)
                                .FirstOrDefaultAsync();

                            if (customerAttribute != null)
                            {
                                var customerInfo = await momDb.CustomerInfo
                                    .FirstOrDefaultAsync(cu => cu.Id == customerAttribute.CustomerInfoId);

                                var customerData = new CustomerDataInHoldTankDTO
                                {
                                    customerAttributeId = id?.CustomerId,
                                    Name = customerInfo?.NameEn,
                                    Email = customerInfo?.Email,
                                    BackOfficeId = customerAttribute?.ReferId,
                                    HasParent = id?.HasParent,
                                    Status = customerInfo.RoleId == roleId ? "Not registered yet" : "Active"
                                };

                                CustomersData.Add(customerData);
                            }

                        }
                    }


                    return Ok(CustomersData);
                }
                else
                {
                    // Return an empty list if NetworkList is empty
                    return Ok(new List<CustomerDataInHoldTankDTO>());
                }
            }
            else
            {
                // Return an empty list if sponsor is null
                return Ok(new List<CustomerDataInHoldTankDTO>());
            }
        }





        ///add parent to new member

        [HttpPost("AddParentToNewMember")]
        public async Task<IActionResult> AddParentToNewMember(AddParentToNewMemberDTO dto)
        {
            if (dto.handSide == "Right" || dto.handSide == "Left")
            {
                var childsofthisparent = await momDb.CustomerNetwork
       .Where(c => c.ParentId == dto.parentId)
       .ToListAsync();
                if (childsofthisparent.Count == 2)
                {
                    return Ok("This customer is parent for 2 childrens, please enter another customer");
                }
                else if (childsofthisparent.Count == 1)
                {
                    //check if the handside == handside enter or no 
                    if (childsofthisparent[0].HandSide == dto.handSide)
                    {
                        return Ok("This hand side is found in this customer, please choose another hand side");
                    }
                    else
                    {
                        var child = await momDb.CustomerNetwork
                           .FirstOrDefaultAsync(c => c.ChildId == dto.childId);

                        child.ParentId = dto.parentId;
                        child.HandSide = dto.handSide;

                        await momDb.SaveChangesAsync();
                        return Ok(child);

                    }

                }


                else
                {
                    var child = await momDb.CustomerNetwork
                            .FirstOrDefaultAsync(c => c.ChildId == dto.childId);

                    child.ParentId = dto.parentId;
                    child.HandSide = dto.handSide;

                    await momDb.SaveChangesAsync();
                    return Ok(child);
                }



            }

            else
            {
                return Ok("Please Enter Valid Hand Side");
            }

        }




        #region  tokens
        // Method to generate a random token
        private string GenerateRandomToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            var token = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return token;
        }

        // Controller action for generating token
        //create token
        [HttpPost("RequestToken")]
        public async Task<IActionResult> RequestToken(RequestTokenDTO dto)
        {
            var userBalance = await momDb.NsStartingBalances
                .Where(u => u.NsId == dto.userId).
                OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (userBalance != null)
            {
                if (userBalance.Remain == 0 && userBalance.paid != 0)
                {
                    return Ok("Your balance = 0");
                }
                else
                {
                    // Check if the user's balance is sufficient for generating a token
                    if (userBalance.Remain >= dto.tokenValue)
                    {
                        //decrease the token value from ns starting balance
                        userBalance.Remain = userBalance.Remain - dto.tokenValue;
                        userBalance.paid = userBalance.paid + dto.tokenValue;

                        await momDb.SaveChangesAsync();



                        string token;
                        PayToken existingToken;

                        // Generate a unique token
                        do
                        {
                            token = dto.userId.ToString() + GenerateRandomToken(8);
                            existingToken = await momDb.PayTokens.FirstOrDefaultAsync(x => x.Token == token);
                        } while (existingToken != null);

                        // Create and save the token
                        var pay = new PayToken
                        {
                            Id = 0,
                            CustomerId = dto.userId,
                            Token = token,
                            Value = dto.tokenValue,
                            IsUsed = false
                        };

                        momDb.PayTokens.Add(pay);
                        await momDb.SaveChangesAsync();
                        return Ok(pay);
                    }
                    else
                    {
                        return BadRequest("Your balance is less than the token value, Your balance = " + userBalance.Remain);
                    }
                }
            }
            else
            {
                return BadRequest("This user doesn't have a balance");
            }
        }


    [HttpPost("RequestTokenFromSignUpBalance")]
     public async Task<IActionResult> RequestTokenFromSignUpBalanceuestToken(RequestTokenDTO dto)
        {
            var userBalance = await momDb.CustomerAccountBalanceSingUps
                .Where(u => u.CustomerId == dto.userId).
                OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (userBalance != null)
            {
                if (userBalance.Balance == 0)
                {
                    return BadRequest("Your balance = 0");
                }
                else
                {
                    // Check if the user's balance is sufficient for generating a token
                    if (userBalance.Balance >= dto.tokenValue)
                    {
                        string token;
                        PayToken existingToken;

                        // Generate a unique token
                        do
                        {
                            token = dto.userId.ToString() + GenerateRandomToken(8);
                            existingToken = await momDb.PayTokens.FirstOrDefaultAsync(x => x.Token == token);
                        } while (existingToken != null);

                        // Create and save the token
                        var pay = new PayToken
                        {
                            Id = 0,
                            CustomerId = dto.userId,
                            Token = token,
                            Value = dto.tokenValue,
                            IsUsed = false
                        };

                        momDb.PayTokens.Add(pay);
                        await momDb.SaveChangesAsync();

                        var customersignup = new CustomerAccountBalanceSingUp
                        {
                            Id = 0,
                            CustomerId =dto.userId,
                            Debit = dto.tokenValue,
                            Credit = 0,
                            Balance = userBalance.Balance - pay.Value,
                            Description = $@"Request token {token} its value {dto.tokenValue}",
                            TransactionDate = DateTime.Now,
                        };
                        momDb.CustomerAccountBalanceSingUps.Add(customersignup);   
                        await momDb.SaveChangesAsync();

                        return Ok(pay);
                    }
                    else
                    {
                        return BadRequest("Your balance is less than the token value, Your balance = " + userBalance.Balance);
                    }
                }
            }
            else
            {
                return BadRequest("This user doesn't have a balance");
            }
        }










       





        [HttpGet("GetUserTokens")]
        public async Task<IActionResult> GetUserTokens(int userId)
        {
            var tokens = await momDb.PayTokens
                .Where(u => u.CustomerId == userId)
                .ToListAsync();

            if (tokens.Count != 0)
            {
                List<GetTokensDTO> UserTokens = new List<GetTokensDTO>();

                foreach (var token in tokens)
                {
                    var customerPaid = await momDb.CustomerAttributes
                        .Where(c => c.Id == token.CreatedBy)
                        .Include(c => c.CustomerInfo)
                        .FirstOrDefaultAsync();

                    GetTokensDTO userToken; // Explicitly specify the type

                    if (customerPaid == null)
                    {
                        userToken = new GetTokensDTO
                        {
                            Id = token.Id,
                            CustomerId = token.CustomerId,
                            Token = token.Token,
                            Value = token.Value,
                            IsUsed = token.IsUsed,
                            CreatedDate = token.CreationDate ?? DateTime.MinValue,
                            Paidby = "",
                            BackOfficeId = "",
                            PaidDate = null, // Provide a default value instead of null
                        };
                    }
                    else
                    {
                        userToken = new GetTokensDTO
                        {
                            Id = token.Id,
                            CustomerId = token.CustomerId,
                            Token = token.Token,
                            Value = token.Value,
                            IsUsed = token.IsUsed,
                            CreatedDate = token.CreationDate ?? DateTime.MinValue,
                            Paidby = customerPaid.CustomerInfo.NameEn,
                            BackOfficeId = customerPaid.ReferId,
                            PaidDate = token.LastUpdateDate,
                        };
                    }

                    UserTokens.Add(userToken);
                }


                return Ok(UserTokens);
            }
            else
            {
                return Ok("You dont have any tokens");
            }


        }


        [HttpGet("SearchAboutToken")]
        public async Task<IActionResult> SearchAboutToken(string tokenNumber)
        {
            try
            {
                // Find the token by token number
                var token = await momDb.PayTokens.FirstOrDefaultAsync(t => t.Token == tokenNumber);

                // If token not found, return 404 Not Found
                if (token == null)
                    return NotFound("Token not found");

                // If token is not used, return 400 Bad Request
                if (token.IsUsed == false)
                    return BadRequest("Token not used before");

                // Variables to hold user data and image URL
                GetCustomerDataDTO userData = null;
                string imgurl = string.Empty;

                // Fetch user data if CreatedBy is not null
                if (token.CreatedBy != null)
                {
                    userData = await GetUserData((int)token.CreatedBy);
                    imgurl = userData.PictureUrl; // Assuming the image URL is part of user data
                }

                // Construct the DTO with relevant data
                var userToken = new TokenDTO
                {
                    Id = token.Id,
                    ImgUrl = imgurl,
                    CustomerId = token.CustomerId,
                    Email = userData?.Email,
                    Token = token.Token,
                    Mobile = userData?.Mobile,
                    WhatsApp = userData?.whatsappmobile,
                    NationalId = userData?.NationalId,
                    Country = userData?.CountryName,
                    Role = userData?.Role,
                    StartDate = userData?.StartDate,
                    Value = token.Value,
                    IsUsed = token.IsUsed,
                    CreatedDate = token.CreationDate ?? DateTime.MinValue,
                    Paidby = userData?.NameEn,
                    BackOfficeId = userData?.BackOfficeId,
                    SponsorId = userData?.SponsorId,
                    PaidDate = token.LastUpdateDate
                };

                // Return 200 OK with the DTO
                return Ok(userToken);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return 500 Internal Server Error
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpGet("SearchAboutToken")]
        //public async Task<IActionResult> SearchAboutToken(string tokenNumber)
        //{
        //    try
        //    {
        //        // Find the token by token number
        //        var token = await momDb.PayTokens.FirstOrDefaultAsync(t => t.Token == tokenNumber);

        //        // If token not found, return 404 Not Found
        //        if (token == null)
        //            return NotFound("Token not found");

        //        if(token.IsUsed == false)
        //            return BadRequest("Token not used before");


        //       // CustomerAttribute customerPaid = null;

        //       // string imgurl = "";

        //        if (token.CreatedBy != null)
        //            GetCustomerDataDTO userData = await GetUserData((int)token.CreatedBy);




        //        var userToken = new TokenDTO
        //        {
        //            Id = token.Id,
        //            ImgUrl =imgurl,
        //            CustomerId = token.CustomerId,
        //            Email = customerPaid?.CustomerInfo.Email,
        //            Token = token.Token,
        //            Mobile = customerPaid?.CustomerInfo.Mobile,
        //            WhatsApp = customerPaid?.CustomerInfo.whatsappmobile,
        //            NationalId = customerPaid?.CustomerInfo.NationalId,
        //            Country = customerPaid?.CustomerInfo.Country.name,
        //            Role = customerPaid?.CustomerInfo.Role.Name,
        //            StartDate = customerPaid?.CustomerInfo.CreationDate,
        //            Value = token.Value,
        //            IsUsed = token.IsUsed,
        //            CreatedDate = token.CreationDate ?? DateTime.MinValue,
        //            Paidby = customerPaid?.CustomerInfo?.NameEn,
        //            BackOfficeId = customerPaid?.ReferId,
        //            PaidDate = token.LastUpdateDate
        //        };

        //        // Return 200 OK with DTO
        //        return Ok(userToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions and return 500 Internal Server Error
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}




        [HttpGet("Sum_NumberOfTokensPaidAndUnpaid")]
        public async Task<IActionResult> Sum_NumberOfTokensPaidAndUnpaid(int userId)
        {
            var tokens = await momDb.PayTokens
                            .Where(u => u.CustomerId == userId)
                            .ToListAsync();

            var unusedTokens = tokens.Where(t => !t.IsUsed);
            var usedTokens = tokens.Where(t => t.IsUsed);

            var sumOfUnusedTokens = unusedTokens.Sum(t => t.Value);
            var sumOfUsedTokens = usedTokens.Sum(t => t.Value);
            
            var NumberOfUnusedTokens = unusedTokens.Count();
            var NumberOfUsedTokens = usedTokens.Count();

            // Return the sums
            return Ok(new
            {
                SumOfUnusedTokens = sumOfUnusedTokens,
                NumberOfUnusedTokens = NumberOfUnusedTokens,
               
                SumOfUsedTokens = sumOfUsedTokens,
                NumberOfUsedTokens = NumberOfUsedTokens

            });
        }


        #endregion


        //GetNSStartingBalance 
        [HttpGet("GetYourBalance")]
        public async Task<IActionResult> GetYourBalance(int userId)
        {
            var balance = await momDb.NsStartingBalances
                .Where(u => u.NsId == userId)
                .OrderByDescending(c=>c.Id)
                .FirstOrDefaultAsync();


            if (balance != null)
            {
                return Ok(balance);
            }
            else
            {
                return BadRequest("This user dont have balance");
            }

        }

        #region Check if Token used or no
       
        
        [HttpGet("CheckTokenStatus")]
        public async Task<IActionResult> CheckTokenStatus(string tokenNumber)
        {
            var token = await momDb.PayTokens.FirstOrDefaultAsync(t => t.Token == tokenNumber);

            if (token == null)
            {
                return NotFound("Token not found");
            }

            // Return token status and value
            return Ok(new { IsUsed = token.IsUsed, Value = token.Value });
        }
        #endregion


        #region   buy packages

        [HttpPost("BuyPackages")]
        public async Task<IActionResult> BuyPackages(BuyPackageDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(errors);
            }

       
            List<decimal> TokenPoints = new List<decimal>();
            List<decimal> PackagesPrices = new List<decimal>();

            // Check for duplicate tokens
            var duplicateTokens = dto.Tokens.GroupBy(x => x)
                                             .Where(g => g.Count() > 1)
                                             .Select(y => y.Key)
                                             .ToList();

            if (duplicateTokens.Any())
            {
                return BadRequest($"Duplicate tokens found: {string.Join(", ", duplicateTokens)}");
            }


            // Collect package prices
            foreach (var id in dto.PackagesIds)
            {
                var pack = await momDb.Packages.FirstOrDefaultAsync(p => p.Id == id);
                if (pack != null)
                {
                    PackagesPrices.Add((decimal)pack.Price);
                }
            }

            // Collect token values
            foreach (var token in dto.Tokens)
            {
                var value = await momDb.PayTokens
                    .Where(t => t.Token == token && !t.IsUsed)
                    .Select(t => t.Value)
                    .FirstOrDefaultAsync();

                if (value != default(decimal))
                {
                    TokenPoints.Add(value);
                }
            }

            if (TokenPoints.Count == 0)
            {
                return BadRequest("Tokens are Invalid");
            }

        

            decimal totalTokenValue = TokenPoints.Sum();
            decimal totalPackagePrice = PackagesPrices.Sum();

            if (totalTokenValue > totalPackagePrice)
            {
                return BadRequest("Tokens values more than packages prices.");
            }


            if (totalTokenValue <  totalPackagePrice)
            {
                return BadRequest("Tokens values less than packages prices.");
            }



            // Process payment
            foreach (var token in dto.Tokens)
            {
                var t = await momDb.PayTokens.FirstOrDefaultAsync(s => s.Token == token && !s.IsUsed);
                if (t != null)
                {
                    t.IsUsed = true;
                    t.CreatedBy = dto.customerAttributeId;
                    t.LastUpdateDate = DateTime.Now;
                    await momDb.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Token has already been used.");
                }
            }

            // Process package selection

            // Generate invoice serial
            Random rand = new Random();
            string invoice = dto.customerAttributeId.ToString() + rand.Next(1000, 10000).ToString();

            foreach (var packId in dto.PackagesIds)
            {

                var packageSelected = await momDb
                    .Packages.FirstOrDefaultAsync(p => p.Id == packId);

                if (packageSelected != null)
                {


                    // return note package
                    var note = await momDb.PackagesTypes
                        .Where(t => t.Id == packageSelected.PackageTypeId)
                        .Select(s => s.Name)
                        .FirstOrDefaultAsync();

                    if (note == "Renwall") 
                    {
                      
                    }
                    var cus = await momDb.CustomerAttributes
                          .Where(c => c.Id == dto.customerAttributeId)
                          .Include(c => c.CustomerInfo)
                          .FirstOrDefaultAsync();

                    if (cus != null)
                        cus.RenewalDateUtc = DateTime.UtcNow.AddYears(1);
                    cus.Renewal = true;
                    await momDb.SaveChangesAsync();

                    //to change role from inactive to active

                    if (cus.CustomerInfo != null)
                        cus.CustomerInfo.RoleId = 3;

                    await momDb.SaveChangesAsync();


                    var customerPackage = new CustomerPackageSelect
                    {
                        CustomerId = dto.customerAttributeId,
                        PackagesId = packageSelected.Id,
                        PackageTypeId = packageSelected.PackageTypeId,
                        Cost = packageSelected.Price,
                        OrignalCost = packageSelected.Price,
                        CreationDate = DateTime.Now,
                        InvoiceSerial = invoice,
                        Notes = note,
                        IsCompleted = true
                    };
                    momDb.Customerselectpackages.Add(customerPackage);
                    await momDb.SaveChangesAsync();

                    //point process for distrubistor to distributor
                    var pointDistributor = await Add_PointProcess_To_Sponsor(new AddPointProcessDTO() { SponsorId = dto.customerAttributeId, PackageId = packId, ProcessTypeId = 1, Value = (decimal)packageSelected.SponsorDistributorToDistributor });
                    //point process for Buisness Value
                    var pointBuisness = await Add_PointProcess_To_Sponsor(new AddPointProcessDTO() { SponsorId = dto.customerAttributeId, PackageId = packId, ProcessTypeId = 3, Value = (decimal)packageSelected.BusinessValue });

                 
                    //sponsor Id from customer network table 
                    var sponsor_Id = await momDb.CustomerNetwork
                           .Where(ss => ss.ChildId == dto.customerAttributeId)
                           .Select(nn => nn.SponsorId)
                           .FirstOrDefaultAsync();

                    if (sponsor_Id != null)
                    {
                       // Profit profit = null;
                        var cust = await momDb.CustomerAttributes.Where(c => c.Id == sponsor_Id).Include(c => c.CustomerInfo).FirstOrDefaultAsync();
                        if (cust?.CustomerInfo.RoleId == 3 && cust.Renewal == true)
                        {
                            await Add_Profit_To_Sponsor(new AddProfitDTO() { SponsorId = sponsor_Id.Value, Bonus_Comission = (decimal)packageSelected.SponsorDistributorToDistributor, IsPaid = false, PointProcessId = pointDistributor.Id, ProcessTypeId = 1 });


                         
                        }

                        //////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\    ///////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\   /////////////////////////\\\\\\\\\\\\\\\\\\\\
                     /*   var Bonus_Generation_Comission = await momDb.Generation.Where(c => c.Note == "Direct Bonus Generation").ToListAsync();
                        int currentSponsorId =(int) sponsor_Id;
                       
                        var Process_types = await momDb.ProcessType.Where(c => c.Process.Contains("Direct Bounce Generation")).OrderBy(c => c.Id).ToListAsync();
                        int processTypeIndex = 0;


                        foreach (var bonus in Bonus_Generation_Comission)
                        {
                            //if the iterations not finished
                            while (true)
                            {
                                var sponsor = await momDb.CustomerNetwork.Where(c => c.ChildId == currentSponsorId).Select(c => c.SponsorId).FirstOrDefaultAsync();
                                if (sponsor == 2 || sponsor == null) break;
                                // iif sponsor != null
                                var customer_Renwal = await momDb.CustomerAttributes.Where(c => c.Id == sponsor).Include(c => c.CustomerInfo).FirstOrDefaultAsync();
                                if (customer_Renwal != null && customer_Renwal.Renewal == true && customer_Renwal.CustomerInfo.RoleId == 3)
                                {
                                    if (customer_Renwal.RankId >= bonus.RankId)
                                    {
                                        var comission = (packageSelected.SponsorDistributorToDistributor) * bonus.GenerationComission;
                                        //Bonus Direct Generation
                                        var addSponsorPoints = Add_PointProcess_To_Sponsor(new AddPointProcessDTO() { SponsorId = (int)sponsor, PackageId = packId, ProcessTypeId = 5, Value = (decimal)comission });
                                        await Add_Profit_To_Sponsor(new AddProfitDTO() { SponsorId = (int)sponsor, Bonus_Comission = (decimal)comission, IsPaid = false, PointProcessId = addSponsorPoints.Id, ProcessTypeId = Process_types[processTypeIndex].Id });
                                        processTypeIndex = (processTypeIndex + 1) % Process_types.Count; //the next index
                                    }
                                    else  // if rank < bonus.RankId
                                    {
                                        currentSponsorId = sponsor.Value;
                                        processTypeIndex = (processTypeIndex + 1) % Process_types.Count; //the next index
                                        continue;
                                    }
                                }
                                else  //if user not active and not renwall
                                {
                                    currentSponsorId = sponsor.Value;
                                    processTypeIndex = (processTypeIndex + 1) % Process_types.Count; //the next index
                                    continue;
                                }
                                currentSponsorId = sponsor.Value;   // to get new child
                            }
                        }  */
                        //////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\    ///////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\   /////////////////////////\\\\\\\\\\\\\\\\\\\\




                        var customerData = await momDb.CustomerAttributes
        .Where(c => c.Id == dto.customerAttributeId).Include(c => c.CustomerInfo)
      .FirstOrDefaultAsync();

                        var sponsorData = await momDb.CustomerAttributes
      .Where(c => c.Id == sponsor_Id).Include(c => c.CustomerInfo).FirstOrDefaultAsync();

                        decimal profitValue = packageSelected.SponsorDistributorToDistributor ?? 0; // Provide a default value if profit is null


                        await mailing.SendEmailToSponsorWhenHisChildBuyPackage(
                            sponsorData.CustomerInfo.Email,
                           sponsorData.CustomerInfo.NameEn,
                            customerData.ReferId,
                            profitValue,
                            customerData.CustomerInfo.NameEn
                        );

                    }


                }
                else
                {
                    return BadRequest("Package not found");
                }
            }

            //CustomerAccountBalanceSingUp
            var tokensString = string.Join(", ", dto.Tokens);

            var cusAccountBalanceSignup = new CustomerAccountBalanceSingUp
            {
                Id = 0,
                CustomerId = dto.customerAttributeId,
                Debit = TokenPoints.Sum(),
                Credit = 0,
                Balance = 0,
                TransactionDate = DateTime.Now,
                Description = "Purchase for tokens: " + tokensString + "\n" + "Invoice Serial: " + invoice
            };
            momDb.CustomerAccountBalanceSingUps.Add(cusAccountBalanceSignup);
            await momDb.SaveChangesAsync();




            return Ok("You paid Successfully");
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Profit> Add_Profit_To_Sponsor(AddProfitDTO dto)
        {
            var profit = new Profit
            {
                DistributorId = dto.SponsorId,
                Profit1 = dto.Bonus_Comission,   //generation comission
                IsPaid = dto.IsPaid,
                PaymentDate = null,
                PointProcessId = dto.PointProcessId,
                ProcessTypeId = dto.ProcessTypeId,      //direct bonus generation 1
                ProfitDate = DateTime.Now
            };
            momDb.Profit.Add(profit);
            await momDb.SaveChangesAsync();

            return profit;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<PointProcess> Add_PointProcess_To_Sponsor(AddPointProcessDTO dto)
        {
            var point_process = new PointProcess
            {
                ForCustomerId = dto.SponsorId,
                PackageId = dto.PackageId,
                ProcessTypeId = dto.ProcessTypeId,
                Value = dto.Value
            };

            momDb.PointProcess.Add(point_process);
            await momDb.SaveChangesAsync();

            return point_process;
        }






        #endregion









        [HttpGet("Get_HoldAmount_and_TotalAmount")]
        public async Task<IActionResult> GetHoldAmountAndTotalAmount(int customerAttributeId)
        {
            try
            {
                DateTime todayDate = DateTime.Today;

                // Find the start and end dates of the current week (previous week's Friday to current week's Thursday)
                DateTime reportFromDate = todayDate
                                .AddDays(-(int)todayDate.DayOfWeek + (int)DayOfWeek.Sunday - 6);
                DateTime reportToDate = reportFromDate.AddDays(6);


                // Calculate HoldAmount for the current week
                var holdAmount = await momDb.Profit
                    .Where(p => p.DistributorId == customerAttributeId &&
                                p.IsPaid == false &&
                                p.ProfitDate >= reportFromDate &&
                                p.ProfitDate <= reportToDate)
                    .SumAsync(p => p.Profit1);

                var avaliable = await momDb.CustomerAccountBalances
                    .Where(c => c.CustomerId == customerAttributeId)
                        .OrderByDescending(c => c.Id).LastOrDefaultAsync();



                // Calculate TotalAmount for all time
                var totalAmount = await momDb.Profit
                    .Where(p => p.DistributorId == customerAttributeId)
                    .SumAsync(p => p.Profit1);
                return Ok(new { HoldAmount = holdAmount, AvaliableAmount = avaliable.Balance, TotalAmount = totalAmount, from = reportFromDate, to = reportToDate });


            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        #region  Create Trainning

        [HttpPost("CreateTrainning")]
        public async Task<IActionResult> CreateTrainning(CreateTrainningDTO dto)
        {
            var user = await momDb.CustomerAttributes
      .Where(c => c.Id == dto.CreatedBy)
      .Include(c => c.CustomerInfo)
      .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("This user Not found");
            }
            else
            {

                Trainning existingcode;
                string code = "";
                do
                {
                    code = dto.CreatedBy.ToString() + GenerateRandomToken(8);
                    existingcode = await momDb.Trainning.FirstOrDefaultAsync(x => x.TrainningCode == code);
                } while (existingcode != null);

              //  string url = $"https://livezoon.com/join?room={code}&name={dto.CreatedBy}&audio=false&video=false&screen=false&notify=false";

               


                var trainning = new Trainning
                {
                    Id = 0,
                    TrainningName = dto.TrainningName,
                    CreatedBy = dto.CreatedBy,
                    TrainningCode = code,
                    Date = dto.Date,
                    //Url = url
                };

                momDb.Trainning.Add(trainning);
                await momDb.SaveChangesAsync();


                await AddToTrainingMapping(trainning.Id, dto.CreatedBy);
                //var trainingforcustomer = new TrainningMapping
                //{
                //    Id = 0,
                //    TrainningId = trainning.Id,
                //    CustomerId = dto.CreatedBy
                //};
                //momDb.TrainningMapping.Add(trainingforcustomer);
                //await momDb.SaveChangesAsync();

                var listOfChildIds = await momDb.CustomerNetwork
             .Where(c => EF.Functions.Like(c.UplineHistoryId, $"%/{dto.CreatedBy}/%"))
             .Select(c => c.ChildId)
             .ToListAsync();

                if(listOfChildIds.Count !=0)
                {
                    foreach(var childId in listOfChildIds)
                    {

                        await AddToTrainingMapping(trainning.Id, (int)childId);

                        //var trainingMap = new TrainningMapping
                        //{
                        //     Id = 0,
                        //     TrainningId= trainning.Id,
                        //     CustomerId = (int)childId
                        //};
                        //momDb.TrainningMapping.Add(trainingMap);
                        //await momDb.SaveChangesAsync();

                    }


                }


                return Ok(trainning);

            }

        }




        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task AddToTrainingMapping(int trainingId, int customerId)
        {
            var trainingForCustomer = new TrainningMapping
            {
                Id = 0,
                TrainningId = trainingId,
                CustomerId = customerId
            };

            momDb.TrainningMapping.Add(trainingForCustomer);
            await momDb.SaveChangesAsync();
        }



        [HttpGet("GetAllTrainningsForCustomer")]
        public async Task<IActionResult> GetAllTrainningsForCustomer(int customerAttribueId)
        {

            var meetings = await momDb.TrainningMapping
     .Where(tm => tm.CustomerId == customerAttribueId)
     .Join(
         momDb.Trainning,
         tm => tm.TrainningId,
         t => t.Id,
         (tm, t) => new { TrainningMapping = tm, Trainning = t }
     )
     .Select(result => new
     {
         TrainingMappingId = result.TrainningMapping.Id,
         TrainingId = result.Trainning.Id,
         TrainingName = result.Trainning.TrainningName,
         TrainingDate = result.Trainning.Date,
         //TrainingUrl = result.Trainning.Url,
         CustomerId = result.TrainningMapping.CustomerId,
         TrainningCode = result.Trainning.TrainningCode
     }).OrderBy(c=>c.TrainingDate)
     .ToListAsync();

            if(meetings.Count != 0)
            {
                return Ok(meetings);
            }
            else
            {
                return NotFound("Not found any meetings");
            }




        }


        [HttpGet("GetTrainningByCode")]
        public async Task<IActionResult> GetTrainningByCode(string TrainningCode )
        {

            var meetings = await momDb.Trainning
     .FirstOrDefaultAsync(t => t.TrainningCode == TrainningCode);
   

            if(meetings !=null)
               return Ok(meetings);
            return NotFound("Not found meeting for this code ");
            




        }

        #endregion




        #region  Create live course meating


        [HttpPost("CreateLiveCourseMeeting")]
        public async Task<IActionResult> CreateLiveCourseMeeting(CreateLiveCourseMeetingDTO dto)
        {
            var instructor = await momDb.Instructors.FirstOrDefaultAsync(c => c.Id == dto.InstructorId);

            if (instructor == null)
            {
                return BadRequest("This Instructor Not found");
            }
            else
            {
                var course = await momDb.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);

                if (course == null)
                    return BadRequest("This Course Not found");
                

                LiveCourseTrainning existingcode;
                string code = "";
                do
                {
                    code = dto.InstructorId.ToString() + GenerateRandomToken(8);
                    existingcode = await momDb.LiveCourseTrainning.FirstOrDefaultAsync(x => x.MeetingCode == code);
                } while (existingcode != null);


                //string url = $"https://livezoon.com/join?room={code}&name={dto.InstructorId}&audio=false&video=false&screen=false&notify=false";

                var meeting = new LiveCourseTrainning
                {
                    Id = 0,
                    CourseId = dto.CourseId,
                    InstructorId = dto.InstructorId,
                    LectureLiveName = dto.LectureLiveName,
                    CreatedBy = dto.InstructorId,
                    Date = dto.Date,
                  //  Url = url,
                    MeetingCode = code,
                    CreationDate = DateTime.Now,
                    IsActive = true
                };

                momDb.LiveCourseTrainning.Add(meeting);
                await momDb.SaveChangesAsync();

                // Return a success response
                return Ok("Live course meeting created successfully.");
            }
        }

        [HttpGet("GetAllLiveCourseMeetingsForCustomer")]
        public async Task<IActionResult> GetAllLiveCourseMeetingsForCustomer(int customerAttributeId)
        {
            var customerCourses = await momDb.CourseCustomerMapping
                .Where(c => c.CustomerId == customerAttributeId && c.Notes == "course live")
                .ToListAsync();

            if (customerCourses.Count == 0)
            {
                return NotFound("You don't have any live courses");
            }

            List<LiveCourseTrainning> customerMeetings = new List<LiveCourseTrainning>();

            foreach (var courseMapping in customerCourses)
            {
                var courseMeetings = await momDb.LiveCourseTrainning
            .Where(c => c.CourseId == courseMapping.CourseId)
            .ToListAsync();

                if (courseMeetings.Count != 0)
                {
                    customerMeetings.AddRange(courseMeetings);
                }
            }
            customerMeetings = customerMeetings.OrderBy(c => c.Date).ToList();
            return Ok(customerMeetings);
        }


        #endregion 

    }
}




#region comment


//[HttpPost("send-message")]
//public async Task<IActionResult> SendMessage(string chatId, string message)
//{
//    await _telegramService.SendMessageAsync(chatId, message);
//    return Ok("Message sent successfully");
//}

//[HttpPost("SendMessageTele")]
//public async Task<IActionResult> SendMessageTele(string message)
//{

//    TelegramBotClient client = new
//        TelegramBotClient("6935466790:AAHBegNUuZw8DK2bvNYVfruK4MUGl626l9E");
//    var botInfo = await client.GetMeAsync();

//    long botId = botInfo.Id;

//    await client.SendTextMessageAsync(botId, message);


//    return Ok("Message sent successfully");
//}







//[HttpPost("update")]
//public async Task<IActionResult> Update([FromBody] Update update)
//{
//    // Check if the update contains a message and the message has text
//    if (update.Message != null && !string.IsNullOrWhiteSpace(update.Message.Text))
//    {
//        // Get the chat ID from the incoming message
//        long chatId = update.Message.Chat.Id;

//        // Process the incoming message (optional)
//        // For example, you could echo back the received message
//        await _botClient.SendTextMessageAsync(chatId, $"You said: {update.Message.Text}");

//        // Return a success response
//        return Ok("ssssssssssssssssssssss");
//    }
//    else
//    {
//        // Return a bad request response if the update does not contain a message or the message has no text
//        return BadRequest();
//    }
//}

///2164746833857898

//[HttpPost("SendMessageToPhoneNumber")]
//public async Task<IActionResult> SendMessageToPhoneNumber(string tonumber, string message)
//{

//    var accountSid = "ACb3042a4508edf8e012580563e94f5799";
//    var authToken = "ecf2af11ed97954bf23617557fd83c47"; // Replace with your actual AuthToken
//    var to = tonumber;
//    var from = "+13347216210";
//    var body = message;

//    //var baseUrl = "https://api.twilio.com/2010-04-01/";

//    var baseUrl = "https://demo.twilio.com/welcome/sms/reply/";

//    var endpoint = $"Accounts/{accountSid}/Messages.json";

//    var client = new HttpClient();
//    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
//        Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}")));

//    var formData = new FormUrlEncodedContent(new[]
//    {
//                new KeyValuePair<string, string>("To", to),
//                new KeyValuePair<string, string>("From", from),
//                new KeyValuePair<string, string>("Body", body)
//            });

//    var response = await client.PostAsync(baseUrl + endpoint, formData);
//    if (response.IsSuccessStatusCode)
//    {
//        return Ok("Message sent successfully.");
//    }
//    else

//        return Ok($"Error sending message");
//}





/////zoooooomm
//[HttpGet("CreateZoomMeeting")]
//public async Task<IActionResult> CreateZoomMeeting()
//{
//    var ZoomApiKey = "rk39kikcSxKnF25EVyNiw";
//    var ZoomApiSecret = "4LdNL1xOUY6aoO8S1lMQ54921VAlfUsY";

//    try
//    {
//        var meetingTopic = "Test Meeting";
//        var agenda = "Meeting agenda"; // Provide agenda for the meeting
//        var start = DateTime.UtcNow.AddHours(1); // Set start time (example: 1 hour from now)
//        var duration = 60; // Duration in minutes

//        var zoomClient = new ZoomClient(new ZoomNet.JwtConnectionInfo(ZoomApiKey, ZoomApiSecret), new ZoomNet.Utilities.ZoomClientOptions());

//        var response = await zoomClient.Meetings.CreateScheduledMeetingAsync(
//            userId: "your_user_id", // Provide the user ID for whom the meeting is scheduled
//            topic: meetingTopic,
//            agenda: agenda,
//            start: start,
//            duration: duration
//        );

//        return Ok(response);
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, ex.Message);
//    }
//}


//[HttpPost("CreateMeeting")]
//public async Task<IActionResult> CreateMeeting()
//{
//    var  ZoomApiKey = "rk39kikcSxKnF25EVyNiw";
//    var ZoomApiSecret = "4LdNL1xOUY6aoO8S1lMQ54921VAlfUsY";

//    try
//    {
//        var meeting = new ScheduledMeeting
//        {
//            Topic = "Test Meeting",
//            StartTime = DateTime.UtcNow.AddHours(1), // Set start time (example: 1 hour from now)
//            Duration = 60, // Duration in minutes
//            Type = MeetingType.Scheduled
//        };

//        var zoomClient = new ZoomClient(new ZoomNet.JwtConnectionInfo
//            (ZoomApiKey, ZoomApiSecret), new ZoomNet.Utilities.ZoomClientOptions());


//        //   var response = await zoomClient.Meetings.CreateMeetingAsync(meeting);


//        var response = await zoomClient.Meetings
//            .CreateScheduledMeetingAsync(meeting);


//        return Ok(response);
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, ex.Message);
//    }
//}



//[HttpPost("SendWhatsAppMessage")]
//public async Task<IActionResult> SendWhatsAppMessage(string toNumber, string message)
//{
//    string testAccountSid = "ACfb81071821fa8c408652d23235e90d02";
//    string testAuthToken = "d42fda5aa0f0a484f0c156daebcc7559";

//    WhatsAppService whatsappService = new WhatsAppService(testAccountSid, testAuthToken);
//    whatsappService.SendMessage(toNumber, message);

//    return Ok("Send massege successfully");

//    //string testAccountSid = "ACfb81071821fa8c408652d23235e90d02";
//    //string testAuthToken = "d42fda5aa0f0a484f0c156daebcc7559";

//    //string testAccountSid = "ACb3042a4508edf8e012580563e94f5799";
//    //string testAuthToken = "ecf2af11ed97954bf23617557fd83c47";


//}

#endregion
