var NotificationType = require('./NotificationType.js');
var Notification = require('./Notification.js');
var ReportProtocol = require('./ReportProtocol.js');
var DailyDataCorrector = require('./DailyDataCorrector.js');
var TimeSpan = require('timespan');

var messages = {
	UndefinedStampsSource : "Source of data can't be undefined or null.",
	NoStampsForDay : "System can't find any stamps of employer for day."
};

/**
 * Create object which creates reports with employer time of work for day.
 * @param {Object} stampsSource Object which can be used to get stamps.
 *                              	It should contain: GetAll, GetByEmployerID. GetByEmployerIDForDay methods.
 */
function TimeOfWorkForDayReport(stampsSource) {

	if(stampsSource == undefined) {
		throw new Error(messages.UndefinedStampsSource);
	}

	this._dataCorrector = new DailyDataCorrector(stampsSource);
}

/**
 * Compute time of work of employer for day.
 * @param {Numeric} employerID Unique identifier of employer.
 * @param {Date} day        Date of target day.
 */
TimeOfWorkForDayReport.prototype.CreateReport = function(employerID, day) {
	protocol = new ReportProtocol();

	employerStamps = this._dataCorrector.CollectStampsForDailyReport(employerID, day, protocol.Notifications);

	// Some critical errors were found.
	if(protocol.GetNotificationsOfType(NotificationType.Error).length > 0) {
		protocol.IsSucceed = false;
		return protocol;
	}

	if(!employerStamps.length) {
		protocol.notifications.push(new Notification(messages.NoStampsForDay, NotificationType.Message));
	}

	timeOfWork = new TimeSpan.TimeSpan(0);

	for(var i = 0; i < employerStamps.length - 1; i += 2) {
		timeOfWork.addMilliseconds(employerStamps[i + 1].Time - employerStamps[i].Time);
	}

	protocol.Result = timeOfWork;
	protocol.IsSucceed = true;

	return protocol;
};

module.exports = TimeOfWorkForDayReport;