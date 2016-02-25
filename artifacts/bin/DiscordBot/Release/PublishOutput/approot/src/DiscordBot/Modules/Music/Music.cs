using System;
using System.Linq;
using Discord.Modules;
using Discord.Commands;
using Discord;
using System.Collections.Concurrent;
using NadekoBot.Classes.Music;
using Timer = System.Timers.Timer;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Music
{
    internal class Music : IModule
    {

        public static ConcurrentDictionary<Server, MusicControls> musicPlayers = new ConcurrentDictionary<Server, MusicControls>();

        internal static string GetMusicStats() {
            var stats = musicPlayers.Where(kvp => kvp.Value?.SongQueue.Count > 0 || kvp.Value?.CurrentSong != null);
            int cnt;
            return $"Playing {cnt = stats.Count()} songs".SnPl(cnt) + $", {stats.Sum(kvp => kvp.Value?.SongQueue?.Count ?? 0)} queued.";
        }

        public Music() : base() {
        }
        private ModuleManager _manager;
        private DiscordClient _client;
       void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("music", cgb => {
                //queue all more complex commands
               // commands.ForEach(cmd => cmd.Init(cgb));

                cgb.CreateCommand("next")
                    
                    .Description("Goes to the next song in the queue.")
                    .Do(async e => {
                        if (musicPlayers.ContainsKey(e.Server) == false) return;
                        await musicPlayers[e.Server].LoadNextSong();
                    });

                cgb.CreateCommand("stop")
                  
                    .Description("Completely stops the music and unbinds the bot from the channel and cleanes up files.")
                    .Do(e => {
                        if (musicPlayers.ContainsKey(e.Server) == false) return;
                        musicPlayers[e.Server].Stop();
                    });

                cgb.CreateCommand("pause")
                    
                    .Description("Pauses or Unpauses the song")
                    .Do(async e => {
                        if (musicPlayers.ContainsKey(e.Server) == false) return;
                        if (musicPlayers[e.Server].TogglePause())
                            await _client.Reply(e,"🎵`Music player paused.`");
                        else
                            await _client.Reply(e, "🎵`Music player unpaused.`");
                    });

                cgb.CreateCommand("q")
                    .Alias("yq")
                    .Description("Queue a song using keywords or link. Bot will join your voice channel. **You must be in a voice channel**.\n**Usage**: `!m q Dream Of Venice`")
                    .Parameter("query", ParameterType.Unparsed)
                    .Do(async e => await QueueSong(e,e.GetArg("query")));

                cgb.CreateCommand("listq")
                    .Alias("ls").Alias("lp")
                    .Description("Lists up to 10 currently queued songs.")
                    .Do(async e => {
                        if (musicPlayers.ContainsKey(e.Server) == false) {
                            await _client.Reply(e, "🎵 No active music player.");
                            return;
                        }
                        var player = musicPlayers[e.Server];
                        string toSend = "🎵 **" + player.SongQueue.Count + "** `videos currently queued.` ";
                        if (player.SongQueue.Count >= 25)
                            toSend += "**Song queue is full!**\n";
                        await _client.Reply(e, toSend);
                        int number = 1;
                        await _client.Reply(e, string.Join("\n", player.SongQueue.Take(10).Select(v => $"`{number++}.` {v.FullPrettyName}")));
                    });

                cgb.CreateCommand("nowplaying")
                 .Alias("playing")
                 .Description("Shows the song currently playing.")
                 .Do(async e => {
                     if (musicPlayers.ContainsKey(e.Server) == false) return;
                     var player = musicPlayers[e.Server];
                     await _client.Reply(e, $"🎵`Now Playing` {player.CurrentSong.FullPrettyName}");
                 });

                cgb.CreateCommand("vol")
                  .Description("Sets the music volume 0-150%")
                  .Parameter("val", ParameterType.Required)
                  .Do(async e => {
                      if (musicPlayers.ContainsKey(e.Server) == false) return;
                      var player = musicPlayers[e.Server];
                      var arg = e.GetArg("val");
                      int volume;
                      if (!int.TryParse(arg, out volume)) {
                          await _client.Reply(e, "Volume number invalid.");
                          return;
                      }
                      volume = player.SetVolume(volume);
                      await _client.Reply(e, $"🎵 `Volume set to {volume}%`");
                  });

                cgb.CreateCommand("min").Alias("mute")
                  .Description("Sets the music volume to 0%")
                  .Do(e => {
                      if (musicPlayers.ContainsKey(e.Server) == false) return;
                      var player = musicPlayers[e.Server];
                      player.SetVolume(0);
                  });

                cgb.CreateCommand("max")
                  .Description("Sets the music volume to 100% (real max is actually 150%).")
                  .Do(e => {
                      if (musicPlayers.ContainsKey(e.Server) == false) return;
                      var player = musicPlayers[e.Server];
                      player.SetVolume(100);
                  });

                cgb.CreateCommand("half")
                  .Description("Sets the music volume to 50%.")
                  .Do(e => {
                      if (musicPlayers.ContainsKey(e.Server) == false) return;
                      var player = musicPlayers[e.Server];
                      player.SetVolume(50);
                  });

                cgb.CreateCommand("shuffle")
                    .Description("Shuffles the current playlist.")
                    .Do(async e => {
                        if (musicPlayers.ContainsKey(e.Server) == false) return;
                        var player = musicPlayers[e.Server];
                        if (player.SongQueue.Count < 2) {
                            await _client.Reply(e, "Not enough songs in order to perform the shuffle.");
                            return;
                        }

                        player.SongQueue.Shuffle();
                        await _client.Reply(e, "🎵 `Songs shuffled.`");
                    });
                
                bool setgameEnabled = true;
                Timer setgameTimer = new Timer();
                setgameTimer.Interval = 20000;
                setgameTimer.Elapsed += (s, e) => {
                    int num = musicPlayers.Where(kvp => kvp.Value.CurrentSong != null).Count();
                    _client.SetGame($"{num} songs".SnPl(num) + $", {musicPlayers.Sum(kvp => kvp.Value.SongQueue.Count())} queued");
                };
                cgb.CreateCommand("setgame")
                    .Description("Sets the game of the bot to the number of songs playing.**Owner only**")
                    .Do(async e => {
                      
                        setgameEnabled = !setgameEnabled;
                        if (setgameEnabled)
                            setgameTimer.Start();
                        else
                            setgameTimer.Stop();

                        await _client.Reply(e, "`Music status " + (setgameEnabled ? "enabled`" : "disabled`"));
                    });

                cgb.CreateCommand("playlist")
                    .Description("Queues up to 25 songs from a youtube playlist specified by a link, or keywords.")
                    .Parameter("playlist", ParameterType.Unparsed)
                    .Do(async e => {
                        if (e.User.VoiceChannel?.Server != e.Server) {
                            await _client.Reply(e, "💢 You need to be in the voice channel on this server.");
                            return;
                        }
                        var ids = await Searches.GetVideoIDs(await Searches.GetPlaylistIdByKeyword(e.GetArg("playlist")));
                        //todo TEMPORARY SOLUTION, USE RESOLVE QUEUE IN THE FUTURE
                        await _client.Reply(e,$"🎵 Attempting to queue **{ids.Count}** songs".SnPl(ids.Count));
                        foreach (var id in ids) {
                            Task.Run(async () => await QueueSong(e, id, true)).ConfigureAwait(false);
                            await Task.Delay(150);
                        }
                        await _client.Reply(e, $"🎵 Attempting to queue **{ids.Count}** songs".SnPl(ids.Count));
                    });

              /*  cgb.CreateCommand("radio").Alias("ra")
                    .Description("Queues a direct radio stream from a link.")
                    .Parameter("radio_link", ParameterType.Required)
                    .Do(async e => {
                        if (e.User.VoiceChannel?.Server != e.Server) {
                            await _client.Reply(e, "💢 You need to be in the voice channel on this server.");
                            return;
                        }
                        await QueueSong(e, e.GetArg("radio_link"),_client, radio: true);
                    });*/

                cgb.CreateCommand("debug")
                    
                    .Description("Writes some music data to console.")
                    .Do(e => {
                      
                        var output = "SERVER_NAME---SERVER_ID-----USERCOUNT----QUEUED\n" +
                            string.Join("\n", musicPlayers.Select(kvp => kvp.Key.Name + "--" + kvp.Key.Id + " --" + kvp.Key.Users.Count() + "--" + kvp.Value.SongQueue.Count));
                        Console.WriteLine(output);
                    });
            });
        }

        private async Task QueueSong(CommandEventArgs e, string query, bool silent = false, bool radio = false) {
            if (e.User.VoiceChannel?.Server != e.Server) {
                await _client.Reply(e, "💢 You need to be in the voice channel on this server.");
                return;
            }
            if (musicPlayers.ContainsKey(e.Server) == false)
                if (!musicPlayers.TryAdd(e.Server, new MusicControls(e.User.VoiceChannel, e))) {
                    await _client.Reply(e, "Failed to create a music player for this server.");
                    return;
                }
            if (query == null || query.Length < 4)
                return;

            var player = musicPlayers[e.Server];

            if (player.SongQueue.Count >= 25) return;

            try {
                var sr = await Task.Run(() => new StreamRequest(e, query, player, _client,radio));

                if (sr == null)
                    throw new NullReferenceException("StreamRequest is null.");

                Message qmsg = null;
                Message msg = null;
                if (!silent) {
                    qmsg = await e.Channel.SendMessage("🎵 `Searching...`");
                    sr.OnResolving += async () => {
                        await qmsg.Edit($"🎵 `Resolving`... \"{query}\"");
                    };
                    sr.OnResolvingFailed += async (err) => {
                        await qmsg.Edit($"💢 🎵 `Resolving failed` for **{query}**");
                    };
                    sr.OnQueued += async () => {
                        await qmsg.Edit($"🎵`Queued`{sr.FullPrettyName}");
                    };
                }
                sr.OnCompleted += async () => {
                    MusicControls mc;
                    if (musicPlayers.TryGetValue(e.Server, out mc)) {
                        if (mc.SongQueue.Count == 0)
                            mc.Stop();
                    }
                    await _client.Reply(e, $"🎵`Finished`{sr.FullPrettyName}");
                };
                sr.OnStarted += async () => {
                    var msgTxt = $"🎵`Playing`{sr.FullPrettyName} `Vol: {(int)(player.Volume * 100)}%`";
                    if (msg == null)
                        await _client.Reply(e, msgTxt);
                    else
                        await msg.Edit(msgTxt);
                    qmsg?.Delete();
                };
                sr.OnBuffering +=  async () => {
                   await _client.Reply(e, $"🎵`Buffering...`{sr.FullPrettyName}");
                };
            } catch (Exception ex) {
                Console.WriteLine();
                await _client.Reply(e, $"💢 {ex.Message}");
                return;
            }
        }
    }
}
