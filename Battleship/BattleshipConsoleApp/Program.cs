using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleShipBrain;
using BattleShipConsoleUI;
using MenuSystem;

namespace BattleshipConsoleApp
{
    class Program : HelperMethods
    {
        private static readonly BSBrain Brain = new BSBrain(new GameConfig());
        private static readonly BSConsoleUI ConsoleUI = new BSConsoleUI();
        
        private static string _basePath = null!;

        static void Main(string[] args)
        {
            Console.Clear();
            _basePath = args.Length == 1 ? args[0] : System.IO.Directory.GetCurrentDirectory();

            var mainMenu = new Menu(() => "", "Battleship", EMenuLevel.Root);

            mainMenu.SetMenuForegroundColor(ConsoleColor.DarkBlue);
            var banner = File.ReadAllText(_basePath + "/BattleshipConsoleApp/banner/banner.txt");
            mainMenu.ShowMenuBanner(banner);

            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("P", "Play game", GameMenu),
                new MenuItem("C", "Change configurations", ConfigurationsMenu),
            });

            mainMenu.Run();
        }
        
        private static string GameMenu()
        {
            var menu = new Menu(() => "", "Play game", EMenuLevel.First);

            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("N", "New game", NewGame),
                new MenuItem("L", "Load game", LoadGameMenu),
            });

            var res = menu.Run();
            return res;
        }

        private static string NewGame()
        {
            PlayGame();
            return "";
        }
        
        private static string LoadGameMenu()
        {
            
            var menu = new Menu(() => "", "Load game", EMenuLevel.SecondOrMore);

            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("F", "Load game from file", LoadGameFromFile),
                new MenuItem("D", "Load game from database", LoadGameFromDb),
            });

            var res = menu.Run();
            return res;
        }

        private static string LoadGameFromFile()
        {
            LoadGamesFromFile();
            PlayGame();
            return "";
        }
        
        private static string LoadGameFromDb()
        {
            LoadGamesFromDb();
            PlayGame();
            return "";
        }

        private static void PlaceShips()
        {
            if (DoesPLayerWantRandomShips(Brain.CurrentPlayerToString()))
            {
                Brain.PlaceShipsRandomlyForAPlayer();
            }
            else
            {
                PlaceShipsManuallyForAPlayer();
            }
            Brain.ChangePlayer();
        }
        
        private static void PlaceShipsManuallyForAPlayer()
        {
            ConsoleUI.ShowWhichPlayerHasToPLaceTheShips(Brain.CurrentPlayerToString());  
            
            IOrderedEnumerable<ShipConfig> configs =
                Brain.Config.ShipConfigs.OrderByDescending(config => config.ShipSizeX * config.ShipSizeY);
            
            foreach (var shipConfig in configs)
            {
                for (var i = 0; i < shipConfig.Quantity; i++)
                {
                    ConsoleUI.DrawPLayerBoard(Brain.GetBoardForOpponentPlayerWithShips());
                    ConsoleUI.ShowShipAndGameConfigsInfo(Brain.Config, shipConfig);
                    
                    if (shipConfig.ShipSizeX == 1 && shipConfig.ShipSizeY == 1)
                    {
                        var position = GetShipCoordinateFromUser("SHIP");
                        Brain.PlaceAShipOnOpponentBoard(shipConfig.Name, position, position, position);
                    }
                    else
                    {
                        do
                        {
                            var startPosition = GetShipCoordinateFromUser("STARTING");
                            var widthPosition = GetShipWidthCoordinateFromUser(startPosition, shipConfig);
                            var heightPosition = GetShipHeightCoordinateFromUser(startPosition, widthPosition, shipConfig);

                            if (Brain.IsShipSafeToPlaceOnOpponentBoard(startPosition, widthPosition, heightPosition))
                            {
                                Brain.PlaceAShipOnOpponentBoard(shipConfig.Name, startPosition, widthPosition, heightPosition);
                                break;
                            }
                            ConsoleUI.ShowInfo("Cannot place ship on other ship or its territory!");
                        } while (true);
                    }
                    Console.WriteLine();
                }
            }
        }
        
        private static Coordinate GetShipWidthCoordinateFromUser(Coordinate startPos, ShipConfig shipConfig) {
            do
            {
                var widthPosition = GetShipCoordinateFromUser("WIDTH");
                if (Brain.IsShipWidthCorrect(startPos, widthPosition, shipConfig))
                {
                    return widthPosition;
                }
                ConsoleUI.ShowInfo(shipConfig.ShipSizeX == shipConfig.ShipSizeY
                    ? $"Given width is not correct. Width must be {shipConfig.ShipSizeX} cell(s) on row {startPos.Y + 1}."
                    : $"Given width is not correct. Width must be {shipConfig.ShipSizeX} or {shipConfig.ShipSizeY} cell(s) on row {startPos.Y + 1}.");
                
            } while (true);
        }        
        
        private static Coordinate GetShipHeightCoordinateFromUser(Coordinate startPos, Coordinate width, ShipConfig shipConfig) {
            do
            {
                var validHeight = Brain.GetValidShipHeightBasedOnStartAndWidthPos(startPos, width, shipConfig);
                if (validHeight == 1) return startPos;
                
                var heightPosition = GetShipCoordinateFromUser("HEIGHT");
                if (Brain.IsShipHeightCorrect(startPos, heightPosition, validHeight))
                {
                    return heightPosition;
                }
                ConsoleUI.ShowInfo($"Given height is not correct. Height must be {validHeight} cell(s) on column {Brain.GetColumnLetterFromIndex(startPos.X)}.");
            } while (true);
        }

        private static Coordinate GetShipCoordinateFromUser(string coordinateType)
        {
            do
            {
                ConsoleUI.ShowShipCoordinateInfo(coordinateType);
                var userValue = Console.ReadLine()?.Trim() ?? "";

                if (Brain.AreCoordinatesInGivenString(userValue))
                {
                    var coordinate = Brain.ConvertCoordinateInStringToCoordinate(userValue);
                    if (Brain.IsGivenCoordinateOnBoard(coordinate.X, coordinate.Y))
                    {
                        if (!Brain.IsShipOnGivenCellOnOpponentBoard(coordinate.X, coordinate.Y))
                        {
                            return coordinate;
                        }
                        ConsoleUI.ShowInfo("Cannot place ship on other ship or its territory!");
                    }
                    else
                    {
                        ConsoleUI.ShowInfo("Given coordinate is not on board!");
                    }
                }
                else
                {
                    ConsoleUI.ShowInfo("Wrong input!");
                }
            } while (true);
        }

        private static void PlayGame()
        {
            if (!Brain.HasCurrentPlayerPLacedShipsOnOpponentBoard())  
            {
                ConsoleUI.WelcomeAndShowShipInfo();
                PlaceShips();
                PlaceShips();
                ConsoleUI.InformThatShipsAreSuccessfullyPlaced();
            }
            
            var gameIsOver = false;
            do
            {
                var board = Brain.GetBoardForCurrentPlayer();
                var opponentBoard = Brain.GetBoardForOpponentPlayerWithShips();
                var playerAorB = Brain.CurrentPlayerToString();
                ConsoleUI.ShowBoardsAndGameInfoToUser(board, opponentBoard, playerAorB);

                bool moveIsMade;
                do
                {
                    var userValue = Console.ReadLine()?.Trim() ?? "";
                    ConsoleUI.WriteNewline();

                    switch (userValue.Length)
                    {
                        case > 0 when userValue.ToUpper().Equals("U"):
                            Brain.UndoMove();
                            moveIsMade = true;
                            break;
                        
                        case > 0 when userValue.ToUpper().Equals("S"):
                            SaveGame();
                            ConsoleUI.ShowInfo("Saving...");
                            moveIsMade = true;
                            gameIsOver = false;
                            break;
                        
                        case > 0 when userValue.ToUpper().Equals("E"):
                            ConsoleUI.ShowInfo("Exiting...");
                            moveIsMade = true;
                            gameIsOver = true;
                            break;
                        
                        default:
                            moveIsMade = ReturnsTrueIfIsMoveMade(userValue);
                            
                            if (moveIsMade)
                            {
                                if (Brain.IsLastMoveWinningMove())
                                {
                                    gameIsOver = true;
                                    ConsoleUI.ShowWinningMessage(playerAorB);
                                }
                                else
                                {
                                    ConsoleUI.ShowMoveResultAndContinueMessage(Brain.LastMoveType(), 
                                        Brain.OpponentPlayerToString());
                                    var opponentWantsToContinue = Console.ReadLine()?.Trim() ?? "";
                                }
                                Brain.ChangePlayer();
                            }
                            break;
                    }
                } while (!moveIsMade);
            } while (!gameIsOver);
            Brain.ResetGame();
        }

        private static bool ReturnsTrueIfIsMoveMade(string userValue)
        { 
            var userValues = userValue.ToCharArray(); 

            if (Brain.AreCoordinatesInGivenString(userValue))
            {
                var coordinate = Brain.ConvertCoordinateInStringToCoordinate(userValue);

                if (Brain.IsGivenCoordinateOnBoard(coordinate.X, coordinate.Y))
                {
                    if (Brain.IsCellWithoutBomb(coordinate.X, coordinate.Y))
                    {
                        Brain.MakeAMove(coordinate.X, coordinate.Y);
                        return true;
                    }
                    ConsoleUI.ShowInfo("Cannot hit same place twice!");
                    ConsoleUI.GiveMoveInfoToUser();
                    return false;
                }
            }
            ConsoleUI.ShowInfo("Wrong input!");
            ConsoleUI.GiveMoveInfoToUser();
            return false;
        }

        private static string ConfigurationsMenu()
        {
            var menu = new Menu(() =>
            {
                ConsoleUI.WriteNewline();
                ConsoleUI.ShowInfo("Current configuration: ");
                return Brain.Config.ToString();
            }, "Configurations", EMenuLevel.First);

            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("D", "Default configuration", SetDefaultConf),
                new MenuItem("C", "Custom configuration", SetCustomConf),
            });

            var res = menu.Run();
            return res;
        }

        private static string SetDefaultConf()
        {
            Brain.SetGameConfig(new GameConfig());
            return "";
        }

        private static string SetCustomConf()
        {
            var menu = new Menu(() => "", "Custom configuration", EMenuLevel.SecondOrMore);

            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("C", "Create new custom configuration", CreateCustomConfig),
                new MenuItem("L", "Load custom configuration", LoadCustomConfigsMenu),
            });

            var res = menu.Run();
            return res;
        }

        private static string CreateCustomConfig()
        {
            var x = GetHeightOrWidthFromUser("width", Brain);
            var y = GetHeightOrWidthFromUser("height", Brain);
            var touchRule = GetShipTouchRuleFromUser();

            var newCustomConfig = new GameConfig();
            newCustomConfig.BoardSizeX = x;
            newCustomConfig.BoardSizeY = y;
            newCustomConfig.EShipTouchRule = touchRule;

            newCustomConfig.ShipConfigs = new List<ShipConfig>();
            var shipConfigs = GetShipConfigsFromUser(newCustomConfig, Brain);
            newCustomConfig.ShipConfigs = shipConfigs;

            SaveCustomConfiguration(newCustomConfig);
            Brain.SetGameConfig(newCustomConfig); 
            
            ConsoleUI.WriteNewline();
            ConsoleUI.ShowInfo("Configuration saved and set.");
            
            return "";
        }
        
        private static string LoadCustomConfigsMenu()
        {
            var menu = new Menu(() => "", "Load custom configurations", EMenuLevel.SecondOrMore);

            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("F", "Load custom configuration from file", LoadCustomConfigsFromFile),
                new MenuItem("D", "Load custom configuration from database", LoadCustomConfigsFromDb),
            });

            var res = menu.Run();
            return res;
        }

        private static string LoadCustomConfigsFromFile()
        {
            var files = Brain.LoadCustomGameConfigsFromDirectory(_basePath);

            if (files.Count == 0)
            {
                ConsoleUI.WriteNewline();
                ConsoleUI.ShowInfo("No custom configurations found.");
                return "";
            }

            var fileNo = GetFileNumberFromUser("Choose configuration file: ", files);
            var fileName = files[fileNo];
            
            Brain.LoadFromFileAndSetCustomConfig(fileName);
            return "";
        }
        
        private static string LoadCustomConfigsFromDb()
        {
            var configsFromDb = Brain.LoadCustomGameConfigsFromDb();

            if (configsFromDb.Count == 0)
            {
                ConsoleUI.WriteNewline();
                ConsoleUI.ShowInfo("No custom configurations found.");
                return "";
            }
            var gameConfig = GetGameConfigEntityFromUser("Choose configuration file: ", configsFromDb);
            
            var selectedConf = Brain.ConvertDomainGameConfigToGameConfig(gameConfig);
            Brain.SetGameConfig(selectedConf);
            
            return "";
        }

        private static string LoadGamesFromFile()
        {
            var files = Brain.LoadSavedGamesFromDirectory(_basePath);

            if (files.Count == 0)
            {
                ConsoleUI.WriteNewline();
                ConsoleUI.ShowInfo("No saved games found.");
                return "";
            }

            var fileNo = GetFileNumberFromUser("Choose a game: ", files);
            var fileName = files[fileNo];
            
            Brain.LoadFromFileAndSetSavedGame(fileName);
            return "";
        }
        
        private static string LoadGamesFromDb()
        {
            var gamesFromDb = Brain.LoadSavedGamesFromDb();

            if (gamesFromDb.Count == 0)
            {
                ConsoleUI.WriteNewline();
                ConsoleUI.ShowInfo("No saved games found.");
                return "";
            }
            var savedGame = GetSavedGameEntityFromUser("Choose a game: ", gamesFromDb);
            
            var selectedGame = Brain.ConvertDomainSavedGameToSaveGameDto(savedGame);
            Brain.SetGame(selectedGame);
            
            return "";
        }

        private static void SaveCustomConfiguration(GameConfig config)
        {
            var customConfName = GetCustomFileNameFromUser("configuration");
            BSBrain.SaveGameConfigToFile(config, _basePath, customConfName);
            BSBrain.SaveGameConfigToDb(config, customConfName);
        }
        
        private static void SaveGame()
        {
            SaveGameToFile();
            SaveGameToDb();
        }      
        
        private static void SaveGameToFile()
        {
            BSBrain.SaveGameToFile(Brain,_basePath, null);
        }     
        
        private static void SaveGameToDb()
        {
            BSBrain.SaveGameToDb(Brain, null);
        }
    }
}
