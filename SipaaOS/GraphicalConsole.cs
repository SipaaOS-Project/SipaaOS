using static Cosmos.HAL.Global;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;
using IL2CPU.API.Attribs;
using PrismAPI.Tools.Extentions;
using PrismAPI.Hardware.GPU;
using Cosmos.Core;
using System;

namespace SipaaOS
{
	/// <summary>
	/// VBE Console class, used to write text output to a high resolution console.
	/// Supports images, shapes, and everything else.
	/// Not completed.
	/// </summary>
	public static unsafe class VBEConsole
	{
		#region Methods

		#region WriteLine

		public static void WriteLine(string Format, object Value1, object Value2, object Value3) => WriteLine(string.Format(Format, Value1, Value2, Value3));
		public static void WriteLine(string Format, object Value1, object Value2) => WriteLine(string.Format(Format, Value1, Value2));
		public static void WriteLine(string Format, params object[] Value) => WriteLine(string.Format(Format, Value));
		public static void WriteLine(char[] Value, int Index, int Count) => WriteLine(Value[Index..(Index + Count)]);
		public static void WriteLine(string Format, object Value) => WriteLine(string.Format(Format, Value));
		public static void WriteLine(string Value) => Write(Value + Environment.NewLine);
		public static void WriteLine(object Value) => WriteLine(Value ?? string.Empty);
		public static void WriteLine(char[] Value) => WriteLine(new string(Value));
		public static void WriteLine(double Value) => WriteLine(Value.ToString());
		public static void WriteLine(float Value) => WriteLine(Value.ToString());
		public static void WriteLine(ulong Value) => WriteLine(Value.ToString());
		public static void WriteLine(bool Value) => WriteLine(Value.ToString());
		public static void WriteLine(long Value) => WriteLine(Value.ToString());
		public static void WriteLine(uint Value) => WriteLine(Value.ToString());
		public static void WriteLine(int Value) => WriteLine(Value.ToString());
		public static void WriteLine() => Write(Environment.NewLine);

		/* Decimal type is not working yet... */
		//public static void WriteLine(decimal aDecimal) => WriteLine(aDecimal.ToString());

		#endregion

		#region ReadLine

		public static string ReadLine(bool RedirectOutput = false)
		{
			string Input = string.Empty;

			while (true)
			{
				if (KeyboardEx.TryReadKey(out ConsoleKeyInfo Key))
				{
					IsCursorEnabled = false;

					switch (Key.Key)
					{
						case ConsoleKey.Enter:
							Write('\n');
							IsCursorEnabled = true;
							return Input;
						case ConsoleKey.Backspace:
							if (Input.Length > 0)
							{
								if (X < SpacingX)
								{
									Y -= (int)Font.Fallback.Size;
									X = (int)Font.Fallback.MeasureString(Buffer.Split('\n')[Y]);
								}
								else
								{
									X -= (int)Font.Fallback.MeasureString(Buffer[^1].ToString());
								}

								Buffer = Buffer[..^1];
								Input = Input[..^1];

								Canvas.DrawFilledRectangle(X, Y, Font.Fallback.Size, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
								Canvas.Update();
							}
							IsCursorEnabled = true;
							break;
						default:
							if (!char.IsControl(Key.KeyChar))
							{
								Input += Key.KeyChar;
								if (!RedirectOutput)
								{
									Write(Key.KeyChar);
								}
							}
							IsCursorEnabled = true;
							break;
					}
				}
			}
		}
		public static string ReadLine()
		{
			return ReadLine(false);
		}

		#endregion

		#region Write

		public static void Write(string Format, object Value1, object Value2, object Value3) => Write(string.Format(Format, Value1, Value2, Value3));
		public static void Write(string Format, object Value1, object Value2) => Write(string.Format(Format, Value1, Value2));
		public static void Write(string Format, params object[] Value) => Write(string.Format(Format, Value));
		public static void Write(string Format, object Value) => Write(string.Format(Format, Value));
		public static void Write(object Value) => Write(Value ?? string.Empty);
		public static void Write(char[] Value) => Write(new string(Value));
		public static void Write(double Value) => Write(Value.ToString());
		public static void Write(float Value) => Write(Value.ToString());
		public static void Write(ulong Value) => Write(Value.ToString());
		public static void Write(long Value) => Write(Value.ToString());
		public static void Write(uint Value) => Write(Value.ToString());
		public static void Write(int Value) => Write(Value.ToString());
		public static void Write(char[] Value, int Index, int Count)
		{
			if ((Value.Length - Index) < Count)
			{
				throw new ArgumentException($"Specified count '{nameof(Count)}' is more than the buffer length.");
			}
			if (Value == null)
			{
				throw new ArgumentNullException(nameof(Value));
			}
			if (Index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(Index));
			}
			if (Count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(Count));
			}

			for (int I = 0; I < Count; I++)
			{
				Write(Value[Index + I]);
			}
		}
		public static void Write(string Value)
		{
			// Erase Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));

			for (int I = 0; I < Value.Length; I++)
			{
				WriteCore(Value[I]);
			}

			// Draw Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
			Canvas.Update();
		}
		public static void Write(string Value, bool useConstWidth)
		{
			// Erase Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));

			for (int I = 0; I < Value.Length; I++)
			{
				WriteCore(Value[I], useConstWidth);
			}

			// Draw Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
			Canvas.Update();
		}
		public static void Write(char Value)
		{
			// Erase Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));

			WriteCore(Value);

			// Draw Cursor
			Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
			Canvas.Update();
		}
		public static void Write(bool Value)
		{
			Write(Value.ToString());
		}

		/* Decimal type is not working yet... */
		//public static void Write(decimal aDecimal) => Write(aDecimal.ToString());

		#endregion

		#region Misc

		public static Color ColorFromConsleColor(ConsoleColor col)
		{
			return col switch
			{
				ConsoleColor.Black => Color.Black,
				ConsoleColor.DarkBlue => Color.Blue - 64,
				ConsoleColor.DarkGreen => Color.Green - 64,
				ConsoleColor.DarkCyan => Color.Cyan - 64,
				ConsoleColor.DarkRed => Color.Red - 64,
				ConsoleColor.DarkMagenta => Color.Magenta - 64,
				ConsoleColor.DarkYellow => Color.Yellow - 64,
				ConsoleColor.Gray => Color.LightGray,
				ConsoleColor.DarkGray => Color.DeepGray - 64,
				ConsoleColor.Blue => Color.Blue,
				ConsoleColor.Green => Color.Green,
				ConsoleColor.Cyan => Color.Cyan,
				ConsoleColor.Red => Color.Red,
				ConsoleColor.Magenta => Color.Magenta,
				ConsoleColor.Yellow => Color.Yellow,
				ConsoleColor.White => Color.White,
				_ => Color.Black,
			};
		}

		private static void WriteCore(char C, bool cw = false)
		{
			Buffer += C;

			switch (C)
			{
				case '\0':
					break;
				case '\r':
					break;
				case '\n':
					X = SpacingX;
					Y += (int)Font.Fallback.Size;
					break;
				case '\b':
					if (cw)
					{
						X -= 8;
					}
					else
					{
						X -= (int)Font.Fallback.MeasureString(Buffer[^1].ToString());
					}
					break;
				case '\t':
					if (cw)
					{
						X += 8 * 4;
					}
					else
					{
						X += (int)(Font.Fallback.Size * 4);
					}
					break;
				default:
					if (char.IsAscii(C))
					{
						Canvas.DrawFilledRectangle(X, Y, Font.Fallback.MeasureString(C.ToString()), Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
						Canvas.DrawString(X, Y, "" + C, Font.Fallback, ColorFromConsleColor(ForegroundColor), false);

						if (X >= Canvas.Width - SpacingX)
						{
							X = SpacingX;
							Y += Font.Fallback.Size;
						}
						else
						{
							if (cw)
								X += 8;
							else
								X += Font.Fallback.MeasureString(C.ToString());
						}
						if (Y == Canvas.Height)
						{
							MemoryOperations.Copy(Canvas.Internal, Canvas.Internal + (Canvas.Width * Font.Fallback.Size), (int)(Canvas.Size - (Canvas.Width * Font.Fallback.Size)));
						}
					}
					break;
			}
		}

		public static void ResetColor()
		{
			ForegroundColor = ConsoleColor.White;
			BackgroundColor = ConsoleColor.Black;
		}
		public static void Clear()
		{
			X = SpacingX;
			Y = SpacingY;
			Canvas.Clear();
			Canvas.Update();
			Buffer = string.Empty;
		}
		public static void Init()
		{
            Canvas = Display.GetDisplay(1024, 768);
			PIT.RegisterTimer(new(() =>
			{
				if (IsCursorVisible && IsCursorEnabled)
				{
					// Draw Cursor
					Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(ForegroundColor));
					Canvas.Update();
					IsCursorVisible = false;
					return;
				}
				else
				{
					// Erase Cursor
					Canvas.DrawFilledRectangle(X, Y, 1, Font.Fallback.Size, 0, ColorFromConsleColor(BackgroundColor));
					Canvas.Update();
					IsCursorVisible = true;
					return;
				}
			}, 500000000, true));
		}

		#endregion

		#endregion

		#region Fields

		private static bool IsCursorVisible { get; set; } = true;
		private static bool IsCursorEnabled { get; set; } = true;
		private static string Buffer { get; set; } = string.Empty;
		public static Display Canvas { get; set; }
		private static int SpacingX { get; set; } = 5;
		private static int SpacingY { get; set; } = 5;
		private static int X { get; set; } = 5;
		private static int Y { get; set; } = 5;

		public static ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
		public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

		#endregion
	}
}