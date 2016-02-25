using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.IO;
namespace DiscordBot.Modules.Public
{
    internal class PublicModule : IModule
    {
        //static string[] mages = { "Agni", "Ah Puch", "Anubis", "Ao Kuang", "Aphrodite", "Chang'e", "Chronos", "Freya", "He Bo", "Hel", "Isis", "Janus", "Kukulkan", "Nox", "Nu Wa", "Poseidon", "Ra","Raijin", "Scylla", "Sol", "Vulcan", "Zeus", "Zhong Kui" };
        //static string[] warriors = { "Amaterasu", "Bellona", "Chaac", "Guan Yu", "Hercules", "Odin", "Osiris", "Ravana", "Sun Wukong", "Tyr", "Vamana" };
        //static string[] hunters = { "Ah Muzen Cab", "Anhur", "Apollo", "Artemis", "Chiron", "Cupid", "Hou Yi", "Medusa", "Neith", "Rama", "Ullr", "Xbalanque" };
        //static string[] guardians = { "Ares", "Athena", "Bacchus", "Cabrakan", "Geb", "Hades", "Khepri", "Kumbhakarna", "Sobek", "Sylvanus", "Xing Tian", "Ymir" };
        //static string[] assassins = { "Arachne", "Awilix", "Bakasura", "Bastet", "Fenrir", "Hun Batz", "Kali", "Loki", "Mercury", "Ne Zha", "Nemesis", "Ratatoskr", "Serqet", "Thanatos", "Thor" };
        static string[] mages;
        static string[] warriors;
        static string[] hunters;
        static string[] guardians;
        static string[] assassins;
        static string[] godsall;
        static string[] japaneses ;
        static string[] hindus ;
        static string[] mayans  ;
        static string[] egyptians  ;
        static string[] chineses  ;
        static string[] greeks;
        static string[] norses;
        static string[] romans;


        static string devKey = "1667"; // devKey goes here
        static string authKey = "510CA6A4A829447A9D064C466410B137";  // authKey goes here 
        static string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        static string urlPrefix = "http://api.smitegame.com/smiteapi.svc/";


        static string signature = "";
        static string session = "";





        static string[] roles = { "mage", "guardian", "hunter", "warrior", "assassin","japanese","hindu","mayan","egyptian","chinese","greek","norse","roman" };
        static Random rand = new Random();
        
        public static bool validade_role(string[] role)
        {

            for (int i = 0; i < role.Length; i++)
            {
                if (!roles.Contains(role[i].ToLower()))
                    return false;

            }

            return true;

        }
        private static void createsession()
        {
            signature = GetMD5Hash(devKey + "createsession" + authKey + timestamp);

            // Call the "createsession" API method & wait for synchronous response
            //
            WebRequest request = WebRequest.Create(urlPrefix + "createsessionjson/" + devKey + "/" + signature + "/" + timestamp);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            // Parse returned JSON into "session" data
            //
            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;
               // Console.WriteLine(jsonString + "\n\n\n\n\n");
                var g = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalSettings.SessionInfo>(jsonString);

                session = g.session_id;
                Console.WriteLine("Session ID - " + session);
                // MessageBox.Show(session);
            }

        }
        private static string GetMD5Hash(string input)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            bytes = md5.ComputeHash(bytes);
            var sb = new System.Text.StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
        public static string generaterole()
        {
            return roles[Convert.ToInt32(rand.Next(0, 4))];
        }
        public static void loadgods()
        {
            createsession();
            signature = GetMD5Hash(devKey + "getgods" + authKey + timestamp);


            // Call the "getgods" API method & wait for synchronous response
            //
            string languageCode = "1";

            WebRequest request = WebRequest.Create(urlPrefix + "getgodsjson/" + devKey + "/" + signature + "/" + session + "/" + timestamp + "/" + languageCode);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            // Parse returned JSON into "gods" data
            //
            using (var web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;
                var jsonString = responseFromServer;

                var GodsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GlobalSettings.Gods>>(jsonString);
                string GodsListStr = "";
                List<string> gods = new List<string>();
                List<string> mage = new List<string>();
                List<string> hunter = new List<string>();
                List<string> assassin = new List<string>();
                List<string> warrior = new List<string>();
                List<string> guardian = new List<string>();
                List<string> japanese = new List<string>();
                List<string> hindu = new List<string>();
                List<string> mayan = new List<string>();
                List<string> egyptian = new List<string>();
                List<string> chinese = new List<string>();
                List<string> greek = new List<string>();
                List<string> norse = new List<string>();
                List<string> roman = new List<string>();

                //List<string> gods,mage,hunter,assassin,warrior,guardian,japanese,hindu,mayan,egyptian,chinese,greek,norse,roman = new List<string>();
                foreach (GlobalSettings.Gods x in GodsList)
                {
                    switch (x.Pantheon)
                    {
                        case "Japanese":
                            japanese.Add(x.Name);
                            break;
                        case "Hindu":
                            hindu.Add(x.Name);
                            break;
                        case "Mayan":
                            mayan.Add(x.Name);
                            break;
                        case "Egyptian":
                            egyptian.Add(x.Name);
                            break;
                        case "Chinese":
                            chinese.Add(x.Name);
                            break;
                        case "Greek":
                            greek.Add(x.Name);
                            break;
                        case "Norse":
                            norse.Add(x.Name);
                            break;
                        case "Roman":
                            roman.Add(x.Name);
                            break;
                        default:
                            ;
                            break;
                    }
                    switch (x.Roles)
                    {
                        case " Mage":
                            mage.Add(x.Name);
                            break;
                        case " Hunter":
                            hunter.Add(x.Name);
                            break;
                        case " Assassin":
                            assassin.Add(x.Name);
                            break;
                        case " Warrior":
                            warrior.Add(x.Name);
                            break;
                        case " Guardian":
                            guardian.Add(x.Name);
                            break;
                        default:
                            ;
                            break;
                    }
                    gods.Add(x.Name);
                }
                godsall = gods.ToArray();
                mages = mage.ToArray();
                hunters = hunter.ToArray();
                warriors = warrior.ToArray();
                guardians = guardian.ToArray();
                assassins = assassin.ToArray();
                japaneses = japanese.ToArray();
               hindus = hindu.ToArray();
                mayans = mayan.ToArray();
                egyptians = egyptian.ToArray();
                chineses = chinese.ToArray();
                greeks = greek.ToArray();
                norses = norse.ToArray();
                romans = roman.ToArray();

                //MessageBox.Show("Here are the Gods: " + GodsListStr);
            }
        }
        public static string generategod(string role)
        {
            try
            {
                int i = mages.Length;
            }
            catch(Exception ex)
            { 
                loadgods();
            }



            switch (role)
            {
                case "all":return godsall[Convert.ToInt32(rand.Next(0, godsall.Length))];
                    
                    
                case "mage": return mages[Convert.ToInt32(rand.Next(0, mages.Length))];

                case "warrior": return warriors[Convert.ToInt32(rand.Next(0, warriors.Length))];

                case "hunter": return hunters[Convert.ToInt32(rand.Next(0, hunters.Length))];

                case "guardian": return guardians[Convert.ToInt32(rand.Next(0, guardians.Length))];

                case "assassin": return assassins[Convert.ToInt32(rand.Next(0, assassins.Length ))];
                case "japanese": return japaneses[Convert.ToInt32(rand.Next(0, japaneses.Length ))];
                case "chinese": return chineses[Convert.ToInt32(rand.Next(0, chineses.Length ))];
                case "hindu": return hindus[Convert.ToInt32(rand.Next(0, hindus.Length))];
                case "mayan": return mayans[Convert.ToInt32(rand.Next(0, mayans.Length))];
                case "greek": return greeks[Convert.ToInt32(rand.Next(0, greeks.Length))];
                case "norse": return norses[Convert.ToInt32(rand.Next(0, norses.Length))];
                case "roman": return romans[Convert.ToInt32(rand.Next(0, romans.Length))];

                default: return "Error!";
            }

        }
        private ModuleManager _manager;
        private DiscordClient _client;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", group =>
            {
                group.MinPermissions((int)PermissionLevel.User);

                group.CreateCommand("request")
                .Description("Sends a private message to the bot owner with your request")
                .Parameter("req", ParameterType.Unparsed)
                .MinPermissions((int)PermissionLevel.User)
                .Do(async e =>
                {
                    if (e.Args[0].Length < 5 || e.Args[0] == null)
                        await _client.Reply(e, "Your request has to be longer than 5 characters.");
                    else
                    { 
                    Channel pm = await _client.CreatePrivateChannel(118021290757062659);
                    await pm.SendMessage(e.User.ToString() + "(" + e.User.Id + ") from " + e.Server.Name + "(" + e.Server.Id + ") said " + e.Args[0]);
                    await _client.Reply(e, "Message sent! Thanks for your suggestion.");
                    }
                });
                group.CreateCommand("god")
                .Description("Generates a random god")
                .Parameter("role", ParameterType.Optional)
                .MinPermissions((int)PermissionLevel.User)
                .Do(async e =>
                {
                    if(e.Args[0] == "" || e.Args[0] == null)
                        await e.Channel.SendMessage(e.User.Mention + " your god is " + generategod("all"));
                    else
                    { 
                        if (validade_role(e.Args))
                            await e.Channel.SendMessage(e.User.Mention + " your god is " + generategod(e.Args[0]));
                        else
                        {
                            await e.Channel.SendMessage("Invalid role. List of roles: " + string.Join(", ", roles));
                        }
                    }
                });
                group.CreateCommand("patch")
                    .Description("Shows the latest patch notes")
                    .MinPermissions((int)PermissionLevel.User)
                    .Do(async e =>
                    {
                    await e.Channel.SendMessage("Latest patch notes - https://www.smitegame.com/pts-notes-rolling-thunder-3-2/");
                    });
                group.CreateCommand("player")
                    .Description("Links players Smite.guru page")
                    .Parameter("Smite nickname",ParameterType.Required)
                    .MinPermissions((int)PermissionLevel.User)
                    .Do(async e =>
                    {
                        await _client.Reply(e, "http://smite.guru/stats/hr/" + e.Args[0] + "/summary");
                    });
                group.CreateCommand("join")
                    .Description("Requests the bot to join another server.")
                    .Parameter("invite url")
                    .MinPermissions((int)PermissionLevel.BotOwner)
                    .Do(async e =>
                    {
                        var invite = await _client.GetInvite(e.Args[0]);
                        if (invite == null)
                        {
                            await _client.Reply(e, $"Invite not found.");
                            return;
                        }
                        else if (invite.IsRevoked)
                        {
                            await _client.Reply(e, $"This invite has expired or the bot is banned from that server.");
                            return;
                        }

                        await invite.Accept();
                        await _client.Reply(e, $"Joined server.");
                    });
                group.CreateCommand("game")
                .Description("Allows you to change the game the bot is playing")
                  .Parameter("game", ParameterType.Unparsed)
                 .MinPermissions((int)PermissionLevel.ServerAdmin)
               .Do(e =>
               {
                   
                   e.User.Client.SetGame(e.Args[0]);


               });
                group.CreateCommand("leave")
                    .Description("Instructs the bot to leave this server.")
                    .MinPermissions((int)PermissionLevel.ServerModerator)
                    .Do(async e =>
                    {
                        await _client.Reply(e, $"Leaving~");
                        await e.Server.Leave();
                    });

                group.CreateCommand("say")
                    .Parameter("Text", ParameterType.Unparsed)
                    .MinPermissions((int)PermissionLevel.BotOwner)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage(e.Message.Resolve(Format.Escape(e.Args[0])));
                    });
                group.CreateCommand("sayraw")
                    .Parameter("Text", ParameterType.Unparsed)
                    .MinPermissions((int)PermissionLevel.BotOwner)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage(e.Args[0]);
                    });
                group.CreateCommand("team")
                   .Description("Generates a random team. The user has the ability to choose the roles.")
                   .Parameter("number", ParameterType.Required)
                   .Parameter("roles",ParameterType.Unparsed)
                   

                   .MinPermissions((int)PermissionLevel.User)
                   .Do(async e =>
                   {
                       if (Regex.IsMatch(e.Args[0], "[3-5]"))
                       {
                           if(e.Args[1] == "")
                           { 
                               string team = e.User.Mention + " your team is ";
                               for (int i = 0; i < Convert.ToInt32(e.Args[0]); i++)
                               {
                                   team += generategod(roles[i]) + " ";
                               }
                               await e.Channel.SendMessage(team);
                           }
                           else
                           {
                               string[] teamarray = new string[Convert.ToInt32(e.Args[0])];
                               string[] teamargs = e.Args[1].Split(' ');
                               int c = 0;
                               while (c != Convert.ToInt32(e.Args[0]))
                               {
                                   if (c < teamargs.Length)
                                       teamarray[c] = teamargs[c];
                                   else
                                       teamarray[c] = generaterole();
                                   c++;
                               }
                               if (validade_role(teamarray))
                               {
                                   string team = e.User.Mention + " your team is  ";
                                   for (int i = 0; i < Convert.ToInt32(e.Args[0]); i++)
                                   {
                                       string god;
                                       do
                                       {
                                           god = generategod(teamarray[i]);

                                       } while (team.Contains(god));

                                       team += god + " ";
                                   }
                                   await e.Channel.SendMessage( team);
                               }
                               else
                                   await e.Channel.SendMessage("Invalid role. List of roles:  " + string.Join(", ", roles));
                           }
                       }
                       else
                           await e.Channel.SendMessage("Error! The number of gods in a team has to be between 3 and 5.");

                   });
                group.CreateCommand("info")
                    .Alias("about")
                    .MinPermissions((int)PermissionLevel.BotOwner)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage(
                            $"{Format.Bold("Info")}\n" +
                            $"- Author: Voltana (ID 53905483156684800)\n" +
                            $"- Library: {DiscordConfig.LibName} ({DiscordConfig.LibVersion})\n" +
                            $"- Runtime: {GetRuntime()} {GetBitness()}\n" +
                            $"- Uptime: {GetUptime()}\n\n" +

                            $"{Format.Bold("Stats")}\n" +
                            $"- Heap Size: {GetHeapSize()} MB\n" +
                            $"- Servers: {_client.Servers.Count()}\n" +
                            $"- Channels: {_client.Servers.Sum(x => x.AllChannels.Count())}\n" +
                            $"- Users: {_client.Servers.Sum(x => x.Users.Count())}"
                        );
                    });
            });
        }

        private string GetRuntime()
#if NET11
            => ".Net Framework 1.1";
#elif NET20
            => ".Net Framework 2.0";
#elif NET35
            => ".Net Framework 3.5";
#elif NET40
            => ".Net Framework 4.0";
#elif NET45
            => ".Net Framework 4.5";
#elif NET451
            => ".Net Framework 4.5.1";
#elif NET452
            => ".Net Framework 4.5.2";
#elif NET46
            => ".Net Framework 4.6";
#elif NET461
            => ".Net Framework 4.6.1";
#elif NETCORE50
            => ".Net Core 5.0";
#elif DNX451
            => "DNX (.Net Framework 4.5.1)";
#elif DNX452
            => "DNX (.Net Framework 4.5.2)";
#elif DNX46
            => "DNX (.Net Framework 4.6)";
#elif DNX461
            => "DNX (.Net Framework 4.6.1)";
#elif DNXCORE50
            => "DNX (.Net Core 5.0)";
#elif DOTNET50 || NETPLATFORM10
            => ".Net Platform Standard 1.0";
#elif DOTNET51 || NETPLATFORM11
            => ".Net Platform Standard 1.1";
#elif DOTNET52 || NETPLATFORM12
            => ".Net Platform Standard 1.2";
#elif DOTNET53 || NETPLATFORM13
            => ".Net Platform Standard 1.3";
#elif DOTNET54 || NETPLATFORM14
            => ".Net Platform Standard 1.4";
#else
            => "Unknown";
#endif

        private static string GetBitness()=> $"{IntPtr.Size * 8}-bit";
        private static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
