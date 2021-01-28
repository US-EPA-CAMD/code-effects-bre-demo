using System;
using System.Collections.Generic;

namespace CodeEffects.Rule.Angular.Demo.Models
{
	public class EvaluateResponse : Result
	{

		public List<string> Results { get; set; }

		public DateTime? BeginDate { get; set; }

		public DateTime? EvaluationBeginDate { get; set; }

		public DateTime? EndDate { get; set; }

		public DateTime? EvaluationEndDate { get; set; }

		public bool Active { get; set; }

		public bool DatesValid { get; set; }

		public bool DatesConsistent { get; set; }

		public int? MaxHrlyHeatInputCapacity { get; set; }

		public EvaluateResponse() : base() { }
	}
}