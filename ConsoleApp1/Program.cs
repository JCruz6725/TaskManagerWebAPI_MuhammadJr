namespace ConsoleApp1 {
    internal class Program {
        static void Main(string[] args) {
        
            



            Data goodData = new(){ string1 = "PersonName", int1 = 55 };

            Data? bad1Data = null;

            Data bad2Data = new(){ string1 = "OtherPersonName", int1 = 3453 };


            //Something.methodA(goodData);

            Something.methodA(bad1Data);

            //Something.methodA(bad2Data.NestedData!.NestedData!);
        }
    }


    public class Data {
        public required string  string1 { get; set; }
        public int? int1 { get; set; }
        public Data? NestedData { get; set; }

    }

    public static class Something {
        public static void methodA(Data? d) {
            Console.WriteLine($"{d!.int1}");
        }
    }
}
