# VoiceActions.NET [![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/VoiceActions.NET/search?l=C%23&o=desc&s=&type=Code) [![NuGet](https://img.shields.io/nuget/v/VoiceActions.NET.svg?style=flat)](https://www.nuget.org/packages/VoiceActions.NET) [![codecov](https://codecov.io/gh/HavenDV/VoiceActions.NET/branch/master/graph/badge.svg)](https://codecov.io/gh/HavenDV/VoiceActions.NET) [![License](https://img.shields.io/github/license/HavenDV/VoiceActions.NET.svg?label=License&maxAge=86400)](LICENSE.md)

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

private ActionsManager ActionsManager { get; set; } = new ActionsManager
{
	// Select recorder which stops after 3000 milliseconds with Windows Multimedia API base recorder
	Recorder = new AutoStopRecorder(new WinmmRecorder(), 3000),
	 // Select Wit.ai voice-to-text converter
	Converter = new WitAiConverter("your-token-here")
};

 ```

Set up actions:

```cs

 // when you say "test" the manager runs the explorer.exe with the "C:/" base folder
ActionsManager.SetCommand("test", "run explorer.exe C:/");
 // when you say "test" the manager runs your custom action
ActionsManager.SetAction("test", () => MessageBox.Show("test"));

 ```

Run:

```cs

// Start the recording process. It stops after 3 seconds (if AutoStopRecorder is selected from the example)
ActionsManager.Start();

// Start the recording process without autostop
ActionsManager.Start(disableAutoStopIfExists: true);

// The first click on the button will start the recording process, the second will leave the recording process and start the action
button.Click += (o, args) => ActionsManager.Change(); 

 ```

# Branches

| master(stable) |               |
|----------------|---------------|
|    Travic CI   |    AppVeyor   |  
| [![Build Status](https://api.travis-ci.org/HavenDV/VoiceActions.NET.svg?branch=master)](https://travis-ci.org/HavenDV/VoiceActions.NET) | [![Build status](https://ci.appveyor.com/api/projects/status/6d1qhja444di11pt/branch/master?svg=true)](https://ci.appveyor.com/project/HavenDV/voiceactions-net/branch/master) |

# Requirements
+ [.NET Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)

# Contacts
* [mail](mailto:havendv@gmail.com)
