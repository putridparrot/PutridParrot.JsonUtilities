using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PutridParrot.JsonUtilities;

namespace Tests.PutridParrot.JsonUtilities
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class TestsSamples
    {
        [Test]
        public void Sample1_BuildingUpObjects()
        {
            var jo = new JObject()
                .AddOrUpdate("name", "Scooby Doo")
                .AddOrUpdate("age", 23)
                .AddOrUpdate("address",
                    new JObject()
                        .AddOrUpdate("line1", "Freaky Town")
                        .AddOrUpdate("line2", "Freaksville"));
            
            Assert.AreEqual("Scooby Doo", jo.Get<string>("name"));
            Assert.AreEqual(23, jo.Get<int>("age"));
            Assert.AreEqual("Freaky Town", jo.Get<string>("address.line1"));
            Assert.AreEqual("Freaksville", jo.Get<string>("address.line2"));
        }

        [Test]
        public void Sample1_ConditionalChange()
        {
            var jo = new JObject()
                .AddOrUpdate("name", "Scooby Doo")
                .AddOrUpdate("age", 23)
                .AddOrUpdate("address",
                    new JObject()
                        .AddOrUpdate("line1", "Freaky Town")
                        .AddOrUpdate("line2", "Freaksville"))
                .IfExists("name", o =>
                {
                    o.AddOrUpdate("name", "Scrappy Doo");
                })
                .IfExists("age", (o, token) =>
                {
                    var currentAge = token.Value<int>();
                    o.Update("age", currentAge - 10);
                });

            Assert.AreEqual("Scrappy Doo", jo.Get<string>("name"));
            Assert.AreEqual(13, jo.Get<int>("age"));
            Assert.AreEqual("Freaky Town", jo.Get<string>("address.line1"));
            Assert.AreEqual("Freaksville", jo.Get<string>("address.line2"));
        }
    }
}
