#region Usages

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

#endregion

namespace SimpleMailer.Mailer.Utils
{
    class MailUtilities
    {
        /// <summary>
        /// Prepare SMTP client using given program options.
        /// </summary>
        /// <param name="options">The options passed from User.</param>
        /// <returns>The configured SMTP client or <c>null</c> if some error occurred.</returns>
        public static SmtpClient PrepareSmtpClient(Options options)
        {
            SmtpClient client = null;
            try
            {
                #region Load XML and extract settings

                var doc = XDocument.Load(options.ConfigFile);
                if (!doc.Descendants("MailSettings").Any())
                    throw new Exception("Settings configuration file should contain 'MailSettings' properties!");
                var mailSettings = doc.Descendants("MailSettings").First();

                var addressSetting = mailSettings.Element("Address");
                Utilities.MakeCheckNotNull(addressSetting,
                    "Mail settings configuration should contain 'Address' property!");
                var sslSetting = mailSettings.Element("SSL");
                Utilities.MakeCheckNotNull(sslSetting, "Mail settings configuration should contain 'SSL' property!");
                var credentialsSetting = mailSettings.Element("Credentials");
                Utilities.MakeCheckNotNull(credentialsSetting,
                    "Mail settings configuration should contain 'Credentials' property!");

                #endregion

                #region Extract endpoint options

                var addressHost = addressSetting.Attribute("Host");
                Utilities.MakeCheckNotNull(addressHost, "Mail Address property should contain 'Host' attribute!");
                var addressPort = addressSetting.Attribute("Port");
                Utilities.MakeCheckNotNull(addressPort, "Mail Address property should contain 'Port' attribute!");
                var sslEnable = sslSetting.Attribute("Enable");
                Utilities.MakeCheckNotNull(sslEnable, "Mail SSL property should contain 'Enable' attribute!");
                var credentialsLogin = credentialsSetting.Attribute("Login");
                Utilities.MakeCheckNotNull(credentialsLogin,
                    "Mail Credentials property should contain 'Login' attribute!");
                var credentialsPassword = credentialsSetting.Attribute("Password");
                Utilities.MakeCheckNotNull(credentialsPassword,
                    "Mail Credentials property should contain 'Password' attribute!");

                #endregion

                client = new SmtpClient
                {
                    Host = addressHost.Value,
                    Port = Convert.ToInt32(addressPort.Value),
                    EnableSsl = Convert.ToBoolean(sslEnable.Value),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(credentialsLogin.Value, credentialsPassword.Value),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred during SMTP client setup! " + e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return client;
        }

        /// <summary>
        /// Create an <see cref="MailMessage"/> object using given program options.
        /// </summary>
        /// <param name="options">The options passed from User.</param>
        /// <returns>Email object or <c>null</c> if some error occurred.</returns>
        public static MailMessage GenerateMessage(Options options)
        {
            MailMessage mail = null;

            try
            {
                mail = new MailMessage();

                mail.From = new MailAddress(options.From);
                foreach (var recipient in options.Recipients)
                    mail.To.Add(new MailAddress(recipient));

                if (!string.IsNullOrEmpty(options.Subject))
                    mail.Subject = options.Subject;

                mail.IsBodyHtml = true;
                var body = new StringBuilder();

                if (!string.IsNullOrEmpty(options.Text))
                   body.Append(string.Format("<p>{0}</p>", options.Text));
                if (!string.IsNullOrEmpty(options.HtmlFile))
                    body.Append(string.Format("<div>{0}</div>", ExtractBodyFromHtml(options.HtmlFile)));

                if(options.Attaches != null)
                    foreach (var attach in options.Attaches)
                        mail.Attachments.Add(new Attachment(attach));

                if (options.Images != null)
                {
                    var l = options.Images.Select(image => new ImageAttachmentPath(mail, image)).ToArray();
                    body = new StringBuilder(string.Format(body.ToString(), l));
                }

                mail.Body = body.ToString();
                    
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred during email object construction! " + e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return mail;
        }

        private static string ExtractBodyFromHtml(string file)
        {
            var content = string.Empty;
            try
            {
                var doc = File.ReadAllText(file);
                var regex = new Regex("^.*?<.*?body.*?>(.*)</.*?body.*?>.*?$");
                var match = regex.Match(doc.Replace("\r\n", ""));
                if (match.Success)
                    content = match.Groups[1].Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred during reading HTML content file! " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return content;
        }
    }
}
