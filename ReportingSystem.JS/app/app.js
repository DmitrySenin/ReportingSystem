var app = {};

app.ReportingSystem = {
	StampType: require('./reports/StampType.js'),
	EmployerTimeStamp: require('./reports/EmployerTimeStamp.js'),
	NotificationType: require('./reports/NotificationType.js'),
	Notification: require('./reports/Notification.js'),
	ReportProtocol: require('./reports/ReportProtocol.js')
};

module.exports = {
	app: app
};