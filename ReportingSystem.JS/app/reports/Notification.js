var NotificationType = require('./NotificationType.js');

/**
 * Create unchangeable notification.
 * @param {String} description Details of notifications.
 * @param {NotificationType} type        Type of notification.
 */
function Notification(description, type) {
	this.Description = description;
	this.Type = type;

	// Make notification unchangeable.
	Object.freeze(this);
}

module.exports = {
	Create: Notification;
};