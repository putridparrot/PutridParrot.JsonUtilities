# PutridParrot.JsonUtilities

![.NET Core](https://github.com/putridparrot/PutridParrot.JsonUtilities/workflows/.NET%20Core/badge.svg)

JSON C# utility classes, currently only targeting Newtonsoft.Json. 

## JsonExtensions

The JsonExtensions class supplies methods for checking for data in a JObject as well as mutating the data, including the ability to map data within a JObject or from another JObject.

_Note: Methods work in a right to left manner (where appropriate) similar to if you were assigning values._

### Example 

One such use of these extension methods is to take an existing JObject, clone (with selected properties) or create a new object and map data from one JObject to another. Another use is just is slightly simpler ways of accessing data from a JObject.

```CSharp
var json = JObject.Parse(Sample.MenuData);
            
var mapped = new JObject()
    .Map("newId", json, "id")
    .Map("openDoc", json, "menu.popup.menuitem[?(@.value == 'Open')]")
    .Map("newObject.value", json, "menu.value")
    .AddOrUpdate("additionalProperty", 123);
```

In this example, assuming you had the following Sample.MenuData 

```CSharp
public const string MenuData = "{\"menu\": {" +
    "\"id\": \"file\"," +
    "\"value\": \"File\"," +
    "\"popup\": {" +
      "\"menuitem\": [" +
        "{\"value\": \"New\", \"onclick\": \"CreateNewDoc()\"}," +
        "{\"value\": \"Open\", \"onclick\": \"OpenDoc()\"}," +
        "{\"value\": \"Close\", \"onclick\": \"CloseDoc()\"}" +
      "]" +
    "}" +
  "}," +
  "\"id\": \"Menu1\"" + 
"}";
```

then the _mapped_ JSON would look like this

```json
{
  "newId": "Menu1",
  "openDoc": {
    "value": "Open",
    "onclick": "OpenDoc()"
  },
  "newObject": {
    "value": "File"
  },
  "additionalProperty": 123
}
```

