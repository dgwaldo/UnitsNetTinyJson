using UnitsNet;

namespace TheCodeProject.Models.TestModel
{
    public class TestDataModel
    {
        public IEnumerable<Length> Distances { get; set; }
        public Length MilesOfRoad { get; set; }
        public string RoadName {get;set;}

    }
}
