# JSON Menu
This tool allows you to make a quick menu to run everything you want with a JSON-formatted file.  
If doesn't exist a file called **menu.json** it makes a default menu as example.

# Syntax/Features
* Configuration:
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
    "folder": "Directory",        //Automatic folder list
    "mask": ["*.png"],            //Array of masks to filters files
    "maxDepth": 1,                //Depth of search
    "showHiddenFiles": false,
    "showHiddenFolders": false,
    "showOnlyFiles": false,       //Doesn't show folders
    "showOnlyFolders": true,      //Doesn't show files
    "sortByName": true            //Default sorting is file type
  }
]
```
* System tray icon:
  * Left click for menu
  * Right click for internal menu:
    * Reload
    * Edit menu (launch default editor)
    * Find icon
    * Exit
* Items:
  * Left click to launch
  * Right click to launch as administrator
  * Middle click to find **path** in explorer
* Menu:
  * "**More...**" submenu if there are over 31 items

# Download
Windows binaries: https://dl.dropboxusercontent.com/u/87610045/Programmi/JSON%20Menu.zip

# Info
Built-in icons by [Yusuke Kamiyamane](http://p.yusukekamiyamane.com/) (Fugue)  
JSON framework by [Newtonsoft](http://www.newtonsoft.com/json)
