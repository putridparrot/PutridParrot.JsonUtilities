//using Newtonsoft.Json.Linq;
//using NUnit.Framework;
//using PutridParrot.JsonUtilities;
//using System;

//namespace Tests.PutridParrot.JsonUtilities
//{
//    public class JsonExtensionTests
//    {
//        private const string Sample1 =
//            "{firstName: \"Scooby\", lastName: \"Doo\",\"friend\": { \"firstName\": \"Shaggy\" }}";

//        [Test]
//        public void Copy_TopLevelProperties()
//        {
//            var jo = JObject.Parse(Sample1);

//            var copy = jo.Clone("firstName", "lastName");

//            Assert.AreEqual("Scooby", copy.SelectToken("$.firstName")?.Value<string>());
//            Assert.AreEqual("Doo", copy.SelectToken("$.lastName")?.Value<string>());
//            Assert.IsNull(copy.SelectToken("$.friend"));
//        }

//        [Test]
//        public void Copy_ComplexProperties()
//        {
//            var jo = JObject.Parse(Sample1);

//            var copy = jo.Clone("friend");

//            Assert.AreEqual("Shaggy", copy.SelectToken("$.friend.firstName")?.Value<string>());
//            Assert.IsNull(copy.SelectToken("$.firstName"));
//            Assert.IsNull(copy.SelectToken("$.lastName"));
//        }

//        [Test, Ignore("Ideas under development")]
//        public void Copy_NestedProperties()
//        {
//            var jo = JObject.Parse("{\"friend.firstName\":\"Shaggy\"}");

//            //var jo = JObject.Parse(Sample1);

//            var copy = jo.Clone("friends.firstName");

//            Assert.AreEqual("Shaggy", copy.SelectToken("$.['friend.firstName']")?.Value<string>());
//            Assert.IsNull(copy.SelectToken("$.firstName"));
//            Assert.IsNull(copy.SelectToken("$.lastName"));
//        }

//        [Test]
//        public void ToString_MissingProperty_ExpectEmptyString()
//        {
//            var jo = JObject.Parse(Sample1);

//            Assert.AreEqual(string.Empty, jo.ToString("unknownProperty"));
//        }

//        [Test]
//        public void ToString_SingleProperty_ExpectStringWithoutComma()
//        {
//            var jo = JObject.Parse(Sample1);

//            Assert.AreEqual("firstName:Scooby", jo.ToString("firstName"));
//        }

//        [Test]
//        public void ToString_MultipleProperties_ExpectCommaSeperatedValues()
//        {
//            var jo = JObject.Parse(Sample1);

//            Assert.AreEqual("firstName:Scooby, lastName:Doo", jo.ToString("firstName", "lastName"));
//        }

//        [Test]
//        public void TryAddOrUpdate_IfPropertyDoesNotExist_ExpectPropertyAdded()
//        {
//            var jo = new JObject();

//            Assert.IsTrue(jo.TryAddOrUpdate("firstName", "Scooby"));
//            Assert.AreEqual("Scooby", jo.SelectToken("$.firstName")?.Value<string>());
//        }

//        [Test]
//        public void TryAddOrUpdate_IfPropertyExists_ExpectPropertyUpdated()
//        {
//            var jo = JObject.Parse(Sample1);

//            Assert.AreEqual("Scooby", jo.SelectToken("$.firstName")?.Value<string>());
//            Assert.IsTrue(jo.TryAddOrUpdate("firstName", "Daffy"));
//            Assert.AreEqual("Daffy", jo.SelectToken("$.firstName")?.Value<string>());
//        }

//        [Test]
//        public void AddOrUpdate_SimplePropertyWhenNonExists_ExpectNewProperty()
//        {
//            var jo = JObject.Parse(Sample1);

//            jo.AddOrUpdate("age", 21);

//            Assert.AreEqual(21, jo.SelectToken("$.age")?.Value<int>());
//        }

//        [Test]
//        public void AddOrUpdate_ComplexPropertyWhenNonExists_ExpectNewProperty()
//        {
//            var jo = JObject.Parse(Sample1);

//            jo.AddOrUpdate("private.date.age", 21, new JsonMapSettings { CompositeToSimple = false });

//            Assert.AreEqual(21, jo.SelectToken("$.private.date.age")?.Value<int>());
//        }

//        [Test]
//        public void MapSimpleProperty_UsingJPath_ExpectSameValueAsSource()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.firstName", newObject);

//            var firstName = newObject.SelectToken("$.firstName")?.Value<string>();
//            Assert.AreEqual("Scooby", firstName);
//        }

//        [Test]
//        public void MapSimpleProperty_UsingJPathWithPrefixDollarDot_ExpectSameValueAsSource()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("firstName", newObject);

//            var firstName = newObject.SelectToken("$.firstName")?.Value<string>();
//            Assert.AreEqual("Scooby", firstName);
//        }

//        [Test]
//        public void MapSimpleProperty_UsingJPath_ExpectNonMappedToNotExist()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.firstName", newObject);

//            Assert.IsNull(newObject.SelectToken("$.lastName"));
//        }

//        [Test]
//        public void MapSimpleProperty_AddIfNonExistent_ExpectPropertyToExistWithNullValue()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.age", newObject, new JsonMapSettings
//                {
//                    AddPathIfMissing = true
//                });

//            Assert.IsNotNull(newObject.ContainsKey("$.age"));
//            Assert.IsNull(newObject.SelectToken("$.age")?.Value<string>());
//        }

//        [Test]
//        public void MapSimpleProperty_UsingJPath_ChangeName_ExpectSameValueAsSource()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.firstName", "$.nom");

//            var firstName = jsonObject.SelectToken("$.nom")?.Value<string>();
//            Assert.AreEqual("Scooby", firstName);
//        }

//        [Test, Description("Maps a composite property to a composite property with the same path/name")]
//        public void MapCompositeProperty_WithoutDestination_AsSource()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.friend.firstName", newObject, new JsonMapSettings
//                {
//                    CompositeToSimple = false
//                });


//            var friendFirstName = newObject.SelectToken("$.friend.firstName")?.Value<string>();
//            Assert.AreEqual("Shaggy", friendFirstName);
//        }

//        [Test, Description("Maps a composite property to a composite property with the same path/name")]
//        public void MapCompositeProperty_WithoutDestination_ToNewProperty()
//        {
//            var temp = JObject.Parse(Sample1);
//            var test1 = temp.SelectToken("$.friend.firstName")?.Value<string>();

//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.friend.firstName", "$.bestFriend.FirstName", new JsonMapSettings
//                {
//                    CompositeToSimple = false
//                });


//            var friendFirstName = jsonObject.SelectToken("$.bestFriend.FirstName")?.Value<string>();
//            Assert.AreEqual("Shaggy", friendFirstName);
//        }


//        [Test, Description("Maps a composite property to a simple property")]
//        public void MapCompositeProperty_WithNonDotDestination_ExpectPropertyWithoutDestinationName()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.friend.firstName", "$.bestFriend");

//            var friendFirstName = jsonObject.SelectToken("$.bestFriend")?.Value<string>();
//            Assert.AreEqual("Shaggy", friendFirstName);
//        }

//        [Test, Description("Maps a composite property to a simple property with different names")]
//        public void MapCompositeProperty_WithDotDestination_ExpectPropertyWithoutDestinationNameWithoutDot()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.friend.firstName", "$.best.Friend");

//            var friendFirstName = jsonObject.SelectToken("$.bestFriend")?.Value<string>();
//            Assert.AreEqual("Shaggy", friendFirstName);
//        }

//        [Test, Description("Maps to an existing path should just overwite")]
//        public void MapSimple_UpdateExistingValue()
//        {
//            var newObject = new JObject();
//            var jsonObject = JObject.Parse(Sample1)
//                .Map("$.firstName", newObject)
//                .Map("$.lastName", newObject, "$.firstName");

//            var firstName = newObject.SelectToken("$.firstName")?.Value<string>();
//            Assert.AreEqual("Doo", firstName);
//        }

//        [Test]
//        public void AddToSimpleProperty_WithGivenValue()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .AddOrUpdate("$.id", 1234);

//            var firstName = jsonObject.SelectToken("$.id")?.Value<int>();
//            Assert.AreEqual(1234, firstName);
//        }

//        [Test]
//        public void AddToCompositeProperty_WithGivenValue()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .AddOrUpdate("$.person.id", 123, new JsonMapSettings
//                {
//                    CompositeToSimple = false
//                });


//            var personId = jsonObject.SelectToken("$.person.id")?.Value<int>();
//            Assert.AreEqual(123, personId);
//        }

//        [Test]
//        public void AddToCompositeProperty_UsingFunction()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .AddOrUpdate("$.person.id", source =>
//                {
//                    return 808;
//                }, new JsonMapSettings
//                {
//                    CompositeToSimple = false
//                });


//            var personId = jsonObject.SelectToken("$.person.id")?.Value<int>();
//            Assert.AreEqual(808, personId);
//        }

//        [Test]
//        public void If_IfPredicateTrue_ExpectValueSet()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .If(() => true, jo =>
//                {
//                    jo.AddOrUpdate("age", 21);
//                });

//            Assert.AreEqual(21, jsonObject.SelectToken("$.age")?.Value<int>());
//        }

//        [Test]
//        public void If_IfPredicateFalse_ExpectValueNotSet()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .If(() => false, jo =>
//                {
//                    jo.AddOrUpdate("age", 21);
//                });

//            Assert.IsNull(jsonObject.SelectToken("$.age")?.Value<int>());
//        }

//        [Test]
//        public void IfExists_IfPropertyDoesNotExist_DoNothing()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .IfExists("age", jo => jo.Add(new JProperty("never", "never")));

//            Assert.IsNull(jsonObject.SelectToken("$.never"));
//        }

//        [Test]
//        public void IfExists_IfPropertyExists_ExpectNewProperty()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .IfExists("firstName", jo => jo.Add(new JProperty("exists", "123")));

//            Assert.AreEqual("123", jsonObject.SelectToken("$.exists")?.Value<string>());
//        }

//        [Test]
//        public void IfNotExists_IfPropertyDoesNotExist_ExpectNewProperty()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .IfNotExists("age", jo => jo.Add(new JProperty("exists", "123")));

//            Assert.AreEqual("123", jsonObject.SelectToken("$.exists")?.Value<string>());
//        }

//        [Test]
//        public void IfNotExists_IfPropertyExists_ExpectNewProperty()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .IfNotExists("firstName", jo => jo.Add(new JProperty("never", "123")));

//            Assert.IsNull(jsonObject.SelectToken("$.never"));
//        }

//        [Test]
//        public void Remove_EnsureSuppliedPropertiesAreRemoved()
//        {
//            var jsonObject = JObject.Parse(Sample1)
//                .Remove(new[]
//                {
//                    "friend",
//                    "firstName"
//                });

//            Assert.IsNull(jsonObject.SelectToken("$.friend")?.Value<string>());
//            Assert.IsNull(jsonObject.SelectToken("$.firstName")?.Value<string>());
//            Assert.IsNotNull(jsonObject.SelectToken("$.lastName")?.Value<string>());
//        }

//        [Test]
//        public void Mutate_TakeExistingDataAndChangeIt_OneTypeChangeToAnother()
//        {
//            var date = "{\"startDate\":\"2012-07-30T19:22:09.1440844Z\"}";

//            // note when using strings that look like date/times conversion can be an issue
//            // so here we explicitly convert to DateTime then back to string
//            var jo = JObject.Parse(date)
//                .Mutate<DateTime, string>("startDate", value => value.ToString("dd-MM-yyyy"));

//            Assert.AreEqual("30-07-2012", jo.SelectToken("$.startDate")?.Value<string>());
//        }

//        [Test]
//        public void Mutate_TakeExistingDataAndChangeIt_WithSameType()
//        {
//            var jo = JObject.Parse("{\"name\":\"Scooby\"}")
//                .Mutate<string>("name", value => $"{value} Doo");

//            Assert.AreEqual("Scooby Doo", jo.SelectToken("$.name")?.Value<string>());
//        }


//        [Test]
//        public void Exists_IfPropertyExists_ExpectTrue()
//        {
//            var jsonObject = JObject.Parse(Sample1);

//            Assert.IsTrue(jsonObject.Exists("firstName"));
//        }

//        [Test]
//        public void Exists_IfPropertyDoesNotExist_ExpectFalse()
//        {
//            var jsonObject = JObject.Parse(Sample1);

//            Assert.IsFalse(jsonObject.Exists("age"));
//        }

//        [Test]
//        public void Map_UsingArrayMapping()
//        {
//            var jsonObject = JObject.Parse("{\"items\":[{name:\"1\"},{name:\"2\"},{name:\"3\"}]}")
//                .Map("$.items[?(@.name == '3')]", "lastItem");

//            Assert.AreEqual("3", jsonObject.SelectToken("$.lastItem.name")?.Value<string>());
//        }
//    }
//}