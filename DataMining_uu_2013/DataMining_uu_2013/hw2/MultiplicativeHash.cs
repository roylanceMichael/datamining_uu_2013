﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining_uu_2012.hw2
{
	using System.Globalization;

	// This class facilitates multiplicative hashing
	public class MultiplicativeHash
	{
		private const int M = 40127;
		private static readonly Random RanToUse = new Random();

		public double Frac(long k)
		{
			const double A = .83;
			var res = k * A;
			return res % 1;
		}

		public int MultiplicativeHashFunc(int m, int x)
		{
			return x;

			//var xLong = Int64.Parse(x.ToString());
			//var maxInt = Int64.Parse(int.MaxValue.ToString());
			//var tempLong = xLong + maxInt;
			//var frac = this.Frac(tempLong);
			//var res = Math.Floor(m * frac);
			//return (int)res;
		}

		public int ConvertToHash(Tuple<string, string> item)
		{
			var res = item.Item1 + " " + item.Item2;
			var hashCode = res.GetHashCode();
			return hashCode;
		}

		public int ConvertToHashTrigramChar(Tuple<char, char, char> item)
		{
			var res = item.Item1 + item.Item2 + item.Item3;
			var hashCode = res.GetHashCode();
			return hashCode;
		}

		public bool UnigramMinHashing(IDictionary<string, int> d1, IDictionary<string, int> d2)
		{
			var completeList = new Dictionary<string, int>();
			foreach (var item in d1.Keys.Where(item => !completeList.ContainsKey(item)))
			{
				completeList[item] = 0;
			}

			foreach (var item in d2.Keys.Where(item => !completeList.ContainsKey(item)))
			{
				completeList[item] = 0;
			}

			// add numbers into the array
			var subtractList = new List<string>();
			var addList = completeList.Keys.ToList();
			while (addList.Count > 0)
			{
				var idx = RanToUse.Next(0, addList.Count - 1);
				subtractList.Add(addList[idx]);
				addList.RemoveAt(idx);
			}

			// we have the randomly sorted elements now...
			var d1FirstElement = subtractList.FirstOrDefault(t => d1.ContainsKey(t));
			var d2FirstElement = subtractList.FirstOrDefault(t => d2.ContainsKey(t));

			return d1FirstElement == d2FirstElement && d1FirstElement != null;
		}

		public bool BigramMinHashing(IDictionary<Tuple<string, string>, int> d1, IDictionary<Tuple<string, string>, int> d2)
		{
			var d1Hashed = d1.Keys.Select(item => this.MultiplicativeHashFunc(M, this.ConvertToHash(item))).ToList();
			var d2Hashed = d2.Keys.Select(item => this.MultiplicativeHashFunc(M, this.ConvertToHash(item))).ToList();
			var completeList = new Dictionary<int, int>();
			foreach (var hash in d1Hashed.Where(hash => !completeList.ContainsKey(hash)))
			{
				completeList[hash] = 0;
			}

			foreach (var hash in d2Hashed.Where(hash => !completeList.ContainsKey(hash)))
			{
				completeList[hash] = 0;
			}

			// add numbers into the array
			var subtractList = new List<int>();
			var addList = completeList.Keys.ToList();
			while (addList.Count > 0)
			{
				var idx = RanToUse.Next(0, addList.Count - 1);
				subtractList.Add(addList[idx]);
				addList.RemoveAt(idx);
			}

			var d1FirstElement = -1;
			var d2FirstElement = -1;
			var found = false;
			foreach (var hashcode in subtractList)
			{
				foreach (var res in d1Hashed)
				{
					if (hashcode != res)
					{
						continue;
					}

					found = true;
					d1FirstElement = res;
					break;
				}
				if (found)
				{
					break;
				}
			}

			found = false;

			foreach (var hashcode in subtractList)
			{
				foreach (var res in d2Hashed)
				{
					if (hashcode != res)
					{
						continue;
					}

					found = true;
					d2FirstElement = res;
					break;
				}
				if (found)
				{
					break;
				}
			}

			var returnBool = d1FirstElement != -1 && d2FirstElement != -1 && d1FirstElement == d2FirstElement;

			return returnBool;
		}

		public decimal TrigramCharMinHashing(IDictionary<Tuple<char, char, char>, int> d1, IDictionary<Tuple<char, char, char>, int> d2, int hashFunctionCount = 50)
		{
			var d1Hashed = d1.Keys.Select(item => this.MultiplicativeHashFunc(M, this.ConvertToHashTrigramChar(item))).ToList();
			var d2Hashed = d2.Keys.Select(item => this.MultiplicativeHashFunc(M, this.ConvertToHashTrigramChar(item))).ToList();
			var completeList = new Dictionary<int, int>();
			foreach (var hash in d1Hashed.Where(hash => !completeList.ContainsKey(hash)))
			{
				completeList[hash] = 0;
			}

			foreach (var hash in d2Hashed.Where(hash => !completeList.ContainsKey(hash)))
			{
				completeList[hash] = 0;
			}

			//create k hash functions
			var allHashFunctions = new List<IList<int>>();
			for (var i = 0; i < hashFunctionCount; i++)
			{
				allHashFunctions.Add(CreateNewHashFunction(completeList));
			}

			var c1 = new List<int>();
			var c2 = new List<int>();

			for (var i = 0; i < hashFunctionCount; i++)
			{
				c1.Add(int.MaxValue);
				c2.Add(int.MaxValue);
			}


			//compare d1 first, then d2
			//for i = 1 to N
			for (var i = 0; i < completeList.Count; i++)
			{
				//if S(i) = 1
				if (d1Hashed.Contains(completeList.Keys.ElementAt(i)))
				{
					//for j=1 to k do
					for (var j = 0; j < hashFunctionCount; j++)
					{
						if (allHashFunctions[j][i] < c1[j])
						{
							c1[j] = allHashFunctions[j][i];
						}
					}
				}
			}

			for (var i = 0; i < completeList.Count; i++)
			{
				//if S(i) = 1
				if (d2Hashed.Contains(completeList.Keys.ElementAt(i)))
				{
					//for j=1 to k do
					for (var j = 0; j < hashFunctionCount; j++)
					{
						var hji = allHashFunctions[j][i];
						var c2J = c2[j];
						if (hji < c2J)
						{
							c2[j] = allHashFunctions[j][i];
						}
					}
				}
			}

			if (c1.Count != c2.Count)
			{
				throw new Exception("c1 count is different than c2 count!");
			}

			var total = (decimal)c1.Where((t, i) => t == c2[i]).Count();

			return 1 - ((decimal)1 / c1.Count) * total;
		}

		private static IList<int> CreateNewHashFunction(IDictionary<int, int> completeList)
		{
			var subtractList = new List<int>();
			var addList = completeList.Keys.ToList();

			//make hard copy
			while (addList.Count > 0)
			{
				var idx = RanToUse.Next(0, addList.Count - 1);
				subtractList.Add(addList[idx]);
				addList.RemoveAt(idx);
			}
			return subtractList;
		}

		public bool TrigramMinHashing(IDictionary<Tuple<string, string, string>, int> d1, IDictionary<Tuple<string, string, string>, int> d2)
		{
			var completeList = new Dictionary<Tuple<string, string, string>, int>();
			foreach (var item in d1.Keys.Where(item => !completeList.Any(t => t.Key.Item1 == item.Item1 && t.Key.Item2 == item.Item2 && t.Key.Item3 == item.Item3)))
			{
				completeList[item] = 0;
			}

			foreach (var item in d2.Keys.Where(item => !completeList.Any(t => t.Key.Item1 == item.Item1 && t.Key.Item2 == item.Item2 && t.Key.Item3 == item.Item3)))
			{
				completeList[item] = 0;
			}

			// add numbers into the array
			var subtractList = new List<Tuple<string, string, string>>();
			var addList = completeList.Keys.ToList();
			while (addList.Count > 0)
			{
				var idx = RanToUse.Next(0, addList.Count - 1);
				subtractList.Add(addList[idx]);
				addList.RemoveAt(idx);
			}

			var d1FirstElement = subtractList.FirstOrDefault(r => d1.Any(t => t.Key.Item1 == r.Item1 && t.Key.Item2 == r.Item2 && t.Key.Item3 == r.Item3));
			var d2FirstElement = subtractList.FirstOrDefault(r => d2.Any(t => t.Key.Item1 == r.Item1 && t.Key.Item2 == r.Item2 && t.Key.Item3 == r.Item3));

			return d1FirstElement != null && d2FirstElement != null && d1FirstElement.Item1 == d2FirstElement.Item1 && d1FirstElement.Item2 == d2FirstElement.Item2 && d1FirstElement.Item3 == d2FirstElement.Item3;
		}
	}
}
