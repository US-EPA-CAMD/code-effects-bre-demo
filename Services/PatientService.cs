﻿using System;
using CodeEffects.Rule.Common;
using CodeEffects.Rule.Attributes;
using CodeEffects.Rule.Angular.Demo.Models;

namespace CodeEffects.Rule.Angular.Demo.Services
{
	/// <summary>
	/// The purpose of this class is to demonstrate the use of external static and instance in-rule methods and actions.
	/// The Patient class uses the ExternalMethodAttribute and ExternalActionAttribute to "reference" methods of this PatientService class.
	/// </summary>
	public class PatientService
	{
		// Instance action (void) method
		[Action("Request More Info", "Requires additional info from the patient")]
		public void RequestInfo(Patient patient, [Parameter(ValueInputType.User, Description = "Output message")] string message)
		{
			patient.Output = message;
		}
	}
}