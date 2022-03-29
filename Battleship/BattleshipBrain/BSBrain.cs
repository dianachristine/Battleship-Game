using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipBrain.BattleshipDAOs;

namespace BattleShipBrain
{
    public class BSBrain
    {
        public int PlayerNoWhoPlays { get; set; }
        public GameConfig Config { get; set; }
        
        public GameBoard[] GameBoards = new GameBoard[2];
        
        private const int MinBoardSize = 5;
        private const int MaxBoardSize = 20; 
        
        private readonly Random _rnd = new Random();
        
        private readonly char[] _letters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };

        private List<Coordinate> _previousMoves = new List<Coordinate>();

        public GameConfig TempConfig = new GameConfig() {ShipConfigs = new List<BattleShipBrain.ShipConfig>()};
        public string TempConfigName = "";
        public List<ShipConfig> TempShipsLeftToPlace = new List<ShipConfig>();
        public Coordinate? TempManualShipPlacementShipStartPos { get; set; }
        public Coordinate? TempManualShipPlacementShipWidthPos { get; set; }
        public bool TempShowContinueButton { get; set; } = false;

        public BSBrain(GameConfig config)
        {
            GameBoards[0] = new GameBoard();
            GameBoards[1] = new GameBoard();
            
            GameBoards[0].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            
            GameBoards[0].Ships = new List<Ship>();
            GameBoards[1].Ships = new List<Ship>();
            
            PlayerNoWhoPlays = 0;
            Config = config;
        }
        
        // boards
        public BoardSquareState[,] GetBoardForCurrentPlayer()
        {
            return CopyOfBoard(GameBoards[PlayerNoWhoPlays].Board);
        }
        
        public BoardSquareState[,] GetBoardForPlayerWithOpponentShips()
        {
            var opponentBoard = CopyOfBoard(GameBoards[PlayerNoWhoPlays].Board);

            foreach (var ship in GameBoards[PlayerNoWhoPlays].Ships)
            {
                foreach (var coordinate in ship.Coordinates)
                {
                    opponentBoard[coordinate.X, coordinate.Y].IsShip = true;
                }
            }
            
            return opponentBoard;
        }      
        
        public BoardSquareState[,] GetBoardForOpponentPlayerWithShips()  // to place ships & to show during game
        {
            var opponentPLayer = PlayerNoWhoPlays == 0 ? 1 : 0;
            var opponentBoard = CopyOfBoard(GameBoards[opponentPLayer].Board);

            foreach (var ship in GameBoards[opponentPLayer].Ships)
            {
                foreach (var coordinate in ship.Coordinates)
                {
                    opponentBoard[coordinate.X, coordinate.Y].IsShip = true;
                }
            }
            
            return opponentBoard;
        }

        private BoardSquareState[,] CopyOfBoard(BoardSquareState[,] board)
        {
            var res = new BoardSquareState[board.GetLength(0),board.GetLength(1)];
            
            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    res[x, y] = board[x, y];
                }
            }
            
            return res;
        }
        
        public int GetMinBoardSize()
        {
            return MinBoardSize;
        }
        
        public int GetMaxBoardSize()
        {
            return MaxBoardSize;
        }

        // ships
        public bool HasCurrentPlayerPLacedShipsOnOpponentBoard()
        {
            return GameBoards[GetOpponent()].Ships.Count > 0 && TempShipsLeftToPlace.Count == 0;
        }
        
        public void PlaceShipsRandomlyForAPlayer()  // TODO optimize
        {
            IOrderedEnumerable<ShipConfig> configs =
                Config.ShipConfigs.OrderByDescending(config => config.ShipSizeX * config.ShipSizeY);

            foreach (var shipConfig in configs)
            {
                for (var i = 0; i < shipConfig.Quantity; i++)
                {
                    if (shipConfig.ShipSizeX == 1 && shipConfig.ShipSizeY == 1)
                    {
                        var position = GetRandomValidShipCoordinateOnBoard();
                        PlaceAShipOnOpponentBoard(shipConfig.Name, position, position, position);
                    }
                    else
                    {
                        do
                        {
                            Coordinate startPosition;
                            Coordinate widthPosition;
                            Coordinate? heightPosition = null;
                            do
                            {
                                startPosition = GetRandomValidShipCoordinateOnBoard();
                                widthPosition = GetRandomShipWidthCoordinate(startPosition, shipConfig);
                                heightPosition = GetRandomShipHeightCoordinate(startPosition, widthPosition, shipConfig);
                            } while (heightPosition == null);

                            var heightPos = heightPosition.Value;
                            
                            if (IsShipSafeToPlaceOnOpponentBoard(startPosition, widthPosition, heightPos))
                            {
                                PlaceAShipOnOpponentBoard(shipConfig.Name, startPosition, widthPosition, heightPos);
                                break;
                            }
                        } while (true);
                    }
                }
            }
        }


        public Coordinate GetRandomValidShipCoordinateOnBoard()
        {
            do
            {
                var x = _rnd.Next(0, Config.BoardSizeX);
                var y = _rnd.Next(0, Config.BoardSizeX);

                if (IsGivenCoordinateOnBoard(x, y) && !IsShipOnGivenCellOnOpponentBoard(x, y))
                {
                    return new Coordinate() { X = x, Y = y };
                }
            } while (true);
        }
        
        public Coordinate GetRandomShipWidthCoordinate(Coordinate startPos, ShipConfig shipConfig) {
            do
            {
                var widthPosition = GetRandomValidShipCoordinateOnBoard();  // TODO optimize
                if (IsGivenCoordinateOnBoard(widthPosition.X, widthPosition.Y) && 
                    !IsShipOnGivenCellOnOpponentBoard(widthPosition.X, widthPosition.Y) 
                    && IsShipWidthCorrect(startPos, widthPosition, shipConfig))
                {
                    return widthPosition;
                }
            } while (true);
        }
        
        public Coordinate? GetRandomShipHeightCoordinate(Coordinate startPos, Coordinate width, ShipConfig shipConfig) {
            var validHeight = GetValidShipHeightBasedOnStartAndWidthPos(startPos, width, shipConfig);
            if (validHeight == 1) return startPos;
            
            var downOrUp = _rnd.Next(0, 2);
            
            var y = downOrUp == 0 ? startPos.Y - validHeight + 1 : startPos.Y + validHeight - 1;
            var heightPosition = new Coordinate() { X = startPos.X, Y = y};
            if (y >= 0 && IsGivenCoordinateOnBoard(heightPosition.X, heightPosition.Y) && 
                !IsShipOnGivenCellOnOpponentBoard(heightPosition.X, heightPosition.Y) && 
                IsShipHeightCorrect(startPos, heightPosition, validHeight))
            {
                return heightPosition;
            }
            
            y = downOrUp == 0 ? startPos.Y + validHeight - 1 : startPos.Y - validHeight + 1;   
            heightPosition = new Coordinate() { X = startPos.X, Y = y};
            if (y >= 0 && IsGivenCoordinateOnBoard(heightPosition.X, heightPosition.Y) && 
                !IsShipOnGivenCellOnOpponentBoard(heightPosition.X, heightPosition.Y) && 
                IsShipHeightCorrect(startPos, heightPosition, validHeight))
            {
                return heightPosition;
            }
            
            return null;
        }
        
        public bool IsShipWidthCorrect(Coordinate startPos, Coordinate width, ShipConfig shipConfig)
        {
            var shipWidth = GetShipWidthOrHeight(width.X, startPos.X);
            return startPos.Y == width.Y && (shipWidth == shipConfig.ShipSizeX || shipWidth == shipConfig.ShipSizeY);
        }
        
        public bool IsShipHeightCorrect(Coordinate startPos, Coordinate height, int validHeight)
        {
            var shipHeight = GetShipWidthOrHeight(height.Y, startPos.Y);
            return startPos.X == height.X && shipHeight == validHeight;
        }
        
        public int GetValidShipHeightBasedOnStartAndWidthPos(Coordinate startPos, Coordinate width, ShipConfig shipConfig)
        {
            var shipHeight = GetShipWidthOrHeight(width.X, startPos.X);
            return shipHeight == shipConfig.ShipSizeX ? shipConfig.ShipSizeY : shipConfig.ShipSizeX;
        }

        private int GetShipWidthOrHeight(int size1, int size2)
        {
            return Math.Abs(size1 - size2) + 1;
        }

        public bool IsShipSafeToPlaceOnOpponentBoard(Coordinate startingPosition, Coordinate widthPos, Coordinate heightPos)
        {
            var width = GetShipWidthOrHeightFromCoordinates(startingPosition.X, widthPos.X);
            var height = GetShipWidthOrHeightFromCoordinates(startingPosition.Y, heightPos.Y);
            var ship = new Ship("", startingPosition, width, height);

            foreach (var coordinate in ship.Coordinates)
            {
                if (IsShipOnGivenCellOnOpponentBoard(coordinate.X, coordinate.Y))
                {
                    return false;
                }
            }
            return true;
        }
        
        public void PlaceAShipOnOpponentBoard(string name, Coordinate startingPosition, Coordinate widthPos, Coordinate heightPos)
            // Coordinates must be validated before this method
        {
            var width = GetShipWidthOrHeightFromCoordinates(startingPosition.X, widthPos.X);
            var height = GetShipWidthOrHeightFromCoordinates(startingPosition.Y, heightPos.Y);
            var ship = new Ship(name, startingPosition, width, height);

            var opponent = PlayerNoWhoPlays == 0 ? 1 : 0;
            var playerShips= GameBoards[opponent].Ships;
            playerShips.Add(ship);
        }
        
        private int GetShipWidthOrHeightFromCoordinates(int startingPos, int widthOrHeightPos)
        {
            int widthOrHeight;
            
            var difference = Math.Abs(startingPos - widthOrHeightPos);
            widthOrHeight = difference + 1;
            
            if (startingPos > widthOrHeightPos)
            {
                widthOrHeight = -difference - 1;
            }
            return widthOrHeight;
        }

        // cells
        public bool IsCellWithoutBomb(int x, int y)
        {
            return GameBoards[PlayerNoWhoPlays].Board[x, y].IsBomb == false;
        }
        
        public bool IsShipOnGivenCellOnOpponentBoard(int x, int y)
        {
            var opponent = PlayerNoWhoPlays == 0 ? 1 : 0;
            foreach (var ship in GameBoards[opponent].Ships)
            {
                var allShipCoordinates = GetAllCoordinatesGivenShipTakesDependingOnTouchRule(ship.Coordinates);
                foreach (var coordinate in allShipCoordinates)
                {
                    if (coordinate.X == x && coordinate.Y == y)
                    {
                        return true;
                    }
                } 
            }
            return false;
        }

        // coordinates
        private List<Coordinate> GetAllCoordinatesGivenShipTakesDependingOnTouchRule(List<Coordinate> ship) 
            /*  Add following coordinates to each ship coordinate x,y
                SideTouch: -
                CornerTouch:  x-1,y;  x+1,y;  x,y-1;  x,y+1
                NoTouch:  x-1,y;  x+1,y;  x,y-1;  x,y+1;  x-1,y-1;  x+1,y-1;  x-1,y+1;  x+1,y+1  */
        {
            if (Config.EShipTouchRule == EShipTouchRule.SideTouch) return ship;
            
            var coordinates = new HashSet<Coordinate>(ship);
            
            foreach (var coordinate in ship)
            {
                coordinates.Add(new Coordinate() { X = coordinate.X - 1, Y = coordinate.Y });
                coordinates.Add(new Coordinate() { X = coordinate.X + 1, Y = coordinate.Y });
                coordinates.Add(new Coordinate() { X = coordinate.X, Y = coordinate.Y - 1 });
                coordinates.Add(new Coordinate() { X = coordinate.X, Y = coordinate.Y + 1 });
                    
                if (Config.EShipTouchRule == EShipTouchRule.NoTouch)
                {
                    coordinates.Add(new Coordinate() { X = coordinate.X - 1, Y = coordinate.Y - 1 });
                    coordinates.Add(new Coordinate() { X = coordinate.X + 1, Y = coordinate.Y - 1 });
                    coordinates.Add(new Coordinate() { X = coordinate.X - 1, Y = coordinate.Y + 1 });
                    coordinates.Add(new Coordinate() { X = coordinate.X + 1, Y = coordinate.Y + 1 });
                }
            }
            return coordinates.ToList();
        }

        public bool IsGivenCoordinateOnBoard(int x, int y)
        {
            return x < Config.BoardSizeX && 0 <= y && y < Config.BoardSizeY;
        }
        
        public bool AreCoordinatesInGivenString(string s)
        {
            if (s.Length is < 2 or > 3) return false;

            if (!char.IsLetter(s[0]) || !char.IsDigit(s[1])) return false;

            return s.Length != 3 || char.IsDigit(s[2]);
        }
        
        public Coordinate ConvertCoordinateInStringToCoordinate(string coordinateInString)
        {
            var userValues = coordinateInString.ToCharArray(); 
            
            var x = Array.IndexOf(_letters, char.Parse(char.ToString(userValues[0]).ToUpper()));
            var y = int.Parse(coordinateInString[1..]) - 1;
            return new Coordinate()
            {
                X = x,
                Y = y
            };
        }        
        
        public string ConvertCoordinateToString(Coordinate coordinate)
        {
            var x = GetColumnLetterFromIndex(coordinate.X);
            var y = coordinate.Y + 1;
            return x + y;
        }

        public string GetColumnLetterFromIndex(int index)
        {
            
            return char.ToString(_letters[index]);
        }
        
        // moves
        public void MakeAMove(int x, int y)
        {
            var board = GetBoardForPlayerWithOpponentShips();
            if (!board[x, y].IsBomb) // is not played already
            {
                GameBoards[PlayerNoWhoPlays].Board[x, y].IsBomb = true;
                
                if (board[x, y].IsShip)
                {
                    GameBoards[PlayerNoWhoPlays].Board[x, y].IsShip = true;
                }
                _previousMoves.Add(new Coordinate() {X = x, Y = y});
            }
        }
        
        public bool IsLastMoveWinningMove()
        {
            var board = GetBoardForPlayerWithOpponentShips();
            
            for (var x = 0; x < board.GetLength(0); x++)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y].IsShip && !board[x, y].IsBomb)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string LastMoveType()
        {
            var lastMove = _previousMoves[^1];
            var board = GetBoardForPlayerWithOpponentShips();
            if (board[lastMove.X, lastMove.Y].IsShip)
            {
                return "HIT";
            } 
            return "MISS";
        }

        public void UndoMove()
        {
            if (_previousMoves.Count == 0) return;
            
            var previousMove = _previousMoves[^1];
            _previousMoves.RemoveAt(_previousMoves.Count - 1);
            ChangePlayer();
            
            var board = GetBoardForPlayerWithOpponentShips();
           
            GameBoards[PlayerNoWhoPlays].Board[previousMove.X, previousMove.Y].IsBomb = false;
                
            if (board[previousMove.X, previousMove.Y].IsShip)
            {
                GameBoards[PlayerNoWhoPlays].Board[previousMove.X, previousMove.Y].IsShip = false;
            }
        } 
        
        // players
        public int GetOpponent()
        {
            return PlayerNoWhoPlays == 0 ? 1 : 0;
        }
        
        public void ChangePlayer()
        {
            PlayerNoWhoPlays = PlayerNoWhoPlays == 0 ? 1 : 0;
        }
        
        public string CurrentPlayerToString()
        {
            return PlayerNoWhoPlays == 0 ? "A" : "B";
        }     
        
        public string OpponentPlayerToString()
        {
            return PlayerNoWhoPlays == 0 ? "B" : "A";
        }

        // configs
        public List<ShipConfig> CopyOfShipConfigs(List<ShipConfig> shipConfigs)
        {
            var res = new List<ShipConfig>();
            foreach (var shipConfig in shipConfigs)
            {
                var newShipConfig = new ShipConfig()
                {
                    Name = shipConfig.Name,
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    Quantity = shipConfig.Quantity
                };
                res.Add(newShipConfig);
            }
            return res;
        }
        
        public void SetGameConfig(GameConfig? gameConfig)
        {
            if (gameConfig == null) return;
            Config = gameConfig;
            GameBoards[0].Board = new BoardSquareState[gameConfig.BoardSizeX, gameConfig.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[gameConfig.BoardSizeX, gameConfig.BoardSizeY];
        }
        
        public bool IsHeightOrWidthBetweenMinAndMaxBoardSize(int size)
        {
            return size is >= MinBoardSize and <= MaxBoardSize;
        }

        public bool DoesShipSizeFitToTableSizeInGivenConfig(int size, GameConfig gameConfig)
        {
            return size <= GetSmallerBoardSizeFromGivenConfig(gameConfig);
        }

        public int GetSmallerBoardSizeFromGivenConfig(GameConfig gameConfig)
        {
            return gameConfig.BoardSizeX >= gameConfig.BoardSizeY ? gameConfig.BoardSizeY : gameConfig.BoardSizeX;
        }

        public int CalculateEmptySpaceInGivenConf(GameConfig gameConfig)
        {
            var boardArea = gameConfig.BoardSizeX * gameConfig.BoardSizeY;

            foreach (var shipConfig in gameConfig.ShipConfigs)
            {
                boardArea -= CalculateAreaShipTakesDependingOnTouchRule(shipConfig, gameConfig.EShipTouchRule);
            }
            return boardArea;
        }
        
        public int CalculateQuantityForGivenGameConfigAndShipConfig(GameConfig gameConfig, ShipConfig shipConfig)
        {
            var currentEmptySpace = CalculateEmptySpaceInGivenConf(gameConfig);
            var newShipArea = CalculateAreaShipTakesDependingOnTouchRule(shipConfig, gameConfig.EShipTouchRule);
            return currentEmptySpace / newShipArea;
        }

        private int CalculateAreaShipTakesDependingOnTouchRule(ShipConfig shipConfig, EShipTouchRule touchRule)
        /*
             * * * * *              |     * * *                    | 
             * X X X * - NO TOUCH   |   * X X X * - CORNER TOUCH   |   X X X  - SIDE TOUCH
             * * * * *              |     * * *                    | 
         */
        {
            var shipArea = shipConfig.ShipSizeX * shipConfig.ShipSizeY;
            var result = shipArea;
     
            if (touchRule == EShipTouchRule.CornerTouch)
                result = shipArea + 2 * shipConfig.ShipSizeX + 2 * shipConfig.ShipSizeY;
            if (touchRule == EShipTouchRule.NoTouch)
                result = shipArea + 2 * shipConfig.ShipSizeX + 2 * shipConfig.ShipSizeY + 4;
            
            if (!shipConfig.Equals(null) && shipConfig.Quantity > 0)
            {
                result *= shipConfig.Quantity;
            }
            return result;
        }
        
        // game
        public void ResetGame() 
        {
            GameBoards[0] = new GameBoard();
            GameBoards[1] = new GameBoard();
            GameBoards[0].Board = new BoardSquareState[Config.BoardSizeX,Config.BoardSizeY];
            GameBoards[1].Board = new BoardSquareState[Config.BoardSizeX,Config.BoardSizeY];
            GameBoards[0].Ships = new List<Ship>();
            GameBoards[1].Ships = new List<Ship>();
            PlayerNoWhoPlays = 0;
            _previousMoves = new List<Coordinate>();
        }
        
        public void SetGame(SaveGameDTO? gameDto)
        {
            if (gameDto == null) return;
            SetGameConfig(gameDto.GameConfig);
            PlayerNoWhoPlays = gameDto.CurrentPlayerNo;
            GameBoards = SaveGameDTO.ConvertSaveGameDtosToGameBoards(gameDto.GameBoards);
            _previousMoves = new List<Coordinate>();
        }

        // file saving & loading
        public static void SaveGameConfigToFile(GameConfig gameConfig, string basePath, string? customFileName)
        {
            var filename = GetCorrectConfigName(customFileName);
            var filePathWithFilename = basePath + "/Configs/Custom/" + filename;

            FileDao.SaveConfig(gameConfig, filePathWithFilename);
        }

        public static void SaveGameToFile(BSBrain brain, string basePath, string? customFileName)
        {
            var filename = GetCorrectSaveGameName(customFileName);
            var filePathWithFilename = basePath + "/SavedGames/" + filename;

            var dto = brain.GetBrainSaveGameDto();
            FileDao.SaveGame(dto, filePathWithFilename);
        }
        
        public List<string> LoadCustomGameConfigsFromDirectory(string basePath)
        {
            var filePath = basePath + "/Configs/Custom/";
            return FileDao.LoadFilesFromGivenFolder(filePath);
        }
        
        public List<string> LoadSavedGamesFromDirectory(string basePath)
        {
            var filePath = basePath + "/SavedGames/";
            return FileDao.LoadFilesFromGivenFolder(filePath);
        }
        
        public void LoadFromFileAndSetCustomConfig(string fileName)
        {
            var config = FileDao.LoadConfigFromFilename(fileName);
            SetGameConfig(config);
        }    
        
        public void LoadFromFileAndSetSavedGame(string fileName)
        {
            var gameDto = FileDao.LoadGameFromFilename(fileName);
            SetGame(gameDto);
        }
        
        // db saving & loading
        public static void SaveGameConfigToDb(GameConfig gameConfig, string? customConfName)
        {
            var confName = GetCorrectConfigName(customConfName);
            DbDao.SaveGameConfig(gameConfig, confName);
        }  
        
        public static void SaveGameToDb(BSBrain brain, string? customGameName)
        {
            var gameName = GetCorrectSaveGameName(customGameName);
            DbDao.SaveGame(brain, gameName);
        }
        
        public List<Domain.GameConfig> LoadCustomGameConfigsFromDb()
        {
            return DbDao.LoadCustomGameConfigs();
        }

        public List<Domain.SavedGame> LoadSavedGamesFromDb()
        {
            return DbDao.LoadSavedGames();
        }

        //
        public void SaveGameConfToFile(GameConfig gameConfig, string basePath, string? customFileName)
        {
            var filename = GetCorrectConfigName(customFileName);
            var filePathWithFilename = basePath + "/Configs/Custom/" + filename;

            FileDao.SaveConfig(gameConfig, filePathWithFilename);
        }
        
        public void SaveGameToFile( string basePath, string? customFileName)
        {
            var filename = GetCorrectSaveGameName(customFileName);
            var filePathWithFilename = basePath + "/SavedGames/" + filename;

            var dto = GetBrainSaveGameDto();
            FileDao.SaveGame(dto, filePathWithFilename);
        }
        
        public void SaveGameConfToDb(GameConfig gameConfig, string? customConfName)
        {
            var confName = GetCorrectConfigName(customConfName);
            DbDao.SaveGameConfig(gameConfig, confName);
        }  

        public void SaveGameToDb(string? customGameName)
        {
            var gameName = GetCorrectSaveGameName(customGameName);
            DbDao.SaveGame(this, gameName);
        }
        
        // saving & loading helper methods
        private static string GetCorrectConfigName(string? customConfigName)
        {
            return string.IsNullOrWhiteSpace(customConfigName)
                ? "custom_config_" + DateTime.Now.ToString("yyyy-MM-dd_H-mm-ss_tt") : customConfigName.Replace(" ", "_");
        }        
        
        private static string GetCorrectSaveGameName(string? customGameName)
        {
            return string.IsNullOrWhiteSpace(customGameName) ? 
                "game_" + DateTime.Now.ToString("yyyy-MM-dd_H-mm-ss_tt") : customGameName.Replace(" ", "_");
        }
        
        public SaveGameDTO GetBrainSaveGameDto()
        {
            return new SaveGameDTO(PlayerNoWhoPlays, Config, GameBoards);;
        }
        
        public GameConfig ConvertDomainGameConfigToGameConfig(Domain.GameConfig domainGameConfig)
        {
            return DbDao.ConvertDomainGameConfigToGameConfig(domainGameConfig);
        }
        
        public SaveGameDTO ConvertDomainSavedGameToSaveGameDto(Domain.SavedGame domainSavedGame)
        {
            return DbDao.ConvertDomainSavedGameToSaveGameDto(domainSavedGame);
        }
    }
}
