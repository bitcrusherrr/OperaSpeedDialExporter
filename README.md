OperaSpeedDialExporter
======================

As the name suggests, this tool can export items from Opera 15+ Speed dial into HTML file thats compatible with importers in Firefox, Maxthon, Chrome etc

To use tha tool:
Build it.
Launch via OperaSDExporter.exe
It will ask you to navigate it to where the opera's speed dial database is, usually its in location like this: "C:\Users\Username\AppData\Roaming\Opera Software\Opera Developer\favorites.db"
After that the tool will go through it and generate an HTML file to your desktop. 
The file will be called: OperaSpeedDial.html

Point a browser of your choice at the file via the bookmark html import. Have your speed dial from opera as bookmarks in another browser!
Browse your stuffs.

I have used following 3rd party work in this:
SQLite libs are from: http://www.nuget.org/packages/System.Data.SQLite
SQLiteDatabase class is from: http://jpreecedev.com/2013/08/07/getting-started-with-sqlite-and-c/
