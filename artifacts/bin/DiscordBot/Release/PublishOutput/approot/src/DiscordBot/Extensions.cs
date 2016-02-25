using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace DiscordBot
{
    public static class Extensions
	{
        public static int KiB(this int value) => value * 1024;
        public static int KB(this int value) => value * 1000;

        public static int MiB(this int value) => value.KiB() * 1024;
        public static int MB(this int value) => value.KB() * 1000;

        public static int GiB(this int value) => value.MiB() * 1024;
        public static int GB(this int value) => value.MB() * 1000;
        public static string TrimTo(this string str, int num)
        {
            if (num < 0)
                throw new ArgumentException("TrimTo argument cannot be less than 0");
            if (num == 0)
                return String.Empty;
            if (num <= 3)
                return String.Join("", str.Select(c => '.'));
            if (str.Length < num)
                return str;
            return string.Join("", str.Take(num - 3)) + "...";
        }
        public static Task Reply(this DiscordClient client, CommandEventArgs e, string text)
			=> Reply(client, e.User, e.Channel, text);
		public async static Task Reply(this DiscordClient client, User user, Channel channel, string text)
		{
			if (text != null)
			{
				if (!channel.IsPrivate)
					await channel.SendMessage($"{user.Name}: {text}");
				else
					await channel.SendMessage(text);
			}
		}
		public static Task Reply<T>(this DiscordClient client, CommandEventArgs e, string prefix, T obj)
			=> Reply(client, e.User, e.Channel, prefix, obj != null ? JsonConvert.SerializeObject(obj, Formatting.Indented) : "null");
		public static Task Reply<T>(this DiscordClient client, User user, Channel channel, string prefix, T obj)
			=> Reply(client, user, channel, prefix, obj != null ? JsonConvert.SerializeObject(obj, Formatting.Indented) : "null");
		public static Task Reply(this DiscordClient client, CommandEventArgs e, string prefix, string text)
			=> Reply(client, e.User, e.Channel, (prefix != null ? $"{Format.Bold(prefix)}:\n" : "\n") + text);
		public static Task Reply(this DiscordClient client, User user, Channel channel, string prefix, string text)
			=> Reply(client, user, channel, (prefix != null ? $"{Format.Bold(prefix)}:\n" : "\n") + text);

		public static Task ReplyError(this DiscordClient client, CommandEventArgs e, string text)
			=> Reply(client, e.User, e.Channel, "Error: " + text);
		public static Task ReplyError(this DiscordClient client, User user, Channel channel, string text)
			=> Reply(client, user, channel, "Error: " + text);
		public static Task ReplyError(this DiscordClient client, CommandEventArgs e, Exception ex)
			=> Reply(client, e.User, e.Channel, "Error: " + ex.GetBaseException().Message);
		public static Task ReplyError(this DiscordClient client, User user, Channel channel, Exception ex)
			=> Reply(client, user, channel, "Error: " + ex.GetBaseException().Message);
	}

	internal static class InternalExtensions
	{
        
        public static string SnPl(this string str, int? num, bool es = false)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (num == null)
                throw new ArgumentNullException(nameof(num));
            return num == 1 ? str.Remove(str.Length - 1, es ? 2 : 1) : str;
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static Task<User[]> FindUsers(this DiscordClient client, CommandEventArgs e, string username, string discriminator)
			=> FindUsers(client, e, username, discriminator, false);
		public static async Task<User> FindUser(this DiscordClient client, CommandEventArgs e, string username, string discriminator)
			=> (await FindUsers(client, e, username, discriminator, true))?[0];
		public static async Task<User[]> FindUsers(this DiscordClient client, CommandEventArgs e, string username, string discriminator, bool singleTarget)
		{
			IEnumerable<User> users;
			if (discriminator == "")
				users = e.Server.FindUsers(username);
			else
			{
				var user = e.Server.GetUser(username, ushort.Parse(discriminator));
				if (user == null)
					users = Enumerable.Empty<User>();
				else
					users = new User[] { user };
			}

			int count = users.Count();
			if (singleTarget)
			{
				if (count == 0)
				{
					await client.ReplyError(e, "User was not found.");
					return null;
				}
				else if (count > 1)
				{
					await client.ReplyError(e, "Multiple users were found with that username.");
					return null;
				}
			}
			else
			{
				if (count == 0)
				{
					await client.ReplyError(e, "No user was found.");
					return null;
				}
			}
			return users.ToArray();
        }
        public static async Task<Channel> FindChannel(this DiscordClient client, CommandEventArgs e, string name, ChannelType type = null)
        {
            var channels = e.Server.FindChannels(name, type);

            int count = channels.Count();
            if (count == 0)
            {
                await client.ReplyError(e, "Channel was not found.");
                return null;
            }
            else if (count > 1)
            {
                await client.ReplyError(e, "Multiple channels were found with that name.");
                return null;
            }
            return channels.FirstOrDefault();
        }

        public static async Task<User> GetUser(this DiscordClient client, CommandEventArgs e, ulong userId)
		{
			var user = e.Server.GetUser(userId);

			if (user == null)
			{
				await client.ReplyError(e, "No user was not found.");
				return null;
			}
			return user;
		}		
	}
}
