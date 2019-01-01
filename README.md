# Portfolio_WPF_App
![https://img.shields.io/badge/Beta-0.0.1.0-green.svg](https://img.shields.io/badge/Beta-2.0.0.0-green.svg)

A small Portfolio app with WPF in a Metro Design and MVVM Design Pattern.

## Design and Wireframe Prototype

In this link you can view the basic design of the app, which was made beforehand. <br/>
You can also play with the Wireframe capabilities. (Press the play button in the top right corner!)

https://www.figma.com/file/PGZbyXHqbWnHZNz4IiCv3sh6/Design-Portfolio-WPF-App?node-id=0%3A1
- - - -
## Short Description of the used libraries and techniques
This app shows the basic usage of the following components:

1. Mahapps Library (Hamburger Menu, Button, SplitButton, DataGrid, ScrollViewer, Checkbox, Login Window, PasswordBox)
2. SQLite Library with Custom DataBaseHandler, QueryBuilder, Custom Trigger 
3. Custom Core DLL (SQLiteHandler, QueryBuilder, FileHandler, XMLSerializer, LogHandler)
4. MVVM Design Pattern with Mediator, RelayCommand
5. Basic Unit Testing with Nunit (Not yet implemented)
- - - -
## Functions
### At start up
At the beginning the app is generating the following files:

* config.ini -> Which saves the name of the last active .xml config file.
* ExampleConfig.xml -> Auto generated at startup if no valid .xml file was found. Includes all necessary Values at Startup.
* example.db -> Auto generated SQLite database with all necessary tables and some test rows.

### Home View
The Home View shows if the DB is connected correctly.<br/>
In the Messages & Report section with some basic information andd clickable Links to the Data View.<br/>
In the top right corner there is also a button to login to get access to the Data View.

![picture alt](https://github.com/JustForFunDeveloper/Portfolio_WPF_App/blob/master/Pictures/HomeView.png?raw=true "Home View")

### Settings View
The Settings View shows the current active config.xml with name and its values.<br/>
With the buttons a new config can be loaded and activated.<br/>
In the User & Password section can the User and Password be changed which is stored in the database.

![picture alt](https://github.com/JustForFunDeveloper/Portfolio_WPF_App/blob/master/Pictures/SettingsView.png?raw=true "Home View")

### Login Window
Standard User: admin<br/>
Standard Password: admin

![picture alt](https://github.com/JustForFunDeveloper/Portfolio_WPF_App/blob/master/Pictures/LoginWindow.png?raw=true "Home View")

### Data View
In the Data View there can be either the Logs from a txt.file be displayed or the data  from the SQLite database.<br/>
The Logs can also be filtered with the checkboxes.<br/>

![picture alt](https://github.com/JustForFunDeveloper/Portfolio_WPF_App/blob/master/Pictures/DataViewLogs.png?raw=true "Home View")

The datagrid can be sorted and multiple entries can be copied to the clipboard.<br/>
The logs as well the data can be saved to a .txt file if necessary.

![picture alt](https://github.com/JustForFunDeveloper/Portfolio_WPF_App/blob/master/Pictures/DataViewDBData.png?raw=true "Home View")

- - - -

# Troubleshooting & FAQ
If you can't find the created database make sure that the x64\ and x86\ folder from Portfolio_WPF_App.Core\bin\Debug is copied into the Portfolio_WPF_App\bin\Debug folder.<br/>
Also check the [Date]_log.txt file for further informatin.
