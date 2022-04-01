using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BusinessLayer.Services
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IOptions<EmailConfiguration> _emailConfigOptions;
        public EmailMessageService(IOptions<EmailConfiguration> emailConfigOptions)
        {
            _emailConfigOptions = emailConfigOptions;
        }

        public async Task SendTempPasswordEmail(UserDTO userDto, string password)
        {
            var html = template.Replace("#login", userDto.Login).Replace("#pw", password);

            await SendEmail(userDto, html);
        }

        private async Task SendEmail(UserDTO userDto, string html)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Personal Finance", _emailConfigOptions.Value.Address));
            emailMessage.To.Add(new MailboxAddress("", userDto.Email));
            emailMessage.Subject = "Password Restore";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = html
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailConfigOptions.Value.Smtp, _emailConfigOptions.Value.Port, true);
            await client.AuthenticateAsync(_emailConfigOptions.Value.Address, _emailConfigOptions.Value.Password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }


        private static string template =>
            @"  <!doctype html>
                <html lang=""en-US"">

                <head>
                    <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"" />
                    <title>Password Restore</title>
                    <meta name=""description"" content=""Password Restore."">
                    <style type=""text/css"">
                        a:hover {text-decoration: underline !important;}
                    </style>
                </head>

                <body marginheight=""0"" topmargin=""0"" marginwidth=""0"" style=""margin: 0px; background-color: #f2f3f8;"" leftmargin=""0"">
                    <!--100% body table-->
                    <table cellspacing=""0"" border=""0"" cellpadding=""0"" width=""100%"" bgcolor=""#f2f3f8""
                        style=""@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;"">
                        <tr>
                            <td>
                                <table style=""background-color: #f2f3f8; max-width:670px;  margin:0 auto;"" width=""100%"" border=""0""
                                    align=""center"" cellpadding=""0"" cellspacing=""0"">
                                    <tr>
                                        <td style=""height:80px;"">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width=""95%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0""
                                                style=""max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);"">
                                                <tr>
                                                    <td style=""height:40px;"">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style=""padding:0 35px;"">
                                                        <h1 style=""color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;"">You have
                                                            requested to restore your password</h1>
                                                        <span
                                                            style=""display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;""></span>
                                                        <p style=""color:#455056; font-size:15px;line-height:24px; margin:0;"">
                                                            We cannot simply send you your old password. But we can generate new temporally
                                                            password and remind your login:
                                                            <br/>
                                                            Login: #login
                                                            <br/>
                                                            Password: #pw
                                                        </p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style=""height:40px;"">&nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    <tr>
                                        <td style=""height:20px;"">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style=""text-align:center;"">
                                            <p style=""font-size:14px; color:rgba(69, 80, 86, 0.7411764705882353); line-height:18px; margin:0 0 0;"">&copy; <strong>www.rakeshmandal.com</strong></p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""height:80px;"">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
    }
}