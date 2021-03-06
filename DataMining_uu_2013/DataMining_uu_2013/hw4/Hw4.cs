﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining_uu_2012.hw4
{
	using System.Text.RegularExpressions;

	using DataMining_uu_2012.utilities;
	public class Matrix
	{
		private readonly int[][] matrix;
		public Matrix(int height, int width)
		{
			matrix = new int[height][];
			for (var i = 0; i < matrix.Length; i++)
			{
				matrix[i] = new int[width];
				// set to 0 now
				for (var j = 0; j < width; j++)
				{
					matrix[i][j] = 0;
				}
			}
		}

		public void SetValue(int column, int row, int value)
		{
			this.matrix[row][column] = value;
		}

		public int GetValue(int column, int row)
		{
			return this.matrix[row][column];
		}
	}

	public class Hw4
	{
		public List<string> S1 { get; private set; }

		public Dictionary<string, int> S1Dict
		{
			get
			{
				return ReturnDict(this.S1);
			}
		}

		public string MajorityS1
		{
			get
			{
				return Majority(this.S1);
			}
		}

		public Tuple<List<int>, List<string>> MisaGriesS1
		{
			get
			{
				return MisraGries(this.S1, 10);
			}
		}

		public List<string> S2 { get; private set; }

		public Dictionary<string, int> S2Dict
		{
			get
			{
				return ReturnDict(this.S2);
			}
		}

		public string MajorityS2
		{
			get
			{
				return Majority(this.S2);
			}
		}

		public Tuple<List<int>, List<string>> MisaGriesS2
		{
			get
			{
				return MisraGries(this.S2, 10);
			}
		}

		public Hw4()
		{
			var s1 = "DataMining_uu_2012.hw4.S1.txt".ReadResource().Trim();
			S1 = Regex.Split(s1, "\\s+").ToList();

			var s2 = "DataMining_uu_2012.hw4.S2.txt".ReadResource().Trim();
			S2 = Regex.Split(s2, "\\s+").ToList();

			var maj = this.MajorityS1;
			var mg1 = string.Empty;
			for (var i = 0; i < this.MisaGriesS1.Item1.Count; i++)
			{
				mg1 += this.MisaGriesS1.Item2[i] + " : " + this.MisaGriesS1.Item1[i] + Environment.NewLine;
			}
			Console.WriteLine(mg1);

			var mg2 = string.Empty;
			for (var i = 0; i < this.MisaGriesS2.Item1.Count; i++)
			{
				mg2 += this.MisaGriesS2.Item2[i] + " : " + this.MisaGriesS2.Item1[i] + Environment.NewLine;
			}
			Console.WriteLine(mg2);

			var countMinDict = CountMinSketch(this.S1, 10, 5);
			var sb = new StringBuilder();
			foreach (var item in countMinDict)
			{
				sb.AppendLine(item.Key + " : " + item.Value);
			}

			var countMinDict1 = CountMinSketch(this.S1, 10, 5);
			var sb1 = new StringBuilder();
			foreach (var item in countMinDict1)
			{
				sb1.AppendLine(item.Key + " : " + item.Value);
			}
		}

		private static Dictionary<string, int> CountMinSketch(IReadOnlyCollection<string> listOfM, int k, int t)
		{
			var counterMatrix = new Matrix(t, k);
			//map it out first
			var mMap = new Dictionary<string, int>();
			var distinctM = listOfM.Distinct().ToList();
			for (var i = 0; i < distinctM.Count; i++)
			{
				mMap[distinctM[i]] = i;
			}

			var hashFunctions = HashUtil.HashFunctions(t, k, mMap.Keys.Count).ToList();

			for (var i = 0; i < listOfM.Count; i++)
			{
				var mapResult = mMap[listOfM.ElementAt(i)];

				for (var j = 0; j < t; j++)
				{
					var hashFunctionResult = hashFunctions[j][mapResult];
					counterMatrix.SetValue(hashFunctionResult, j,
						counterMatrix.GetValue(hashFunctionResult, j) + 1
						);
				}
			}

			var resultDictionary = new Dictionary<string, int>();

			foreach(var kvp in mMap)
			{
				var minResult = int.MaxValue;
				for (var i = 0; i < t; i++)
				{
					var hashMap = hashFunctions[i][kvp.Value];
					var tempResult = counterMatrix.GetValue(hashMap, i);
					if (tempResult < minResult)
					{
						minResult = tempResult;
					}
				}
				resultDictionary[kvp.Key] = minResult;
			}
			return resultDictionary;
		}

		private static string Majority(IEnumerable<string> listOfItems)
		{
			var c = -1;
			var l = string.Empty;
			foreach (var m in listOfItems)
			{
				if (m == l)
				{
					c++;
				}
				else
				{
					c--;
				}

				if (c <= -1)
				{
					c = 1;
					l = m;
				}
			}
			return l;
		}

		private static Tuple<List<int>, List<string>> MisraGries(IList<string> listOfItems, int k)
		{
			if (k < 1)
			{
				return new Tuple<List<int>, List<string>>(new List<int>(), new List<string>());
			}

			var c = new List<int>(k);
			var l = new List<string>(k);
			var m = listOfItems.Count;

			for (var idx = 0; idx < k; idx++)
			{
				c.Add(-1);
				l.Add(string.Empty);
			}

			for (var i = 0; i < m; i++)
			{
				var j = l.IndexOf(listOfItems[i]);

				if (j >= 0)
				{
					c[j]++;
				}
				else if (!c.Any(t => t < 0))
				{
					for (var idx = 0; idx < k; idx++)
					{
						c[idx]--;
					}
				}
				else
				{
					// find counter that is -1
					var jIdx = -1;
					for (var jj = 0; jj < k; jj++)
					{
						if (c[jj] < 0 || string.IsNullOrWhiteSpace(l[jj]))
						{
							jIdx = jj;
							break;
						}
					}

					if (jIdx >= 0)
					{
						c[jIdx] = 1;
						l[jIdx] = listOfItems[i];
					}
				}
				for (var jj = 0; jj < k; jj++)
				{
					if (c[jj] <= 0)
					{
						l[jj] = string.Empty;
					}
				}
			}
			return new Tuple<List<int>, List<string>>(c, l);
		}

		private static Dictionary<string, int> ReturnDict(IEnumerable<string> vals)
		{
			var returnDict = new Dictionary<string, int>();

			if (vals != null)
			{
				foreach (var item in vals)
				{
					if (returnDict.ContainsKey(item))
					{
						returnDict[item]++;
					}
					else
					{
						returnDict[item] = 1;
					}
				}
			}

			return returnDict;
		}
	}
}
