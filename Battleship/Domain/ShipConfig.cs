using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ShipConfig
    {
        public int ShipConfigId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = default!;
        
        [Display(Name = "Ship Size X")]
        [Range(1, 20)]        
        public int ShipSizeX { get; set; }
        
        [Display(Name = "Ship Size Y")]
        [Range(1, 20)]
        public int ShipSizeY { get; set; } 
        
        [Range(1, 400)]
        public int Quantity { get; set; } 
        
        public int GameConfigId { get; set; }
        public GameConfig GameConfig { get; set; } = default!;
    }
}
