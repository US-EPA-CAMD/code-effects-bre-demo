using System;

using CodeEffects.Rule.Attributes;

using CodeEffects.Rule.Angular.Demo.Models;

namespace CodeEffects.Rule.Angular.Demo.Services
{
	public class EvaluationService
	{
		[Action(DisplayName = "Return Result")]
		public void ReturnResult(EvaluateRequest request, string msg)
		{
			if (!string.IsNullOrEmpty(request.EvaluatedValue))
				msg = msg.Replace("{value}", request.EvaluatedValue.ToString());

			msg = msg.Replace("{recId}", request.UnitCapacity.Id.ToString());

			msg = msg.Replace("&nbsp;", " ");
			request.Results.Add(msg);
		}
	}
}