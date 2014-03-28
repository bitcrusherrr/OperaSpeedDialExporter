using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperaSDExporter
{
    class BookmarkFolder
    {
        public string Name;
        public string FolderGuid;
        public List<Bookmark> Bookmarks = new List<Bookmark>();
    }
}
