﻿using System;
using System.Collections.Generic;

namespace SipaaOS.Core.Text
{
    internal class IniReader
    {
        internal IniReader(string Source)
        {
            this.Source = Source.Replace("\r", "");
            this.Lines = Source.Split('\n');
        }

        internal string Source { get; private set; }
        internal string[] Lines { get; private set; }

        internal string ReadString(string key, string? section = null)
        {
            string _section = string.Empty;
            for (int i = 0; i < Lines.Length; i++)
            {
                string line = Lines[i];

                int equalIndex = line.IndexOf('=');

                if (equalIndex == -1)
                {
                    string trimmed = line.Trim();
                    if (trimmed[0] == '[' && trimmed[trimmed.Length - 1] == ']')
                    {
                        _section = trimmed.Substring(1, trimmed.Length - 2);
                        continue;
                    }
                    else
                    {
                        if (line.Trim() == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            throw new Exception($"Invalid INI syntax on line {i + 1}.");
                        }
                    }
                }
                if (equalIndex < 1)
                {
                    throw new Exception($"Invalid INI syntax on line {i + 1}.");
                }
                string _key = line.Substring(0, equalIndex).Trim();
                if (key == _key)
                {
                    if (section != null)
                    {
                        if (section != _section)
                        {
                            continue;
                        }
                    }
                    if (line.Length >= 3)
                    {
                        return line.Substring(equalIndex + 1).Trim();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            throw new Exception("Key not found.");
        }

        internal int ReadInt(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Format exception: {value}");
            }
        }

        internal bool ReadBool(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Format exception: {value}");
            }
        }

        internal long ReadLong(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (long.TryParse(value, out long result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Format exception: {value}");
            }
        }

        internal float ReadFloat(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (float.TryParse(value, out float result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Format exception: {value}");
            }
        }

        internal List<string> GetSections()
        {
            List<string> sections = new List<string>();
            foreach (var line in this.Lines)
            {
                if (line.IndexOf('=') == -1)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                    {
                        sections.Add(trimmed.Substring(1, trimmed.Length - 2));
                    }
                }
            }
            return sections;
        }

        internal bool TryReadString(string key, out string value, string? section = null)
        {
            try
            {
                value = ReadString(key, section);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        internal bool TryReadBool(string key, out bool value, string? section = null)
        {
            try
            {
                value = ReadBool(key, section);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        internal bool TryReadInt(string key, out int value, string? section = null)
        {
            try
            {
                value = ReadInt(key, section);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        internal bool TryReadFloat(string key, out float value, string? section = null)
        {
            try
            {
                value = ReadFloat(key, section);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}