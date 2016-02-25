using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace DiscordBot
{
    public class Multiserver
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        public bool isCommandEnabled(string server,string command)
        {
            if(server == null || command == null)
                return false;
            else
            {
                IniFile Test = new IniFile("./Server Settings/" + server + ".ini");
                
                    string value = Test.ReadValue("Commands", command);
                if (value.Contains("false"))
                    return false;
                else
                    return true;
                
            }
        }
    }
}
