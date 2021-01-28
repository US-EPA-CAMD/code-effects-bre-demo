import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EvaluateRequest } from '../eval-request';

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css'],
})

export class HomeComponent implements OnInit
{
	http: HttpClient;
	baseUrl: string;
	results: string[];
	evaluating: boolean = false;
	evalRequest: EvaluateRequest = new EvaluateRequest();

	constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string)
	{
		this.http = http;
		this.baseUrl = baseUrl;
	}

	ngOnInit(): void
	{
	}

	evaluate(model: EvaluateRequest)
	{
		const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');

		this.results = [];
		this.evalRequest.evalBeginDate = null;
		this.evalRequest.evalEndDate = null;
		this.evalRequest.active = false;
		this.evalRequest.datesValid = false;
		this.evalRequest.datesConsistent = false;
		this.evaluating = true;
		this.http.post<IEvaluateResponse>(this.baseUrl + 'api/rule/evaluate', {
				evaluationBeginDate: model.evalPeriodBeginDate,
				evaluationEndDate: model.evalPeriodEndDate,
				unitCapacity: {
					beginDate: model.beginDate,
					endDate: model.endDate,
					maxHrlyHeatInputCapacity: model.maxHrlyHeatInputCapacity
				}
			},
			{
				headers: headers
			})
			.subscribe(
				response =>
				{
					this.results = response.results;
					this.evalRequest.beginDate = response.beginDate;
					this.evalRequest.endDate = response.endDate;
					this.evalRequest.evalBeginDate = response.evaluationBeginDate;
					this.evalRequest.evalEndDate = response.evaluationEndDate;
					this.evalRequest.active = response.active;
					this.evalRequest.datesValid = response.datesValid;
					this.evalRequest.datesConsistent = response.datesConsistent;
					this.evalRequest.maxHrlyHeatInputCapacity = response.maxHrlyHeatInputCapacity;
					this.evaluating = false;
				},
				error =>
				{
					console.log(error);
					this.evaluating = false;
				}
			);
	}
}

interface IEvaluateResponse
{
	results: string[];
	evalPeriodBeginDate: Date;
	evalPeriodEndDate: Date;
	beginDate: Date;
	endDate: Date;
	evaluationBeginDate: Date;
	evaluationEndDate: Date;
	active: boolean;
	datesValid: boolean;
	datesConsistent: boolean;
	maxHrlyHeatInputCapacity: number;
}
