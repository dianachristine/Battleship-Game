using System;
using System.Collections.Generic;
using BattleShipBrain;
using BattleShipConsoleUI;

namespace BattleshipConsoleApp
{
    public class HelperMethods
    {
        private static readonly BSConsoleUI ConsoleUI = new BSConsoleUI();

        public static int GetHeightOrWidthFromUser(string widthOrHeight, BSBrain brain)
        {
            do
            {
                ConsoleUI.GiveBoardSizeInfoToUser(widthOrHeight, brain.GetMinBoardSize(), brain.GetMaxBoardSize());
                var size = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(size, out _) && brain.IsHeightOrWidthBetweenMinAndMaxBoardSize(int.Parse(size)))
                {
                    return int.Parse(size);
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }
        
        public static EShipTouchRule GetShipTouchRuleFromUser()
        {
            do
            {
                ConsoleUI.GiveTouchRuleInfoToUser();
                var touchRule = Console.ReadLine()?.Trim().ToUpper() ?? "";
                if (!string.IsNullOrWhiteSpace(touchRule))
                {
                    switch (touchRule)
                    {
                        case "N":
                            return EShipTouchRule.NoTouch;
                        case "C":
                            return EShipTouchRule.CornerTouch;
                        case "S":
                            return EShipTouchRule.SideTouch;
                    }
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }
        
        public static List<ShipConfig> GetShipConfigsFromUser(GameConfig gameConfig, BSBrain brain)
        {
            var shipConfigs = new List<ShipConfig>();
            var copyOfGameConfig = gameConfig.CreateCopyOfGameConfig();
            string userChoice;
            
            do
            {
                ConsoleUI.ShowCurrentShipConfig(shipConfigs);
                ConsoleUI.ShowShipConfigChoices();
                
                userChoice = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (userChoice.Equals("S") && shipConfigs.Count > 0)
                {
                    break;
                }

                if (userChoice.Equals("A") && brain.CalculateEmptySpaceInGivenConf(copyOfGameConfig) > 0)  // check if at least ship in size 1x1 fits to the board
                {
                    var shipConfig = new ShipConfig();
                    
                    shipConfig.ShipSizeX = GetShipSizeFromUser("WIDTH", copyOfGameConfig, brain);
                    shipConfig.ShipSizeY = GetShipSizeFromUser("HEIGHT", copyOfGameConfig, brain);
                    shipConfig.Quantity = GetShipQuantityFromUser(copyOfGameConfig, shipConfig, brain);
                    shipConfig.Name = GetShipNameFromUser();

                    shipConfigs.Add(shipConfig);
                    copyOfGameConfig.ShipConfigs.Add(shipConfig);
                    
                } else if (userChoice.Equals("S") && shipConfigs.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Cannot create game configuration without ships!");
                } else
                {
                    Console.WriteLine();
                    Console.WriteLine("Wrong input!");
                }
            } while (true);

            return shipConfigs;
        }
        
        private static int GetShipSizeFromUser(string widthOrHeight, GameConfig gameConfig, BSBrain brain)
        {
            do
            {
               ConsoleUI.ShowShipSizeInfo(widthOrHeight, brain.GetSmallerBoardSizeFromGivenConfig(gameConfig));
                
                var width = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(width, out _) && brain.DoesShipSizeFitToTableSizeInGivenConfig(int.Parse(width), gameConfig))
                {
                    return int.Parse(width);
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }
        
        private static int GetShipQuantityFromUser(GameConfig gameConfig, ShipConfig shipConfig, BSBrain brain)
        {
            do
            {
                var maxQuantity = brain.CalculateQuantityForGivenGameConfigAndShipConfig(gameConfig, shipConfig); 
                ConsoleUI.ShowShipQuantityInfo(maxQuantity);
                
                var quantity = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(quantity, out _))
                {
                    if (int.Parse(quantity) > maxQuantity)
                    {
                        ConsoleUI.ShowInfo("Too many ships - doesn't fit to the board!");
                    }
                    else if (int.Parse(quantity) <= 0)
                    {
                        ConsoleUI.ShowInfo("Quantity cannot be less or equal to 0!");
                    }
                    else
                    {
                        return int.Parse(quantity);
                    }
                }
                else
                {
                    ConsoleUI.ShowInfo("Wrong input!");
                }
            } while (true);
        }
        
        private static string GetShipNameFromUser()
        {
            do
            {
                ConsoleUI.ShowShipNameInfo();
                
                var name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                ConsoleUI.ShowInfo("Ship name cannot be empty!");
            } while (true);
        }

        public static bool DoesPLayerWantRandomShips(string player)
        {
            do
            {
                ConsoleUI.ShowUserShipPlacementOptions(player);
                
                var randomOrNot = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(randomOrNot))
                {
                    switch (randomOrNot.ToLower())
                    {
                        case "r":
                            return true;
                        case "m":
                            return false;
                    }
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }

        public static Domain.GameConfig GetGameConfigEntityFromUser(string infoForUser, List<Domain.GameConfig> configs)
        {
            do
            {
                ConsoleUI.ShowConfigsFromDbToUser(configs);
                ConsoleUI.ShowInfoWithoutNewline(infoForUser);
                var choice = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(choice))
                {
                    if (int.TryParse(choice, out _))
                    {
                        if (int.TryParse(choice, out _) && int.Parse(choice) >= 0 && int.Parse(choice) < configs.Count)
                        {
                            var index = int.Parse(choice);
                            return configs[index];
                        }
                    }
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }    
        
        public static Domain.SavedGame GetSavedGameEntityFromUser(string infoForUser, List<Domain.SavedGame> games)
        {
            do
            {
                ConsoleUI.ShowGamesFromDbToUser(games);
                ConsoleUI.ShowInfoWithoutNewline(infoForUser);
                var choice = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(choice))
                {
                    if (int.TryParse(choice, out _) && int.Parse(choice) >= 0 && int.Parse(choice) < games.Count)
                    {
                        var index = int.Parse(choice);
                        return games[index];
                    }
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }
        
        public static string? GetCustomFileNameFromUser(string info)
        {
            ConsoleUI.ShowCustomOrDefaultNameInfo(info);
            
            var filename = Console.ReadLine();
            return filename;
        }
        
        public static int GetFileNumberFromUser(string infoForUser, List<string> files)
        {
            do
            {
                ConsoleUI.ShowFilesToUser(files);
                ConsoleUI.ShowInfoWithoutNewline(infoForUser);
                var choice = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(choice))
                {
                    if (int.TryParse(choice, out _) && int.Parse(choice) >= 0 && int.Parse(choice) < files.Count)
                    {
                        return int.Parse(choice);
                    }
                }
                ConsoleUI.ShowInfo("Wrong input!");
            } while (true);
        }
    }
}
