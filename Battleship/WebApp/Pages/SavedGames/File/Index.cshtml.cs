using System.Collections.Generic;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages_SavedGames
{
    public class FileIndexModel : PageModel
    {
        public static BSBrain Brain { get; set; } = new BSBrain(new BattleShipBrain.GameConfig());

        public List<string> LoadedGames { get;set; } = new List<string>();
        public List<string> LoadedGamesNames { get;set; } = new List<string>();

        public void OnGet()
        {
            LoadedGames = Brain.LoadSavedGamesFromDirectory("C:/Users/chris/RiderProjects/icd0008-2021f/Project");
            for (var i = 0; i < LoadedGames.Count; i++)
            {
                var file = LoadedGames[i].Split("/")[^1];
                LoadedGamesNames.Add( file);
            }
        }
    }
}
