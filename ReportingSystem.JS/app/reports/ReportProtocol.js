var NotificationType = require('./NotificationType.js');

/**
 * Create instance of protocol.
 */
function ReportProtocol() {
	this.Result = undefined;
	this.IsSucceed = undefined;
	this.Notifications = [];
}

/**
 * Find notifications with passed type in protocol. 
 * @param {NotificationType} neededType Necessary type of notification.
 * @return {Array} Array of notifications with needed type or empty array.
 */
ReportProtocol.prototype.GetNotificationsOfType = function(neededType) {
	return this.Notifications.filter(function(item){
		return !NotificationType.Compare(item.Type, neededType);
	});
}

module.exports = ReportProtocol;