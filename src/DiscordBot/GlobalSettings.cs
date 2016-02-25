using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DiscordBot
{
    public class GlobalSettings
	{
		private const string path = "./config/global.json";
		private static GlobalSettings _instance = new GlobalSettings();

		public static void Load()
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"{path} is missing.");
			_instance = JsonConvert.DeserializeObject<GlobalSettings>(File.ReadAllText(path));

		}
		public static void Save()
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new StreamWriter(stream))
				writer.Write(JsonConvert.SerializeObject(_instance, Formatting.Indented));
		}

		//Discord
		public class DiscordSettings
		{
			[JsonProperty("username")]
			public string Email = "blood_raiser@hotmail.com";
			[JsonProperty("password")]
			public string Password = "pedro123";
		}
		[JsonProperty("discord")]
		private DiscordSettings _discord = new DiscordSettings();
		public static DiscordSettings Discord => _instance._discord;

		//Users
		public class UserSettings
		{
            [JsonProperty("dev")]
            public ulong DevId = 118021290757062659;
		}
		[JsonProperty("users")]
		private UserSettings _users = new UserSettings();
		public static UserSettings Users => _instance._users;

		//Github
		public class GithubSettings
		{
			[JsonProperty("username")]
			public string Username;
			[JsonProperty("password")]
			public string Password;
			[JsonIgnore]
			public string Token => Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));
		}
		[JsonProperty("github")]
		private GithubSettings _github = new GithubSettings();
		public static GithubSettings Github => _instance._github;
        public class SessionInfo
        {
            public string ret_msg { get; set; }
            public string session_id { get; set; }
            public string timestamp { get; set; }

        }

        public class Menuitem
        {
            public string description { get; set; }
            public string value { get; set; }
        }

        public class Rankitem
        {
            public string description { get; set; }
            public string value { get; set; }
        }

        public class AbilityDescription
        {
            public string description { get; set; }
            public string secondaryDescription { get; set; }
            public List<Menuitem> menuitems { get; set; }
            public List<Rankitem> rankitems { get; set; }
            public string cooldown { get; set; }
            public string cost { get; set; }
        }

        public class AbilityRoot
        {
            public AbilityDescription itemDescription { get; set; }
        }

        public class Gods
        {
            public int abilityId1 { get; set; }
            public int abilityId2 { get; set; }
            public int abilityId3 { get; set; }
            public int abilityId4 { get; set; }
            public int abilityId5 { get; set; }
            public AbilityRoot abilityDescription1 { get; set; }
            public AbilityRoot abilityDescription2 { get; set; }
            public AbilityRoot abilityDescription3 { get; set; }
            public AbilityRoot abilityDescription4 { get; set; }
            public AbilityRoot abilityDescription5 { get; set; }
            public int id { get; set; }
            public string Pros { get; set; }
            public string Type { get; set; }
            public string Roles { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string OnFreeRotation { get; set; }
            public string Lore { get; set; }
            public int Health { get; set; }
            public Double HealthPerLevel { get; set; }
            public Double Speed { get; set; }
            public Double HealthPerFive { get; set; }
            public Double HP5PerLevel { get; set; }
            public Double Mana { get; set; }
            public Double ManaPerLevel { get; set; }
            public Double ManaPerFive { get; set; }
            public Double MP5PerLevel { get; set; }
            public Double PhysicalProtection { get; set; }
            public Double PhysicalProtectionPerLevel { get; set; }
            public Double MagicProtection { get; set; }
            public Double MagicProtectionPerLevel { get; set; }
            public Double PhysicalPower { get; set; }
            public Double PhysicalPowerPerLevel { get; set; }
            public Double AttackSpeed { get; set; }
            public Double AttackSpeedPerLevel { get; set; }
            public string Pantheon { get; set; }
            public string Ability1 { get; set; }
            public string Ability2 { get; set; }
            public string Ability3 { get; set; }
            public string Ability4 { get; set; }
            public string Ability5 { get; set; }
            public string Item1 { get; set; }
            public string Item2 { get; set; }
            public string Item3 { get; set; }
            public string Item4 { get; set; }
            public string Item5 { get; set; }
            public string Item6 { get; set; }
            public string Item7 { get; set; }
            public string Item8 { get; set; }
            public string Item9 { get; set; }
            public int ItemId1 { get; set; }
            public int ItemId2 { get; set; }
            public int ItemId3 { get; set; }
            public int ItemId4 { get; set; }
            public int ItemId5 { get; set; }
            public int ItemId6 { get; set; }
            public int ItemId7 { get; set; }
            public int ItemId8 { get; set; }
            public int ItemId9 { get; set; }
            public string ret_msg { get; set; }
        }
    }
}
