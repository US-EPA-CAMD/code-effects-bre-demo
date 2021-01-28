export class EvaluateRequest
{
	public evalPeriodBeginDate: Date;
	public evalPeriodEndDate: Date;
	public beginDate: Date;
	public endDate: Date;
	public evalBeginDate: Date;
	public evalEndDate: Date;
	public active: boolean;
	public datesValid: boolean;
	public datesConsistent: boolean;
	public maxHrlyHeatInputCapacity: number;
}
