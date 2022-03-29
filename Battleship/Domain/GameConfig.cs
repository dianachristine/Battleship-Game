using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameConfig
    {
        public int GameConfigId { get; set; }

        [MaxLength(150)]
        [Display(Name = "Settings Name")]
        public string GameConfigName { get; set; }  = default!;
        
        [Range(5, 20)]
        [Display(Name = "Board Size X")]
        public int BoardSizeX { get; set; }
        
        [Range(5, 20)]
        [Display(Name = "Board Size Y")]
        public int BoardSizeY { get; set; }
        
        [Range(0, 2)]
        [Display(Name = "Touch Rule")]
        public int EShipTouchRule { get; set; }
        
        public ICollection<ShipConfig> ShipConfigs { get; set; }  = default!;
        
        public ICollection<SavedGame> SavedGames { get; set; }  = default!;
    }
}
