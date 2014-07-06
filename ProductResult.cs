using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhashtagApiFileUpload
{
	public class ResultDataModel
	{
		public long patternId { get; set; }
		public string patternName { get; set; }
		public double probability { get; set; }
	}

	public class ResultsModel
	{

		public string type { get; set; }

		public string message { get; set; }

		public ResultDataModel[] data { get; set; }
	}
}
