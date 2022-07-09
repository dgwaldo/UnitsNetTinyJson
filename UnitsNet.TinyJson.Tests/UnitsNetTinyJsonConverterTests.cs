using Newtonsoft.Json;
using TheCodeProject.Models.TestModel;
using UnitsNet.Units;

namespace UnitsNet.TinyJson.Tests
{
    [TestClass]
    public class UnitsNetTinyJsonConverterTests
    {

        private readonly JsonSerializerSettings serializerOpts = new()
        {
            Converters = new[] { new UnitsNetTinyJsonConverter() }
        };

        [TestMethod]
        public void Unit_Should_Serialize_To_Tiny_Json()
        {
            //Arrange
            var unitValueFt = new Length(100, LengthUnit.Foot);

            //Act
            var result = JsonConvert.SerializeObject(unitValueFt, serializerOpts);

            //Assert
            Assert.AreEqual("\"100|ft\"", result);
        }

        [TestMethod]
        public void UnitCollection_Should_Serialize_To_Tiny_JsonArray()
        {
            //Arrange
            var unitValuesFt = new[] {
                new Length(100, LengthUnit.Foot),
                new Length(120, LengthUnit.Foot),
                new Length(150, LengthUnit.Foot)
            };

            //Act
            var result = JsonConvert.SerializeObject(unitValuesFt, serializerOpts);

            //Assert
            Assert.AreEqual(@"[""100|ft"",""120|ft"",""150|ft""]", result);
        }

        [TestMethod]
        public void Object_With_UnitProperties_Should_Serialize_To_Tiny_Json()
        {
            //Arrange
            var testModel = new TestDataModel 
            {
                Distances = new[] {
                    new Length(5280, LengthUnit.Foot),
                    new Length(10560, LengthUnit.Foot)
                },
                MilesOfRoad = new Length(10, LengthUnit.Mile),
                RoadName = "Highway 1"
            };

            //Act
            var result = JsonConvert.SerializeObject(testModel, serializerOpts);

            //Assert
            var expectedJson = "{\"Distances\":[\"5280|ft\",\"10560|ft\"],\"MilesOfRoad\":\"10|mi\",\"RoadName\":\"Highway 1\"}";
            Assert.AreEqual(expectedJson, result);
        }


        [TestMethod]
        public void Value_Unit_Round_Decimals()
        {
            //Arrange
            var unitValueFt = new Length(100.99999, LengthUnit.Foot);

            //Act
            var result = JsonConvert.SerializeObject(unitValueFt, serializerOpts);

            //Assert
            Assert.AreEqual("\"101|ft\"", result);
        }

        [TestMethod]
        public void Value_Unit_Read()
        {
            //Arrange
            var strValue = "\"100|ft\"";

            //Act
            var result = JsonConvert.DeserializeObject<Length>(strValue, serializerOpts);

            //Assert
            Assert.AreEqual("100 ft", result.ToString());
        }

        [TestMethod]
        public void Value_Unit_Array_Read()
        {
            //Arrange
            var strValue = @"[""100|ft"",""120|ft"",""150|ft""]";

            //Act
            var result = JsonConvert.DeserializeObject<IEnumerable<Length>>(strValue, serializerOpts);

            //Assert
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Length>));
            Assert.AreEqual(result.First().Value, 100);
            Assert.AreEqual(result.First().Unit, LengthUnit.Foot);
        }

        [TestMethod]
        public void NullableUnit_WhenEmptyJson_ReturnsNull()
        {
            //Arrange
            var strValue = "{}";

            //Act
            var result = JsonConvert.DeserializeObject<Length?>(strValue, serializerOpts);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NullableUnit_WhenNull_ReturnsNull()
        {
            //Arrange
            var strValue = "null";

            //Act
            var result = JsonConvert.DeserializeObject<Length?>(strValue, serializerOpts);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NullableUnit_WhenJsonArrayIsEmpty_ReturnsNull()
        {
            //Arrange
            var strValue = "[]";

            //Act
            var result = JsonConvert.DeserializeObject<Length?>(strValue, serializerOpts);

            //Assert
            Assert.IsNull(result);
        }

    }
}