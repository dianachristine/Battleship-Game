using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace BattleshipBrain.BattleshipDAOs
{
    public class DbDao
    {
        public static void SaveGameConfig(GameConfig gameConfig, string confName)
        {
            var gameConfigToSave = ConvertGameConfigToDomainGameConfig(gameConfig, confName);
            
            using var db = new ApplicationDbContext();
            
            db.GameConfigs.Add(gameConfigToSave);
            db.SaveChanges();
        } 
        public static int SaveGame(BSBrain brain, string gameName)
        {
            var savedGame = ConvertToDomainSavedGame(brain, gameName);
            
            using var db = new ApplicationDbContext();
            
            db.SavedGames.Add(savedGame);
            db.SaveChanges();
            return savedGame.SavedGameId;
        } 
        
        public static List<Domain.GameConfig> LoadCustomGameConfigs()
        {
            using var db = new ApplicationDbContext();
            List<Domain.GameConfig> gameConfigs = new List<Domain.GameConfig>();
            
            foreach (var gameConfig in db.GameConfigs
                .Include(table => table.ShipConfigs)
                .OrderBy(conf => conf.GameConfigId))
            {
                gameConfigs.Add(gameConfig);
            }
            return gameConfigs;
        }
        
        public static List<Domain.SavedGame> LoadSavedGames()
        {
            using var db = new ApplicationDbContext();
            List<Domain.SavedGame> savedGames = new List<Domain.SavedGame>();

            foreach (var savedGame in db.SavedGames
                .Include(table => table.GameBoards)
                .Include(table => table.GameConfig)
                .Include("GameConfig.ShipConfigs")
                .Include("GameBoards.Ships")
                .Include("GameBoards.Ships.Coordinates")
                .OrderBy(game => game.SavedGameId))
            {
                savedGames.Add(savedGame);
            }

            return savedGames;
        }
        
        private static Domain.GameConfig ConvertGameConfigToDomainGameConfig(GameConfig gameConfig, string confName)
        {
            var domainGameConfig = new Domain.GameConfig()
            {
                BoardSizeX = gameConfig.BoardSizeX,
                BoardSizeY = gameConfig.BoardSizeY,
                EShipTouchRule = (int)gameConfig.EShipTouchRule,
                ShipConfigs = new Collection<Domain.ShipConfig>(),
                GameConfigName = confName
            };

            foreach (var shipConfig in gameConfig.ShipConfigs)
            {
                var shipConfigToSave = new Domain.ShipConfig()
                {
                    Name = shipConfig.Name,
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    Quantity = shipConfig.Quantity
                };

                domainGameConfig.ShipConfigs.Add(shipConfigToSave);
            }

            return domainGameConfig;
        }
        
        public static Domain.SavedGame ConvertToDomainSavedGame(BSBrain brain, string gameName)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = false
            };

            var dto = new SaveGameDTO(brain.PlayerNoWhoPlays, brain.Config, brain.GameBoards);
            var gameConfig = DbDao.ConvertGameConfigToDomainGameConfig(dto.GameConfig, "Configuration from saved game");

            var savedgame = new Domain.SavedGame()
            {
                SavedGameName = gameName,
                CurrentPlayerNo = dto.CurrentPlayerNo,
                GameConfig = gameConfig,
                GameBoards = new List<Domain.GameBoard>()
            };

            foreach (var gameBoard in dto.GameBoards)
            {
                var gameBoardToSave = new Domain.GameBoard()
                {
                    Board = JsonSerializer.Serialize(gameBoard.Board, jsonOptions),
                    Ships = new List<Domain.Ship>()
                };
                
                foreach (var ship in gameBoard.Ships)
                {
                    var shipToSave = new Domain.Ship()
                    {
                        Name = ship.Name,
                        Coordinates = new List<Domain.Coordinate>()
                    };

                    foreach (var coordinate in ship.Coordinates)
                    {
                        var coordinateToSave = new Domain.Coordinate()
                        {
                            X = coordinate.X,
                            Y = coordinate.Y
                        };
                        shipToSave.Coordinates.Add(coordinateToSave);
                    }
                    gameBoardToSave.Ships.Add(shipToSave);
                }
                savedgame.GameBoards.Add(gameBoardToSave);
            }

            return savedgame;
        }
        
        public static GameConfig ConvertDomainGameConfigToGameConfig(Domain.GameConfig domainGameConfig)
        {
            var convertedConfig = new GameConfig()
            {
                BoardSizeX = domainGameConfig.BoardSizeX,
                BoardSizeY = domainGameConfig.BoardSizeY,
                EShipTouchRule = (EShipTouchRule) domainGameConfig.EShipTouchRule,
                ShipConfigs = new List<ShipConfig>()
            };

            foreach (var domainShipConfig in domainGameConfig.ShipConfigs)
            {
                var convertedShipConfig = new ShipConfig()
                {
                    ShipSizeX = domainShipConfig.ShipSizeX,
                    ShipSizeY = domainShipConfig.ShipSizeY,
                    Quantity = domainShipConfig.Quantity,
                    Name = domainShipConfig.Name
                };
                convertedConfig.ShipConfigs.Add(convertedShipConfig);
            }

            return convertedConfig;
        }
        
        public static SaveGameDTO ConvertDomainSavedGameToSaveGameDto(Domain.SavedGame domainSavedGame)
        {
            var gameConfig = ConvertDomainGameConfigToGameConfig(domainSavedGame.GameConfig);

            var convertedGame = new SaveGameDTO()
            {
                CurrentPlayerNo = domainSavedGame.CurrentPlayerNo,
                GameConfig = gameConfig,
                GameBoards = new SaveGameDTO.GameBoardDTO[2]
            };

            var gameBoards = new List<SaveGameDTO.GameBoardDTO>();

            foreach (var gameBoard in domainSavedGame.GameBoards)
            {
                var convertedGameBoard = new SaveGameDTO.GameBoardDTO()
                {
                    Board = JsonSerializer.Deserialize<List<List<BoardSquareState>>>(gameBoard.Board)!,
                    Ships = new List<Ship>()
                };
                
                foreach (var ship in gameBoard.Ships)
                {
                    var convertedShip = new Ship(ship.Name, new List<Coordinate>());

                    foreach (var coordinate in ship.Coordinates)
                    {
                        var coordinateToSave = new Coordinate()
                        {
                            X = coordinate.X,
                            Y = coordinate.Y
                        };
                        convertedShip.Coordinates.Add(coordinateToSave);
                    }
                    convertedGameBoard.Ships.Add(convertedShip);
                }
                gameBoards.Add(convertedGameBoard);
            }

            for (var i = 0; i < 2; i++)
            {
                convertedGame.GameBoards[i] = gameBoards[i];
            }
            return convertedGame;
        }
    }
}
