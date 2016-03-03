var app = {};

app.ReportingSystem = {
	StampType: require('./reports/StampType.js'),
	EmployerTimeStamp: require('./reports/EmployerTimeStamp.js'),
	NotificationType: require('./reports/NotificationType.js'),
	Notification: require('./reports/Notification.js'),
	ReportProtocol: require('./reports/ReportProtocol.js')
};

DailyDataCorrector = require('./reports/DailyDataCorrector.js');

var source = {
	GetByEmployerIDForDay() {
		return [
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.Out, new Date(2016, 3, 3, 9, 0, 0)),
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.Out, new Date(2016, 3, 3, 9, 30, 0)),
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.In, new Date(2016, 3, 3, 10, 0, 0))
		];
	}
};

ddc = new DailyDataCorrector(source);

notes = [];

data = ddc.CollectStampsForDailyReport(1, new Date(2016, 3, 3), notes);

console.log(data);
console.log(notes);

module.exports = {
	app: app
};