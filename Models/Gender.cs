using CodeEffects.Rule.Attributes;

namespace CodeEffects.Rule.Angular.Demo.Models
{
	public enum Gender
	{
		Male,
		Female,
		[ExcludeFromEvaluation]
		Unknown
	}
}