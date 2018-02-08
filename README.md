# [VoiceActions.NET](https://github.com/HavenDV/VoiceActions.NET) [![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/VoiceActions.NET/search?l=C%23&o=desc&s=&type=Code) [![NuGet](https://img.shields.io/nuget/v/VoiceActions.NET.svg?style=flat)](https://www.nuget.org/packages/VoiceActions.NET) [![codecov](https://codecov.io/gh/HavenDV/VoiceActions.NET/branch/master/graph/badge.svg)](https://codecov.io/gh/HavenDV/VoiceActions.NET) [![License](https://img.shields.io/github/license/HavenDV/VoiceActions.NET.svg?label=License&maxAge=86400)](LICENSE.md) [![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%201.3-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.3.md)

# Description
This library allows you to assign any actions performed on the voice command.

Available recorders:
+ Windows Multimedia API(winmm.dll)

Available voice to text converters:
+ Wit.ai
+ Yandex SpeechKit

# Example

Create manager object:

```cs

var manager = new Manager<Action>
{
    // Select Windows Multimedia API recorder
    Recorder = new WinmmRecorder(),
    // Select Wit.ai voice-to-text converter
    Converter = new WitAiConverter("your-token-here")
};

 ```

Set up actions:

```cs

// when you say "open file explorer"(case insensitive) the manager runs the explorer.exe with the "C:/" base folder
manager.Storage["open file explorer"] = () => Process.Start("explorer.exe", "C:/");
// when you say any text(include empty text) the manager runs your custom action
manager.NewText += text => Console.WriteLine($"You say: {text}");

 ```

Run:

```cs

// Start the recording process
manager.Start();

// Start the recording process. It stops after 3000 milliseconds
manager.StartWithTimeout(3000);

// The first click on the button will start the recording process, the second will leave the recording process and start the action
button.Click += (o, args) => manager.Change();

// The first click on the button will start the recording process, the second or after 3000 milliseconds timeout it will leave the recording process and start the action. 
button.Click += (o, args) => manager.ChangeWithTimeout(3000);

 ```

# Branches

| master(stable) |               |
|----------------|---------------|
|    Travic CI   |    AppVeyor   |  
| [![Build Status](https://api.travis-ci.org/HavenDV/VoiceActions.NET.svg?branch=master)](https://travis-ci.org/HavenDV/VoiceActions.NET) | [![Build status](https://ci.appveyor.com/api/projects/status/6d1qhja444di11pt/branch/master?svg=true)](https://ci.appveyor.com/project/HavenDV/voiceactions-net/branch/master) |

# Contacts
* [mail](mailto:havendv@gmail.com)
