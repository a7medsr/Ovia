using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Ovia.Models;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace Ovia.Services.SendEmails
{
    public class MailingServices : IMailingServices
    {
        private readonly MailSettings _mailSettings;

        public MailingServices(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(
            string mailTo,
            string subject,
            string body,
            IList<IFormFile> attachments)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var email = new MimeMessage
                    {
                        Sender = MailboxAddress.Parse(_mailSettings.Email),
                        Subject = subject
                    };

                    email.To.Add(MailboxAddress.Parse(mailTo));

                    var builder = new BodyBuilder();

                    if (attachments != null)
                    {
                        byte[] FileBytes;
                        foreach (var file in attachments)
                        {
                            using var ms = new MemoryStream();
                            file.CopyTo(ms);
                            FileBytes = ms.ToArray();
                            builder.Attachments.Add(file.FileName, FileBytes,
                                 ContentType.Parse(file.ContentType)
                                );

                        }
                    }

                    builder.HtmlBody = body;
                    email.Body = builder.ToMessageBody();
                    email.From.Add(new MailboxAddress
                        (_mailSettings.DisplayName, _mailSettings.Email));

                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore SSL certificate validation for now
                        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                        smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);

                        await smtp.SendAsync(email);

                        smtp.Disconnect(true);
                    }
                    // ...
                }


            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error connecting to the mail server: {ex}");
                // Rethrow the exception or handle it as needed
                throw;
            }





        }


        public async Task SendPasswordResetEmail(string userEmail, string name,string newPassword)
        {
            try
            {
                await SendEmailAsync(userEmail,
                    "Ruwad : Requested new password",
                    $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }}
                            
                            .container {{
                                max-width: 600px;
                                margin: 20px auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 10px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}
                        
                            h4 {{
                                color: #333;
                            }}
                        
                            p {{
                                color: #666;
                                margin-bottom: 15px;
                            }}
                        
                            strong {{
                                color: #007bff;
                            }}
                        
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #999;
                            }}
                        
                            .message {{
                                padding: 15px;
                                border: 1px solid #ddd;
                                border-radius: 5px;
                                background-color: #f9f9f9;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h4>Dear {name}</h4>
                            <div class='message'>
                                <p>You have requested to reset your password for your Ruwad  account.</p>
                                <p>Your new auto-generated password is: <strong>{newPassword}</strong></p>
                                <p>For security reasons, please do not share this password with anyone.</p>
                            </div>
                            <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
                        </div>
                    </body>
                    </html>
                    ", null);

                Console.WriteLine("Password reset email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
            }
        }


        public async Task SendWelcomeEmail(string userEmail, string username, string referId, string password)
        {
            await SendEmailAsync(userEmail,
                "Ruwad : Welcome Email",
                $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
            
                h2 {{
                    color: #333;
                }}
            
                p {{
                    color: #666;
                    margin-bottom: 15px;
                }}
            
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #999;
                }}
            
                .message {{
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Welcome to Ruwad , {username}!</h2>
                <div class='message'>
                    <p>Thank you for joining our network marketing community. <br>
                    We are excited to have you on board.</p>
                    <p>Back Office Id: <strong>{referId}</strong></p>
                    <p>Password : <strong>{password}</strong></p>
                    <p>Here, you will discover endless opportunities for growth, success, and collaboration.</p>
                    <p>Feel free to explore our platform and connect with fellow members to maximize your experience.</p>
                </div>
                <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
            </div>
        </body>
        </html>
        ", null);

        }


        public async Task SendEmailToSponsor(string userEmail, string username, string sponsorId, string childName, string childId)
        {
            await SendEmailAsync(userEmail,
                "Ruwad : New user join to your network",
                $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
            
                h2 {{
                    color: #333;
                }}
                
                ul {{
                    list-style-type: none;
                    padding: 0;
                }}
                
                li {{
                    margin-bottom: 10px;
                }}
                
                .message {{
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #999;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Dear {username}!</h2>
                <div class='message'>
                    <p>A new user has joined your network.</p>
                    <ul>
                        <li><strong>Name:</strong> {childName}</li>
                        <li><strong>Back Office Id:</strong> {childId}</li>
                    </ul>
                    <p>Your Back Office Id: <strong>{sponsorId}</strong></p>
                    <p>Here, you will discover endless opportunities for growth, success, and collaboration.</p>
                    <p>Feel free to explore our platform and connect with fellow members to maximize your experience.</p>
                </div>
                <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
            </div>
        </body>
        </html>
        ", null);
        }

        public async Task SendEmailSupportTicket(string email, string eventName,
            string fullName, string email1, string phone, string message)
        {



            await SendEmailAsync(email,
                "Ruwad : Support Ticket",
                $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
    
        h2 {{
            color: #333;
        }}
        
        ul {{
            list-style-type: none;
            padding: 0;
        }}
        
        li {{
            margin-bottom: 10px;
        }}
        
        .message {{
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        
        .footer {{
            margin-top: 20px;
            font-size: 14px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Support Ticket</h2>

        <div class='message'>
            <ul>
                <li><strong>Event:</strong> {eventName}</li>
                <li><strong>Name:</strong> {fullName}</li>
                <li><strong>Phone:</strong> {phone}</li>
                <li><strong>Email:</strong> {email1}</li>
                <li><strong>Message:</strong> {message}</li>

            </ul>
        </div>
        <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
    </div>
</body>
</html>
", null);

        }

        public async Task SendEmailToSponsorWhenHisChildBuyPackage(string userEmail, 
            string username, string ChildReferId, decimal profit, string childName)
        {
            await SendEmailAsync(userEmail,
                "Ruwad :Child in your network buy a package",
                $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
            
                h2 {{
                    color: #333;
                }}
                
                ul {{
                    list-style-type: none;
                    padding: 0;
                }}
                
                li {{
                    margin-bottom: 10px;
                }}
                
                .message {{
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #999;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Dear {username}!</h2>
                <div class='message'>
                    <p>A new user  in  your network buy a package.</p>
                    <ul>
                        <li><strong>his Name:</strong> {childName}</li>
                        <li><strong>his Back Office Id:</strong> {ChildReferId}</li>
                        <li><strong>Your comission  is :</strong> {profit}</li>

                    </ul>
                    <p>Here, you will discover endless opportunities for growth, success, and collaboration.</p>
                    <p>Feel free to explore our platform and connect with fellow members to maximize your experience.</p>
                </div>
                <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
            </div>
        </body>
        </html>
        ", null);
        }





        public async Task sendEmailToCustomerWhenAdminConvertHisMoney
            (string userEmail, string username, decimal Comission, DateTime date)
        {
            await SendEmailAsync(userEmail,
                "Ruwad : Ruwad convert your commission",
                $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
            
                h2 {{
                    color: #333;
                    border-bottom: 1px solid #ddd;
                    padding-bottom: 10px;
                }}
                
                .message {{
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #999;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Dear {username}!</h2>
                <div class='message'>
                    <p>Ruwad  has converted your commission.</p>
                    <ul>
                        <li><strong>Your commission:</strong> ${Comission}</li>
                        <li><strong>Conversion Date:</strong> {date}</li>
                    </ul>
                    <p>Here, you will discover endless opportunities for growth, success, and collaboration.</p>
                    <p>Feel free to explore our platform and connect with fellow members to maximize your experience.</p>
                </div>
                <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
            </div>
        </body>
        </html>
        ", null);
        }

        public async Task sendSubscribeEmail(string userEmail)
        {
            var atIndex = userEmail.IndexOf('@');
            var username = atIndex != -1 ? userEmail.Substring(0, atIndex) : "User";

            await SendEmailAsync(userEmail,
                                 "Ruwad : Subscription Confirmation",
                                 $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background-color: #fff;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }}
            
                h2 {{
                    color: #333;
                    border-bottom: 1px solid #ddd;
                    padding-bottom: 10px;
                }}
                
                .message {{
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #999;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Dear {username},</h2>
                <div class='message'>
                    <p>Thank you for subscribing to Ruwad !</p>
                    <p>We're excited to have you on board. You're now part of a vibrant community dedicated to growth, success, and collaboration.</p>
                    <p>Feel free to explore our platform and stay updated with our latest notifications.</p>
                </div>
                <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
            </div>
        </body>
        </html>
        ",null);
        }


    
        public async Task SendTicketSupportReply(string UserEmail, string Reply, IFormFile file)
        {
            var atIndex = UserEmail.IndexOf('@');
            var username = atIndex != -1 ? UserEmail.Substring(0, atIndex) : "User";

            await SendEmailAsync(UserEmail,
                                 "Ticket Support Reply",
                                 $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        
        h2 {{
            color: #333;
            border-bottom: 1px solid #ddd;
            padding-bottom: 10px;
        }}
        
        .message {{
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        
        .footer {{
            margin-top: 20px;
            font-size: 14px;
            color: #999;
        }}

        /* Adjustments */
        .message p {{
            margin: 0; /* Remove default margin */
        }}

        .footer br {{
            line-height: 20px; /* Adjust spacing between lines */
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Dear {username},</h2>
        <div class='message'>
            <p>{Reply}</p>

        </div>
        <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
    </div>
</body>
</html>", file != null ? new List<IFormFile> { file } : null);
        }

        public async Task SendEmailToProfessor(string userEmail, string name, string referId, string password)
        {
            await SendEmailAsync(userEmail, "Registration Email",
                                $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
    
        h2 {{
            color: #333;
            border-bottom: 1px solid #ddd;
            padding-bottom: 10px;
        }}
        
        .message {{
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        
        .footer {{
            margin-top: 20px;
            font-size: 14px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Dear {name},</h2>
        <div class='message'>
            <p>Thank you for registering in our community.</p>
            <p>ID: <strong>{referId}</strong></p>
            <p>Password: <strong>{password}</strong></p>
        </div>
        <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
    </div>
</body>
</html>
", null);
        }


        public async Task SendOTP(string userEmail, string username, string otp)
        {
            await SendEmailAsync(userEmail,
                                 "Confirmation Otp",
                                 $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
    
        h2 {{
            color: #333;
            border-bottom: 1px solid #ddd;
            padding-bottom: 10px;
        }}
        
        .message {{
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        
        .footer {{
            margin-top: 20px;
            font-size: 14px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Dear {username},</h2>
        <div class='message'>
            <p>Thank you for requesting a new OTP.</p>
            <p>Please confirm your OTP before 10 minutes.</p>
            <p>Your Confirmation otp : <strong>{otp}</strong></p>
        </div>
        <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
    </div>
</body>
</html>
", null);
        }
        public async Task SendEventTicket(string UserEmail, string Name, string TicketNumber)
        {
            
                await SendEmailAsync(UserEmail,
                                     "Event Ticket",
                                     $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
    
        h2 {{
            color: #333;
            border-bottom: 1px solid #ddd;
            padding-bottom: 10px;
        }}
        
        .message {{
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f9f9f9;
        }}
        
        .footer {{
            margin-top: 20px;
            font-size: 14px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Dear {Name}</h2>
        <div class='message'>
            <p>Thank you for joining the event.</p>
            <p>Your Ticket number: <strong>{TicketNumber}</strong>.</p>
            <p>You can see your ticket <a href='https://Ruwad -net.com/ticket/{TicketNumber}'>here</a>.</p>
        </div>
        <p class='footer'>Best regards,<br/>The Ruwad  Team</p>
    </div>
</body>
</html>
", null);
           
        }



        //<img src=""{{ImgUrl}}"" style=""max-width: 100%; height: auto;""> <!-- Add image with responsive styling -->



    }
}
