using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeEffects.Rule.Angular.Demo.Models
{
	public class Result
	{
		public bool IsRuleEmpty { get; set; }
		public bool IsRuleValid { get; set; }
		public string Output { get; set; }
		public string ClientInvalidData { get; set; }

		public Result()
		{
			IsRuleEmpty = false;
			IsRuleValid = true;
		}
	}
}
