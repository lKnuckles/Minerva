using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Scp2Xml
{
    class Program
    {
        private static string[] myoptions = {"1 - World*-mmap.scp to MobsList.xml" , "2 - World*-npc.scp to NpcsList.xml", "3 - World*-terrain.scp to TerrainsList.xml", "4 - Warmap*.scp to WarMapsList.xml"
        ,"5 - Ability.scp to Ability.xml", "6 - Achievement.scp to Achievement.xml", "7 - AlzControl.scp to AlzControl.xml", "8 - Arena.scp to Arena.xml", "9 - Assistant.scp to Assistant.xml", "10 - BelongItem.scp to BelongItem.xml"
        ,"11 - BlessingBead.scp to BlessingBead.xml", "12 - CashItem.scp to CashItem.xml", "13 - ChangeItem.scp to ChangeItem.xml", "14 - ChangeShape.scp to ChangeShape.xml", "15 - Const.scp to Const.xml", "16 - Core.scp to Core.xml"
        ,"17 - Craft.scp to Craft.xml", "18 - Destroy.scp to Destroy.xml", "19 - DungeonEntryException.scp to DungeonEntryException.xml", "20 - Event.scp to Event.xml", "21 - GiftBoxWrapper.scp to GiftBoxWrapper.xml"
        ,"22 - Item.scp to Item.xml", "23 - ItemReward.scp to ItemReward.xml", "24 - Level.scp to Level.xml", "25 - Market.scp to Market.xml", "26 - MissionDungeon.scp to MissionDungeon.xml", "27 - Mobs.scp to Mobs.xml"
        ,"28 - MultiBuff.scp to MultiBuff.xml", "29 - Multiple.scp to Rates.xml", "30 - NPCShop.scp to NPCShop.xml", "31 - OptionPool.scp to OptionPool.xml", "32 - Pet.scp to Pet.xml", "33 - PremiumService.scp to PremiumService.xml"
        ,"34 - Product.scp to Product.xml", "35 - Quest.scp to Quest.xml", "36 - QuestDungeon.scp to QuestDungeon.xml", "37 - Rank.scp to Rank.xml", "38 - SP_Dice.scp to SP_Dice.xml", "39 - ServerMob.scp to ServerMob.xml"
        ,"40 - SetEffect.scp to SetEffect.xml", "41 - Skill.scp to Skill.xml", "42 - SpecialInventory.scp to SpecialInventory.xml", "43 - SpecialItem.scp to SpecialItem.xml", "44 - Terrain.scp to Terrain.xml", "45 - Title.scp to Title.xml"
        ,"46 - TitleUseException.scp to TitleUseException.xml", "47 - War.scp to War.xml", "48 - Warp.scp to Warp.xml", "49 - WorldDrop.scp to WorldDrop.xml", "50 - All to .xml"};

        static void Main(string[] args)
        {
            Beginning:

            #region Options
            for (int i = 0; i < myoptions.Length; i++)
            {
                if (i % 2 == 0)
                    Console.Write(String.Format("{0,-65}", myoptions[i]));
                else
                    Console.WriteLine(String.Format("{0,10}", myoptions[i]));

            }
            #endregion

            Console.WriteLine("\nInsert a number (1-50):");

            int num = int.Parse(Console.ReadLine());

            if (num > 0 && num <= 50)
            {
                switch (num)
                {
                    #region All to .xml
                    case 50:
                        for (int i = 1; i < 50; i++)
                        {
                            Options(i);
                        }
                        break;
                    #endregion

                    #region Others
                    default:
                        Options(num);
                        break;
                        #endregion
                }
                Console.Clear();
                Console.WriteLine("Done!");
                Console.WriteLine("Do you want to convert another? (y/n)");
                if (Console.ReadLine().ToLower().StartsWith("y"))
                {
                    Console.Clear();
                    goto Beginning;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Wrong Value, Try Again? (y/n)");
                if (Console.ReadLine().ToLower().StartsWith("y"))
                {
                    Console.Clear();
                    goto Beginning;
                }
            }
        }


        private static void Options(int numb)
        {
            switch (numb)
            {
                #region World*-mmap.scp to MobsList.xml
                case 1:
                    for (int i = 1; i < 10; i++)
                        if(File.Exists(Directory.GetCurrentDirectory() + "\\Data\\world" + i + "-mmap.scp"))
                            File.Move(Directory.GetCurrentDirectory() + "\\Data\\world" + i + "-mmap.scp", Directory.GetCurrentDirectory() + "\\Data\\world0" + i + "-mmap.scp");

                    #region Vars
                        String line;                        
                        string[] files = Directory.EnumerateFiles(Directory.GetCurrentDirectory()+"\\Data", "*mmap.scp").OrderBy(f => f).ToArray();
                        string[] mobKeys = { "mob", "id", "x", "y", "width", "height", "spawn", "SpawnDefault", "EvtProperty", "EvtMobs","EvtInterval", "MissionGate", "PerfectDrop", "DropType", "MinDrop", "MaxDrop", "DropAuthority", "ServerMob","Loot_Delay" };
                        string[] mmap = { "MissionMMAP", "QDungeonIdx", "MMapidx", "Cells" };
                    #endregion

                    #region File Removal
                    if (files.Length == 0)
                            throw new Exception("No World*-mmap.scp was found in the data folder!");

                        if (File.Exists(Directory.GetCurrentDirectory() + "\\Data\\MobList.xml"))
                            File.Delete(Directory.GetCurrentDirectory() + "\\Data\\MobList.xml");
                    #endregion

                    XmlTextWriter tw = new XmlTextWriter(File.Open("Data\\MobList.xml", FileMode.Append), Encoding.UTF8);
                    tw.WriteStartElement("MobList");

                    foreach (var file in files) 
                    {


                        #region Getting World Id
                            int length = file.Length - (Directory.GetCurrentDirectory().ToString() + "\\Data\\").Length;
                            string WorldID = file.Remove(0, (file.Length -length+5)).Remove(length - 14);
                        #endregion


                        #region Loading Files and creating writer
                            TextReader tr = new StreamReader(file);
                            tw.Formatting = Formatting.Indented;
                        #endregion

             
                        #region Writes first line of MobsList
                            tw.WriteStartElement("World");

                            tw.WriteAttributeString("id", WorldID);

                            tw.WriteStartElement("Mobs");
                        #endregion

                        #region File Reading and Xml Structuring
                        while ((line = tr.ReadLine()) != null)
                            {
                                if (String.IsNullOrEmpty(line) == false && Char.IsNumber(line, 0) == true && line.Split('\t').Length>4)
                                {
                                    tw.WriteStartElement(mobKeys[0]);
                                    for (int i = 1; i < mobKeys.Length; i++)
                                        if (i < 7)
                                            tw.WriteAttributeString(mobKeys[i], line.Split('\t')[i]);
                                        else if (line.Contains("&lt;") == true)
                                            tw.WriteAttributeString(mobKeys[i], "0");
                                    tw.WriteEndElement();
                                }
                                else if (line.Split('\t').Length == 4 && line.Split('\t')[0].Equals("["+mmap[0]+"]") == true && String.IsNullOrEmpty(line) == false)
                                {
                                    tw.WriteEndElement();
                                    tw.WriteStartElement("MissionMmap");
                                }
                                else if(line.Split('\t').Length== 4 && Char.IsNumber(line, 0) == true && String.IsNullOrEmpty(line) == false)
                                {
                                    tw.WriteStartElement("mission");
                                    for (int i = 1; i < mmap.Length; i++)
                                        if (i < 4)
                                            tw.WriteAttributeString(mmap[i], line.Split('\t')[i]);
                                        else if (line.Contains("&lt;") == true)
                                            tw.WriteAttributeString(mmap[i], "0");
                                    tw.WriteEndElement();
                                }
                        }

                        tw.WriteEndElement();
                        tw.WriteEndElement();
                        tr.Close();

                        #endregion

                    }

                    tw.WriteEndElement();
                    tw.Close();
                    break;
                #endregion

                #region World*-npc.scp to NpcsList.xml
                case 2:
                    break;
                #endregion

                #region World*-terrain.scp to TerrainsList.xml
                case 3:
                    break;
                #endregion

                #region WarMap*.scp to WarMapsList.xml
                case 4:
                    break;
                #endregion

                #region Ability.scp to Ability.xml
                case 5:
                    break;
                #endregion

                #region Achievement.scp to Achievement.xml
                case 6:
                    break;
                #endregion

                #region AlzControl.scp to AlzControl.xml
                case 7:
                    break;
                #endregion

                #region Arena.scp to Arena.xml
                case 8:
                    break;
                #endregion

                #region Assistant.scp to Assistant.xml
                case 9:
                    break;
                #endregion

                #region BelongItem.scp to BelongItem.xml
                case 10:
                    break;
                #endregion

                #region BlessingBead.scp to BlessingBead.xml
                case 11:
                    break;
                #endregion

                #region CashItem.scp to CashItem.xml
                case 12:
                    break;
                #endregion

                #region ChangeItem.scp to ChangeItem.xml
                case 13:
                    break;
                #endregion

                #region ChangeShape.scp to ChangeShape.xml
                case 14:
                    break;
                #endregion

                #region Const.scp to Const.xml
                case 15:
                    break;
                #endregion

                #region Core.scp to Core.xml
                case 16:
                    break;
                #endregion

                #region Craft.scp to Craft.xml
                case 17:
                    break;
                #endregion

                #region Destroy.scp to Destroy.xml
                case 18:
                    break;
                #endregion

                #region DungeonEntryException.scp to DungeonEntryException.xml
                case 19:
                    break;
                #endregion

                #region Event.scp to Event.xml
                case 20:
                    break;
                #endregion

                #region GiftBoxWrapper.scp to GiftBoxWrapper.xml
                case 21:
                    break;
                #endregion

                #region Item.scp to Item.xml
                case 22:
                    break;
                #endregion

                #region ItemReward.scp to ItemReward.xml
                case 23:
                    break;
                #endregion

                #region Level.scp to Level.xml
                case 24:
                    break;
                #endregion

                #region Market.scp to Market.xml
                case 25:
                    break;
                #endregion

                #region MissionDungeon.scp to MissionDungeon.xml
                case 26:
                    break;
                #endregion

                #region Mobs.scp to Mobs.xml
                case 27:
                    break;
                #endregion

                #region MultiBuff.scp to MultiBuff.xml
                case 28:
                    break;
                #endregion

                #region Multiple.scp to Rates.xml
                case 29:
                    break;
                #endregion

                #region NPCShop.scp to NPCShop.xml
                case 30:
                    break;
                #endregion

                #region OptionPool.scp to OptionPool.xml
                case 31:
                    break;
                #endregion

                #region Pet.scp to Pet.xml
                case 32:
                    break;
                #endregion

                #region PremiumService.scp to PremiumService.xml
                case 33:
                    break;
                #endregion

                #region Product.scp to Product.xml
                case 34:
                    break;
                #endregion

                #region Quest.scp to Quest.xml
                case 35:
                    break;
                #endregion

                #region QuestDungeon.scp to QuestDungeon.xml
                case 36:
                    break;
                #endregion

                #region Rank.scp to Rank.xml
                case 37:
                    break;
                #endregion

                #region SP_Dice.scp to SP_Dice.xml
                case 38:
                    break;
                #endregion

                #region ServerMob.scp to ServerMob.xml
                case 39:
                    break;
                #endregion

                #region SetEffect.scp to SetEffect.xml
                case 40:
                    break;
                #endregion

                #region Skill.scp to Skill.xml
                case 41:
                    break;
                #endregion

                #region SpecialInventory.scp to SpecialInventory.xml
                case 42:
                    break;
                #endregion

                #region SpecialItem.scp to SpecialItem.xml
                case 43:
                    break;
                #endregion

                #region Terrain.scp to Terrain.xml
                case 44:
                    break;
                #endregion

                #region Title.scp to Title.xml
                case 45:
                    break;
                #endregion

                #region TitleUseException.scp to TitleUseException.xml
                case 46:
                    break;
                #endregion

                #region War.scp to War.xml
                case 47:
                    break;
                #endregion

                #region Warp.scp to Warp.xml
                case 48:
                    break;
                #endregion

                #region WorldDrop.scp to WorldDrop.xml
                case 49:
                    break;
                    #endregion
            }
        }
    }
}
