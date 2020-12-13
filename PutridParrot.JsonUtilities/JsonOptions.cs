namespace PutridParrot.JsonUtilities
{
    // TODO:
    // 1. Map simple property to simple property
    // 2. Map composite property to simple property, i.e. a.b maps to c
    // 3. Map composite property to composite property. i.e. a.b maps to c.d
    // 4. Map from an array to array
    // 5. Map from single item to array
    // 6. Map to join, i.e. map property a & b to c

    public class JsonOptions
    {
        public static readonly JsonOptions Default = new JsonOptions
        {
            AddPropertyIfMissing = false,
            //CompositeToSimple = true
        };

        public bool AddPropertyIfMissing { get; set; }
        //public bool CompositeToSimple { get; set; }
    }
}
