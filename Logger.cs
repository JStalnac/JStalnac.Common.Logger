/*
 * Copyright 2020 JStalnac
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Pastel;

namespace JStalnac.Common.Logging
{
    /// <summary>
    /// Log level that the logger uses.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug,
        /// <summary>
        /// Information log level.
        /// </summary>
        Info,
        /// <summary>
        /// Warning log level.
        /// </summary>
        Warning,
        /// <summary>
        /// Error log level.
        /// </summary>
        Error,
        /// <summary>
        /// Important log level.
        /// </summary>
        Important,
        /// <summary>
        /// Critical log level.
        /// </summary>
        Critical,
    }

    public sealed class Logger
    {
        private readonly string name;
        private static LogLevel level = LogLevel.Info;
        private static string logFile;
        // I'm not American
        private static string datetimeFormat = "dd/MM/yyyy HH:mm:ssZzzz";

        private static readonly Regex nameRegex = new Regex("[\x00-\x1F\x7F]", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        /// <param name="name">The name the logger will log messages with.</param>
        public Logger(string name)
        {
            // Sanitize the input
            string cleanName = nameRegex.Replace(name, "");
            if (String.IsNullOrEmpty(cleanName) || String.IsNullOrWhiteSpace(cleanName))
                throw new ArgumentNullException(nameof(name));
            this.name = cleanName;
        }

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        /// <summary>
        /// Creates a console window using AllocConsole.
        /// Works only on Windows
        /// </summary>
        public static void CreateConsole()
        {
            // Only try calling AllocConsole on Windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                AllocConsole();
        }

        /// <summary>
        /// Sets the log file used. If no file is set messages won't be logged to a file.
        /// </summary>
        /// <param name="path"></param>
        public static void SetLogFile(string path)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            try
            {
                // This has flaws
                Path.GetFullPath(path);
            }
            catch
            {
                throw new ArgumentException("Invalid path", nameof(path));
            }

            logFile = path;
        }

        /// <summary>
        /// Sets the minimum log level. The default level is <see cref="LogLevel.Info"/>
        /// </summary>
        /// <param name="logLevel"></param>
        public static void SetLogLevel(LogLevel logLevel) => level = logLevel;

        /// <summary>
        /// Sets the datetime format used.
        /// </summary>
        /// <param name="format"></param>
        public static void SetDateTimeFormat(string format)
        {
            if (String.IsNullOrEmpty(format) || String.IsNullOrWhiteSpace(format))
                throw new ArgumentException(nameof(format));

            try
            {
                DateTime.Now.ToString(format);
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid datetime format");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new FormatException("Invalid datetime");
            }
            datetimeFormat = format;
        }

        /// <summary>
        /// Gets a new logger for the specified type.
        /// </summary>
        /// <returns></returns>
        public static Logger GetLogger<T>(T type)
        {
            return new Logger(typeof(T).FullName);
        }

        /// <summary>
        /// Gets a new logger with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Logger GetLogger(string name)
        {
            return new Logger(name);
        }

        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Debug(string message) => Write(message, LogLevel.Debug);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Debug"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Debug(string message, Exception e) => Write(message, LogLevel.Debug, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Debug"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Debug(object obj) => Write(obj, LogLevel.Debug);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Info"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Info(string message) => Write(message, LogLevel.Info);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Info"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Info(string message, Exception e) => Write(message, LogLevel.Info, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Infp"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Info(object obj) => Write(obj, LogLevel.Info);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Warning(string message) => Write(message, LogLevel.Warning);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Warning(string message, Exception e) => Write(message, LogLevel.Warning, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Warning(object obj) => Write(obj, LogLevel.Warning);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Error(string message) => Write(message, LogLevel.Error);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Error"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Error(string message, Exception e) => Write(message, LogLevel.Error, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Error"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Error(object obj) => Write(obj, LogLevel.Error);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Critical"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Critical(string message) => Write(message, LogLevel.Critical);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Critical"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Critical(string message, Exception e) => Write(message, LogLevel.Critical, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Critical"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Critical(object obj) => Write(obj, LogLevel.Critical);

        static object writeLock = new object();

        /// <summary>
        /// Color used for the <see cref="LogLevel.Debug"/> log level.
        /// </summary>
        /// <value></value>
        public static Color DebugColor { get; set; } = Color.FromArgb(0x0f960d);
        /// <summary>
        /// Color used for the <see cref="LogLevel.Info"/> log level.
        /// </summary>
        /// <value></value>
        public static Color InfoColor { get; set; } = Color.FromArgb(0xeaeaea);
        /// <summary>
        /// Color used for the <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <value></value>
        public static Color WarningColor { get; set; } = Color.FromArgb(0xc6ad0b);
        /// <summary>
        /// Color used for the <see cref="LogLevel.Error"/> log level.
        /// </summary>
        /// <value></value>
        public static Color ErrorColor { get; set; } = Color.FromArgb(0xd30c0c);
        /// <summary>
        /// Color used for the <see cref="LogLevel.Important"/> log level.
        /// </summary>
        /// <value></value>
        public static Color ImportantColor { get; set; } = Color.FromArgb(0x02fcf4);
        /// <summary>
        /// Color used for the <see cref="LogLevel.Critical"/> log level.
        /// </summary>
        /// <value></value>
        public static Color CriticalColor { get; set; } = Color.FromArgb(0xff0000);
        /// <summary>
        /// Default color used for messages
        /// </summary>
        /// <value></value>
        public static Color DefaultColor { get; set; } = Color.LightGray;

        /// <summary>
        /// Writes a log message using the provided log level including an exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        public void Write(string message, LogLevel level, Exception exception = null)
        {
            if (Logger.level <= level)
            {
                // This allows us to write safely multiple lines
                // and to a file.
                lock(writeLock)
                {
                    if (message == null || String.IsNullOrEmpty(message.Trim()))
                        if (exception == null)
                            message = "null"; // No message, no exception

                    var lines = new List<string>();

                    // Append message
                    if (message != null)
                    {
                        message = message.Trim();
                        lines.AddRange(message.Split('\n'));
                    }

                    // Append exception
                    if (exception != null)
                        lines.AddRange(exception.ToString().Split('\n'));

                    string prefix = $"[{DateTime.Now.ToString(datetimeFormat, CultureInfo.InvariantCulture)}] [{name}] [{level}]";

                    // Begin write to file
                    Task fileWrite = null;
                    try
                    {
                        if (!String.IsNullOrEmpty(logFile))
                            fileWrite = File.AppendAllLinesAsync(logFile, lines.Select(x => $"{prefix} {x?.TrimEnd()}"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Failed to write to log file: {ex.Message}");
                    }

                    // Select color
                    Color color;
                    switch (level)
                    {
                        // Change these colors if you want to.
                        case LogLevel.Debug:
                            color = DebugColor;
                            break;
                        case LogLevel.Info:
                            color = InfoColor;
                            break;
                        case LogLevel.Warning:
                            color = WarningColor;
                            break;
                        case LogLevel.Error:
                            color = ErrorColor;
                            break;
                        case LogLevel.Important:
                            color = ImportantColor;
                            break;
                        case LogLevel.Critical:
                            color = CriticalColor;
                            break;
                        default:
                            color = Color.LightGray;
                            break;
                    }

                    // Write to console

                    // dumb optimization
                    prefix = prefix.Pastel(color);
                    foreach (string line in lines)
                        // Prevent ANSI escape sequences in the message
                        Console.WriteLine($"{prefix} {line.Replace("\x1b", "")}");
                    
                    // End write to file
                    if (fileWrite != null)
                        fileWrite.Wait();
                }
            }
        }
        /// <summary>
        /// Writes a log message using an object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="level">Log level</param>
        public void Write(object obj, LogLevel level)
        {
            Write(obj?.ToString(), level, null);
        }
    }
}