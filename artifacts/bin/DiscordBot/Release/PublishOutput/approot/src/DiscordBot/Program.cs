using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using DiscordBot.Modules.Admin;
using DiscordBot.Modules.Colors;
using DiscordBot.Modules.Feeds;
using DiscordBot.Modules.Github;
using DiscordBot.Modules.Modules;
using DiscordBot.Modules.Public;
using DiscordBot.Modules.Status;
using DiscordBot.Modules.Twitch;
using DiscordBot.Modules.Music;
using DiscordBot.Modules.Sql;
using DiscordBot.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().Start(args);

        private const string AppName = "VoltBot";
        private const string AppUrl = "https://github.com/RogueException/DiscordBot";
        sql serversettings = new sql();
        public DiscordClient _client;

        private void Start(string[] args)
        {
#if !DNXCORE50
            Console.Title = $"{AppName} (Discord.Net v{DiscordConfig.LibVersion})";
#endif

            GlobalSettings.Load();

            _client = new DiscordClient(x =>
            {
                x.AppName = AppName;
                x.AppUrl = AppUrl;
                x.MessageCacheSize = 0;
                x.UsePermissionsCache = false;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = OnLogMessage;
               
                
            })

            .UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = false;
                x.HelpMode = HelpMode.Public;
                x.ExecuteHandler = OnCommandExecuted;
                
                x.ErrorHandler = OnCommandError;
            })
            
            .UsingModules()
            .UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
                x.EnableMultiserver = true;
                x.EnableEncryption = true;
                x.Bitrate = AudioServiceConfig.MaxBitrate;
                x.BufferLength = 10000;
                
            })
            .UsingPermissionLevels(PermissionResolver)

            .AddService<SettingsService>()
            .AddService<HttpService>()

            .AddModule<AdminModule>("Admin", ModuleFilter.ServerWhitelist)
            .AddModule<ColorsModule>("Colors", ModuleFilter.ServerWhitelist)
            //.AddModule<FeedModule>("Feeds", ModuleFilter.ServerWhitelist)
            //.AddModule<GithubModule>("Repos", ModuleFilter.ServerWhitelist)
            .AddModule<ModulesModule>("Modules", ModuleFilter.None)
            .AddModule<PublicModule>("Public", ModuleFilter.None)
            .AddModule<TwitchModule>("Twitch", ModuleFilter.None)
            .AddModule<StatusModule>("Status", ModuleFilter.ServerWhitelist)
           .AddModule<Music>("Music", ModuleFilter.ServerWhitelist)
              .AddModule<DiscordBot.Modules.Sql.sql>("sql", ModuleFilter.None);



            //.AddModule(new ExecuteModule(env, exporter), "Execute", ModuleFilter.ServerWhitelist);            

#if PRIVATE
            PrivateModules.Install(_client);
#endif
            /* IAudioClient test = new IAudioClient();
             test.*/
            //Convert this method to an async function and connect to the server
            //DiscordClient will automatically reconnect once we've established a connection, until then we loop on our end
            //Note: ExecuteAndWait is only needed for Console projects as Main can't be declared as async. UI/Web applications should *not* use this function.

            _client.UserJoined += async (s, e) =>
            {
                DiscordBot.Modules.Sql.sql sqll = new Modules.Sql.sql();
                string message = sqll.getsinglevalue("servers", "server = " + e.Server.Id, "welcome");
                if(message != "" && message != null && message.Length > 0)
                {
                    Channel pm = await _client.CreatePrivateChannel(e.User.Id);
                    await pm.SendMessage(message);
                }
              
            };
            
        _client.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try
                    {
                        await _client.Connect(GlobalSettings.Discord.Email, GlobalSettings.Discord.Password);
                        _client.SetGame("O Bot");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _client.Log.Error($"Login Failed", ex);
                        await Task.Delay(_client.Config.FailedReconnectDelay);
                    }
                }
            });

        }



        //Display errors that occur when a user tries to run a command
        //(In this case, we hide argcount, parsing and unknown command errors to reduce spam in servers with multiple bots)
        private void OnCommandError(object sender, CommandErrorEventArgs e)
        {
            string msg = e.Exception?.GetBaseException().Message;
            if (msg == null) //No exception - show a generic message
            {
                switch (e.ErrorType)
                {
                    case CommandErrorType.Exception:
                        msg = "Unknown error.";
                        break;
                    case CommandErrorType.BadPermissions:
                        msg = "You do not have permission to run this command.";
                        break;
                    case CommandErrorType.BadArgCount:
                        msg = "You provided the incorrect number of arguments for this command. Use !help <command> for help";
                        break;
                    case CommandErrorType.InvalidInput:
                        msg = "Unable to parse your command, please check your input.";
                        break;
                    case CommandErrorType.UnknownCommand:
                        msg = "That command does not exist. Use !help for a list of available commands";
                        break;
                }
            }
            if (msg != null)
            {
                _client.ReplyError(e, msg);
                _client.Log.Error("Command", msg);
            }
        }
        
        private void OnCommandExecuted(object sender, CommandEventArgs e)
        {
            _client.Log.Info("Command", $"{e.Command.Text} ({e.User.Name})");
           /* if (serversettings.iscommandenabled(e.Server.Id.ToString(), e.Command.Text.ToLower()) == false)
            {
                Exception ex = new Exception();
              
                CommandErrorEventArgs test = new CommandErrorEventArgs(CommandErrorType.Exception, e, ex);
                OnCommandError(sender, test);
                //_client.ReplyError(e, "This command is disabled");
                
            }*/


        }

        private void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            //Color
            ConsoleColor color;
            switch (e.Severity)
            {
                case LogSeverity.Error: color = ConsoleColor.Red; break;
                case LogSeverity.Warning: color = ConsoleColor.Yellow; break;
                case LogSeverity.Info: color = ConsoleColor.White; break;
                case LogSeverity.Verbose: color = ConsoleColor.Gray; break;
                case LogSeverity.Debug: default: color = ConsoleColor.DarkGray; break;
            }

            //Exception
            string exMessage;
            Exception ex = e.Exception;
            if (ex != null)
            {
                while (ex is AggregateException && ex.InnerException != null)
                    ex = ex.InnerException;
                exMessage = ex.Message;
            }
            else
                exMessage = null;

            //Source
            string sourceName = e.Source?.ToString();

            //Text
            string text;
            if (e.Message == null)
            {
                text = exMessage ?? "";
                exMessage = null;
            }
            else
                text = e.Message;

            //Build message
            StringBuilder builder = new StringBuilder(text.Length + (sourceName?.Length ?? 0) + (exMessage?.Length ?? 0) + 5);
            if (sourceName != null)
            {
                builder.Append('[');
                builder.Append(sourceName);
                builder.Append("] ");
            }
            for (int i = 0; i < text.Length; i++)
            {
                //Strip control chars
                char c = text[i];
                if (!char.IsControl(c))
                    builder.Append(c);
            }
            if (exMessage != null)
            {
                builder.Append(": ");
                builder.Append(exMessage);
            }

            text = builder.ToString();
            //if (e.Severity <= LogSeverity.Info)
            //{
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            //}
            /*#if DEBUG
                        System.Diagnostics.Debug.WriteLine(text);
            #endif*/
        }

        private int PermissionResolver(User user, Channel channel)
        {
            if (user.Id == GlobalSettings.Users.DevId)
                return (int)PermissionLevel.BotOwner;
            if (user.Server != null)
            {
                if (user == channel.Server.Owner)
                    return (int)PermissionLevel.ServerOwner;

                var serverPerms = user.ServerPermissions;
                if (serverPerms.ManageRoles)
                    return (int)PermissionLevel.ServerAdmin;
                if (serverPerms.ManageMessages && serverPerms.KickMembers && serverPerms.BanMembers)
                    return (int)PermissionLevel.ServerModerator;

                var channelPerms = user.GetPermissions(channel);
                if (channelPerms.ManagePermissions)
                    return (int)PermissionLevel.ChannelAdmin;
                if (channelPerms.ManageMessages)
                    return (int)PermissionLevel.ChannelModerator;
            }
            return (int)PermissionLevel.User;
        }
    }
}
