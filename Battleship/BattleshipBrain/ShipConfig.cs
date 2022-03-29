using System.Text.Json;

namespace BattleShipBrain
{
    public class ShipConfig
    {
        public string Name { get; set; } = null!;

        public int Quantity { get; set; }
        
        public int ShipSizeY { get; set; } 
        public int ShipSizeX { get; set; }
        
        public override string ToString()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, jsonOptions);
        }
    }
}
