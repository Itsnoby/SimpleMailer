using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace SimpleMailer.Mailer.Utils
{
    /// <summary>
    /// Set object used to add images to message body.
    /// </summary>
    class ImageAttachmentPath : IFormattable
    {
        private static int _attachIndex;
        public string Image { get; set; }
        public MailMessage MessageContainer { get; set; }

        /// <param name="messageContainer">Email object which will be sent.</param>
        /// <param name="image">Path to image.</param>
        public ImageAttachmentPath(MailMessage messageContainer, string image)
        {
            MessageContainer = messageContainer;
            Image = image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">Format item which can contain one of format strings:
        /// <list type="bullet">
        /// <item> 
        /// <description><c>"L"</c> or <c>""</c> - Align image to the left.</description> 
        /// </item> 
        /// <item>
        /// <description><c>"R"</c> - Align image to the right.</description> 
        /// </item>
        /// <item>
        /// <description><c>"C"</c> - Align image to the center.</description> 
        /// </item> 
        /// </list> </param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(Image))
                return string.Empty;
            var imageCid = AddImageToMessage(Image);
            if (string.IsNullOrEmpty(imageCid))
                return string.Empty;

            var imageStyle = new StringBuilder("display: block; ");
            if (String.IsNullOrEmpty(format)) format = "L";

            switch (format.ToUpperInvariant())
            {
                case "C":
                    imageStyle.Append("margin-right: auto; ");
                    goto case "R";
                case "R":
                    imageStyle.Append("margin-left: auto; ");
                    break;
                // case "L": break;
            }

            return string.Format("<img style=\"{0}\" src=\"cid:{1}\">", imageStyle, imageCid).ToString(formatProvider);
        }

        /// <summary>
        /// Add image to Email object.
        /// </summary>
        /// <param name="src">Path to image.</param>
        /// <returns>Content id of the added image or <c>null</c> if some problem occurred.</returns>
        private string AddImageToMessage(string src)
        {
            try
            {
                var imageAttachment = new Attachment(src);
                imageAttachment.ContentDisposition.Inline = true;
                imageAttachment.ContentId = string.Format("img{0}", _attachIndex++);
                MessageContainer.Attachments.Add(imageAttachment);
                return imageAttachment.ContentId;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred during adding image file! " + src);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }
    }
}
