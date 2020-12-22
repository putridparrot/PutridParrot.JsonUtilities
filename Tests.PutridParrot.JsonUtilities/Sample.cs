namespace Tests.PutridParrot.JsonUtilities
{
    public static class Sample
    {
        /// <summary>
        /// Simple sample of a potential menu data source
        /// </summary>
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
    }
}
