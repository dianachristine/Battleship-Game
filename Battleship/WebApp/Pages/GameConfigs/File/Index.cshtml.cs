using System.Collections.Generic;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages_GameConfigs
{
    public class FileIndexModel : PageModel
    {
        public static BSBrain Brain { get; set; } = new BSBrain(new BattleShipBrain.GameConfig());

        public List<string> LoadedConfigs { get;set; } = new List<string>();
        public List<string> LoadedConfigsNames { get;set; } = new List<string>();

        public void OnGet()
        {
            LoadedConfigs = Brain.LoadCustomGameConfigsFromDirectory("C:/Users/chris/RiderProjects/icd0008-2021f/Project");
            for (var i = 0; i < LoadedConfigs.Count; i++)
            {
                var file = LoadedConfigs[i].Split("/")[^1];
                LoadedConfigsNames.Add( file);
            }
        }
    }
}
