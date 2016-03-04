var TimeSpan = require('timespan');

var NotificationType = require('./NotificationType.js');
var Notification = require('./Notification.js');
var ReportProtocol = require('./ReportProtocol.js');
var DailyDataCorrector = require('./DailyDataCorrector.js');
var Respite = require('./Respite.js');

var messages = {
	UndefinedStampsSource : "Source of data can't be undefined or null."
};

/**
 * Create object which can create reports about respites.
 * @param {Object} dataSource Object which can be used to get stamps.
 *                              	It should contain: GetAll, GetByEmployerID. GetByEmployerIDForDay methods.
 */
function RespitesForDay(stampsSource) {
	if(stampsSource == undefined) {
		throw new Error(messages.UndefinedStampsSource);
	}

	this._dataCorrector = new DailyDataCorrector(stampsSource);
}

/**
 * Collect all respites of employer for day.
 * @param {Numeric} employerID  Unique identifier of employer.
 * @param {Date} day         Target day.
 * @param {TimeSpan} maxDuration Maximum duration of respite.
 */
RespitesForDay.prototype.CreateReport = function(employerID, day, maxDuration) {
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

	respites = [];

	// Start with 1st because we interested of when
    // employer Out and when In but first stamp is In.
	for(i = 1; i < employerStamps.length - 1; i += 2) {
		var duration = employerStamps[i + 1].Time - employerStamps[i].Time;

		if(duration <= maxDuration.totalMilliseconds()) {
			respites.push(new Respite.Respite(employerStamps[i].Time, employerStamps[i + 1].Time));
		}
	}

	protocol.Result = respites;
	protocol.IsSucceed = true;

	return protocol;
};

module.exports = {
	RespitesForDay
};