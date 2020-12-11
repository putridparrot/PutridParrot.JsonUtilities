namespace PutridParrot.JsonUtilities
{
    // TODO:
    // 1. Map simple property to simple property
    // 2. Map composite property to simple property, i.e. a.b maps to c
    // 3. Map composite property to composite property. i.e. a.b maps to c.d
    // 4. Map from an array to array
    // 5. Map from single item to array
    // 6. Map to join, i.e. map property a & b to c

    public class JsonMapSettings
    {
        public static readonly JsonMapSettings Default = new JsonMapSettings
        {
            AddPathIfMissing = false,
            CompositeToSimple = true
        };

        public bool AddPathIfMissing { get; set; }
        public bool CompositeToSimple { get; set; }
    }
}
