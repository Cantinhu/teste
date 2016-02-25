using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;


using Discord.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
namespace DiscordBot.Modules.Sql
{
    internal class sql : IModule
    {

       // Multiserver serversettings = new Multiserver();

        private ModuleManager _manager;
        private DiscordClient _client;
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        // private DataSet DS = new DataSet();
        // private DataTable DT = new DataTable();
        public bool iscommandenabled(string server, string cmd)
        {
            string teste = getsinglevalue("disabledcommands", "server = \"" + server + "\" and command = \"" + cmd + "\"", "server");
           // _client.Log.Info("iscommandenabled", teste);
            if (teste == System.Convert.ToString(server))
                return false;
            else
                return true;
        }
        public string sqlexecutenonquery(string cmd)
        {
            try
            {
                
                sql_con = new SQLiteConnection("Data Source = db.sqlite; Version = 3;");
                sql_con.Open();
                SQLiteCommand command = new SQLiteCommand(cmd, sql_con);
                string rows = command.ExecuteNonQuery().ToString();
                sql_con.Close();
                return rows;

            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
            
        }
        string enquote(string palavra)
        {
            return "\"" + palavra + "\"";
        }
        public string getsinglevalue(string table,string where,string parameter)
        {
            string ret = "";
            try
            { 
                sql_con = new SQLiteConnection("Data Source = db.sqlite;");
                sql_con.Open();
                string sql = "select " + parameter + " from " + table + " where " + where;
                sql_cmd = new SQLiteCommand(sql, sql_con);
                SQLiteDataReader reader = sql_cmd.ExecuteReader();
                if (reader.Read())
                    ret = System.Convert.ToString(reader[0]);
                sql_con.Close();
                return ret;
            }
            catch (System.Exception ex)
            { 
                return ex.Message;
            }


        }
        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;
            manager.CreateCommands("", group =>
            {
                group.MinPermissions((int)PermissionLevel.BotOwner);
                group.CreateCommand("startdb")
                   .MinPermissions((int)PermissionLevel.BotOwner)
                   .Do(async e =>
                   {
                       /* try
                        {
                          //  SQLiteConnection.CreateFile("db.sqlite");
                           /* sql_con = new SQLiteConnection("Data Source = db.sqlite;");
                            sql_con.Open();

                            string cmd = "create table IF NOT EXISTS \"disabledcommands\" (\"server\" int,\"command\" varchar(20) not null)";
                            sql_cmd = new SQLiteCommand(cmd, sql_con);
                             int rowsdisable = sql_cmd.ExecuteNonQuery();
                            await _client.Reply(e, "Table Disabledcommands created (" + rowsdisable + ")");
                            sql_cmd.Dispose();
                            cmd = "create table IF NOT EXISTS \"servers\" (\"server\" int,\"welcome\" text)";
                            sql_cmd = new SQLiteCommand(cmd, sql_con);
                            rowsdisable = sql_cmd.ExecuteNonQuery();
                            await _client.Reply(e, "Table servers created (" + rowsdisable + ")");
                           // sql_cmd.Dispose();
                            foreach (var server in _client.Servers)
                            {
                                cmd = "insert into \"servers\" values (" + server.Id + ")";
                                rowsdisable = sql_cmd.ExecuteNonQuery();
                                await _client.Reply(e, "Inserted server " + server.Id + " in table servers");
                            }
                            sql_con.Close();



                        }
                        catch (System.Exception ex)
                        {
                            await _client.Reply(e, "Error - " + ex.Message);
                        }*/
                       await _client.Reply(e, "sd");
                   });
                group.CreateCommand("execute")
                .Parameter("sqlquery",ParameterType.Unparsed)
                  .MinPermissions((int)PermissionLevel.ServerAdmin)
                  .Do(async e =>
                  {
                      
                          await _client.Reply(e, sqlexecutenonquery(e.Args[0]));
                     
                  });
                group.CreateCommand("view")
                .Parameter("table", ParameterType.Required)
                .MinPermissions((int)PermissionLevel.BotOwner)
                .Do(async e =>
                {

                });
            });
        }
    }
}
