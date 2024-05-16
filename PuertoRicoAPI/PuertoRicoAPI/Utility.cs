using Newtonsoft.Json;

namespace PuertoRicoAPI
{
    public static class Utility
    {
        public static T CloneJson<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static int Mod(int n,int m)
        {
            return ((n % m) + m) % m;
        }

        public static void Line()
        {
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
        }

        public enum ColorName
        {
            yellow,
            blue,
            white,
            burlywood,
            black,
            violet
        }
    }
}
