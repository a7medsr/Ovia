using MailChimp.Net.Models;
using Ovia.DTO;
using Ovia.Models;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Ovia.Services
{

    public class Certificate
        {
        MomEntity Context = new MomEntity();
        public async Task<CertificateDTO> getCertificateAsync(string CourseName, string InstructorName, string userName, int courseId, int userId, CertificateDTO certificate)
            {
            try
            {
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync("851527");
                var FileName = CourseName + " Certificate.pdf";
                var FilePath = "\\Users\\" + userId + "\\Certificates\\";
                var FullPath = Directory.GetCurrentDirectory() + "\\wwwroot" + FilePath;


                if (!Directory.Exists(FullPath))
                {
                    Directory.CreateDirectory(FullPath);
                }

                FullPath += FileName;

                var fileInfo = new FileInfo(FullPath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                #region PdfAsync


                var chromPath = Directory.GetCurrentDirectory() + "/.local-chromium/Win64-851527/chrome-win/chrome.exe";

                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {


                    ExecutablePath = chromPath,
                    Headless=true

                });
                var CertificateDate = certificate.CertificateDate ?? DateTime.Now;
                var CertificateDateFormat = CertificateDate.ToString("MMMM") + " " + CertificateDate.Day + ", " + CertificateDate.Year;
                Referrer serial = new Referrer();

                certificate.CertificateId = certificate.CertificateId; //??  serial.GenCertificateId(); //samuel
                 using var page =  browser.NewPageAsync().Result;
                // await page.GoToAsync("file:///F:/WORK/Plan%20B/Certificate/certificate/Certificate%20.html");
                string html = File.ReadAllText("./wwwroot/certificate/Certificate.html");
                html = html.Replace("{UserName}", userName);
                html = html.Replace("{CourseName}", CourseName);
                html = html.Replace("{InstructorName}", InstructorName);
                html = html.Replace("{CertificateDate}", CertificateDateFormat);
                html = html.Replace("{CertificateId}",certificate.CertificateId );

                await page.SetContentAsync(html);
                await page.PdfAsync(FullPath, new PdfOptions { PreferCSSPageSize = true, PrintBackground = true, Landscape = true });

                var customerCourse = Context.CourseCustomerMapping.Where(c => c.CustomerId == userId && c.CourseId == courseId).FirstOrDefault();
                customerCourse.Certificate = FilePath + FileName;
                customerCourse.CertificateDate = CertificateDate;
                customerCourse.CertificateId = certificate.CertificateId;
                Context.SaveChanges();

                #endregion
                certificate.CertificateDate = CertificateDate;
                certificate.CertificateURL = FilePath + FileName;
                return certificate;
            }
            catch(Exception ex)
            {
                return null;
            }
            }
        public bool CheckFileCreated(string Path)
        {
         
            var FullPath = Directory.GetCurrentDirectory() + "\\wwwroot" + Path;


         return   !Directory.Exists(FullPath);
            
        }


    }
}
