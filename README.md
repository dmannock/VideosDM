*Open sourced legacy personal project - no longer maintained - see below*

# VideosDM (NextPVR plugin)

This is a plugin for NextPVR I wrote many years ago for personal use. It targets version **.NET 2.0**, since at the time that's what NextPVR used. Therefore, the code remains unrefined, verbose and hasn't seen change. On the plus side, it works just as well now as it did when I wrote it, even with the latest versions (>3.x.x) and does everything I've needed all these years. 

I'm putting the source out there 'as is', hopefully someone else finds a use for it. Feel free to enhance, see contributing below.

## Features ##
- Video sorting by Name, Date, Size (asc/desc)
- Normal playback with resuming
- Play all files in a folder
- Following of links
- Test console (no need to run NextPVR for debugging)

## Dependencies ##
- Requires **.NET 2.0** NextPVR libs (NShared.dll and NUtility.dll) from versions 2.x.x (maybe even 1.x.x but I can't remember)

## Build ##
- Copy the above dependencies to the \libs\NPVR directory
- Build VideosDM (plugin files will be copied to the NextPVR plugin directory)

## Setup ##
- Enable plugin - Open settings (right click > Settings > Plugins > Check VideosDM)
- Set videos path - Plugins > VideosDM > Folder Path
- Videos DM will now show on the main menu

## Credits ##
- [NextPVR](http://www.nextpvr.com/) creators and community. Without them there would be no platform to begin with.
- [ShellLinkNative](http://www.msjogren.net/dotnet/) by Mattias Sjögren.

## Contributing ##
If anyone does find a use and wants to contribute I'm happy to resurrect this project even though it has been parked for some time.

## Licence ##
MIT