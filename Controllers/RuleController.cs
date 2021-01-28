using System;
using CodeEffects.Rule.Angular.Demo.Models;
using CodeEffects.Rule.Angular.Demo.Services;
using CodeEffects.Rule.Common;
using CodeEffects.Rule.Core;
using CodeEffects.Rule.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CodeEffects.Rule.Angular.Demo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RuleController : ControllerBase
	{
		private readonly IWebHostEnvironment environment;

		public RuleController(IConfiguration config, IWebHostEnvironment environment)
		{
			this.environment = environment;
		}

		// Returns an initial 
		[HttpGet("settings/{editorId}")]
		public ClientSettings GetSettings(string editorId)
		{
			RuleEditor editor = GetRuleEditor(editorId);

			ClientSettings settings = new ClientSettings
			{
				EditorData = editor.GetInitialSettings(),
				SourceData = editor.GetClientSettings(),
			};

			return settings;
		}

		// Loads an existing rule
		// GET api/<RuleController>/divRuleEditor/5
		[HttpGet("{editorId}/{id}")]
		public IActionResult Get(string editorId, Guid id)
		{
			string ruleXml = new StorageService(environment).LoadRuleXml(id.ToString());

			RuleEditor editor = GetRuleEditor(editorId);
			editor.LoadRuleXml(ruleXml);

			string ruleJson = editor.GetClientRuleData();

			// Send it back to the server
			return new JsonResult(ruleJson);
		}

		// Saves the rule
		// POST api/<RuleController>
		[HttpPost("{editorId}")]
		public Result Post([FromBody] string value, string editorId)
		{
			Result result = new Result();

			RuleEditor editor = GetRuleEditor(editorId);
			editor.LoadClientData(value);

			if (editor.Rule.IsEmpty())
			{
				result.IsRuleEmpty = true;
			}
			else if (!editor.Rule.IsValid(new StorageService(environment).LoadRuleXml))
			{
				result.IsRuleValid = false;
				result.ClientInvalidData = editor.GetClientInvalidData();
			}
			else
			{
				new StorageService(environment).SaveRule(
					editor.Rule.Id.ToString(),
					editor.Rule.GetRuleXml(),
					editor.Rule.IsLoadedRuleOfEvalType == null || (bool)editor.Rule.IsLoadedRuleOfEvalType
				);

				result.Output = editor.Rule.Id.ToString();
			}

			return result;
		}

		// Deletes an existing rule
		// DELETE api/<RuleController>/5
		[HttpDelete("{id}")]
		public JsonResult Delete(Guid id)
		{
			// Delete the rule from the storage file by its ID
			new StorageService(environment).DeleteRule(id.ToString());

			return new JsonResult(null);
		}

		// Evaluates a rule against the data from test form
		[HttpPost("evaluate")]
		public JsonResult Evaluate([FromBody] EvaluateRequest request)
		{
			EvaluateResponse response = new EvaluateResponse();

			// Get an instance of the Editor
			//RuleEditor editor = GetRuleEditor(editorId);

			// Tell the editor not to enforce the rule name requirement -
			// we don't save the rule, just evaluate it, so we don't care about its name
			//editor.RuleNameIsRequired = false;

			// Load the rule data from the client
			//editor.LoadClientData(evaluateRequest.RuleData);

			//if (editor.IsEmpty)
			//{
			//	response.IsRuleEmpty = true;
			//}
			//else if (!editor.IsValid)
			//{
			//	// The rule is invalid. Load the invalid data and pass it to the client
			//	response.IsRuleValid = false;
			//	response.ClientInvalidData = editor.GetClientInvalidData();
			//}
			//else
			//{
				EvaluationParameters p =
					new EvaluationParameters
					{
						Precision = 1,
						MidpointRounding = MidpointRounding.AwayFromZero,
						RuleGetter = new StorageService(this.environment).LoadRuleXml
					};

				StorageService storage = new StorageService(environment);

				// The rule is valid. Evaluate it.
				// The parameter Storage.LoadRuleXml is a delegate that evaluator
				// needs for looking up reusable rules that this rule might reference.
				Evaluator evaluator = new Evaluator(request.GetType(), storage.GetRuleSetXml(), p);

				//foreach (var id in storage.GetExecutionRuleIds())
				//{
				//	evaluator.Evaluate(request, id);
				//}

				evaluator.Evaluate(request, EvaluationScope.All, false);

				// Sending back the evaluated instance of the Patient
				response.Active = request.UnitCapacity.Active;
				response.BeginDate = request.UnitCapacity.BeginDate;
				response.DatesConsistent = request.UnitCapacity.DatesConsistent;
				response.DatesValid = request.UnitCapacity.DatesValid;
				response.EndDate = request.UnitCapacity.EndDate;
				response.EvaluationBeginDate = request.UnitCapacity.EvaluationBeginDate;
				response.EvaluationEndDate = request.UnitCapacity.EvaluationEndDate;
				response.MaxHrlyHeatInputCapacity = request.UnitCapacity.MaxHrlyHeatInputCapacity;

				response.Results = request.Results;
			//}

			return new JsonResult(response);
		}

		private RuleEditor GetRuleEditor(string editorId)
		{
			//Type type = typeof(Models.Patient);
			//Type type = typeof(Models.UnitCapacity);
			Type type = typeof(Models.EvaluateRequest);

			RuleEditor editor = new RuleEditor(editorId)
			{
				Mode = RuleType.Execution,
				SourceType = type
			};

			StorageService storage = new StorageService(environment);

			// These are the rules displayed in the Rules menu of the ToolBar
			editor.ToolBarRules = storage.GetAllRules();
			// These are reusable evaluation rules displayed in context menus of the Rule Area; see documentation details on reusable rules
			editor.ContextMenuRules = storage.GetEvaluationRules();

			return editor;
		}
	}
}
