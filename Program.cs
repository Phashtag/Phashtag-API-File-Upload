using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PhashtagApiFileUpload
{
	class Program
	{

		private static string authorization64;
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Phashtag file upload tester 1.0");
			Console.WriteLine("Create a username and password at api.phashtag.com");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("Username: ");
			string username = Console.ReadLine();
			Console.Write("Password: ");
			string password = Console.ReadLine();
			authorization64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));

			Console.Clear();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Type the file paths for files you wish to scan.");
			Console.WriteLine("To scan multiple at a time, separate files by &.");
			Console.WriteLine("Type exit to exit.");
			Console.ForegroundColor = ConsoleColor.Gray;


			while (true)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write("File Name(s): ");
				string command = Console.ReadLine();

				if (command == "exit") {
					break;
				}

				string[] files = command.Trim().Split('&');

				var tasks = new List<Task>();

				Console.ForegroundColor = ConsoleColor.White;

				foreach (string f in files)
				{
					string file = f.Trim();
					if (file.StartsWith("\"") && file.EndsWith("\"")) {
						file = file.Substring(1, file.Length - 2);
					}

					if (string.IsNullOrWhiteSpace(file)) {
						continue;
					}

					if (!File.Exists(file))
					{
						Console.WriteLine(file + " does not exist.");
						continue;
					}

					tasks.Add(Task.Run(() =>
					{
						ScanAsync(file);
					}));
				}
				foreach (var t in tasks) {
					t.Wait();
				}
			}
		}

		private static void ScanAsync(string f)
		{
			string printResultBuffer = "\n";
			ResultsModel result = null;
			try
			{
				result = ScanFile(f);

				printResultBuffer += Path.GetFileName(f) + "\n";
				if (result != null)
				{
					if (result.type == "data")
					{
						foreach (var r in result.data) {
							printResultBuffer += "\t PID:" + r.patternId + ",PNAME:" + r.patternName + ",PROB:" + r.probability + "\n";
						}
					}
					else
					{
						printResultBuffer += "\t" + result.message + "\n";
					}
				}
				else
				{
					printResultBuffer += f + "\tUnable to get result.\n";
				}
			}
			catch (Exception e)
			{
				printResultBuffer += "\t" + e.Message;
			}

			lock (Console.Out)
			{
				
				Console.WriteLine(printResultBuffer);
			} 
		}

		static ResultsModel ScanFile(string filename)
		{

			using (var wc = new WebClient()) {
				wc.Headers.Add("Authorization", "basic " + authorization64);

				byte[] responsebytes = null;
				//NOTE - Pattern 0 (in the URL) is the demo pattern. Change it to the pattern you wish to test for.
				responsebytes = wc.UploadFile("http://api.phashtag.com/v1/pattern/0/testonfile", filename);
				string responsebody = System.Text.Encoding.Default.GetString(responsebytes);
				return JsonConvert.DeserializeObject<ResultsModel[]>(responsebody).FirstOrDefault();
			}
		}
	}
}
