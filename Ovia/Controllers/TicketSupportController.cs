using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services.SendEmails;
using Ovia.Services.StorageFiles;
using System.Text.Json.Serialization;


namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketSupportController : ControllerBase
    {
        private readonly MomEntity momDb;
        private readonly IStorageService storage;
        private readonly IMailingServices mailingService;
        public TicketSupportController(
            MomEntity _momDb,
            IStorageService _storage,
            IMailingServices _mailingService

            )
        {
            momDb = _momDb;
            storage = _storage;
            mailingService = _mailingService;

        }


        #region ticket support

        //add Complaint 
        [HttpPost("AddComplaint")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddComplaint([FromForm] AddComplaintDTO dto)
        {

            try
            {
                var user = await momDb.CustomerAttributes
                    .Where(u => u.Id == dto.UserId)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    var complaint = new Complaint
                    {
                        UserId = dto.UserId,
                        Subject = dto.Subject,
                        Question = dto.Question,
                        Status = TicketEnum.Open
                    };
                    momDb.Complaint.Add(complaint);
                    await momDb.SaveChangesAsync();


                    if (dto.File != null)
                    {
                        var uploadedFile = await storage.Upload(dto.File);

                        var attach = new ComplaintAttachment
                        {
                            ComplaintId = complaint.Id,
                            FileKey = uploadedFile.Key,
                            FileName = uploadedFile.FileName,
                            FileExtension = uploadedFile.Extension,
                            FileSize = uploadedFile.FileSize
                        };

                        momDb.ComplaintAttachment.Add(attach);
                        await momDb.SaveChangesAsync();
                    }


                    return Ok("Ticket support sent successfully");
                }

                else
                {
                    return NotFound("User not found");
                }



            }

            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error

            }

        }


        //add Complaint Reply
        [HttpPost("AddComplaintReply")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddComplaintReply([FromForm] AddComplaintReplyDTO dto)
        {
            try
            {
                var Comp = await momDb.Complaint
                     .Where(c => c.Id == dto.ComplaintId && c.Status != TicketEnum.Close)
                     .FirstOrDefaultAsync();

                if (Comp != null && Comp.Status != TicketEnum.Close)
                {
                    var compReply = new ComplaintReply
                    {
                        ComplaintId = dto.ComplaintId,
                        Reply = dto.Reply,
                        AdminId = dto?.AdminId ?? 0
                    };

                    momDb.ComplaintReply.Add(compReply);
                    await momDb.SaveChangesAsync();

                    if (dto.File != null)
                    {
                        var uploadedFile = await storage.Upload(dto.File);

                        var attach = new ComplaintAttachment
                        {
                            ComplaintId = dto.ComplaintId,
                            FileKey = uploadedFile.Key,
                            FileName = uploadedFile.FileName,
                            FileExtension = uploadedFile.Extension,
                            FileSize = uploadedFile.FileSize
                        };

                        momDb.ComplaintAttachment.Add(attach);
                        await momDb.SaveChangesAsync();


                        Comp.Status = TicketEnum.Reply;
                        await momDb.SaveChangesAsync();

                    }






                    //store in notification
                    // if (dto.AdminId != null)
                    // {
                    //     var adminname = await momDb
                    //         .CustomerAttributes.Where(u => u.Id == dto.AdminId)
                    //         .Include(c=> c.CustomerInfo)
                    //         .FirstOrDefaultAsync();

                    //     /////store in create project notifications
                    //     var userid = Comp.UserId;
                    //     var projectId = Comp.Id;
                    //     var adminid = dto.AdminId;
                    //     var title = @$"new reply for complaint :'{Comp.Subject}'";
                    //     var content = @$"{adminname.CustomerInfo.NameEn} replied to your complaint";
                    //     var isExistMessege = messagesDbContext.CreateProjectNotifications
                    //.Any(x => x.IsRead == false && x.ProjectId == Comp.Id && x.ClientId == adminid);
                    //     if (!isExistMessege || isExistMessege)
                    //     {
                    //         var notification = new CreateProjectNotifications(projectId, userid, adminid, title, content,
                    //             false, DateTime.Today, "Complaint");
                    //         await messagesDbContext.CreateProjectNotifications.AddAsync(notification);
                    //     }
                    //     await messagesDbContext.SaveChangesAsync();
                    //     await hubContext.Clients.User(userid.ToString()).SendAsync("ReceiveNotification", title, content);
                    // }




                    return Ok("Reply sent successfully");  // Return Ok if everything is successful
                }
                else
                {
                    return NotFound("Complaint not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error
            }
        }





        //update Complaint Status
        [HttpPut("{complaintId}/UpdateComplaintStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateComplaintStatus(int complaintId)
        {

            var complaint = await momDb.Complaint
                 .Where(c => c.Id == complaintId && c.Status != TicketEnum.Close)
                 .FirstOrDefaultAsync();

            if (complaint != null)
            {
                complaint.Status = TicketEnum.Close;
                await momDb.SaveChangesAsync();

                return Ok("Complaint status Updated Successfully!");

            }

            else
            {
                return NotFound("Complaint not found!");

            }



        }


        // Get all user complaint     
        [HttpGet("{userId}/GetUserComplaints")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ComplaintDetailsDTO>))]
        public async Task<ActionResult> GetUserComplaints(int userId)
        {
            var complaints = await momDb.Complaint
                   .Where(c => c.UserId == userId)
                   .ToListAsync();

            if (complaints.Count != 0)
            {
                return Ok(complaints);
            }
            else
            {
                return Ok("not found any complaints");
            }


        }

        //for admins
        [HttpGet("GetAllComplaintsForAdmins")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ComplaintDetailsDTO>))]
        public async Task<ActionResult> GetAllComplaintsForAdmins()
        {
            var complaints = await momDb.Complaint.ToListAsync();

            if (complaints.Count != 0)
            {
                return Ok(complaints);
            }
            else
            {
                return Ok("not found any complaints");
            }
        }




        [HttpGet("{complaintId}/GetComplaintDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ComplaintDetailsDTO>))]
        public async Task<ActionResult> GetComplaintDetails(int complaintId)
        {
            try
            {
                var complaints = await momDb.Complaint
                    .Where(c => c.Id == complaintId)
                    .FirstOrDefaultAsync();


                if (complaints != null)
                {
                    var username = await momDb.CustomerAttributes
                        .Where(u => u.Id == complaints.UserId)
                        .Include(c => c.CustomerInfo)
                        .FirstOrDefaultAsync();


                    List<ComplaintReplyDTO> repliesList = new List<ComplaintReplyDTO>();

                    var replies = await momDb.ComplaintReply
                        .Where(c => c.ComplaintId == complaints.Id)
                        .OrderBy(c => c.CreationDate) // Order by CreatedDate in ascending order (old to new)
                        .ToListAsync();

                    foreach (var reply in replies)
                    {
                        var admin = await momDb.CustomerAttributes
                            .Where(u => u.Id == reply.AdminId)
                            .FirstOrDefaultAsync();

                        var createDate = reply.CreationDate ?? DateTimeOffset.MinValue; // Assign DateTimeOffset.MinValue if reply.CreationDate is null

                        var replydto = new ComplaintReplyDTO
                        {
                            ComplaintReplyId = reply.Id, // Corrected property name
                            Reply = reply.Reply, // Assuming there's a property named ReplyId in ComplaintReplyDTO
                            CreateDate = createDate
                        };

                        if (admin != null)
                        {
                            // Admin information is available, use it
                            replydto.AdminId = admin.Id;
                            replydto.AdminName = admin.CustomerInfo.NameEn;
                            replydto.Reply = reply.Reply;
                        }
                        else
                        {
                            // Admin information is not available, mark it as a user reply
                            replydto.AdminId = complaints.UserId; // or set it to the user's ID if needed
                            replydto.AdminName = username.CustomerInfo.NameEn; // or set it to the user's name if needed
                            replydto.Reply = "User Reply: " + reply.Reply; // Mark as user reply
                        }

                        repliesList.Add(replydto);
                    }

                    List<string> urls = new List<string>();

                    var attachments = await momDb.ComplaintAttachment
                        .Where(c => c.ComplaintId == complaints.Id)
                        .ToListAsync();

                    if (attachments.Count != 0)
                    {
                        foreach (var a in attachments)
                        {
                            var url = a.FileKey.SetDownloadFileUrlByKey(storage);
                            urls.Add(url);
                        }
                    }

                    var dto = new ComplaintDetailsDTO
                    {
                        ComplaintId = complaints.Id,
                        UserId = complaints?.UserId ?? 0,
                        UserName = username.CustomerInfo.NameEn,
                        Subject = complaints.Subject,
                        Question = complaints.Question,
                        Status = complaints.Status,
                        Replies = repliesList,
                        AttachmentsUrl = urls
                    };



                    return Ok(dto);
                }
                else
                {
                    return NotFound("No complaints found for the user");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error
            }
        }













        /*
        // Get complaint details including replies and attachments
        [HttpGet("{userId}/GetUserComplaintDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ComplaintDetailsDTO>))]
        public async Task<ActionResult> GetUserComplaintDetails(Guid userId)
        {
            try
            {
                var complaints = await _settingsDbContext.Complaints
                    .Where(c => c.UserId == userId && c.Status !=TicketEnum.Close)
                    .ToListAsync();

                List<ComplaintDetailsDTO> complaintDetails = new List<ComplaintDetailsDTO>();

                if (complaints.Count != 0)
                {
                    foreach (var co in complaints)
                    {
                        List<ComplaintReplyDTO> repliesList = new List<ComplaintReplyDTO>();

                        var replies = await _settingsDbContext.ComplaintReplies
                            .Where(c => c.complaintId == co.Id)
                            .OrderBy(c => c.CreatedDate) // Order by CreatedDate in ascending order (old to new)
                            .ToListAsync();

                        if (replies.Count != 0)
                        {
                            foreach (var reply in replies)
                            {
                                var admin = await identityDbContext.Users
                                    .Where(u => u.Id == reply.AdminId)
                                    .FirstOrDefaultAsync();

                                var replydto = new ComplaintReplyDTO
                                {
                                    complaintReplyId = reply.Id,
                                    AdminId = admin?.Id, // This can be null
                                    AdminName = admin?.Name, // This can be null
                                    Reply = reply.Reply,
                                    CreateDate = reply.CreatedDate
                                };

                                repliesList.Add(replydto);
                            }
                        }

                        List<string> urls = new List<string>();

                        var attachments = await _settingsDbContext.ComplaintAttachments
                            .Where(c => c.complaintId == co.Id)
                            .ToListAsync();

                        if (attachments.Count != 0)
                        {
                            foreach (var a in attachments)
                            {
                                var url = a.FileKey.SetDownloadFileUrlByKey(storage);
                                urls.Add(url);
                            }
                        }

                        var dto = new ComplaintDetailsDTO
                        {
                            ComplaintId = co.Id,
                            UserId = co.UserId,
                            subject = co.subject,
                            Question = co.Question,
                            Status = co.Status,
                            Replies = repliesList,
                            AttachmentsUrl = urls
                        };

                        complaintDetails.Add(dto);
                    }

                    return Ok(complaintDetails);
                }
                else
                {
                    return NotFound("No complaints found for the user");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error
            }
        }

        */










        #endregion







        #region ticket support not registered

        //add ticket support
        [HttpPost("AddTicketSupport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddTicketSupport([FromForm] AddTicketSupporttDTO dto)
        {

            try
            {

                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState); // Return BadRequest with ModelState errors
                }
                var key = "";
                if (dto.File != null)
                {
                    var uploadedFile = await storage.Upload(dto.File);
                    key = uploadedFile.Key;
                }

                var ticket = new TicketSupport
                {
                    Id = 0,
                    Email = dto.Email,
                    Subject = dto.Subject,
                    Question = dto.Question,
                    Status = TicketEnum.Open,
                    Key = key
                };

                momDb.TicketSupport.Add(ticket);
                await momDb.SaveChangesAsync();
                return Ok("Ticket support sent successfully");






            }

            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error

            }

        }






        //Add Ticket Support Reply
        [HttpPost("AddTicketSupportReply")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddTicketSupportReply([FromForm] AddComplaintReplyDTO dto)
        {
            try
            {
                var Comp = await momDb.TicketSupport
                     .Where(c => c.Id == dto.ComplaintId && c.Status != TicketEnum.Close)
                     .FirstOrDefaultAsync();


                

                if (Comp != null && Comp.Status != TicketEnum.Close)
                {
                    var key = "";
                        if(dto.File != null)
                    {
                        var uploaded = await storage.Upload(dto.File);
                        key = uploaded.Key;
                    }


                    var ticketReply = new TicketSupportReply
                    {
                        TicketId = dto.ComplaintId,
                        Reply = dto.Reply,
                        AdminId = dto?.AdminId ?? 0,
                        Key = key
                    };

                    momDb.TicketSupportReply.Add(ticketReply);
                    await momDb.SaveChangesAsync();

                    Comp.Status = TicketEnum.Reply;
                    await momDb.SaveChangesAsync();



                    await mailingService.SendTicketSupportReply(Comp.Email , ticketReply.Reply,
                       dto.File !=null ? dto.File : null);





                    //store in notification
                    // if (dto.AdminId != null)
                    // {
                    //     var adminname = await momDb
                    //         .CustomerAttributes.Where(u => u.Id == dto.AdminId)
                    //         .Include(c=> c.CustomerInfo)
                    //         .FirstOrDefaultAsync();

                    //     /////store in create project notifications
                    //     var userid = Comp.UserId;
                    //     var projectId = Comp.Id;
                    //     var adminid = dto.AdminId;
                    //     var title = @$"new reply for complaint :'{Comp.Subject}'";
                    //     var content = @$"{adminname.CustomerInfo.NameEn} replied to your complaint";
                    //     var isExistMessege = messagesDbContext.CreateProjectNotifications
                    //.Any(x => x.IsRead == false && x.ProjectId == Comp.Id && x.ClientId == adminid);
                    //     if (!isExistMessege || isExistMessege)
                    //     {
                    //         var notification = new CreateProjectNotifications(projectId, userid, adminid, title, content,
                    //             false, DateTime.Today, "Complaint");
                    //         await messagesDbContext.CreateProjectNotifications.AddAsync(notification);
                    //     }
                    //     await messagesDbContext.SaveChangesAsync();
                    //     await hubContext.Clients.User(userid.ToString()).SendAsync("ReceiveNotification", title, content);
                    // }




                    return Ok("Reply sent successfully");  // Return Ok if everything is successful
                }
                else
                {
                    return NotFound("Complaint not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error
            }
        }



        //update Complaint Status
        [HttpPut("{TicketId}/UpdateTicketSupportStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateTicketSupportStatus(int TicketId)
        {

            var complaint = await momDb.TicketSupport
                 .Where(c => c.Id == TicketId && c.Status != TicketEnum.Close)
                 .FirstOrDefaultAsync();

            if (complaint != null)
            {
                complaint.Status = TicketEnum.Close;
                await momDb.SaveChangesAsync();

                return Ok("Complaint status Updated Successfully!");

            }

            else
            {
                return NotFound("Complaint not found!");

            }



        }





        //for admins
        [HttpGet("GetAllTicketSupportForAdmins")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllTicketSupportForAdmins()
        {
            var complaints = await momDb.TicketSupport.ToListAsync();

            if (complaints.Count != 0)
            {
                return Ok(complaints);
            }
            else
            {
                return Ok("not found any complaints");
            }
        }




        [HttpGet("{TicketId}/GetTicketSupportDetails")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TicketSupportDetailsDTO>))]
        public async Task<ActionResult> GetTicketSupportDetails(int TicketId)
        {
            try
            {
                var ticket = await momDb.TicketSupport
                    .Where(c => c.Id == TicketId)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                {
                    return NotFound("No ticket found with the specified ID");
                }

                var replies = await momDb.TicketSupportReply
                    .Where(c => c.TicketId == TicketId)
                    .OrderBy(c => c.CreationDate) // Order by CreationDate in ascending order
                    .ToListAsync();

                var repliesList = new List<ComplaintReplyDTO>();

                foreach (var reply in replies)
                {
                    var admin = await momDb.CustomerAttributes
                        .Where(u => u.Id == reply.AdminId).Include(c=> c.CustomerInfo)
                        .FirstOrDefaultAsync();

                    var createDate = reply.CreationDate ?? DateTimeOffset.MinValue;

                    var replydto = new ComplaintReplyDTO
                    {
                        ComplaintReplyId = reply.Id,
                        Reply = reply.Reply,
                        CreateDate = createDate,
                    };

                    if (admin != null)
                    {
                        replydto.AdminId = admin.Id;
                        replydto.AdminName = admin.CustomerInfo.NameEn;
                    }

                    repliesList.Add(replydto);
                }

                var urls = new List<string>();

                if (ticket.Key != null)
                {
                    var url = ticket.Key.SetDownloadFileUrlByKey(storage);
                    urls.Add(url);
                }

                foreach (var reply in replies)
                {
                    if (reply.Key != null)
                    {
                        var url = reply.Key.SetDownloadFileUrlByKey(storage);
                        urls.Add(url);
                    }
                }

                var dto = new TicketSupportDetailsDTO
                {
                    TicketId = ticket.Id,
                    UserEmail = ticket.Email,
                    Subject = ticket.Subject,
                    Question = ticket.Question,
                    Status = ticket.Status,
                    Replies = repliesList,
                    AttachmentsUrl = urls
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");  // Return BadRequest in case of an error
            }
        }

        #endregion










    }
}














