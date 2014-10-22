#region Usages

using System;
using SimpleMailer.Mailer;
using SimpleMailer.Mailer.Utils;

#endregion

namespace SimpleMailer
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            SendMail(options);
        }



        private static void SendMail(Options options)
        {
            Console.Write("Sending email.. ");
            var tickStart = Environment.TickCount;

            #region Prepare objects

            var client = MailUtilities.PrepareSmtpClient(options);
            Utilities.MakeCheckNotNull(client, "Cannot send email due to SMTP construction problem!");
            var email = MailUtilities.GenerateMessage(options);
            Utilities.MakeCheckNotNull(email, "Cannot send email due to email generation problem!"); 

            #endregion


            #region Sending process

            try
            {
                using (client)
                {
                    using (email)
                    {
                        client.Send(email);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred during sending email! " + e.Message);
                Console.WriteLine(e.StackTrace);
            } 

            #endregion

            Console.WriteLine("Sending finished in {0} ms!", Environment.TickCount - tickStart);
        }
    }
}
