using DataAccess.Abstract;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfMailDal : IMailDal
    {
        public void SendMail(SendMailDto sendMailDto)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(sendMailDto.mailParameter.Email);
                mailMessage.To.Add(sendMailDto.email);
                mailMessage.Subject = sendMailDto.subject;
                mailMessage.Body = sendMailDto.body;
                mailMessage.IsBodyHtml = true;

                //mailMessage.Attachments.Add();

                using (SmtpClient smtp = new SmtpClient(sendMailDto.mailParameter.SMTP))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(sendMailDto.mailParameter.Email,
                        sendMailDto.mailParameter.Password);
                    smtp.EnableSsl = sendMailDto.mailParameter.SSL;
                    smtp.Port = sendMailDto.mailParameter.Port;

                    smtp.Send(mailMessage);
                }
            }
        }
    }
}
