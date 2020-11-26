using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PutridParrot.JsonUtilities;

namespace Tests.PutridParrot.JsonUtilities
{
    public class JsonExtensionTests
    {
        private const string Sample1 =
            "{firstName: \"Scooby\", lastName: \"Doo\",\"friend\": { \"firstName\": \"Shaggy\" }}";

        [Test]
        public void Copy_TopLevelProperties()
        {
            var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("firstName", "lastName");

            Assert.AreEqual("Scooby", copy.SelectToken("$.firstName")?.Value<string>());
            Assert.AreEqual("Doo", copy.SelectToken("$.lastName")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.friend"));
        }

        [Test]
        public void Copy_CompliexProperties()
        {
            var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("friend");

            Assert.AreEqual("Shaggy", copy.SelectToken("$.friend.firstName")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.firstName"));
            Assert.IsNull(copy.SelectToken("$.lastName"));
        }

        [Test, Ignore("Ideas under development")]
        public void Copy_NestedProperties()
        {
            var jo = JObject.Parse("{\"friend.firstName\":\"Shaggy\"}");

            //var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("friends.firstName");

            Assert.AreEqual("Shaggy", copy.SelectToken("$.['friend.firstName']")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.firstName"));
            Assert.IsNull(copy.SelectToken("$.lastName"));
        }
    }
}