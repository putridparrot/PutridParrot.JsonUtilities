using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PutridParrot.JsonUtilities;

namespace Tests.PutridParrot.JsonUtilities
{
    [ExcludeFromCodeCoverage]
    [TestFixture, Description("Changes made to the source object")]
    public class TestsOnSelf
    {
        [Test, Description("Map simple data item to a new named property, without removing old property")]
        public void Map_SimpleTopLevelItem_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Map("menuId", "id");

            Assert.AreEqual("Menu1", json.SelectToken("menuId")?.Value<string>());
            Assert.AreEqual("Menu1", json.SelectToken("id")?.Value<string>());
        }

        [Test, Description("Map non existent data item to a new named property, should not create property")]
        public void Map_SimpleTopLevelItem_WhenDoesNotExist_ExpectNotMapped()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Map("id1", "menuId");

            Assert.IsNull(json.SelectToken("menuId")?.Value<string>());
            Assert.IsNull(json.SelectToken("id1")?.Value<string>());
        }

        [Test,
         Description(
             "Map non existent data item to a new named property with option to add missing, should create property")]
        public void Map_SimpleTopLevelItem_WhenDoesNotExist_ExpectNewProperty()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Map("id1", "id", new JsonOptions {AddPropertyIfMissing = true});

            Assert.IsTrue(json.Exists("id"));
            Assert.IsTrue(json.Exists("id1"));
        }

        [Test, Description("Map complex data item to a new named property, without removing old property")]
        public void Map_ComplexTopLevelItem_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Map("menu.popup", "popupmenu");

            Assert.NotNull(json.SelectToken("popupmenu.menuitem") is JArray);
            Assert.NotNull(json.SelectToken("menu.popup.menuitem") is JArray);
        }

        [Test, Description("Check if simple property exists")]
        public void Exists_SimplePropertyWhereItExists_ExpectTrue()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.Exists("id"));
        }

        [Test, Description("Check if simple property does not exist")]
        public void Exists_SimplePropertyWhereItDoesNotExists_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsFalse(json.Exists("myid"));
        }

        [Test, Description("Check if complex property exists")]
        public void Exists_ComplexPropertyWhereItExists_ExpectTrue()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.Exists("menu.popup"));
        }

        [Test, Description("Check if complex property does not exist")]
        public void Exists_ComplexPropertyWhereItDoesNotExists_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsFalse(json.Exists("myid.value"));
        }

        [Test, Description("Try to add an item which doesn't exist")]
        public void TryAddOrUpdate_WhenItemDoesNotExist_ExpectItToBeAdded()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.TryAddOrUpdate("newProperty", 1234));
            Assert.AreEqual(1234, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("Try to update an item which exists")]
        public void TryAddOrUpdate_WhenItemDoesExist_ExpectItToBeUpdate()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.TryAddOrUpdate("id", 1234));
            Assert.AreEqual(1234, json.SelectToken("id")?.Value<int>());
        }

        [Test, Description("Try to add an item which doesn't exist")]
        public void AddOrUpdate_WhenItemDoesNotExist_ExpectItToBeAdded()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.AddOrUpdate("newProperty", 1234);

            Assert.AreEqual(1234, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("Try to update an item which exists")]
        public void AddOrUpdate_WhenItemDoesExist_ExpectItToBeUpdate()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.AddOrUpdate("id", 1234);

            Assert.AreEqual(1234, json.SelectToken("id")?.Value<int>());
        }

        [Test, Description("Try to add an item which doesn't exist using a func")]
        public void AddOrUpdate_UsingFunc_WhenItemDoesNotExist_ExpectItToBeAdded()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.AddOrUpdate("newProperty", j => 1234);

            Assert.AreEqual(1234, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("Try to update an item which exists using a func")]
        public void AddOrUpdate_UsingFunc_WhenItemDoesExist_ExpectItToBeUpdate()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.AddOrUpdate("id", j => 1234);

            Assert.AreEqual(1234, json.SelectToken("id")?.Value<int>());
        }


        [Test, Description("Get the top level property names")]
        public void GetProperties_ExpectTopLevelProperties()
        {
            var json = JObject.Parse(Sample.MenuData);

            var properties = json.GetProperties();
            Assert.AreEqual(2, properties.Count);
        }

        [Test, Description("IfExists property exists then execute block")]
        public void IfExists_IfPropertyExists_TheExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.IfExists("id", j => { j.AddOrUpdate("newProperty", 123); });

            Assert.AreEqual(123, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("IfExists property does not exist then do not execute block")]
        public void IfExists_IfPropertyDoesNotExists_ThenDoNotExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.IfExists("id1", j => { j.AddOrUpdate("newProperty", 123); });

            Assert.IsFalse(json.Exists("newProperty"));
        }

        [Test, Description("IfNotExists property exists then execute block")]
        public void IfNotExists_IfPropertyExists_TheExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.IfNotExists("id1", j => { j.AddOrUpdate("newProperty", 123); });

            Assert.AreEqual(123, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("IfNotExists property does not exist then do not execute block")]
        public void IfNotExists_IfPropertyDoesNotExists_ThenDoNotExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.IfNotExists("id", j => { j.AddOrUpdate("newProperty", 123); });

            Assert.IsFalse(json.Exists("newProperty"));
        }

        [Test, Description("If true then execute block")]
        public void If_IfTrue_ExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.If(j => true, j => { j.AddOrUpdate("newProperty", 123); });

            Assert.AreEqual(123, json.SelectToken("newProperty")?.Value<int>());
        }

        [Test, Description("If false then execute block")]
        public void If_IfFalse_ExecuteBlock()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.If(j => false, j => { j.AddOrUpdate("newProperty", 123); });

            Assert.IsFalse(json.Exists("newProperty"));
        }

        [Test, Description("Get's a value where one exists")]
        public void Get_WhenItExists_ExpectTheCorrectValue()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.AreEqual("file", json.Get<string>("menu.id"));
        }

        [Test, Description("Get's a default when property does not exist")]
        public void Get_WhenPropertyDoesNotExists_ExpectDefaultT()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.AreEqual(default(string), json.Get<string>("menu.id2"));
        }

        [Test, Description("Remove no items")]
        public void Remove_WithNoPropertiesListed_ExpectNoneRemoved()
        {
            var json = JObject.Parse(Sample.MenuData);
            var before = JObject.Parse(Sample.MenuData);

            json.Remove(new string[] { });

            Assert.AreEqual(before, json);
        }

        [Test, Description("Remove no items"), Ignore("Looks like remove doesn't handle complex types")]
        public void Remove_MultipleProperties_ExpectPropertiesRemoved()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Remove(new[] {"id", "menu.id"});

            Assert.IsFalse(json.Exists("id"));
            Assert.IsFalse(json.Exists("menu.id"));
            Assert.IsTrue(json.Exists("menu.value"));
            Assert.IsTrue(json.Exists("menu.popup"));
        }

        [Test]
        public void Update_IfPropertyExists_ExpectUpdated()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Update<string>("id", s => "hello");

            Assert.AreEqual("hello", json.SelectToken("id")?.Value<string>());
        }

        [Test]
        public void Update_AndChangeTypeIfPropertyExists_ExpectUpdated()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Update<string, int>("id", s => 1234);

            Assert.AreEqual(1234, json.SelectToken("id")?.Value<int>());
        }

        [Test]
        public void Update_FromValueIfPropertyExists_ExpectUpdated()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Update("id", "hello");

            Assert.AreEqual("hello", json.SelectToken("id")?.Value<string>());
        }

        [Test]
        public void Update_FromValueChangeTypeIfPropertyExists_ExpectUpdated()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Update<string, int>("id", 1234);

            Assert.AreEqual(1234, json.SelectToken("id")?.Value<int>());
        }


        [Test]
        public void Update_IfPropertyDoesNotExists_ExpectNotChange()
        {
            var json = JObject.Parse(Sample.MenuData);

            json.Update<string>("id1", s => "hello");

            Assert.IsFalse(json.Exists("id1"));
        }

        [Test]
        public void TryGetValue_WhenPropertyDoesNotExist_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsFalse(json.TryGetValue<string>("id1", out var j));
            Assert.AreEqual(default(string), j);
        }

        [Test]
        public void TryGetValue_WhenPropertyDoesExist_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.TryGetValue<string>("id", out var j));
            Assert.AreEqual("Menu1", j);
        }

        [Test]
        public void ToCsv_SingleProperty_ExpectCsvTypeOutput()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.AreEqual("id:Menu1", json.ToCsv("id"));
        }

        [Test]
        public void ToCsv_MultiplePropertes_ExpectCsvTypeOutput()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.AreEqual("id:Menu1, menu.id:file", json.ToCsv("id", "menu.id"));
        }


        [Test]
        public void Sample_MappingAndAddingProperties()
        {
            var json = JObject.Parse(Sample.MenuData);
            
            var mapped = new JObject()
                .Map("newId", json, "id")
                .Map("openDoc", json, "menu.popup.menuitem[?(@.value == 'Open')]")
                .Map("newObject.value", json, "menu.value")
                .AddOrUpdate("additionalProperty", 123);

            Assert.AreEqual("Menu1", mapped.Get<string>("newId"));
            Assert.AreEqual("OpenDoc()", mapped.SelectToken("openDoc.onclick")?.Value<string>());
            Assert.AreEqual("File", mapped.Get<string>("newObject.value"));
            Assert.AreEqual(123, mapped.Get<int>("additionalProperty"));
        }

        [Test]
        public void IsArray_FromProperty_WhenNotArray_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsFalse(json.IsArray("menu.popup"));
        }

        [Test]
        public void IsArray_FromProperty_WhenIsArray_ExpectTrue()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.IsArray("menu.popup.menuitem"));
        }

        [Test]
        public void IsArray_WhenNotArray_ExpectFalse()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsFalse(json.SelectToken("menu.popup").IsArray());
        }

        [Test]
        public void IsArray_WhenIsArray_ExpectTrue()
        {
            var json = JObject.Parse(Sample.MenuData);

            Assert.IsTrue(json.SelectToken("menu.popup.menuitem").IsArray());
        }

    }
}
