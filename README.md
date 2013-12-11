![Logo](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/Logo/Logo32.png "Logo") ReSharper.EnhancedTooltip
======

What's ReSharper.EnhancedTooltip?
--------------
It's a plugin for [JetBrains ReSharper](http://www.jetbrains.com/resharper/) 8.1 that colorizes the tooltip as well as the parameter information popup.  

Installation
------------
Visual Studio 2010, 2012 and 2013 are supported.  
ReSharper 8.1 EAP 8.1.22.190 or later must be installed.  
Install the plugin using the built-in Extension Manager from the ReSharper menu. Don't forget to select _Include Prerelease_ in the Extension Manager!  

Tooltip Highlighting
--------------------
The tooltip that appears on mouse over is now syntax highlighted. Note that only the tooltips provided by ReSharper are currently colored. It is strongly suggested that you enable _Color identifiers_ in _ReSharper Options > Code Inspection > Settings_ since there's almost no tooltip at all if this setting is disabled.  
![Tooltip Highlighting](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/Screenshots/Tooltip.png "Tooltip Highlighting")

Parameter Info Highlighting
---------------------------
The parameter information popup is now syntax highlighted.
The colors used for the types either the one having a name starting by _ReSharper_ if _Color identifiers_ is used, or the Visual Studio ones starting with _User Types_ otherwise.  
![Parameter Info Highlighting](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/Screenshots/ParameterInfo.png "Parameter Info Highlighting")

Limitations
-----------
- The plugin currently only works with ReSharper 8.1 EAP.  
- The plugin currently works only on C# files. The standard tooltip and parameter information popup are used in other file types.  
- The parameter information popup isn't highlighted for calls using named parameters.  

Licensed under [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0)
