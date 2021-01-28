using System;
using System.Collections.Generic;

using CodeEffects.Rule.Attributes;
using CodeEffects.Rule.Angular.Demo.Services;

namespace CodeEffects.Rule.Angular.Demo.Models
{
	[ExternalAction(typeof(EvaluationService), "ReturnResult")]

	public class EvaluateRequest
	{
		public string EvaluatedValue { get; set; }

		public DateTime? EvaluationBeginDate { get; set; }

		public DateTime? EvaluationEndDate { get; set; }

		public UnitCapacity UnitCapacity { get; set; }

		public DateTime MaxFutureDate
		{
			get { return DateTime.Parse("2075-01-01"); }
		}

        [ExcludeFromEvaluation]
        public List<string> Results { get; } = new List<string>();

        public bool IsValidDate(DateTime? value)
		{
			return value == null || (value != null && value >= DateTime.MinValue);
		}

		public string DateToString(DateTime? value)
		{
			return value.ToString();
		}
	}
}
