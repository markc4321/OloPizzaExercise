using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace OloSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var fileToProcess = "http://files.olo.com/pizzas.json";
                
                IEnumerable<Topping> result = Topping20PizzaCombos(fileToProcess);

                var rankCtr = 0;
                foreach (Topping t in result) 
                {
                    rankCtr++;
                    Console.WriteLine("Rank: {0}  Times Ordered: {1}  Combo: {2}", rankCtr, t.count, String.Join(", ", t.toppings.ToArray()));

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error occured. Message {0}", ex.Message);
            }

        }

        private static IEnumerable<Topping> Topping20PizzaCombos(string fileToProcess)
        {
            
            var jsonString = GetFile(fileToProcess);

            var toppingsList = decodeJSON(jsonString);

            //Not a combo if only one item?
            var combos = toppingsList.Where(i => i.toppings.Count > 1);

            var uniqueToppings = new List<Topping>();

            countCombos(uniqueToppings,combos);

            var top20 = uniqueToppings.OrderByDescending(i => i.count).Take(20);

            return top20;

        }

        private static void countCombos(List<Topping> uniqueToppings, IEnumerable<Topping> combos)
        {
            foreach (Topping t in combos)
            {
                //Is t in uniqueToppings list.
                if (!CompareLists(uniqueToppings, t.toppings))
                {
                    t.count = 1;
                    uniqueToppings.Add(t);
                }
            }
        }

        private static bool CompareLists(List<Topping> uniqueToppings, List<string> toppingsToCompare)
        {
            if (uniqueToppings.Count == 0 || toppingsToCompare.Count == 0)
                return false;

            try
            {
                for (int i = 0; i < uniqueToppings.Count; i++)
                {
                    //foundOne = true;
                    if (uniqueToppings[i].toppings.Count == toppingsToCompare.Count)
                    {
                        var difference = uniqueToppings[i].toppings.Except(toppingsToCompare);
                        if (difference.Count() > 0)
                        {
                            continue;
                        }
                        else
                        {
                            uniqueToppings[i].count++;
                            return true;
                        }
                    }
                }


            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        private static List<Topping> decodeJSON(string jsonString)
        {
           
            var ds = new JavaScriptSerializer();
            var obj = new List<Topping>();
            try
            {
                obj = ds.Deserialize<List<Topping>>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return obj;
        }



        private static string GetFile(string fileToProcess = "")
        {
            if (fileToProcess == "")
                throw new Exception("File name not specified");

            var jsonString = "";

            try
            {
                using (var client = new WebClient())
                {
                    jsonString = client.DownloadString(fileToProcess);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return jsonString;
        }


        public class Topping
        {
            //public string[] toppings { get; set; }
            public List<string> toppings { get; set; }
            public int count { get; set; }
        }

    }
}