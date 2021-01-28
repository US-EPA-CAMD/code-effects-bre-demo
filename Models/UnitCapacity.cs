using System;

namespace CodeEffects.Rule.Angular.Demo.Models
{
	public class UnitCapacity
	{
		public UnitCapacity()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }

		public DateTime? BeginDate { get; set; }

		public DateTime? EvaluationBeginDate { get; set; }

		public DateTime? EndDate { get; set; }

		public DateTime? EvaluationEndDate { get; set; }

		public bool Active { get; set; }

		public bool DatesValid { get; set; }

		public bool DatesConsistent { get; set; }

		public int? MaxHrlyHeatInputCapacity { get; set; }
	}
}