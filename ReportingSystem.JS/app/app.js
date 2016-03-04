var app = {};

app.ReportingSystem = {
	StampType: require('./reports/StampType.js'),
	EmployerTimeStamp: require('./reports/EmployerTimeStamp.js'),
	NotificationType: require('./reports/NotificationType.js'),
	Notification: require('./reports/Notification.js'),
	ReportProtocol: require('./reports/ReportProtocol.js'),
	TimeOfWorkForDayReport: require('./reports/TimeOfWorkForDayReport.js'),
	RespitesForDayReport: require('./reports/RespitesForDayReport.js')
};

var timespan = require('timespan');

var targetDay = new Date(2016, 3, 3);
var source = {
	GetByEmployerIDForDay() {
		return [
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.Out, new Date(2016, 3, 3, 9, 0, 0)),
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.Out, new Date(2016, 3, 3, 9, 30, 0)),
			new app.ReportingSystem.EmployerTimeStamp(1, app.ReportingSystem.StampType.In, new Date(2016, 3, 3, 10, 0, 0))
		];
	}
};

var workTimeReporter = new app.ReportingSystem.TimeOfWorkForDayReport(source);

protocol = workTimeReporter.CreateReport(1, targetDay);

console.log(protocol);
console.log('Total hours = ' + protocol.Result.totalHours());

var respitesReporter = new app.ReportingSystem.RespitesForDayReport.RespitesForDay(source);
var maxDuration = timespan.fromMinutes(16);

protocol = respitesReporter.CreateReport(1, targetDay, maxDuration);

console.log(protocol);

module.exports = {
	app: app
};