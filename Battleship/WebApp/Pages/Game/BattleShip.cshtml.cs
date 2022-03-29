using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameConfig = BattleShipBrain.GameConfig;

namespace WebAppTest.Pages.Game
{
    public class GameModel : PageModel
    {
        public static BSBrain Brain { get; set; } = default!;
        public bool ShowPlayerBoard { get; set; } = true;
        public bool GameIsOver { get; set; } = false;
        public bool GameIsSaved { get; set; } = false;
        
        public string? ShipPlacingErrorMessage { get; set; }
        public string? GameErrorMessage { get; set; }

        [BindProperty] public string? GameFileName { get; set; }
        [BindProperty] public string? SettingsFileName { get; set; }
        [BindProperty] public string? EntityId { get; set; }
        [BindProperty] public string? SettingsEntityId { get; set; }
        
        [BindProperty] public string? RandomShips { get; set; }
        [BindProperty] public string? ManualShips { get; set; }
        
        [BindProperty] public string? OpponentWantsToContinue { get; set; }

        private readonly DAL.ApplicationDbContext _context;

        public GameModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public void OnGet(int x, int y)
        {
            GameErrorMessage = null;
            if (x == -1 & y == -1)
            {
                Brain = new BSBrain(new GameConfig());
                Brain.ResetGame();
                GameIsSaved = false;
            }
            else if (x == -1 & y == -2)
            {
                GameIsSaved = false;
            }
            else if (x == -0 & y == -1)
            {
                Brain.UndoMove();
                GameIsSaved = false;
                Brain.TempShowContinueButton = false;
            }
            else if (x == -1 & y == -0)
            {
                if (GameIsSaved == false)
                {
                    Brain.SaveGameToDb(null);
                    Brain.SaveGameToFile("C:/Users/chris/RiderProjects/icd0008-2021f/Project", null);
                    GameIsSaved = true;
                }
            }
            else
            {
                if (Brain.TempShipsLeftToPlace.Count > 0)
                {
                    
                    var shipCoordinate = new Coordinate() {X = x, Y = y};

                    if (Brain.TempShipsLeftToPlace[0].ShipSizeX == 1 && Brain.TempShipsLeftToPlace[0].ShipSizeY == 1)
                    {
                        if (Brain.IsShipSafeToPlaceOnOpponentBoard(shipCoordinate, shipCoordinate, shipCoordinate))
                        {
                            Brain.PlaceAShipOnOpponentBoard(Brain.TempShipsLeftToPlace[0].Name, 
                                shipCoordinate, shipCoordinate, shipCoordinate);
                            RemoveAlreadyPlacedShipConfig();
                        }
                        else
                        {
                            ShipPlacingErrorMessage = "Cannot place ship on other ship or its territory!";
                        }
                    }
                    else
                    {
                        if (Brain.TempManualShipPlacementShipStartPos == null)
                        {
                            Brain.TempManualShipPlacementShipStartPos = shipCoordinate;
                        }
                        else if (Brain.TempManualShipPlacementShipWidthPos == null)
                        {
                            if (Brain.IsShipWidthCorrect(Brain.TempManualShipPlacementShipStartPos.Value,shipCoordinate, Brain.TempShipsLeftToPlace[0]))
                            {
                                Brain.TempManualShipPlacementShipWidthPos = shipCoordinate;
                                
                                var validHeight = Brain.GetValidShipHeightBasedOnStartAndWidthPos(Brain.TempManualShipPlacementShipStartPos.Value, 
                                    shipCoordinate, Brain.TempShipsLeftToPlace[0]);
                                if (validHeight == 1)
                                {
                                    if (Brain.IsShipSafeToPlaceOnOpponentBoard(
                                        Brain.TempManualShipPlacementShipStartPos.Value,
                                        Brain.TempManualShipPlacementShipWidthPos.Value, shipCoordinate))
                                    {
                                        Brain.PlaceAShipOnOpponentBoard(Brain.TempShipsLeftToPlace[0].Name,
                                            Brain.TempManualShipPlacementShipStartPos.Value,
                                            Brain.TempManualShipPlacementShipWidthPos.Value, 
                                            Brain.TempManualShipPlacementShipStartPos.Value);
                                        RemoveAlreadyPlacedShipConfig();
                                        Brain.TempManualShipPlacementShipStartPos = null;
                                        Brain.TempManualShipPlacementShipWidthPos = null;
                                    }
                                    else
                                    {
                                        Brain.TempManualShipPlacementShipStartPos = null;
                                        Brain.TempManualShipPlacementShipWidthPos = null;
                                        ShipPlacingErrorMessage = "Cannot place ship on other ship or its territory!";
                                    }
                                }
                            }
                            else
                            {
                                ShipPlacingErrorMessage = Brain.TempShipsLeftToPlace[0].ShipSizeX == Brain.TempShipsLeftToPlace[0].ShipSizeY
                                    ? $"Given width is not correct. Width must be {Brain.TempShipsLeftToPlace[0].ShipSizeX} cell(s) on row {Brain.TempManualShipPlacementShipStartPos.Value.Y + 1}."
                                    : $"Given width is not correct. Width must be {Brain.TempShipsLeftToPlace[0].ShipSizeX} or {Brain.TempShipsLeftToPlace[0].ShipSizeY} cell(s) on row { Brain.TempManualShipPlacementShipStartPos.Value.Y + 1}.";
                            }
                        }
                        else
                        {
                            var validHeight = Brain.GetValidShipHeightBasedOnStartAndWidthPos(Brain.TempManualShipPlacementShipStartPos.Value, 
                                Brain.TempManualShipPlacementShipWidthPos.Value, Brain.TempShipsLeftToPlace[0]);

                            if (Brain.IsShipHeightCorrect(Brain.TempManualShipPlacementShipStartPos.Value,
                                shipCoordinate, validHeight))
                            {
                                if (Brain.IsShipSafeToPlaceOnOpponentBoard(Brain.TempManualShipPlacementShipStartPos.Value, 
                                    Brain.TempManualShipPlacementShipWidthPos.Value, shipCoordinate))
                                {
                                    Brain.PlaceAShipOnOpponentBoard(Brain.TempShipsLeftToPlace[0].Name,
                                        Brain.TempManualShipPlacementShipStartPos.Value,
                                        Brain.TempManualShipPlacementShipWidthPos.Value, shipCoordinate);
                                    RemoveAlreadyPlacedShipConfig();
                                    Brain.TempManualShipPlacementShipStartPos = null;
                                    Brain.TempManualShipPlacementShipWidthPos = null;
                                }
                                else
                                {
                                    Brain.TempManualShipPlacementShipStartPos = null;
                                    Brain.TempManualShipPlacementShipWidthPos = null;
                                    ShipPlacingErrorMessage = "Cannot place ship on other ship or its territory!";
                                }
                            }
                            else
                            {
                                ShipPlacingErrorMessage =
                                    $"Given height is not correct. Height must be {validHeight} cell(s) on column {Brain.GetColumnLetterFromIndex(Brain.TempManualShipPlacementShipStartPos.Value.X)}.";
                            }
                        }
                    }

                    if (Brain.TempShipsLeftToPlace.Count == 0)
                    {
                        Brain.ChangePlayer();
                    }
                }
                else
                {
                    if (Brain.IsCellWithoutBomb(x, y))
                    {
                        Brain.MakeAMove(x, y);
                        GameIsSaved = false;
                        if (Brain.IsLastMoveWinningMove())
                        {
                            GameIsOver = true;
                        }
                        else
                        {
                            Brain.TempShowContinueButton = true;
                        }
                    }
                    else
                    {
                        GameErrorMessage = "Cannot hit same place twice!";
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ShipPlacingErrorMessage = null;
            if (GameFileName != null)
            {
                Brain = new BSBrain(new GameConfig());
                Brain.LoadFromFileAndSetSavedGame(GameFileName);
            }

            if (EntityId != null)
            {
                Brain = new BSBrain(new GameConfig());
                var savedGame = await _context.SavedGames
                    .Include(table => table.GameBoards)
                    .Include(table => table.GameConfig)
                    .Include("GameConfig.ShipConfigs")
                    .Include("GameBoards.Ships")
                    .Include("GameBoards.Ships.Coordinates")
                    .FirstOrDefaultAsync(m => m.SavedGameId == int.Parse(EntityId));

                var selectedGame = Brain.ConvertDomainSavedGameToSaveGameDto(savedGame);
                Brain.SetGame(selectedGame);
            }
            
            if (SettingsFileName != null)
            {
                Brain = new BSBrain(new GameConfig());
                Brain.LoadFromFileAndSetCustomConfig(SettingsFileName);
                return Redirect("/Game/BattleShip?x=-1&y=-2");
            }

            if (SettingsEntityId != null)
            {
                Brain = new BSBrain(new GameConfig());
                var gameConfig = await _context.GameConfigs
                    .Include(table => table.ShipConfigs)
                    .FirstOrDefaultAsync(m => m.GameConfigId == int.Parse(SettingsEntityId));

                var selectedConf = Brain.ConvertDomainGameConfigToGameConfig(gameConfig);
                Brain.SetGameConfig(selectedConf);
                return Redirect("/Game/BattleShip?x=-1&y=-2");
            }

            if (RandomShips != null)
            {
                Brain.PlaceShipsRandomlyForAPlayer(); // places ships on opponent board
                Brain.ChangePlayer();
                return Redirect("/Game/BattleShip?x=-1&y=-2");
            }
            
            if (ManualShips != null)
            {
                Brain.TempShipsLeftToPlace = Brain.CopyOfShipConfigs(Brain.Config.ShipConfigs);
            }

            if (OpponentWantsToContinue != null)
            {
                Brain.TempShowContinueButton = false;
                Brain.ChangePlayer();
                return Redirect("/Game/BattleShip?x=-1&y=-2");
            }

            return Page();
        }

        public void RemoveAlreadyPlacedShipConfig()
        {
            if (Brain.TempShipsLeftToPlace[0].Quantity == 1)
            {
                Brain.TempShipsLeftToPlace.RemoveAt(0);
            }
            else
            {
                Brain.TempShipsLeftToPlace[0].Quantity -= 1;
            }
        }
    }
}
