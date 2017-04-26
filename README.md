# JSON Menu
This tool allows you to make a quick menu to run everything you want with a JSON-formatted file.  
If doesn't exist **menu.json** it creates a default menu as example.

# Features
  * Custom items
  * Automatic folders list
  * [NirLauncher](http://launcher.nirsoft.net/) packages
  * GUI/Console recognition
  * Multiple monitor size

# Navigation
  System tray icon:
  * Left click for custom menu
  * Right click for internal menu:
    * Show sample menu
    * Edit menu (launches default editor)
    * Find icon
    * Reload menu
    * Launch at startup
    * Exit
  
  Items:
  * Left click to launch
  * Right click to launch as administrator
  * Middle click to find final **path** in explorer

# Syntax
```javascript
[                                 //Array of objects
  {
    "text": "Label",              //...of the item
    "icon": "Path,ID",            //...of the custom icon that override file/directory icon
    "path": "Path",               //...of the file/directory to launch
    "args": "Arguments",          //...of the executable path
    "workDir": "Directory",       //...of the executable path
    "items": []                   //New menu under this item
  },
  "separator",
  {
    "path": "Directory",          //Automatic list of folders tree
    "mask": ["*.png"],            //Array of masks to filters files
    "maxDepth": 1,                //Maximum tree depth
    "showHiddenFiles": false,
    "showHiddenFolders": false,
    "showOnlyFiles": false,       //Doesn't show folders
    "showOnlyFolders": true,      //Doesn't show files
    "sortByName": true            //By default, sorting mode is by type
  },
  {
    "NirPack": ".\\package.nlp"   //NirLauncher package
  }
]
```

# Info
Built-in icons by [Yusuke Kamiyamane](http://p.yusukekamiyamane.com/) (Fugue)  
JSON framework by [Newtonsoft](http://www.newtonsoft.com/json)
