using System;
using NUnit.Framework;
using PutridParrot.JsonUtilities;
using Newtonsoft.Json.Linq;

namespace Tests.PutridParrot.JsonUtilities
{
    public class JsonMapperTests
    {
        private const string Sample1 =
            "{firstName: \"Scooby\", lastName: \"Doo\",\"friend\": { \"firstName\": \"Shaggy\" }}";

        [Test]
        public void MapSimpleProperty_UsingJPath_ExpectSameValueAsSource()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.firstName")
                .Create();

            var firstName = jsonObject.SelectToken("$.firstName")?.Value<string>();
            Assert.AreEqual("Scooby", firstName);
        }

        [Test]
        public void MapSimpleProperty_UsingJPathWithPrefixDollarDot_ExpectSameValueAsSource()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("firstName")
                .Create();

            var firstName = jsonObject.SelectToken("$.firstName")?.Value<string>();
            Assert.AreEqual("Scooby", firstName);
        }

        [Test]
        public void MapSimpleProperty_UsingJPath_ExpectNonMappedToNotExist()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.firstName")
                .Create();

            Assert.IsNull(jsonObject.SelectToken("$.lastName"));
        }

        [Test]
        public void MapSimpleProperty_AddIfNonExistent_ExpectPropertyToExistWithNullValue()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.age", new JsonMapSettings
                {
                    AddPathIfMissing = true
                })
                .Create();

            Assert.IsNotNull(jsonObject.ContainsKey("$.age"));
            Assert.IsNull(jsonObject.SelectToken("$.age")?.Value<string>());
        }

        [Test]
        public void MapSimpleProperty_UsingJPath_ChangeName_ExpectSameValueAsSource()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.firstName", "$.nom")
                .Create();

            var firstName = jsonObject.SelectToken("$.nom")?.Value<string>();
            Assert.AreEqual("Scooby", firstName);
        }

        [Test, Description("Maps a composite property to a composite property with the same path/name")]
        public void MapCompositeProperty_WithoutDestination_AsSource()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.friend.firstName", new JsonMapSettings
                {
                    CompositeToSimple = false
                })
                .Create();


            var friendFirstName = jsonObject.SelectToken("$.friend.firstName")?.Value<string>();
            Assert.AreEqual("Shaggy", friendFirstName);
        }

        [Test, Description("Maps a composite property to a composite property with the same path/name")]
        public void MapCompositeProperty_WithoutDestination_ToNewProperty()
        {
            var temp = JObject.Parse(Sample1);
            var test1 = temp.SelectToken("$.friend.firstName")?.Value<string>();

            var jsonObject = new JsonMapper(Sample1)
                .Map("$.friend.firstName", "$.bestFriend.FirstName", new JsonMapSettings
                {
                    CompositeToSimple = false
                })
                .Create();


            var friendFirstName = jsonObject.SelectToken("$.bestFriend.FirstName")?.Value<string>();
            Assert.AreEqual("Shaggy", friendFirstName);
        }


        [Test, Description("Maps a composite property to a simple property")]
        public void MapCompositeProperty_WithNonDotDestination_ExpectPropertyWithoutDestinationName()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.friend.firstName", "$.bestFriend")
                .Create();

            var friendFirstName = jsonObject.SelectToken("$.bestFriend")?.Value<string>();
            Assert.AreEqual("Shaggy", friendFirstName);
        }

        [Test, Description("Maps a composite property to a simple property with different names")]
        public void MapCompositeProperty_WithDotDestination_ExpectPropertyWithoutDestinationNameWithoutDot()
        {
            var jsonObject = new JsonMapper(Sample1)
                .Map("$.friend.firstName", "$.best.Friend")
                .Create();

            var friendFirstName = jsonObject.SelectToken("$.bestFriend")?.Value<string>();
            Assert.AreEqual("Shaggy", friendFirstName);
        }
    }
}
