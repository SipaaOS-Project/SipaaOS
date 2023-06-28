﻿using PrismAPI.Filesystem.Formats.INI;
using SipaaOS.Core;
using SipaaOS.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Console = SipaaOS.VBEConsole;

namespace SipaaOS.Users
{
    internal static class UserManager
    {
        internal static List<User> Users = new List<User>();

        private const string oldUserDataPath = @"0:\users.ini";
        private const string userDataPath = @"0:\etc\users.ini";

        internal static void Load()
        {
            /**if (File.Exists(oldUserDataPath))
            {
                Log.Info("UserManager", "Upgrading users.ini...");
                string text = File.ReadAllText(oldUserDataPath);
                File.WriteAllText(userDataPath, text);
                File.Delete(oldUserDataPath);
            }**/
            if (File.Exists(userDataPath))
            {
                try
                {
                    string text = File.ReadAllText(userDataPath);
                    var reader = new IniReader(text);
                    foreach (string section in reader.GetSections())
                    {
                        int dotIndex = section.IndexOf('.');
                        if (dotIndex == -1)
                        {
                            throw new Exception("Unrecognised key in users.ini.");
                        }
                        string type = section.Substring(0, dotIndex);
                        string key = section.Substring(dotIndex + 1);
                        switch (type)
                        {
                            case "User":
                                string username = key;
                                string password = reader.ReadString("password", section);
                                bool admin = reader.ReadBool("admin", section);
                                bool expired = reader.ReadBool("expired", section);

                                User user = new User(username, password, admin);
                                user.PasswordExpired = expired;
                                Users.Add(user);

                                //Util.PrintTask($"DEBUG: UserMgr loadusr {username} {admin}");
                                break;
                            case "Message":
                                User from = GetUser(reader.ReadString("from", section));
                                User to = GetUser(reader.ReadString("to", section));
                                string body = reader.ReadString("body", section);
                                DateTime sent = new DateTime(reader.ReadLong("sent", section));

                                Message message = new Message(from, body, sent);
                                to.Messages.Add(message);
                                //Util.PrintTask($"DEBUG: UserMgr loadmsg {from.Username} {to.Username}");
                                break;
                            default:
                                throw new FormatException($"Unrecognised type {type}.");
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.SKBugCheck(new($"Unable to load user data: {e.ToString()}"));
                }
            }
            else
            {
                //Log.Info("UserManager", $"Default user 'admin' was created.");
                Console.WriteLine("Default user 'admin' was created.");
                User user = AddUser("admin", "password", admin: true, flush: false);
                user.PasswordExpired = true;
            }
        }

        internal static User AddUser(string username, string password, bool admin, bool flush = true)
        {
            if (GetUser(username) != null)
                throw new InvalidOperationException($"A user named {username} already exists!");

            if (username.IndexOf(' ') != -1)
                throw new InvalidOperationException("Usernames may not contain spaces.");

            if (username.IndexOf(':') != -1)
                throw new InvalidOperationException("Usernames may not contain colons.");

            if (username.IndexOf('.') != -1)
                throw new InvalidOperationException("Usernames may not contain dots.");

            if (username.IndexOf('~') != -1)
                throw new InvalidOperationException("Usernames may not contain tildes.");

            if (username.IndexOf('/') != -1 || username.IndexOf('\\') != -1)
                throw new InvalidOperationException("Usernames may not contain path separators.");

            User user = new User(username, HashPasswordSha256(password), admin);
            Users.Add(user);

            if (flush)
            {
                Flush();
            }

            if (admin)
            {
                Console.WriteLine($"Admin user '{username}' was added");
            }
            else
            {
                Console.WriteLine($"User '{username}' was added");
            }

            return user;
        }

        internal static void CreateHomeDirectory(User user)
        {
            if (!Directory.Exists(@"0:\users"))
                Directory.CreateDirectory(@"0:\users");

            if (!Directory.Exists($@"0:\users\{user.Username}"))
                Directory.CreateDirectory($@"0:\users\{user.Username}");
        }

        internal static bool DeleteUser(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                {
                    Users.Remove(user);
                    Flush();
                    Console.WriteLine($"User {username} was deleted.");
                    //Log.Info("UserManager", $"User '{username}' was deleted.");
                    return true;
                }
            }
            return false;
        }

        internal static User GetUser(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                    return user;
            }
            return null;
        }

        internal static bool SetAdmin(string username, bool admin)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                {
                    user.Admin = admin;
                    Flush();
                    Console.WriteLine($"{user.Username}'s admin status is now {admin}");
                    //Log.Info("UserManager", $"{user.Username}'s admin status was set to {admin}.");
                    return true;
                }
            }
            return false;
        }

        internal static void Flush()
        {
            try
            {
                var builder = new IniBuilder();
                foreach (User user in Users)
                {
                    builder.BeginSection($"User.{user.Username}");
                    builder.AddKey("password", user.Password);
                    builder.AddKey("admin", user.Admin);
                    builder.AddKey("expired", user.PasswordExpired);
                }
                Random random = new Random((int)DateTime.Now.Ticks);
                int messageIndex = 0;
                foreach (User user in Users)
                {
                    foreach (Message message in user.Messages)
                    {
                        builder.BeginSection($"Message.{messageIndex}");
                        builder.AddKey("from", message.From.Username);
                        builder.AddKey("to", user.Username);
                        builder.AddKey("body", message.Body.ToString());
                        builder.AddKey("sent", message.Sent.Ticks);
                        messageIndex++;
                    }
                }
                File.WriteAllText(userDataPath, builder.ToString());
            }
            catch (Exception e)
            {
                ExceptionHandler.SKBugCheck(new($"Failed to flush user data: {e.ToString()}"));
                //Log.Warning("UserManager", $"Failed to flush user data: {e.ToString()}");
            }
        }

        internal static string HashPasswordSha256(string password)
        {
            Sha256 sha256 = new Sha256();

            byte[] passwordBytesUnhashed = Encoding.Unicode.GetBytes(password);
            sha256.AddData(passwordBytesUnhashed, 0, (uint)passwordBytesUnhashed.Length);

            return Convert.ToBase64String(sha256.GetHash());
        }
    }
}