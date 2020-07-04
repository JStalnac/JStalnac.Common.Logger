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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pastel;

namespace JStalnac.Common.Logger
{
    /// <summary>
    /// Log level that the logger uses.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Critical log level.
        /// </summary>
        Critical,
        /// <summary>
        /// Error log level.
        /// </summary>
        Error,
        /// <summary>
        /// Warning log level.
        /// </summary>
        Warning,
        /// <summary>
        /// Information log level.
        /// </summary>
        Information,
        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug
    }

    public sealed class Logger
    {
        private readonly string name;
        private static LogLevel level = LogLevel.Information;
        private static string logFile;
        // I'm not American
        private static string datetimeFormat = "dd/MM/yyyy HH:mm:ssZzzz";

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        /// <param name="name">The name the logger will log messages with.</param>
        public Logger(string name)
        {
            this.name = name;
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
        /// Sets the minimum log level. The default level is <see cref="LogLevel.Information"/>
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
        /// Writes a log message on <see cref="LogLevel.Information"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Info(string message) => Write(message, LogLevel.Information);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Information"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Info(string message, Exception e) => Write(message, LogLevel.Information, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Infp"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Info(object obj) => Write(obj, LogLevel.Information);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level.
        /// </summary>
        /// <param name="message">Log message</param>
        public void Warn(string message) => Write(message, LogLevel.Warning);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level including an exception.
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name ="e">Exception</param>
        public void Warn(string message, Exception e) => Write(message, LogLevel.Warning, e);
        /// <summary>
        /// Writes a log message on <see cref="LogLevel.Warning"/> log level using the provided object's <see cref="System.Object.ToString"/> method.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Warn(object obj) => Write(obj, LogLevel.Warning);
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
        /// Writes a log message using the provided log level including an exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        public void Write(string message, LogLevel level, Exception exception = null)
        {
            if (Logger.level >= level)
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
                            fileWrite = File.AppendAllLinesAsync(logFile, lines.Select(x => $"{prefix} {x}"));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Failed to write to log file: {ex.Message}");
                    }

                    // Write to console
                    Color color = Color.LightGray;

                    // Select color
                    switch (level)
                    {
                        // Change these colors if you want to.
                        case LogLevel.Debug:
                            color = Color.FromArgb(0x0f960d);
                            break;
                        case LogLevel.Information:
                            color = Color.FromArgb(0xe5e5e5);
                            break;
                        case LogLevel.Warning:
                            color = Color.FromArgb(0xc6ad0b);
                            break;
                        case LogLevel.Error:
                            color = Color.FromArgb(0xd30c0c);
                            break;
                        case LogLevel.Critical:
                            color = Color.FromArgb(0xff0000);
                            break;
                    }

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