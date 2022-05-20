using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using Newtonsoft.Json;
using System.Collections.Generic;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Doctor", "Trenton", "0.0.1")]
    public class Doctortest : CovalencePlugin
    {
        [PluginReference] Plugin Economics, HumanNPC;

        #region Json
        private static string TEMPLATE = @"
        [
          {
            ""name"": ""Doctortest"",
            ""parent"": ""Hud"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.14 0.14 0.14 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.232 0.147"",
                ""anchormax"": ""0.779 0.842""
              },
              {
                ""type"": ""NeedsCursor""
              }
            ]
          },
          {
            ""name"": ""title"",
            ""parent"": ""Doctortest"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 50,
                ""align"": ""MiddleCenter"",
                ""text"": ""Doctor""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.336 0.78"",
                ""anchormax"": ""0.643 0.94""
              }
            ]
          },
          {
            ""name"": ""exit"",
            ""parent"": ""Doctortest"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""doctor.close"",
                ""close"": """",
                ""color"": ""0 0 0 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.874 0.836"",
                ""anchormax"": ""0.974 0.976""
              }
            ]
          },
          {
            ""name"": ""xtext"",
            ""parent"": ""exit"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 0 0 1"",
                ""fontSize"": 25,
                ""align"": ""MiddleCenter"",
                ""text"": ""X""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          },
          {
            ""name"": ""cost"",
            ""parent"": ""Doctortest"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 25,
                ""align"": ""MiddleCenter"",
                ""text"": ""Cost:""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.397 0.648"",
                ""anchormax"": ""0.481 0.704""
              }
            ]
          },
          {
            ""name"": ""amount"",
            ""parent"": ""Doctortest"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 25,
                ""align"": ""MiddleCenter"",
                ""text"": ""{amount}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.503 0.646"",
                ""anchormax"": ""0.563 0.702""
              }
            ]
          },
          {
            ""name"": ""Heal"",
            ""parent"": ""Doctortest"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""doctor.heal"",
                ""close"": """",
                ""color"": ""0.31 0.75 0.31 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.137 0.366"",
                ""anchormax"": ""0.851 0.566""
              }
            ]
          },
          {
            ""name"": ""d948-0f2a-c26f"",
            ""parent"": ""Heal"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 50,
                ""align"": ""MiddleCenter"",
                ""text"": ""Heal""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          }
        ]
        ";
        #endregion


        private void OnUseNPC(BasePlayer npc, BasePlayer player)
        {
            configData = Config.ReadObject<ConfigData>();
            var npcid = configData.npcid;
            if (npc.userID == npcid)
            {
                player.Command("doctor");

            }
        }

        void OnUserConnected(IPlayer player)
        {
        }

        [Command("doctor")]
        private void CommandDoctortest(IPlayer player, string command, string[] args)
        {
            configData = Config.ReadObject<ConfigData>();
            var bPlayer = (BasePlayer)player.Object;
            var cost = configData.amount.ToString();
            var filledTemplate = TEMPLATE.Replace("{amount}", cost);

            CuiHelper.DestroyUi(bPlayer, "Doctortest");
            CuiHelper.AddUi(bPlayer, filledTemplate);
        }

        [Command("doctor.close")]
        private void CommandDoctorclose(IPlayer player, string command, string[] args)
        {
            player.Reply("Closing Menu!");
            CuiHelper.DestroyUi((BasePlayer)player.Object, "Doctortest");
        }

        [Command("doctor.heal")]
        private void CommandDoctorHeal(IPlayer player, string command, string[] args)
        {
            if (Economics != null && Economics.IsLoaded)
            {
                configData = Config.ReadObject<ConfigData>();
                double amount = configData.amount;
                if (Economics.Call<bool>("Withdraw", player.Id, amount))
                {
                    player.Reply($"Paid {amount} for the Doctor service");
                    player.Heal(100);
                    player.Reply("Healed to full Health!");
                    storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("Doctortest");
                    if (!storedData.playerData.ContainsKey(player.Id))
                    {
                        PlayerStats derp = new PlayerStats();
                        storedData.playerData.Add(player.Id, derp);
                        SaveData();

                    }
                    else
                    CuiHelper.DestroyUi((BasePlayer)player.Object, "Doctortest");
                    storedData.playerData[player.Id].healed++;
                    SaveData();

                }
                else
                {
                    player.Reply($"Withdrawal of {amount} failed");
                    CuiHelper.DestroyUi((BasePlayer)player.Object, "Doctortest");
                }
            }
            else
            {
                player.Reply("Economics is not loaded, withdraw failed");
            }
        }

        [Command("doctortestcommand")]
        private void doctortestcommand(IPlayer player)
        {
            var playerinfo = storedData.playerData[player.Id];
            player.Reply($"You have healed {playerinfo.healed} times");
        }

        [Command("doctor.withdraw")]
        private void WithdrawCommand(IPlayer player, string command, string[] args)
        {
            if (Economics != null && Economics.IsLoaded)
            {
                configData = Config.ReadObject<ConfigData>();
                double amount = configData.amount;
                if (args.Length > 0 && double.TryParse(args[0], out amount))
                {
                    if (Economics.Call<bool>("Withdraw", player.Id, amount))
                    {
                        player.Reply($"Withdrew {amount} from your balance");
                    }
                    else
                    {
                        player.Reply($"Withdrawal of {amount} failed");
                    }
                }
                else
                {
                    player.Reply("Please enter a valid amount to withdraw");
                }
            }
            else
            {
                player.Reply("Economics is not loaded, withdraw failed");
            }
        }

        #region Config
        private ConfigData configData;
        class ConfigData
        {
            [JsonProperty("NPC ID")]
            public double npcid = 10821539840;

            [JsonProperty("Doctor Cost")]
            public double amount = 100;
        }

        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                return false;
            }
            SaveConfig(configData);
            return true;
        }

        void Init()
        {
            if (!LoadConfigVariables())
            {
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
                return;
            }
        }

        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");
            configData = new ConfigData();
            SaveConfig(configData);
        }

        void SaveConfig(ConfigData config)
        {
            Config.WriteObject(config, true);
        }
        #endregion

        #region Data
        StoredData storedData;

        class StoredData
        {
            public Dictionary<string, PlayerStats> playerData = new Dictionary<string, PlayerStats>();
        }

        class PlayerStats
        {
            public int healed = 0;
        }

        void Loaded()
        {
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("Doctortest");
            Interface.Oxide.DataFileSystem.WriteObject("Doctortest", storedData);
        }

        void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject("Doctortest", storedData);
        }

        #endregion
    }
}
