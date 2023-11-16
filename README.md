# JSON Menu

Customizable menu for the notification area on windows, using JSON and winforms. A sample menu is generated on the first run. This is an hobby project from 2016 and it's not mantained.

## Gallery

<img src="/.repo/left-click.png" alt="Left click" width="49%"> <img src="/.repo/right-click.png" alt="Right click" width="49%"><br>
<img src="/.repo/icons-small.png" alt="Icon picker - small" width="49%"> <img src="/.repo/icons-large.png" alt="Icon picker - large" width="49%">

## Features

- [x] Manual entries
- [x] Automatic directory indexing
- [x] [NirLauncher](http://launcher.nirsoft.net/) packages indexing
- [x] GUI/Console recognition
- [x] Detect current monitor size
- [x] Icon picker
- [x] Launch at startup
- [x] Edit JSON configuration with default app

### Mouse navigation

- Left click — json menu
    - Left click — open
    - Right click — run as administrator
    - Middle click — locate resolved path in explorer
- Right click — internal menu

## Download

See [releases](https://github.com/Microeinstein/JSON-Menu/releases).

## How to build

- Dependencies
    - **.NET framework** 4.5, winforms
    - **Newtonsoft.Json** 9.0.1 ([nuget](https://www.nuget.org/packages/Newtonsoft.Json/9.0.1)) — has vulnerability, must be updated

- Other requirements
    - Windows 7 or later _(?)_
    - Visual Studio

Being a Visual Studio project, compilation steps will be determined automatically.

## Configuration

```javascript
[                                 // list of entries
  {
    "path": "prog.exe",           // file to open
    "args": "/flag /?",           // execution arguments
    "workDir": "D:\\",            // working directory
    "text": "Label",              // label override
    "icon": "res.dll,num",        // icon override
    "items": []                   // submenu entries
  },
  "separator",
  {
    "path": "D:\\Games",          // automatic directory indexing
    "mask": ["*.png"],            // inclusive glob filtering masks
    "maxDepth": 1,                // limit indexing depth
    "showHiddenFiles": false,
    "showHiddenFolders": false,
    "showOnlyFiles": false,
    "showOnlyFolders": true,
    "sortByName": true            // (sort by type by default)
  },
  {
    "NirPack": ".\\package.nlp"   // NirLauncher package
  }
]
```

## License

[GPLv3](COPYING) ([resources](/Resources) excluded)
