using SipaaOS.Core;
using SipaaOS.Users;
using Console = SipaaOS.VBEConsole;
using System;

namespace SipaaOS
{
    internal static class LoginPrompt
    {
        internal static bool PromptLogin(bool clear = false)
        {
            if (clear)
                Console.Clear();
            Console.Write("Username: ");

            var username = Console.ReadLine().Trim();
            if (username.Trim() == string.Empty)
            {
                return false;
            }

            User user = UserManager.GetUser(username);
            if (user != null)
            {
                if (user.LockedOut)
                {
                    Console.WriteLine($"This account has been locked out due to too many failed login attempts.");

                    TimeSpan remaining = user.LockoutEnd - DateTime.Now;
                    if (remaining.Minutes > 0)
                    {
                        Console.WriteLine($"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                    }
                    else
                    {
                        Console.WriteLine($"Try again in {remaining.Seconds}s.");
                    }

                    return false;
                }
                Console.Write($"Password for {username}: ");
                ReadLineExResult result = Util.ReadLineEx(mask: true);
                if (user.Authenticate(result.Input))
                {
                    Kernel.CurrentUser = user;
                    //Log.Info("LoginPrompt", $"{user.Username} logged on.");
                    Console.WriteLine();
                    if (user.PasswordExpired)
                    {
                        Console.WriteLine("Your password has expired. Please set a new password:");
                        user.ResetPasswordConsole(result.Input);
                    }
                    Console.Clear();
                    Console.WriteLine($"Welcome to SipaaOS, an UNIX like OS!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Incorrect password.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Unknown user.");
                return false;
            }
        }
    }
}