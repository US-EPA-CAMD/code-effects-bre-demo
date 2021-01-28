import { Component, Inject, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { EvaluateRequest } from '../eval-request';

@Component({
	selector: 'app-test-form',
	templateUrl: './test-form.component.html',
	styleUrls: ['./test-form.component.css']
})
export class TestFormComponent implements OnInit
{
	@Input() model: EvaluateRequest;
	@Output() onEvaluate = new EventEmitter<EvaluateRequest>();

	constructor()
	{
		this.model = new EvaluateRequest();
	}

	ngOnInit()
	{
	}

	onSubmit()
	{
		this.onEvaluate.emit(this.model);
	}
}
