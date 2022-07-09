using UnitsNet;

namespace TinyJson.WebApi.Models
{
    public class TestDataModel
    {
        public IEnumerable<Length> Distances { get; set; }
        public Length MilesOfRoad { get; set; }
        public string RoadName { get; set; }

    }
}
