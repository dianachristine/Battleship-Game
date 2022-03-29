using System.Collections.Generic;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShipConfig = Domain.ShipConfig;

namespace WebAppTest.Pages.GameConfigs.DB
{
    public class ShipConfigsDb : PageModel
    {
        [BindProperty]
        public ShipConfig ShipConfig { get; set; } = default!;
        
        public string? ShipConfigErrorMessage { get; set; }

        [BindProperty(Name = "ShipConfig.Name")]
        public string? ShipConfigName { get; set; }
        
        [BindProperty(Name = "ShipConfig.ShipSizeX")]
        public string? ShipSizeX { get; set; }
        
        [BindProperty(Name = "ShipConfig.ShipSizeY")]
        public string? ShipSizeY { get; set; }
        
        [BindProperty(Name = "ShipConfig.Quantity")]
        public string? ShipQuantity { get; set; }


        [BindProperty(Name = "GameConfig.GameConfigName")]
        public string? GameConfigName { get; set; }

        [BindProperty(Name = "GameConfig.BoardSizeX")]
        public string? BoardSizeX { get; set; }

        [BindProperty(Name = "GameConfig.BoardSizeY")]
        public string? BoardSizeY { get; set; }

        [BindProperty(Name = "GameConfig.EShipTouchRule")]
        public string? EShipTouchRule { get; set; }
        
        
        [BindProperty] public string? SaveSettings { get; set; }
        public static BSBrain Brain { get; set; } = new BSBrain(new BattleShipBrain.GameConfig());

        public async Task<IActionResult> OnPostAsync()
        {
            if (IsInitialPostRequest())
            {
                Brain.TempConfigName = GameConfigName!;
                
                Brain.TempConfig.BoardSizeX= int.Parse(BoardSizeX!);
                Brain.TempConfig.BoardSizeY = int.Parse(BoardSizeY!);
                Brain.TempConfig.EShipTouchRule = (BattleShipBrain.EShipTouchRule) int.Parse(EShipTouchRule!);
            }

            if (IsShipConfigPostRequest())
            { 
                var copyOfGameConfig =  Brain.TempConfig.CreateCopyOfGameConfig();
                var shipConfig = new BattleShipBrain.ShipConfig()
                {
                    Name = ShipConfigName!,
                    ShipSizeX = int.Parse(ShipSizeX!),
                    ShipSizeY = int.Parse(ShipSizeY!),
                    Quantity = int.Parse(ShipQuantity!)
                };
                
                var maxQuantity = Brain.CalculateQuantityForGivenGameConfigAndShipConfig(copyOfGameConfig, shipConfig);

                if (Brain.CalculateEmptySpaceInGivenConf(copyOfGameConfig) > 0)
                {
                    if (Brain.DoesShipSizeFitToTableSizeInGivenConfig(int.Parse(ShipSizeX!), copyOfGameConfig) && 
                        Brain.DoesShipSizeFitToTableSizeInGivenConfig(int.Parse(ShipSizeY!), copyOfGameConfig))
                    {
                        if (int.Parse(ShipQuantity!) <= maxQuantity)
                        {
                            Brain.TempConfig.ShipConfigs.Add(shipConfig);
                        }
                        else
                        {
                            ShipConfigErrorMessage = $"Too many ships - doesn't fit to the board ({Brain.TempConfig.BoardSizeX}x{Brain.TempConfig.BoardSizeY})!";
                        }
                    }
                    else
                    {
                        ShipConfigErrorMessage = $"Given size does not fit to the board ({Brain.TempConfig.BoardSizeX}x{Brain.TempConfig.BoardSizeY})!";
                    }
                }
                else
                {
                    ShipConfigErrorMessage = "There is no empty space on board!";
                }
            }
            
            if (IsSaveChangesPostRequest())
            {
                if ( Brain.TempConfig.ShipConfigs.Count > 0)
                {
                    Brain.SaveGameConfToDb( Brain.TempConfig, Brain.TempConfigName);
                    Brain.TempConfig = new BattleShipBrain.GameConfig() {ShipConfigs = new List<BattleShipBrain.ShipConfig>()};
                    Brain.TempConfigName = "";
                    return Redirect("./Index");
                }
                else
                {
                    ShipConfigErrorMessage = "Cannot create game configuration without ships!";
                }
            }
            return Page();
        }

        private bool IsInitialPostRequest()
        {
            return GameConfigName != null && BoardSizeX != null && BoardSizeY != null && EShipTouchRule != null;
        }

        private bool IsShipConfigPostRequest()
        {
            return ShipConfigName != null && ShipSizeX != null && ShipSizeY != null && ShipQuantity != null;
        }

        private bool IsSaveChangesPostRequest()
        {
            return SaveSettings != null;
        }
    }
}
