using ConsoleKeyEx = Cosmos.System.ConsoleKeyEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipaaOS.Core
{
    /// <summary>
    /// Describes the result of a ReadLineEx operation. 
    /// </summary>
    public struct ReadLineExResult
    {
        public ReadLineExResult(string input) : this()
        {
            Input = input;
        }

        public ReadLineExResult(ConsoleKeyEx cancelKey)
        {
            Cancelled = true;
            CancelKey = cancelKey;
        }

        /// <summary>
        /// The user input to ReadLineEx, if it was not cancelled.
        /// </summary>
        public string Input = string.Empty;

        /// <summary>
        /// If the ReadLineEx was cancelled by a cancel key.
        /// </summary>
        public bool Cancelled = false;

        /// <summary>
        /// The key that was used to cancel the ReadLineEx.
        /// This will be <see cref="ConsoleKeyEx.NoName"/> if the operation was not cancelled by the user.
        /// </summary>
        public ConsoleKeyEx CancelKey = ConsoleKeyEx.NoName;
    }
    internal class Util
    {
        private static void SetCursorPosWrap(int left, int top)
        {
            if (left < 0)
            {
                left = Console.WindowWidth - 1;
                top--;
            }
            if (left >= Console.WindowWidth)
            {
                left = 0;
                top++;
            }

            Console.SetCursorPosition(left, top);
        }

        /// <summary>
        /// Read line extended.
        /// </summary>
        /// <param name="cancelKey">An optional key that will cancel the function and return null.</param>
        /// <param name="mask">Whether to mask the password.</param>
        /// <returns>The text entered, or null if cancelKey was pressed.</returns>
        internal static ReadLineExResult ReadLineEx(Cosmos.System.ConsoleKeyEx[]? cancelKeys = null, bool mask = false, string initialValue = "", bool clearOnCancel = false)
        {
            var chars = new List<char>(32);
            Cosmos.System.KeyEvent current;
            int currentCount = 0;
            if (initialValue != null)
            {
                chars.AddRange(initialValue.ToCharArray());
                Console.Write(initialValue);
                currentCount = initialValue.Length;
            }
            while ((current = Cosmos.System.KeyboardManager.ReadKey()).Key != Cosmos.System.ConsoleKeyEx.Enter)
            {
                if (cancelKeys != null && Array.IndexOf(cancelKeys, current.Key) != -1)
                {
                    if (clearOnCancel)
                    {
                        while (chars.Count > 0)
                        {
                            int curCharTemp = Console.GetCursorPosition().Left;
                            chars.RemoveAt(currentCount - 1);
                            SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);

                            for (int x = currentCount - 1; x < chars.Count; x++)
                            {
                                Console.Write(chars[x]);
                            }

                            Console.Write(' ');

                            SetCursorPosWrap(curCharTemp - 1, Console.GetCursorPosition().Top);

                            currentCount--;
                        }
                    }
                    return new ReadLineExResult(cancelKey: current.Key);
                }
                if (current.Key == Cosmos.System.ConsoleKeyEx.NumEnter)
                {
                    break;
                }
                if (current.Key == Cosmos.System.ConsoleKeyEx.Backspace)
                {
                    if (currentCount > 0)
                    {
                        int curCharTemp = Console.GetCursorPosition().Left;
                        chars.RemoveAt(currentCount - 1);
                        SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);

                        for (int x = currentCount - 1; x < chars.Count; x++)
                        {
                            Console.Write(chars[x]);
                        }

                        Console.Write(' ');

                        SetCursorPosWrap(curCharTemp - 1, Console.GetCursorPosition().Top);

                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.LeftArrow)
                {
                    if (currentCount > 0)
                    {
                        SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);
                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.RightArrow)
                {
                    if (currentCount < chars.Count)
                    {
                        SetCursorPosWrap(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top);
                        currentCount++;
                    }
                    continue;
                }

                if (current.KeyChar == '\0')
                {
                    continue;
                }

                if (currentCount == chars.Count)
                {
                    chars.Add(current.KeyChar);
                    Console.Write(mask ? '*' : chars[chars.Count - 1]);
                    currentCount++;
                }
                else
                {
                    var temp = new List<char>();

                    for (int x = 0; x < chars.Count; x++)
                    {
                        if (x == currentCount)
                        {
                            temp.Add(current.KeyChar);
                        }

                        temp.Add(chars[x]);
                    }

                    chars = temp;

                    for (int x = currentCount; x < chars.Count; x++)
                    {
                        Console.Write(mask ? '*' : chars[x]);
                    }

                    SetCursorPosWrap(Console.GetCursorPosition().Left - (chars.Count - currentCount) - 1, Console.GetCursorPosition().Top);
                    currentCount++;
                }
            }
            Console.WriteLine();

            char[] final = chars.ToArray();
            return new ReadLineExResult(new string(final));
        }
    }
}
