using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AES
{
	class Program
	{

		static object lockable = new object();

		static sbyte[] key = new sbyte[] { 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 };

		static int[] sBox = new int[] {
			0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
			0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
			0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
			0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
			0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
			0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
			0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
			0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
			0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
			0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
			0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
			0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
			0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
			0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
			0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
			0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 };

		private static int[] rCon = new int[] {
			0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
			0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39,
			0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a,
			0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8,
			0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef,
			0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc,
			0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b,
			0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3,
			0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94,
			0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20,
			0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35,
			0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f,
			0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04,
			0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63,
			0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd,
			0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d };


		private static int[] w = new int[4 * (10 + 1)];

		private static int[] expandKey()
		{
			int temp, i = 0;
			while (i < 4)
			{
				w[i] = 0x00000000;
				w[i] |= key[4 * i] << 24;
				w[i] |= key[4 * i + 1] << 16;
				w[i] |= key[4 * i + 2] << 8;
				w[i] |= key[4 * i + 3];
				i++;
			}
			i = 4;
			while (i < 4 * (10 + 1))
			{
				temp = w[i - 1];
				if (i % 4 == 0)
				{
					temp = subWord(rotWord(temp)) ^ (rCon[i / 4] << 24);
				}
				else if (4 > 6 && (i % 4 == 4))
				{
					temp = subWord(temp);
				}
				else
				{
				}
				w[i] = w[i - 4] ^ temp;
				i++;
			}
			return w;
		}

		private static int rotWord(int word)
		{
			return (word << 8) | ((int)((uint)(word & 0xFF000000) >> 24));
		}

		public static byte[] ECB_encrypt_parallel(sbyte[] text)
		{
			MemoryStream memStream = new MemoryStream();
			var binaryWrite = new BinaryWriter(memStream);

			List<KeyValuePair<long, sbyte[]>> blocks = new List<KeyValuePair<long, sbyte[]>>();
			blocks.Capacity = 20000;
			long m = 0;
			for (long i = 0; i < text.Length; i += 16)
			{
				sbyte[] text1 = new sbyte[16];
				try
				{
					Array.Copy(text, i, text1, 0, 16);
					blocks.Add(new KeyValuePair<long, sbyte[]>(m, text1));
					m++;
				}
				catch (Exception e)
				{
					Console.WriteLine("Block size error");
				}
			}
			Dictionary<long, byte[]> result = new Dictionary<long, byte[]>();

			var res = Parallel.ForEach(blocks, block =>
			{
				byte[] res = (byte[])(Array)encrypt(block.Value);
				lock (lockable)
				{
					result[block.Key] = res;
				}
			});
			for (int i = 0; i < result.Count; i++)
			{
				binaryWrite.Write(result[i]);
			}
			if (res.IsCompleted)
			{
				return memStream.ToArray(); ;
			}
			return null;
		}

		public static byte[] ECB_encrypt_consistent(sbyte[] text)
		{
			MemoryStream memStream = new MemoryStream();
			var binaryWrite = new BinaryWriter(memStream);
			for (int i = 0; i < text.Length; i += 16)
			{
				sbyte[] text1 = new sbyte[16];
				try
				{
					Array.Copy(text, i, text1, 0, 16);
					binaryWrite.Write((byte[])(Array)encrypt(text1));
				}
				catch (Exception e)
				{
					Console.WriteLine("Кол-во символов должно быть кратно 16");
				}
			}
			return memStream.ToArray();
		}

		private static sbyte[] encrypt(sbyte[] text)
		{
			int[][][] state = new int[2][][];
			state[0] = new int[4][];
			state[1] = new int[4][];

			state[0][0] = new int[4];
			state[0][1] = new int[4];
			state[0][2] = new int[4];
			state[0][3] = new int[4];

			state[1][0] = new int[4];
			state[1][1] = new int[4];
			state[1][2] = new int[4];
			state[1][3] = new int[4];


			if (text.Length != 16)
			{
				Console.WriteLine("Only 16-byte blocks can be encrypted");
			}
			sbyte[] output = new sbyte[text.Length];

			for (int i = 0; i < 4; i++)
			{ 
				for (int j = 0; j < 4; j++)
				{ 
					state[0][j][i] = text[i * 4 + j] & 0xff;
				}
			}

			cipher(state[0], state[1]);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					output[i * 4 + j] = (sbyte)(state[1][j][i] & 0xff);
				}
			}
			return output;
		}

		private static int[][] cipher(int[][] input, int[][] output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input.Length; j++)
				{
					output[i][j] = input[i][j];
				}
			}
			int actual = 0;
			output = addRoundKey(output, actual);

			for (actual = 1; actual < 10; actual++)
			{
				output = subBytes(output);
				output = shiftRows(output);
				output = mixColumns(output);
				output = addRoundKey(output, actual);
			}
			output = subBytes(output);
			output = shiftRows(output);
			output = addRoundKey(output, actual);
			return output;
		}

		private static int[][] addRoundKey(int[][] s, int round)
		{
			for (int c = 0; c < 4; c++)
			{
				for (int r = 0; r < 4; r++)
				{
					s[r][c] = s[r][c] ^ ((int)((uint)(w[round * 4 + c] << (r * 8)) >> 24));
				}
			}
			return s;
		}


		private static int[][] subBytes(int[][] state)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					state[i][j] = subWord(state[i][j]) & 0xFF;
				}
			}
			return state;
		}

		private static int subWord(int word)
		{
			int subWord = 0;
			for (int i = 24; i >= 0; i -= 8)
			{
				int input = (int)((uint)(word << i) >> 24);
				subWord |= sBox[input] << (24 - i);
			}
			return subWord;
		}

		private static int[][] shiftRows(int[][] state)
		{
			int temp1, temp2, temp3, i;

			temp1 = state[1][0];
			for (i = 0; i < 4 - 1; i++)
			{
				state[1][i] = state[1][(i + 1) % 4];
			}
			state[1][4 - 1] = temp1;

			temp1 = state[2][0];
			temp2 = state[2][1];
			for (i = 0; i < 4 - 2; i++)
			{
				state[2][i] = state[2][(i + 2) % 4];
			}
			state[2][4 - 2] = temp1;
			state[2][4 - 1] = temp2;

			temp1 = state[3][0];
			temp2 = state[3][1];
			temp3 = state[3][2];
			for (i = 0; i < 4 - 3; i++)
			{
				state[3][i] = state[3][(i + 3) % 4];
			}
			state[3][4 - 3] = temp1;
			state[3][4 - 2] = temp2;
			state[3][4 - 1] = temp3;

			return state;
		}

		private static int[][] mixColumns(int[][] state)
		{
			int temp0, temp1, temp2, temp3;
			for (int c = 0; c < 4; c++)
			{

				temp0 = mult(0x02, state[0][c]) ^ mult(0x03, state[1][c]) ^ state[2][c] ^ state[3][c];
				temp1 = state[0][c] ^ mult(0x02, state[1][c]) ^ mult(0x03, state[2][c]) ^ state[3][c];
				temp2 = state[0][c] ^ state[1][c] ^ mult(0x02, state[2][c]) ^ mult(0x03, state[3][c]);
				temp3 = mult(0x03, state[0][c]) ^ state[1][c] ^ state[2][c] ^ mult(0x02, state[3][c]);

				state[0][c] = temp0;
				state[1][c] = temp1;
				state[2][c] = temp2;
				state[3][c] = temp3;
			}

			return state;
		}

		private static int mult(int a, int b)
		{
			int sum = 0;
			while (a != 0)
			{
				if ((a & 1) != 0)
				{
					sum = sum ^ b;
				}
				b = xtime(b);
				a = (int)((uint)a >> 1);
			}
			return sum;

		}
		private static int xtime(int b)
		{
			if ((b & 0x80) == 0)
			{
				return b << 1;
			}
			return (b << 1) ^ 0x11b;
		}


		static void Main(string[] args)
		{
			w = expandKey();
			string contents = System.IO.File.ReadAllText(@"test.txt"); ;
			var str = Encoding.UTF8.GetBytes(contents);
			sbyte[] signed = Array.ConvertAll(str, b => unchecked((sbyte)b));
			var start = Environment.TickCount;
			var parallel = ECB_encrypt_parallel(signed);
			var end = Environment.TickCount;
			var start2 = Environment.TickCount;
			var consistent = ECB_encrypt_consistent(signed);
			var end2 = Environment.TickCount;
			Console.WriteLine("Общее время парал " + (end - start) + "мс");
			Console.WriteLine("Общее время посл " + (end2 - start2) + "мс");
			/**
			sbyte[] signed2 = Array.ConvertAll(parallel, b => unchecked((sbyte)b));
			sbyte[] signed3 = Array.ConvertAll(consistent, b => unchecked((sbyte)b));
			*/
			/**
			foreach (sbyte a in parallel)
			{
                Console.Write(a + " ");
			}
            Console.WriteLine("");
            foreach (sbyte a in consistent)
            {
                Console.Write(a + " ");
            }
            */
		}
	}
}
