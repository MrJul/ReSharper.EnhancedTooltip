![Logo](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/images/Logo32.png "Logo") ReSharper.EnhancedTooltip
======

What's ReSharper.EnhancedTooltip?
--------------
It's a plugin for [JetBrains ReSharper](http://www.jetbrains.com/resharper/) that enhances the tooltip and parameter information popup.  

- Colorizes tooltips and parameter information popups for C#.
- Displays icons for identifiers and issues in the tooltip.
- Uses the colors and font configured in Visual Studio.
- Can optionally display more documentation (return type, remarks).
- Can optionally display base type and implemented interfaces in the tooltip.
- Can optionally display full attribute information in the tooltip.
- Everything is configurable.

Installation
------------
Visual Studio 2012, 2013, 2015, 2017 and 2019 are supported.  
ReSharper 2020.2 must be installed.  
(Note: older versions are still available for ReSharper 8.2, 9.x, 10.0, 2016.x, 2017.x, 2018.x, 2019.x and 2020.1).

Install the plugin using the built-in Extension Manager from the ReSharper menu.  

Tooltip Highlighting
--------------------
The tooltip that appears on mouse over is now syntax highlighted. Note that only the tooltips provided by ReSharper are currently colored.  
![Tooltip Highlighting](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/images/Tooltip.png "Tooltip Highlighting")

Parameter Info Highlighting
---------------------------
The parameter information popup is now syntax highlighted.
The colors used for the types are either the ones having a name starting by _ReSharper_ if _Color identifiers_ is used, or the Visual Studio ones starting with _User Types_ otherwise.  
![Parameter Info Highlighting](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/images/ParameterInfo.png "Parameter Info Highlighting")

Options
-------
Enhanced Tooltip provides many options to configure the display as you like.  
The options page is located at ReSharper → Options → Environment → IntelliSense → Enhanced Tooltip.  
![Options Page](https://raw.github.com/MrJul/ReSharper.EnhancedTooltip/master/images/Options.png "Options Page")

Current Limitations
-----------
- Colors only work in C# files. The standard tooltip and parameter information popup are used in other file types.
- The parameter information popup isn't highlighted for calls using named parameters.

Licensed under [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0)
