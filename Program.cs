using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace OperaSDExporter
{
    class Program
    {
        static List<object> BookmarkItems = new List<object>();

        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog dbFile = new OpenFileDialog();
            dbFile.Filter = "Opera Favorites.db|favorites.db";
            dbFile.Title = "Locate Opera favorites.db";

            var result = dbFile.ShowDialog();

            if (result == DialogResult.OK && dbFile.FileName.Contains("favorites.db"))
            {
                try
                {
                    SQLiteDatabase database = new SQLiteDatabase(dbFile.FileName);
                    SQLiteCommand a = database.GetCommand("SELECT * FROM favorites");
                    DataRowCollection rows = database.GetDataTable(a).Rows;

                    #region Awful code
                    //Find list of all "folders"
                    foreach (DataRow row in rows)
                    {
                        //position 0 item guid
                        //position 2 name
                        //position 4 type: 1 - folder 0 - url
                        int position = 0;
                        string guid = string.Empty;
                        string name = string.Empty;
                        bool isFolder = false;

                        foreach (var item in row.ItemArray)
                        {
                            switch (position)
                            {
                                case 0:
                                    guid = item as string;
                                    break;
                                case 2:
                                    name = item as string;
                                    break;
                                case 4:
                                    if ((long)item == 1)
                                        isFolder = true;
                                    break;
                            }
                            position++;
                        }

                        if (isFolder)
                        {
                            BookmarkItems.Add(new BookmarkFolder() { Name = name, FolderGuid = guid });
                        }
                    }

                    //Find list of all "bookmarks"
                    foreach (DataRow row in rows)
                    {
                        //position 0 item guid
                        //position 2 name
                        //position 3 url
                        //position 4 type: 1 - folder 0 - url
                        //position 5 parent guid, if item is in folder
                        int position = 0;
                        string guid = string.Empty;
                        string name = string.Empty;
                        string url = string.Empty;
                        string parentGuid = string.Empty;

                        foreach (var item in row.ItemArray)
                        {
                            switch (position)
                            {
                                case 0:
                                    guid = item as string;
                                    break;
                                case 2:
                                    name = item as string;
                                    break;
                                case 3:
                                    url = item as string;
                                    break;
                                case 5:
                                    parentGuid = item as string;
                                    break;
                            }
                            position++;
                        }

                        if (url != null)
                        {
                            if (parentGuid == string.Empty || parentGuid == null)
                                BookmarkItems.Add(new Bookmark() { Name = name, URL = url });
                            else
                            {
                                BookmarkFolder folder = BookmarkItems.FirstOrDefault(x => x is BookmarkFolder && (x as BookmarkFolder).FolderGuid == parentGuid) as BookmarkFolder;

                                if (folder != null)
                                    folder.Bookmarks.Add(new Bookmark() { Name = name, URL = url });
                            }
                        }
                    }

                    #endregion

                    //Construct html string for the bookmark import file
                    string bookmarkFile = "<!DOCTYPE NETSCAPE-Bookmark-file-1><META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\"><TITLE>Bookmarks</TITLE>"
                        + Environment.NewLine + "<H1>Opera 15+ SpeedDial</H1>"
                        + Environment.NewLine + "<DL><p>" + Environment.NewLine;

                    //Add overall Opera SD Export Folder 
                    bookmarkFile += "<DT><H3 FOLDED>Opera SD</H3>"+ Environment.NewLine +"<DL><p>";

                    foreach (var item in BookmarkItems)
                    {
                        if (item is Bookmark)
                        {
                            bookmarkFile += BookmarkString(item as Bookmark);
                        }
                        else if (item is BookmarkFolder)
                        {
                            bookmarkFile += BookmarkFolderString(item as BookmarkFolder);
                        }
                    }

                    //Close overall file and voerall folder
                    bookmarkFile += Environment.NewLine + "<DL><p>" + Environment.NewLine + "</DL><p>";

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    path = Path.Combine(path, "OperaSpeedDial.html");
                    File.WriteAllText(path, bookmarkFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Nope, we crashed!", MessageBoxButtons.OK);
                }
                finally
                {
                    MessageBox.Show("You can find the exported html on the desktop", "SpeedDial export completed!", MessageBoxButtons.OK);
                }
            }
        }

        //format folder into html string
        private static string BookmarkFolderString(BookmarkFolder bookmarkFolder)
        {
            string result = "<DT><H3 FOLDED>" + bookmarkFolder.Name + "</H3>" + Environment.NewLine + "<DL><p>" + Environment.NewLine;

            foreach (var item in bookmarkFolder.Bookmarks)
                result += BookmarkString(item);

            return result += "</DL><p>" + Environment.NewLine;
        }

        //Format bookmark into the html string
        private static string BookmarkString(Bookmark bookmark)
        {
            return "<DT><A HREF=\"" + bookmark.URL + "\">" + bookmark.Name + "</A>" + Environment.NewLine;
        }
    }
}
