using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BattleShipBrain
{
    public class Ship
    {
        [JsonInclude]
        public string Name { get; private set; } = null!;
        
        [JsonInclude]
        public readonly List<Coordinate> Coordinates = new List<Coordinate>();
        
        public Ship(string name, Coordinate startPosition, int width, int height)
        {
            Name = name;
            var startPlusWidth = startPosition.X + width;
            var startPlusHeight = startPosition.Y + height;

            int xStart;
            int xEnd;
            int yStart;
            int yEnd;

            if (startPosition.X < startPlusWidth)
            {
                xStart = startPosition.X;
                xEnd = startPlusWidth;
            }
            else
            {
                xStart = startPlusWidth + 1;
                xEnd = startPosition.X + 1;
            }

            if (startPosition.Y < startPlusHeight)
            {
                yStart = startPosition.Y;
                yEnd = startPlusHeight;
            }
            else
            {
                yStart = startPlusHeight + 1;
                yEnd = startPosition.Y + 1;
            }

            for (var x = xStart; x < xEnd; x++)
            {
                for (var y = yStart; y < yEnd; y++)
                {
                    Coordinates.Add(new Coordinate(){X = x, Y = y});
                }
            }
        }
        
        [JsonConstructor]
        public Ship(string name, List<Coordinate> coordinates)
        {
            Name = name;
            Coordinates = coordinates;
        }
        
        public int GetShipSize() => Coordinates.Count;
        
        public int GetShipDamageCount(BoardSquareState[,] board) =>
            // count all the items that match the predicate
            Coordinates.Count(coordinate => board[coordinate.X, coordinate.Y].IsBomb);

        public bool IsShipSunk(BoardSquareState[,] board) =>
            // returns true when all the items in the list match predicate
            Coordinates.All(coordinate => board[coordinate.X, coordinate.Y].IsBomb);
    }
}
