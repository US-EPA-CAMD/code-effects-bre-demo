import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TestFormComponent } from '../test-form/test-form.component';
//import { Patient } from '../Patient';
//import { UnitCapacity } from '../UnitCapacity';

//$rule and $ce are defined in src/assets/js/codeeffects/editor.js
declare var $rule: any;

@Component({
	selector: 'app-rule-editor',
	templateUrl: './rule-editor.component.html',
	styleUrls: ['./rule-editor.component.css']
})

export class RuleEditorComponent implements OnInit
{
	http: HttpClient;
	baseUrl: string;
	ruleEditor: any;
	//patient: Patient = new Patient();
	//unitCapacity: UnitCapacity = new UnitCapacity();
	public info = "Demo version. Evaluations are delayed for 6 second.";

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string)
	{
		this.http = http;
		this.baseUrl = baseUrl;
	}

	ngOnInit(): void
	{
		this.http.get<ISettings>(this.baseUrl + 'api/rule/settings/divRuleEditor')
			.subscribe(
				settings =>
				{
					//There's an issue where navingating back places $rule into indetermined state. Clearing the context fixes that.
					this.ruleEditor = $rule.Context.getControl('divRuleEditor');
					if (this.ruleEditor != null)
						this.ruleEditor.dispose();
					$rule.Context.clear();

					//Initialize the Rule Editor with the editor data (localized strings) from the API settings action.
					this.ruleEditor = $rule.init(settings.editorData);
					this.ruleEditor.clear();

					//If built-in tool bar is used for managing rules, set up callbacks for loading, saving, and deleting rules.
					const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');
					this.ruleEditor.setClientActions(
						//get rule, called when a rule menu item is selected
						ruleId => this.http
							.get<string>(this.baseUrl + "api/rule/divRuleEditor/" + ruleId)
							.subscribe(rule => this.onRuleGet(rule)),
						//delete rule, called when the delete link is clicked
						ruleId => this.http
							.delete(this.baseUrl + "api/rule/" + ruleId)
							.subscribe(_ => this.onRuleDelete()),
						//save rule, called when the save link is clicked
						rule => this.http
							.post<ISaveResult>(this.baseUrl + "api/rule/divRuleEditor", JSON.stringify(rule), { headers: headers })
							.subscribe(result => this.onRuleSave(result))
					);

					//Load source data describing available elements
					this.ruleEditor.loadSettings(settings.sourceData);
				},
				error => console.log(error)
			);
	}

	//evaluate(model: Patient)
	//{
	//	const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');
	//	this.info = "Evaluating with 6 seconds trial delay...";

	//	this.http.post<IEvaluateResponse>(this.baseUrl + 'api/rule/evaluate/divRuleEditor', { patient: model, ruleData: JSON.stringify(this.ruleEditor.extract()) }, { headers: headers })
	//		.subscribe(
	//			response =>
	//			{
	//				if (response.isRuleEmpty)
	//					this.info = "The rule is empty";
	//				else if (!response.isRuleValid)
	//				{
	//					this.info = "The rule is invalid. Please move your mouse over the invalid element(s) to see details.";
	//					this.ruleEditor.loadInvalids(response.clientInvalidData);
	//				}
	//				else
	//				{
	//					this.info = response.output;
	//					this.patient = response.patient;
	//				}
	//			},
	//			error =>
	//			{
	//				this.info = "Evaluation error. Check console for details.";
	//				console.log(error);
	//			}
	//		);
	//}

	onRuleGet(rule: string): void
	{
		this.ruleEditor.loadRule(rule);
		this.info = "Rule is loaded";
	};

	onRuleSave(result: ISaveResult): void
	{
		if (result.isRuleEmpty)
			this.info = "The rule is empty";
		else if (!result.isRuleValid)
		{
			this.ruleEditor.loadInvalids(result.clientInvalidData);
			this.info = "The rule is not valid";
		}
		else
		{
			// Server returns rule ID using the Output property of ProcessingResult C# type.
			// The editor needs this ID if the saved rule was a new rule. Pass it to the editor.
			this.ruleEditor.saved(result.output);
			this.info = "The rule was saved successfully";

			//referencing the #ceTldivRuleEditor element is a temporary hack until CodeEffects adds a getter for that property. Consider using custom UI.
			let toolBarName: HTMLInputElement = <HTMLInputElement>document.getElementById("ceTldivRuleEditor");
			let ruleName = toolBarName.value;
		}
	};

	onRuleDelete(): void
	{
		// Let the editor know that there were no errors and
		// the rule of codeeffects.getRuleId() ID was deleted successfully
		this.ruleEditor.deleted(this.ruleEditor.getRuleId());
		this.ruleEditor.clear();
		this.info = "Rule was deleted successfully";
	};
}

interface ISettings
{
	editorData: string;
	sourceData: string;
}

interface ISaveResult
{
	isRuleEmpty: boolean;
	isRuleValid: boolean;
	output: string;
	clientInvalidData: string;
}

//interface IEvaluateResponse extends ISaveResult
//{
//	//isRuleValid: boolean;
//	//IsRuleEmpty: boolean;
//	//output: string;
//	//clientInvalidData: string;
//	patient: Patient;
//}
