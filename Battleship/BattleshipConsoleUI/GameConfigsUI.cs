using System;
using System.Collections.Generic;
using BattleShipBrain;

namespace BattleShipConsoleUI
{
    public class GameConfigsUI
    {
        public void GiveBoardSizeInfoToUser(string widthOrHeight, int min, int max)
        {
            Console.WriteLine();
            Console.WriteLine($"Default {widthOrHeight} is 10");
            Console.WriteLine($"Please choose the {widthOrHeight} for your board (in range {min}-{max}): ");
            Console.Write(">");
        }
        
        public void GiveTouchRuleInfoToUser()
        {
            Console.WriteLine();
            Console.WriteLine($"By default boats can not touch each other");
            Console.WriteLine($"Please choose Touch Rule - N (No Touch), C (Corner Touch), S (Side Touch): ");
            Console.Write(">");
        }

        public void ShowCurrentShipConfig(List<ShipConfig> shipConfigs)
        {
            Console.WriteLine();
            if (shipConfigs.Count == 0)
            {
                Console.WriteLine("Currently there are no ship configurations.");
            }
            else
            {
                Console.WriteLine("Current ships for each player: ");
                foreach (var shipConfig in shipConfigs)
                {
                    Console.WriteLine(shipConfig.ToString());
                }
            }
        }
        
        public void ShowShipConfigChoices()
        {
            Console.WriteLine();
            Console.WriteLine("A - add a new ship");
            Console.WriteLine("S - save ships configuration");
            Console.Write(">");
        }

        public void ShowShipSizeInfo(string widthOrHeight, int maxSize)
        {
            Console.WriteLine();
            Console.WriteLine($"Please choose the {widthOrHeight} for your ship (in range 1-{maxSize}): ");
            Console.Write(">");
        }
        
        public void ShowShipQuantityInfo(int maxSize)
        {
            Console.WriteLine();
            Console.WriteLine(maxSize <= 1
                ? $"Please choose the QUANTITY for your ship (only 1 is possible): "
                : $"Please choose the QUANTITY for your ship (in range 1-{maxSize}): ");
            Console.Write(">");
        }
        
        public void ShowShipNameInfo()
        {
            Console.WriteLine();
            Console.WriteLine("Please choose the NAME for your ship(s): ");
            Console.Write(">");
        }
    }
}
