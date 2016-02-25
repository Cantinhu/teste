using System;
using System.Threading.Tasks;
using Discord.Modules;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Discord.Commands;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;

namespace DiscordBot.Modules
{
    class Searches : IModule {
        private Random _r;
        public Searches() : base() {
            // commands.Add(new OsuCommands());
            _r = new Random();
        }
        private ModuleManager _manager;
        private DiscordClient _client;
        void IModule.Install(ModuleManager manager) {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb => {

                //commands.ForEach(cmd => cmd.Init(cgb));

                cgb.CreateCommand("yt")
                    .Parameter("query", ParameterType.Unparsed)
                    .Description("Searches youtubes and shows the first result")
                    .Do(async e => {
                        if (!(await ValidateQuery(e.Channel, e.GetArg("query")))) return;

                        var str = await ShortenUrl(await FindYoutubeUrlByKeywords(e.GetArg("query")));
                        if (string.IsNullOrEmpty(str.Trim())) {
                            await _client.Reply(e,"Query failed");
                            return;
                        }
                        await _client.Reply(e, str);
                    });

               /* cgb.CreateCommand("~ani")
                    .Alias("~anime").Alias("~aq")
                    .Parameter("query", ParameterType.Unparsed)
                    .Description("Queries anilist for an anime and shows the first result.")
                    .Do(async e => {
                        if (!(await ValidateQuery(e.Channel, e.GetArg("query")))) return;

                        var result = await GetAnimeQueryResultLink(e.GetArg("query"));
                        if (result == null) {
                            await e.Send("Failed to find that anime.");
                            return;
                        }

                        await e.Send(result.ToString());
                    });

                cgb.CreateCommand("~mang")
                    .Alias("~manga").Alias("~mq")
                    .Parameter("query", ParameterType.Unparsed)
                    .Description("Queries anilist for a manga and shows the first result.")
                    .Do(async e => {
                        if (!(await ValidateQuery(e.Channel, e.GetArg("query")))) return;

                        var result = await GetMangaQueryResultLink(e.GetArg("query"));
                        if (result == null) {
                            await e.Send("Failed to find that anime.");
                            return;
                        }
                        await e.Send(result.ToString());
                    });

                cgb.CreateCommand("~randomcat")
                    .Description("Shows a random cat image.")
                    .Do(async e => {
                        try {
                            await e.Send(JObject.Parse(new StreamReader(
                                WebRequest.Create("http://www.random.cat/meow")
                                    .GetResponse()
                                    .GetResponseStream())
                                .ReadToEnd())["file"].ToString());
                        } catch (Exception) { }
                    });

                cgb.CreateCommand("~i")
                   .Description("Pulls a first image using a search parameter. Use ~ir for different results.\n**Usage**: ~i cute kitten")
                   .Parameter("query", ParameterType.Unparsed)
                       .Do(async e => {
                           if (string.IsNullOrWhiteSpace(e.GetArg("query")))
                               return;
                           try {
                               var reqString = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(e.GetArg("query"))}&cx=018084019232060951019%3Ahs5piey28-e&num=1&searchType=image&fields=items%2Flink&key={NadekoBot.creds.GoogleAPIKey}";
                               var obj = JObject.Parse(await GetResponseAsync(reqString));
                               await e.Send(obj["items"][0]["link"].ToString());
                           } catch (Exception ex) {
                               await e.Send($"💢 {ex.Message}");
                           }
                       });

                cgb.CreateCommand("~ir")
                   .Description("Pulls a random image using a search parameter.\n**Usage**: ~ir cute kitten")
                   .Parameter("query", ParameterType.Unparsed)
                       .Do(async e => {
                           if (string.IsNullOrWhiteSpace(e.GetArg("query")))
                               return;
                           try {
                               var reqString = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(e.GetArg("query"))}&cx=018084019232060951019%3Ahs5piey28-e&num=1&searchType=image&start={ _r.Next(1, 150) }&fields=items%2Flink&key={NadekoBot.creds.GoogleAPIKey}";
                               var obj = JObject.Parse(await GetResponseAsync(reqString));
                               await e.Send(obj["items"][0]["link"].ToString());
                           } catch (Exception ex) {
                               await e.Send($"💢 {ex.Message}");
                           }
                       });

                cgb.CreateCommand("~hentai")
                    .Description("Shows a random NSFW hentai image from gelbooru and danbooru with a given tag. Tag is optional but preffered.\n**Usage**: ~hentai yuri")
                    .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e => {
                        string tag = e.GetArg("tag");
                        if (tag == null)
                            tag = "";
                        await e.Send(":heart: Gelbooru: " + await GetGelbooruImageLink(tag));
                        await e.Send(":heart: Danbooru: " + await GetDanbooruImageLink(tag));
                    });
                cgb.CreateCommand("~danbooru")
                    .Description("Shows a random hentai image from danbooru with a given tag. Tag is optional but preffered.\n**Usage**: ~hentai yuri")
                    .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e => {
                        string tag = e.GetArg("tag");
                        if (tag == null)
                            tag = "";
                        await e.Send(await GetDanbooruImageLink(tag));
                    });
                cgb.CreateCommand("~gelbooru")
                    .Description("Shows a random hentai image from gelbooru with a given tag. Tag is optional but preffered.\n**Usage**: ~hentai yuri")
                    .Parameter("tag", ParameterType.Unparsed)
                    .Do(async e => {
                        string tag = e.GetArg("tag");
                        if (tag == null)
                            tag = "";
                        await e.Send(await GetGelbooruImageLink(tag));
                    });
                cgb.CreateCommand("~cp")
                    .Description("We all know where this will lead you to.")
                    .Parameter("anything", ParameterType.Unparsed)
                    .Do(async e => {
                        await e.Send("http://i.imgur.com/MZkY1md.jpg");
                    });
                cgb.CreateCommand("~boobs")
                    .Description("Real adult content.")
                    .Do(async e => {
                        try {
                            var obj = JArray.Parse(await GetResponseAsync($"http://api.oboobs.ru/boobs/{_r.Next(0, 9304)}"))[0];
                            await e.Send($"http://media.oboobs.ru/{ obj["preview"].ToString() }");
                        } catch (Exception ex) {
                            await e.Send($"💢 {ex.Message}");
                        }
                    });
                cgb.CreateCommand("lmgtfy")
                    .Alias("~lmgtfy")
                    .Description("Google something for an idiot.")
                    .Parameter("ffs", ParameterType.Unparsed)
                    .Do(async e => {
                        if (e.GetArg("ffs") == null || e.GetArg("ffs").Length < 1) return;
                        await e.Send(await $"http://lmgtfy.com/?q={ Uri.EscapeUriString(e.GetArg("ffs").ToString()) }".ShortenUrl());
                    });

               /* cgb.CreateCommand("~hs")
                  .Description("Searches for a Hearthstone card and shows its image. Takes a while to complete.\n**Usage**:~hs Ysera")
                  .Parameter("name", ParameterType.Unparsed)
                  .Do(async e => {
                      var arg = e.GetArg("name");
                      if (string.IsNullOrWhiteSpace(arg)) {
                          await e.Send("💢 Please enter a card name to search for.");
                          return;
                      }
                      await e.Channel.SendIsTyping();
                      var res = await GetResponseAsync($"https://omgvamp-hearthstone-v1.p.mashape.com/cards/search/{Uri.EscapeUriString(arg)}",
                          new Tuple<string, string>[] {
                              new Tuple<string, string>("X-Mashape-Key", NadekoBot.creds.MashapeKey),
                          });
                      try {
                          var items = JArray.Parse(res);
                          List<System.Drawing.Image> images = new List<System.Drawing.Image>();
                          if (items == null)
                              throw new KeyNotFoundException("Cannot find a card by that name");
                          int cnt = 0;
                          items.Shuffle();
                          foreach (var item in items) {
                              if (cnt >= 4)
                                  break;
                              if (!item.HasValues || item["img"] == null)
                                  continue;
                              cnt++;
                              images.Add(System.Drawing.Bitmap.FromStream(await GetResponseStream(item["img"].ToString())));
                          }
                          if (items.Count > 4) {
                              await e.Send("⚠ Found over 4 images. Showing random 4.");
                          }
                          Console.WriteLine("Start");
                          await e.Channel.SendFile(arg + ".png", (await images.MergeAsync()).ToStream(System.Drawing.Imaging.ImageFormat.Png));
                          Console.WriteLine("Finish");
                      } catch (Exception ex) {
                          await e.Send($"💢 Error {ex.Message}");
                      }
                  });

                cgb.CreateCommand("~osu")
                  .Description("Shows osu stats for a player\n**Usage**:~osu Name")
                  .Parameter("usr", ParameterType.Unparsed)
                  .Do(async e => {
                      if (string.IsNullOrWhiteSpace(e.GetArg("usr")))
                          return;

                      using (WebClient cl = new WebClient()) {
                          try {
                              cl.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                              cl.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.2; Win64; x64)");
                              cl.DownloadDataAsync(new Uri($"http://lemmmy.pw/osusig/sig.php?uname={ e.GetArg("usr") }&flagshadow&xpbar&xpbarhex&pp=2"));
                              cl.DownloadDataCompleted += async (s, cle) => {
                                  try {
                                      await e.Channel.SendFile($"{e.GetArg("usr")}.png", new MemoryStream(cle.Result));
                                      await e.Send($"`Profile Link:`https://osu.ppy.sh/u/{Uri.EscapeDataString(e.GetArg("usr"))}\n`Image provided by https://lemmmy.pw/osusig`");
                                  } catch (Exception) { }
                              };
                          } catch {
                              await e.Channel.SendMessage("💢 Failed retrieving osu signature :\\");
                          }
                      }
                  });*/

                //todo when moved from parse
                /*
                cgb.CreateCommand("~osCantinhu - Today at 8:55 PM
!help
DiscordBot - Today at 8:55 PM
These are the commands you can use:
Modules: modules*
Public: god, patch, player, join, game, leave, say, sayraw, team, info
Twitch: streams*
Music: music*

Run help <command> for more information.
Cantinhu - Today at 8:55 PM
!help music
DiscordBot - Today at 8:55 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:55 PM
!music pl https://www.youtube.com/playlist?list=PLyCOzA7tra20R2yG2e6bWNx_pm3QyJYWi
YouTube
#Disturbed
Indestructible


DiscordBot - Today at 8:55 PM
Cantinhu: 🎵 Attempting to queue 0 songs
Cantinhu: 🎵 Attempting to queue 0 songs
Cantinhu - Today at 8:56 PM
!help music
DiscordBot - Today at 8:56 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:56 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:56 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 8:57 PM
!music p
DiscordBot - Today at 8:57 PM
Cantinhu: 🎵Music player paused.
Cantinhu - Today at 8:58 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:58 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 8:58 PM
!music play
DiscordBot - Today at 8:58 PM
Cantinhu: Error: That command does not exist. Use !help for a list of available commands
Cantinhu - Today at 8:58 PM
!help music
DiscordBot - Today at 8:58 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:59 PM
!music playing
DiscordBot - Today at 8:59 PM
Cantinhu: Error: Object reference not set to an instance of an object.
Cantinhu - Today at 8:59 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:59 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 9:49 PM
!modules enable music
DiscordBot - Today at 9:49 PM
Cantinhu: Module music was enabled for server Os Cornetas.
Cantinhu - Today at 9:49 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:50 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 9:50 PM
!music leave
DiscordBot - Today at 9:50 PM
Cantinhu: Error: That command does not exist. Use !help for a list of available commands
Cantinhu - Today at 9:50 PM
!help music
DiscordBot - Today at 9:50 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 9:50 PM
!music stop
DiscordBot - Today at 9:50 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu - Today at 9:54 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:54 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 9:54 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


!music q httpsCantinhu - Today at 8:55 PM
!help
DiscordBot - Today at 8:55 PM
These are the commands you can use:
Modules: modules*
Public: god, patch, player, join, game, leave, say, sayraw, team, info
Twitch: streams*
Music: music*

Run help <command> for more information.
Cantinhu - Today at 8:55 PM
!help music
DiscordBot - Today at 8:55 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:55 PM
!music pl https://www.youtube.com/playlist?list=PLyCOzA7tra20R2yG2e6bWNx_pm3QyJYWi
YouTube
#Disturbed
Indestructible


DiscordBot - Today at 8:55 PM
Cantinhu: 🎵 Attempting to queue 0 songs
Cantinhu: 🎵 Attempting to queue 0 songs
Cantinhu - Today at 8:56 PM
!help music
DiscordBot - Today at 8:56 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:56 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:56 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 8:57 PM
!music p
DiscordBot - Today at 8:57 PM
Cantinhu: 🎵Music player paused.
Cantinhu - Today at 8:58 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:58 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 8:58 PM
!music play
DiscordBot - Today at 8:58 PM
Cantinhu: Error: That command does not exist. Use !help for a list of available commands
Cantinhu - Today at 8:58 PM
!help music
DiscordBot - Today at 8:58 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 8:59 PM
!music playing
DiscordBot - Today at 8:59 PM
Cantinhu: Error: Object reference not set to an instance of an object.
Cantinhu - Today at 8:59 PM
!music q https://youtu.be/aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


DiscordBot - Today at 8:59 PM
🎵Queued【 Disturbed - Indestructible [Official Music Video] 】YouTube(edited)
Cantinhu - Today at 9:49 PM
!modules enable music
DiscordBot - Today at 9:49 PM
Cantinhu: Module music was enabled for server Os Cornetas.
Cantinhu - Today at 9:49 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:50 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 9:50 PM
!music leave
DiscordBot - Today at 9:50 PM
Cantinhu: Error: That command does not exist. Use !help for a list of available commands
Cantinhu - Today at 9:50 PM
!help music
DiscordBot - Today at 9:50 PM
music
Sub Commands: 
n, next, s, stop, p, pause, q, yq, lq, ls, lp, np, playing, vol, min, mute, max, half, sh, pl, radio, ra, debug
Cantinhu - Today at 9:50 PM
!music stop
DiscordBot - Today at 9:50 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu - Today at 9:54 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:54 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 9:54 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:57 PM
🎵Queued【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube(edited)
Cantinhu - Today at 9:57 PM
!music next
DiscordBot - Today at 9:57 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 10:05 PM
!modules enable music
DiscordBot - Today at 10:05 PM
Cantinhu: Module music was enabled for server Os Cornetas.
Cantinhu - Today at 10:05 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 10:06 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 10:08 PM
!music q https://www.youtube.com/watch?v=aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


!music next
DiscordBot - Today at 10:08 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu: 🎵Playing【 Disturbed - Indestructible [Official Music Video] 】YouTube Vol: 50%
://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 9:57 PM
🎵Queued【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube(edited)
Cantinhu - Today at 9:57 PM
!music next
DiscordBot - Today at 9:57 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 10:05 PM
!modules enable music
DiscordBot - Today at 10:05 PM
Cantinhu: Module music was enabled for server Os Cornetas.
Cantinhu - Today at 10:05 PM
!music q https://www.youtube.com/watch?v=u9Dg-g7t2l4
YouTube
Disturbed
Disturbed - The Sound Of Silence [Official Music Video]


DiscordBot - Today at 10:06 PM
Cantinhu: 🎵Playing【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube Vol: 50%
Cantinhu - Today at 10:08 PM
!music q https://www.youtube.com/watch?v=aWxBrI0g1kE
YouTube
Disturbed
Disturbed - Indestructible [Official Music Video]


!music next
DiscordBot - Today at 10:08 PM
Cantinhu: 🎵Finished【 Disturbed  - The Sound Of Silence [Official Music Vi... 】YouTube
Cantinhu: 🎵Playing【 Disturbed - Indestructible [Official Music Video] 】YouTube Vol: 50%
ubind")
                    .Description("Bind discord user to osu name\n**Usage**: ~osubind My osu name")
                    .Parameter("osu_name", ParameterType.Unparsed)
                    .Do(async e => {
                        var userName = e.GetArg("user_name");
                        var osuName = e.GetArg("osu_name");
                        var usr = e.Server.FindUsers(userName).FirstOrDefault();
                        if (usr == null) {
                            await e.Send("Cannot find that discord user.");
                            return;
                        }
                    });
                */
            });
        }

        public static async Task<Stream> GetResponseStream(string v) {
            var wr = (HttpWebRequest)WebRequest.Create(v);
            try {
                return (await (wr).GetResponseAsync()).GetResponseStream();
            } catch (Exception ex) {
                Console.WriteLine("error in getresponse stream " + ex);
                return null;
            }
        }

        public static async Task<string> GetResponseAsync(string v) =>
            await new StreamReader((await ((HttpWebRequest)WebRequest.Create(v)).GetResponseAsync()).GetResponseStream()).ReadToEndAsync();

        public static async Task<string> GetResponseAsync(string v, IEnumerable<Tuple<string, string>> headers) {
            var wr = (HttpWebRequest)WebRequest.Create(v);
            foreach (var header in headers) {
                wr.Headers.Add(header.Item1, header.Item2);
            }
            return await new StreamReader((await wr.GetResponseAsync()).GetResponseStream()).ReadToEndAsync();
        }

        private string token = "";
       /* private async Task<AnimeResult> GetAnimeQueryResultLink(string query) {
            try {
                var cl = new RestSharp.RestClient("http://anilist.co/api");
                var rq = new RestSharp.RestRequest("/auth/access_token", RestSharp.Method.POST);

                RefreshAnilistToken();

                rq = new RestSharp.RestRequest("/anime/search/" + Uri.EscapeUriString(query));
                rq.AddParameter("access_token", token);

                var smallObj = JArray.Parse(cl.Execute(rq).Content)[0];

                rq = new RestSharp.RestRequest("anime/" + smallObj["id"]);
                rq.AddParameter("access_token", token);
                return await Task.Run(() => JsonConvert.DeserializeObject<AnimeResult>(cl.Execute(rq).Content));
            } catch (Exception) {
                return null;
            }
        }*/
        //todo kick out RestSharp and make it truly async
      /*  private async Task<MangaResult> GetMangaQueryResultLink(string query) {
            try {
                RefreshAnilistToken();

                var cl = new RestSharp.RestClient("http://anilist.co/api");
                var rq = new RestSharp.RestRequest("/auth/access_token", RestSharp.Method.POST);
                rq = new RestSharp.RestRequest("/manga/search/" + Uri.EscapeUriString(query));
                rq.AddParameter("access_token", token);

                var smallObj = JArray.Parse(cl.Execute(rq).Content)[0];

                rq = new RestSharp.RestRequest("manga/" + smallObj["id"]);
                rq.AddParameter("access_token", token);
                return await Task.Run(() => JsonConvert.DeserializeObject<MangaResult>(cl.Execute(rq).Content));
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }*/

        private void RefreshAnilistToken() {
            try {
                var cl = new RestSharp.RestClient("http://anilist.co/api");
                var rq = new RestSharp.RestRequest("/auth/access_token", RestSharp.Method.POST);
                rq.AddParameter("grant_type", "client_credentials");
                rq.AddParameter("client_id", "kwoth-w0ki9");
                rq.AddParameter("client_secret", "Qd6j4FIAi1ZK6Pc7N7V4Z");
                var exec = cl.Execute(rq);
                /*
                Console.WriteLine($"Server gave me content: { exec.Content }\n{ exec.ResponseStatus } -> {exec.ErrorMessage} ");
                Console.WriteLine($"Err exception: {exec.ErrorException}");
                Console.WriteLine($"Inner: {exec.ErrorException.InnerException}");
                */

                token = JObject.Parse(exec.Content)["access_token"].ToString();
            } catch (Exception ex) {
                Console.WriteLine($"Failed refreshing anilist token:\n {ex}");
            }
        }

        private static async Task<bool> ValidateQuery(Discord.Channel ch, string query) {
            if (string.IsNullOrEmpty(query.Trim())) {
                await ch.SendMessage("Please specify search parameters.");
               
              return false;
            }
            return true;
        }

        public static async Task<string> FindYoutubeUrlByKeywords(string v) {
          
            try {
                //maybe it is already a youtube url, in which case we will just extract the id and prepend it with youtube.com?v=
                var match = new Regex("(?:youtu\\.be\\/|v=)(?<id>[\\da-zA-Z\\-_]*)").Match(v);
                if (match.Length > 1) {
                    string str = $"http://www.youtube.com?v={ match.Groups["id"].Value }";
                    return str;
                }
                string key = "AIzaSyBnXSobjjpIGBjHFe2gy5-V-9iM6F1wbWo";
                WebRequest wr = WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=1&q=" + Uri.EscapeDataString(v) + "&key=" + key);

                var sr = new StreamReader((await wr.GetResponseAsync()).GetResponseStream());

                dynamic obj = JObject.Parse(await sr.ReadToEndAsync());
                return "http://www.youtube.com/watch?v=" + obj.items[0].id.videoId.ToString();
            } catch (Exception ex) {
                Console.WriteLine($"Error in findyoutubeurl: {ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<string> GetPlaylistIdByKeyword(string v) {
          
            try {
                string key = "AIzaSyBnXSobjjpIGBjHFe2gy5-V-9iM6F1wbWo";
                WebRequest wr = WebRequest.Create($"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=1&q={Uri.EscapeDataString(v)}&type=playlist&key={key}");

                var sr = new StreamReader((await wr.GetResponseAsync()).GetResponseStream());

                dynamic obj = JObject.Parse(await sr.ReadToEndAsync());
                return obj.items[0].id.playlistId.ToString();
            } catch (Exception ex) {
                Console.WriteLine($"Error in GetPlaylistIdbykeyword: {ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<List<string>> GetVideoIDs(string v) {
            List<string> toReturn = new List<string>();
           
            try {
                string key = "AIzaSyBnXSobjjpIGBjHFe2gy5-V-9iM6F1wbWo";
                WebRequest wr = WebRequest.Create($"https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults={25}&playlistId={v}&key={key}");

                var sr = new StreamReader((await wr.GetResponseAsync()).GetResponseStream());

                dynamic obj = JObject.Parse(await sr.ReadToEndAsync());

                foreach (var item in obj.items) {
                    toReturn.Add("http://www.youtube.com/watch?v=" + item.contentDetails.videoId);
                }
                return toReturn;
            } catch (Exception ex) {
                Console.WriteLine($"Error in GetvideosId: {ex.Message}");
                return new List<string>();
            }
        }


      
        public static async Task<string> ShortenUrl(string url) {
          
            try {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + "AIzaSyBnXSobjjpIGBjHFe2gy5-V-9iM6F1wbWo");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync())) {
                    string json = "{\"longUrl\":\"" + url + "\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (await httpWebRequest.GetResponseAsync()) as HttpWebResponse;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string responseText = await streamReader.ReadToEndAsync();
                    string MATCH_PATTERN = @"""id"": ?""(?<id>.+)""";
                    return Regex.Match(responseText, MATCH_PATTERN).Groups["id"].Value;
                }
            } catch (Exception ex) { Console.WriteLine(ex.ToString()); return ""; }
        }
    }
}
