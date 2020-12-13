using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PutridParrot.JsonUtilities;

namespace Tests.PutridParrot.JsonUtilities
{
    [ExcludeFromCodeCoverage]
    [TestFixture, Description("Changes made to the new object leaving source unchanged")]
    public class TestsOnNewItem
    {
        [Test, Description("Map simple data item to a new object with the supplied name, without removing old")]
        public void Map_SimpleTopLevelItem_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("menuId", json, "id");

            Assert.AreEqual("Menu1", newObject.SelectToken("menuId")?.Value<string>());
            Assert.AreEqual("Menu1", json.SelectToken("id")?.Value<string>());
        }

        [Test, Description("Map non existent data item to a new named property, should not create property")]
        public void Map_SimpleTopLevelItem_WhenDoesNotExist_ExpectNotMapped()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("menuId", json, "id1");

            Assert.IsNull(newObject.SelectToken("menuId")?.Value<string>());
            Assert.IsNull(json.SelectToken("id1")?.Value<string>());
        }

        [Test, Description("Map non existent data item to a new named property with option to add missing, should create property")]
        public void Map_SimpleTopLevelItem_WhenDoesNotExist_ExpectNewProperty()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("menuId", json, "id1", new JsonOptions { AddPropertyIfMissing = true });

            Assert.IsTrue(newObject.Exists("menuId"));
            Assert.IsFalse(json.Exists("id1"));
        }


        [Test, Description("Map complex data item to a new named property, without removing old property")]
        public void Map_ComplexTopLevelItem_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("popupmenu", json, "menu.popup");

            Assert.NotNull(newObject.SelectToken("popupmenu.menuitem") is JArray);
            Assert.NotNull(json.SelectToken("menu.popup.menuitem") is JArray);
        }

        [Test, Description("Map simple data item to a new object with the same name, without removing old")]
        public void Map_SimpleTopLevelItemSameName_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("id", json);

            Assert.AreEqual("Menu1", newObject.SelectToken("id")?.Value<string>());
            Assert.AreEqual("Menu1", json.SelectToken("id")?.Value<string>());
        }

        [Test, Description("Map complex data item to a new named property with same name, without removing old property")]
        public void Map_ComplexTopLevelItemSameName_WhenExist_ExpectMapped()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = new JObject();

            newObject.Map("menu.popup", json);

            Assert.NotNull(newObject.SelectToken("menu.popup.menuitem") is JArray);
            Assert.NotNull(json.SelectToken("menu.popup.menuitem") is JArray);
        }

        [Test, Description("Clone all properties (although DeepClone on JObject may be a better option)")]
        public void Clone_NoSetPropertiesSoCloneAll()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = json.Clone();

            Assert.AreEqual(json.ToString(), newObject.ToString());
        }

        [Test, Description("Clone selected properties")]
        public void Clone_SelectedProperties_CloneOnlySelect()
        {
            var json = JObject.Parse(Sample.MenuData);
            var newObject = json.Clone("id", "menu");

            Assert.IsTrue(newObject.Exists("id"));
            Assert.IsTrue(newObject.Exists("menu"));
        }
    }
}
