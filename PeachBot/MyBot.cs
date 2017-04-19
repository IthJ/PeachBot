using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RiotApi.Net.RestClient;
using RiotApi.Net.RestClient.Configuration;

namespace PeachBot
{
    class MyBot
    {
        DiscordClient discord;
        IRiotClient riotClient = new RiotClient(""); //Installed the unoffical Riot API tooks - yet to be used.
        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true; // This allows for the command to work through mentions.
            });
    
            var commands = discord.GetService<CommandService>();
            var doubleLines = Environment.NewLine + Environment.NewLine;
            commands.CreateCommand("CD")
                .Parameter("DesiredChamp", ParameterType.Required)
                .Do(async (e) =>
                {
                    try
                    {
                        var DesiredChamp = e.GetArg("DesiredChamp");

                        if(DesiredChamp.Contains(".")) //Replaces " ' " or " . " with empty space.
                            {
                                DesiredChamp.Replace(".", "");
                            }
                                else if(DesiredChamp.Contains("'"))
                                {
                                    DesiredChamp.Replace("'", "");
                                }

                        DesiredChamp = DesiredChamp.Substring(0).ToUpper();
                        var formattedChamp = DesiredChamp.Substring(0,1); // To avoid anyone typing in the champ name in only Captials or just LowerCase. Grabs the first letter.
                        formattedChamp += DesiredChamp.Substring(1).ToLower(); // Makes the other letters lower case.
                        
                        using (StreamReader fileRdr = new StreamReader("ChampList.txt"))
                        {
                            do
                            {
                                String champList = await fileRdr.ReadLineAsync();
                                if (formattedChamp == champList)
                                {
                                    String skillsCDs = Environment.NewLine + fileRdr.ReadLineAsync().Result;
                                    for (int i = 0; i <= 3; i++)
                                    {
                                        skillsCDs += Environment.NewLine + fileRdr.ReadLineAsync().Result;
                                    }
                                    await e.Channel.SendMessage("**" + champList + "**" + skillsCDs );
                                    break;
                                }
                            }
                            while (formattedChamp != fileRdr.ReadLineAsync().Result);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        await e.Channel.SendMessage("I can't find that champion :$");
                    }
                });
            commands.CreateCommand("Help")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(" ```!Hi ~ Will return a greeting from PeachBot." + 
                        Environment.NewLine + "!Rank [IGN] ~ Will give you a link to the players Rank! " + 
                        Environment.NewLine + "!PatchNotes ~ Link to Riot's PatchNotes Page!" +
                        Environment.NewLine + "!Counter [ChampionName] ~ Will give you a link to that Champion's Counter!" +
                        Environment.NewLine + "!Stats [ChampionName] ~ Stats for the Champion of your choice!" +
                        Environment.NewLine + "!Build [ChampionName] ~ Probuild link for the Champion!" +
                        Environment.NewLine + "!Role [Top/Middle/Support/ADC/Jungle/Empty for All Roles] ~ Stats for that position!" +
                        doubleLines + "!Thanks ~ Peach will reply with a cute fighting emoji." +
                        Environment.NewLine + "!LoveYou ~ Peach will reply with a msg." +
                        doubleLines + "Remember to use the prefix '!' before every command! :3```");
                });

            commands.CreateCommand("Hi")
                .Do(async (e) =>
                {
                    Random random = new Random();
                    int ranMsg = random.Next(0, 5);

                    switch (ranMsg)
                    {
                        case 0:
                            await e.Channel.SendMessage("Hello :3");
                            break;
                        case 1:
                            await e.Channel.SendMessage("Sup 8)");
                            break;
                        case 2:
                            await e.Channel.SendMessage("Ciao~ :)");
                            break;
                        case 3:
                            await e.Channel.SendMessage("Bonjour! :j");
                            break;
                        case 4:
                            await e.Channel.SendMessage("Hola c:");
                            break;
                        default:
                            await e.Channel.SendMessage("Kon'nichiwa");
                            break;
                    }
                });

            commands.CreateCommand("Thanks")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("(๑•̀ㅂ•́)و");
                });

            commands.CreateCommand("eDYRanks") // Little love to a past team.
                .Do(async (e) =>
                {
                    var eDYLinks = "https://na.op.gg/summoner/userName=Urf%20Wind%20Fire" + Environment.NewLine + "https://na.op.gg/summoner/userName=Infake" + Environment.NewLine + "https://na.op.gg/summoner/userName=Snawp" + Environment.NewLine + "https://na.op.gg/summoner/userName=Umi" + Environment.NewLine + "https://na.op.gg/summoner/userName=VoidIing";
                    await e.Channel.SendMessage("The OGs: " + Environment.NewLine + eDYLinks);

                });
            
            commands.CreateCommand("Rank")
                    .Parameter("Name", ParameterType.Required)
                    .Description("This command will be able to search up anyone's rank by simply calling the command along with their IGN right after. Make sure not to leave any spaces for the names! Ex: UrfWindFire instead of Urf Wind Fire")
                    .Do(async (e) =>
                    {
                        var userName = e.GetArg("Name");

                        try
                        {
                            String rankLink = "https://na.op.gg/summoner/userName=" + userName;
                            await e.Channel.SendMessage(rankLink);
                        }
                        catch
                        {
                            await e.Channel.SendMessage("You forgot to include a IGN! Ex: !Rank Faker");
                        }
                    
                    });

            commands.CreateCommand("PatchNotes")
                .Do(async (e) =>
                {
                    var patchNotes = "http://na.leagueoflegends.com/en/news/game-updates/patch";
                    var PBEpatchNotes = "http://www.surrenderat20.net/search/label/PBE/";
                    
                    await e.Channel.SendMessage("Here you go :) " + doubleLines + "Recent Patch Notes: " + Environment.NewLine + "Live Patch: " + patchNotes + Environment.NewLine + "PBE Patch: " + PBEpatchNotes);
                });

            commands.CreateCommand("Counter")
                .Parameter("Champion", ParameterType.Required)
                .Do(async (e) =>
                {
                    var ChampionName = e.GetArg("Champion");
                    await e.Channel.SendMessage("http://lolcounter.com/champions/" + ChampionName);
                });

            commands.CreateCommand("Stats")
                .Parameter("Champion", ParameterType.Required)
                .Do(async (e) =>
                {
                    var ChampionName = e.GetArg("Champion");
                    await e.Channel.SendMessage("https://na.op.gg/champion/" + ChampionName);
                });

            commands.CreateCommand("Build")
               .Parameter("Champion", ParameterType.Required)
               .Do(async (e) =>
               {
                   var ChampionName = e.GetArg("Champion");
                   await e.Channel.SendMessage("http://www.probuilds.net/champions/details/" + ChampionName + Environment.NewLine 
                                                + "https://na.op.gg/champion/" + ChampionName + Environment.NewLine
                                                + "http://www.mobafire.com/league-of-legends/toplist/top-10-" + ChampionName + "-guides");

               });

            commands.CreateCommand("Role")
                .Parameter("Position", ParameterType.Required)
                .Do(async (e) =>
                {
                    var Position = e.GetArg("Position");
                    await e.Channel.SendMessage("http://champion.gg/statistics/#?sortBy=general.overallPosition&order=ascend&roleSort=" + Position);
                });

            commands.CreateCommand("LoveYou")
                .Do(async (e) =>
                {
                    Random random = new Random();
                    int ranMsg = random.Next(0, 100);
                    string msgtobeSent = "";

                    if (ranMsg == 1)
                    {
                        msgtobeSent = "Love you too. (♥ω♥*)";
                    }
                    else if(ranMsg <= 25 && ranMsg != 1)
                    {
                        if(e.User.Name == "Infake") //Shut up, Kevin
                        {
                            msgtobeSent = "shut up, Kevin. ╭( ･ㅂ･)و ̑̑";
                        }
                            else
                            {
                                msgtobeSent = "You're okay. (｡+･`ω･´)";
                            }   
                    }
                    else if (ranMsg <= 50 && ranMsg != 1)
                    {
                        msgtobeSent = "wow, thanks. ((╬●∀●)";
                    }
                    else if (ranMsg <= 75 && ranMsg != 1)
                    {
                        msgtobeSent = "I was wondering when you'd admit that. ₍₍ ◝(•̀ㅂ•́)◟ ⁾⁾";
                    }
                    else if (ranMsg <= 100 && ranMsg != 1)
                    {
                        msgtobeSent = "stop, u. ( •̀ω•́ )σ";
                    }

                    await e.Channel.SendMessage(msgtobeSent);
                });

            discord.ExecuteAndWait(async () =>
                { 
                    await discord.Connect("YOUR APP BOT USER TOKEN", TokenType.Bot);
                });
        
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
