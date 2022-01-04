<h1><center> MezLogger </center></h1>
<h1><center> A simple easy to use on screen hud textAPI for VRChat mods using Melonloader!</center></h1>

Instructions for usage, Download MezLogger.cs from the repo and import it into your project

_Notice you will need to be using at least C# V8 to use this, I personally use V9 in my own projects. You will receive 
errors trying to use it on a version below 8. This isn't a hard fix you will just need to go where your " csproj " file is and add `<LangVersion>9</LangVersion>` below the line `<ProjectGUID> blablabla </ProjectGuid>` ( so that LangVer becomes line 8 )_

Once you have imported MezLogger.cs into your project and have set it up you will want to call `MelonCoroutines.Start(MezLogger.MakeUI());`  or whatever corresponds in your own project to this coroutine on application start to init MezLogger.

MezLogger works simularly to `Melonlogger.Msg("Text")` , `Melonlogger.Warn("Text")` & `Melonlogger.Error("Text")` but instead you write `MezLogger.Msg("Text", 2.5f)` , `MezLogger.Warn("Text", 2.5f)` or `MezLogger.Error("Text", 2.5f)` the float value after the text being how long its displayed on the HUD for (in seconds).
