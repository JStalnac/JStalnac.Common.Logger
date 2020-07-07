# JStalnac.Common.Logging
A small logging utility for .Net C#

One file, easy to include!

Allows logging on different levels and also to a file.

## Requirements
 - [Pastel](https://www.nuget.org/packages/Pastel)
    - Install via 
        - .Net CLI:
            - `dotnet add package Pastel`
        - Package Manager
            - `Install-Package Pastel`

## Usage
#### Create a new `Logger`:
```cs
// Like this
var logger = Logger.GetLogger("My Logger");
// or this
Logger.GetLogger(this);
// or like this
new Logger("My Logger");
```

#### Writing a log message
```cs
// Through a shortcut
logger.Info("Hello World!");

// Or directly
logger.Write("Hello World!", LogLevel.Information);

// You can also write exceptions
try
{
    throw new Exception("A very serious error");
}
catch (Exception ex)
{
    logger.Error("An error occured: ", ex);
}

// Objects through their own ToString() methods
logger.Info(1);

// Log levels that are too low won't be logged
logger.Debug("Shh");

// Output:
//    [04/07/2020 16:51:14Z+03:00] [Main] [Information] Hello World!
//    [04/07/2020 16:51:14Z+03:00] [Main] [Information] Hello World!
//    [04/07/2020 16:51:14Z+03:00] [Main] [Error] An error occured:
//    [04/07/2020 16:51:14Z+03:00] [Main] [Error] System.Exception: A very serious error
//    [04/07/2020 16:51:14Z+03:00] [Main] [Error]    at LogTest.Program.Main(String[] args) in ...
//    [04/07/2020 16:51:14Z+03:00] [Main] [Information] 1
```

#### Set the minumum log level
```cs
Logger.SetLogLevel(LogLevel.Information);
```

#### Set log file
```cs
Logger.SetLogFile("logs/log.txt");
```

#### Set datetime format
- See: [Microsoft Docs: Custom date and time format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)
```cs
Logger.SetDateTimeFormat("G");

logger.Info("Bye bye!");

// Output:
//    [07/04/2020 17:00:13] [Main] [Information] Bye bye!
// Funny that you can see when I wrote this
```

#### Creating a new console window (Windows only)
Useful in GUI applications that don't have consoles.
```cs
Logger.CreateConsole();
```
