using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PutridParrot.JsonUtilities;

namespace Tests.PutridParrot.JsonUtilities
{
    public class JsonExtensionTests
    {
        private const string Sample1 =
            "{firstName: \"Scooby\", lastName: \"Doo\",\"friends\": { \"firstName\": \"Shaggy\" }}";

        [Test]
        public void Copy_TopLevelProperties()
        {
            var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("firstName", "lastName");

            Assert.AreEqual("Scooby", copy.SelectToken("$.firstName")?.Value<string>());
            Assert.AreEqual("Doo", copy.SelectToken("$.lastName")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.friends"));
        }

        [Test]
        public void Copy_CompliexProperties()
        {
            var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("friends");

            Assert.AreEqual("Shaggy", copy.SelectToken("$.friends.firstName")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.firstName"));
            Assert.IsNull(copy.SelectToken("$.lastName"));
        }

        [Test, Ignore("Ideas under development")]
        public void Copy_NestedProperties()
        {
            var jo = JObject.Parse("{\"friends.firstName\":\"Shaggy\"}");

            //var jo = JObject.Parse(Sample1);

            var copy = jo.Copy("friends.firstName");

            Assert.AreEqual("Shaggy", copy.SelectToken("$.['friends.firstName']")?.Value<string>());
            Assert.IsNull(copy.SelectToken("$.firstName"));
            Assert.IsNull(copy.SelectToken("$.lastName"));
        }
    }
}