using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Chat.Services;

namespace Chat.Extensions
{
    public class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(
            this EmailSender emailSender,
            string email,
            string link
        )
        {
            return emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>."
            );
        }

        public static Task SendResetPasswordAsync(
            this EmailSender emailSender,
            string email,
            string callbackUrl
        )
        {
            return emailSender.SendEmailAsync(
                email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            );
        }
    }
}
