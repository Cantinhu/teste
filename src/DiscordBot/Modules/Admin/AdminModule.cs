using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;


using Discord.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace DiscordBot.Modules.Admin
{

    /// <summary> Provides easy access to manage users from chat. </summary>
    /// 

    internal class AdminModule : IModule
	{

        Sql.sql serversettings = new Sql.sql();
       
        private ModuleManager _manager;
		private DiscordClient _client;
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
       // private DataSet DS = new DataSet();
       // private DataTable DT = new DataTable();

        void IModule.Install(ModuleManager manager)
		{
			_manager = manager;
			_client = manager.Client;
           
			manager.CreateCommands("", group =>
			{
				group.PublicOnly();
                group.CreateCommand("isenabled")
                .MinPermissions((int)PermissionLevel.BotOwner)
                .Parameter("command",ParameterType.Required)
                .Do(async e =>
                {

                    await _client.Reply(e, serversettings.iscommandenabled(e.Server.Id.ToString(), e.Args[0].ToString()).ToString());

                });
                group.CreateCommand("serverid")
                 .MinPermissions((int)PermissionLevel.BotOwner)
                 .Do(async e =>
                 {
                     if (serversettings.iscommandenabled(e.Server.Id.ToString(), e.Command.Text))
                         await _client.Reply(e, e.Server.Id.ToString());
                     else
                         await _client.Reply(e, "This command is disabled on this server.");

                 });
                group.CreateCommand("saveserversettings")
                 .MinPermissions((int)PermissionLevel.BotOwner)
                 .Do(async e =>
                 {
                 foreach (Server s in _client.Servers)
                 {
                     IniFile teste = new IniFile("./Server Settings/" + s.Id.ToString() + ".ini");
                        
                             teste.WriteValue("Commands", "TestCommand", "TesteValue");
                         
                     }
                     await _client.Reply(e, "Done");
                     
                 });
                group.CreateCommand("disablecmd")
                .Parameter("command",ParameterType.Required)
                 .MinPermissions((int)PermissionLevel.ServerAdmin)
                 .Do(async e =>
                 {
                     try
                     {
                         e.Args[0] = e.Args[0].ToLower();
                         sql_con = new SQLiteConnection("Data Source = db.sqlite;");
                         sql_con.Open();
                         string sql = "select server from disabledcommands where server = \"" + e.Server.Id.ToString() + "\" and command = \"" + e.Args[0].ToString() + "\"";
                         sql_cmd = new SQLiteCommand(sql, sql_con);
                         SQLiteDataReader reader = sql_cmd.ExecuteReader();
                         int rows = 0;
                         if (!reader.Read())
                         {
                             sql = "insert into disabledcommands values (\"" + e.Server.Id.ToString() + "\",\"" + e.Args[0].ToString() + "\")";
                             sql_cmd = new SQLiteCommand(sql, sql_con);
                             rows = sql_cmd.ExecuteNonQuery();
                           
                         }
                         sql_con.Close();
                         await _client.Reply(e, "Command disabled (" + rows + ")");


                         
                         
                     }
                     catch (System.Exception ex)
                     {
                         await  _client.Reply(e,ex.Message);
                     }


                    // await _client.Reply(e, "Command " + e.Args[0] + " disabled.");

                 });
                group.CreateCommand("enablecmd")
              .Parameter("command", ParameterType.Unparsed)
               .MinPermissions((int)PermissionLevel.ServerAdmin)
               .Do(async e =>
               {
                   e.Args[0] = e.Args[0].ToLower();

                   try
                   {
                       sql_con = new SQLiteConnection("Data Source = db.sqlite;");
                       sql_con.Open();
                       string sql = "select server from disabledcommands where server = \"" + e.Server.Id.ToString() + "\" and command = \"" + e.Args[0].ToString() + "\"";
                       sql_cmd = new SQLiteCommand(sql, sql_con);
                       SQLiteDataReader reader = sql_cmd.ExecuteReader();
                       int rows = 0;
                       if (reader.Read())
                       {
                           sql = "delete from disabledcommands where server = " + e.Server.Id.ToString() + " and command = \"" + e.Args[0].ToString() + "\"";
                           sql_cmd = new SQLiteCommand(sql, sql_con);
                           rows = sql_cmd.ExecuteNonQuery();
                           await _client.Reply(e, "Command enabled (" + rows + ")");
                       }
                       else
                           await _client.Reply(e, "This command was not disabled.");
                       
                       sql_con.Close();

                   }
                   catch (System.Exception ex)
                   {
                       await _client.Reply(e, ex.Message);
                   }

               });
                group.CreateCommand("kick")
					.Description("Kicks a user from this server.")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
                            
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

                        await user.Kick();
						await _client.Reply(e, $"Kicked user {user.Name}.");
					});
              
				group.CreateCommand("ban")
					.Description("Bans a user from this server.")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

                        await user.Server.Ban(user);
						await _client.Reply(e, $"Banned user {user.Name}.");
					});

				group.CreateCommand("mute")
                    .Alias("castigar")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

						await user.Edit(isMuted: true);
						await _client.Reply(e, $"Muted user {user.Name}.");
					});
				group.CreateCommand("unmute")
                    .Alias("perdoar")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

						await user.Edit(isMuted: false);
						await _client.Reply(e, $"Unmuted user {user.Name}.");
					});
				group.CreateCommand("deafen")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

						await user.Edit(isDeafened: true);
						await _client.Reply(e, $"Deafened user {user.Name}.");
					});
				group.CreateCommand("undeafen")
					.Parameter("user")
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ServerModerator)
					.Do(async e =>
					{
						var user = await _client.FindUser(e, e.Args[0], e.Args[1]);
						if (user == null) return;

						await user.Edit(isDeafened: false);
						await _client.Reply(e, $"Undeafened user {user.Name}.");
					});

				group.CreateCommand("cleanup")
					.Parameter("count")
					.Parameter("user", ParameterType.Optional)
					.Parameter("discriminator", ParameterType.Optional)
					.MinPermissions((int)PermissionLevel.ChannelModerator)
					.Do(async e =>
					{
						int count = int.Parse(e.Args[0]);
						string username = e.Args[1];
						string discriminator = e.Args[2];
						User[] users = null;

						if (username != "")
						{
							users = await _client.FindUsers(e, username, discriminator);
							if (users == null) return;
						}

						IEnumerable<Message> msgs;
						var cachedMsgs = e.Channel.Messages;
						if (cachedMsgs.Count() < count)
							msgs = (await e.Channel.DownloadMessages(count));
						else
							msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(count);

						if (username != "")
							msgs = msgs.Where(x => users.Contains(x.User));

						if (msgs.Any())
						{
                            foreach (var msg in msgs)
                                await msg.Delete();
							await _client.Reply(e, $"Cleaned up {msgs.Count()} messages.");
						}
						else
							await _client.ReplyError(e, $"No messages found.");
					});
			});
		}
	}
}