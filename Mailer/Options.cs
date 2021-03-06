﻿#region Usages

using CommandLine;
using CommandLine.Text;

#endregion

namespace SimpleMailer.Mailer
{
    internal class Options
    {
        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [Option('c', "config", Required = true, HelpText = "Path to connection configuration file.  Set full path to the file. ")]
        public string ConfigFile { get; set; }

        [Option('f', "from", Required = true, HelpText = "Email sender.")]
        public string From { get; set; }

        [OptionArray('r', "recipients", Required = true, HelpText = "List of recipients.")]
        public string[] Recipients { get; set; }

        [Option('s', "subject", HelpText = "Email subject.")]
        public string Subject { get; set; }

        [Option('t', "text", HelpText = "Message body.")]
        public string Text { get; set; }

        [OptionArray('h', "html", HelpText = "Paths to HTMl documents, their content will be added to message body. Set full path to each file. ")]
        public string[] HtmlFiles { get; set; }

        [OptionArray('a', "attaches", HelpText = "Email attaches. Set full path to the files.")]
        public string[] Attaches { get; set; }

        [OptionArray('i', "images", HelpText = "Add images to email which can be used in message body with -t option. Set full path to the files.")]
        public string[] Images { get; set; }
    }
}
