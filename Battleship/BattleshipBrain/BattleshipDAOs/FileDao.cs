using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;

namespace BattleshipBrain.BattleshipDAOs
{
    public class FileDao
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        public static void SaveConfig(GameConfig gameConfig, string filePathWithFilename)
        {
            var jsonStr = JsonSerializer.Serialize(gameConfig, JsonOptions);
            
            var saveFilename = filePathWithFilename + ".json";
            System.IO.File.WriteAllText(saveFilename, jsonStr);
        }
        
        public static void SaveGame(SaveGameDTO saveGameDto, string filePathWithFilename)
        {
            var jsonStr = JsonSerializer.Serialize(saveGameDto, JsonOptions);
            
            var saveFilename = filePathWithFilename + ".json";
            System.IO.File.WriteAllText(saveFilename, jsonStr);
        }
        
        public static List<string> LoadFilesFromGivenFolder(string fileFolderPath)
        {
            return System.IO.Directory.EnumerateFiles(fileFolderPath, "*.json").ToList();
        }

        public static GameConfig? LoadConfigFromFilename(string fileName)
        {
            var configText = LoadFileFromFilename(fileName);
            var config = JsonSerializer.Deserialize<GameConfig>(configText);
            return config;
        }   
        
        public static SaveGameDTO? LoadGameFromFilename(string fileName)
        {
            var gameText = LoadFileFromFilename(fileName);
            var game = JsonSerializer.Deserialize<SaveGameDTO>(gameText, JsonOptions);
            return game;
        }

        private static string LoadFileFromFilename(string fileName)
        {
            return System.IO.File.ReadAllText(fileName);
        }
    }
}
